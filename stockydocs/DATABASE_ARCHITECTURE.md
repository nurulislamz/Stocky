# Stocky - Database Architecture Review & Event Sourcing Analysis

---

## 1. Current Architecture Overview

### 1.1 Entity Relationship Diagram (Conceptual)

```
UserModel (1)──────(1) PortfolioModel (1)──────(N) StockHoldingModel
    │                       │
    │                       ├──────(N) AssetTransactionModel
    │                       │
    │                       └──────(N) FundsTransactionModel
    │
    ├──────(1) UserPreferencesModel
    │
    ├──────(N) WatchlistModel
    │
    └──────(N) PriceAlertModel
```

### 1.2 Current Model Summary

| Entity | Purpose | Mutable? | Indexed Columns |
|--------|---------|----------|-----------------|
| `UserModel` | Account info, credentials | Yes | Email (unique) |
| `PortfolioModel` | Aggregate balance sheet | Yes (heavily) | UserId (unique) |
| `StockHoldingModel` | Current position snapshot | Yes (shares, avg cost) | Ticker, (PortfolioId, Ticker) unique |
| `AssetTransactionModel` | Buy/sell/delete log | Append-only (init) | (PortfolioId, CreatedAt) |
| `FundsTransactionModel` | Deposit/withdrawal log | Append-only | PortfolioId |
| `WatchlistModel` | Symbols user is tracking | Yes | (UserId, Symbol) unique |
| `PriceAlertModel` | Price threshold triggers | Yes (IsTriggered) | (Symbol, IsTriggered), (UserId, Symbol) unique |
| `UserPreferencesModel` | UI/notification settings | Yes | UserId (unique) |

### 1.3 Architecture Style

The current design is a **State-Sourced (CRUD)** architecture:

- **PortfolioModel** and **StockHoldingModel** hold the *current state* directly. They are mutated in-place on every buy/sell/deposit/withdraw.
- **AssetTransactionModel** and **FundsTransactionModel** are *append-only logs*, essentially acting as a rudimentary audit trail.
- The "source of truth" for balances is the mutable state in `PortfolioModel`, not the transaction log.

---

## 2. Issues With the Current Architecture

### 2.1 State Drift Risk

Because `PortfolioModel.CashBalance`, `InvestedAmount`, and `TotalValue` are updated in-place alongside separate transaction records, there is no mechanism to detect or correct drift between the aggregate state and the sum of transactions. A bug in a single repository method can silently corrupt the running totals.

### 2.2 Audit Gaps

- **No "who/when/why" metadata** on state changes. `BaseModel.UpdatedAt` tells you *when* a row changed, but not *what* changed or *who* initiated it.
- **Delete transactions** record `null` for Quantity/Price/NewAverageCost, losing the financial details of what was deleted.
- **No rollback capability** - once a holding is removed or a balance is updated, the previous state is gone.

### 2.3 Concurrency

No optimistic concurrency tokens (`RowVersion` / `ConcurrencyCheck`) on `PortfolioModel` or `StockHoldingModel`. Two simultaneous buy requests could produce incorrect balances.

### 2.4 Financial Precision

- `CashBalance`, `InvestedAmount`, `TotalValue` use `Precision(18,2)` which is fine for display but may lose precision during intermediate calculations. Consider using `(18,4)` internally and rounding only for display.
- `Shares` uses `Precision(18,4)` which is good for fractional shares, but the repository does integer-style arithmetic in some places.

---

## 3. Event Sourcing: Analysis for Stocky

### 3.1 What is Event Sourcing?

In event sourcing, you never store the current state directly. Instead, you store an **immutable, ordered sequence of events** (facts that happened). The current state is derived by replaying all events from the beginning. An event store is the single source of truth.

```
Traditional (what you have now):
  Portfolio.CashBalance = 500  (mutable row)

Event Sourced:
  Event 1: FundsDeposited { Amount: 1000 }
  Event 2: StockBought { Symbol: AAPL, Qty: 5, Price: 100 }
  → Derived CashBalance = 1000 - 500 = 500
```

### 3.2 Pros of Event Sourcing for Stocky

| Benefit | How it helps Stocky |
|---------|-------------------|
| **Complete audit trail** | Every buy, sell, deposit, withdrawal is an immutable event. Regulators, users, and developers can trace exactly what happened and when. |
| **Debugging** | You can replay events to reproduce any historical state. "Why is this balance wrong?" becomes answerable. |
| **State reconstruction** | If you find a bug in your balance calculation, you can fix the logic and replay events to get the correct state. No need for manual data patches. |
| **Temporal queries** | "What was my portfolio worth on March 1st?" becomes a replay-to-date query, not a reporting afterthought. |
| **Separation of concerns** | Write model (events) is decoupled from read model (projections). You can optimise read models independently. |
| **Undo/rollback** | Compensating events can be appended (e.g., "BuyCancelled") without modifying history. |

### 3.3 Cons of Event Sourcing for Stocky

| Drawback | Impact |
|----------|--------|
| **Complexity** | Significantly more complex than CRUD. You need event store, projections, snapshots, eventual consistency patterns. |
| **Learning curve** | Team must understand aggregates, commands, events, projections, idempotency. |
| **Storage growth** | Every action creates a new row forever. For a personal portfolio tracker with low write volume, this is negligible. For a high-frequency trading platform, it matters. |
| **Query difficulty** | You can't just `SELECT * FROM Portfolios WHERE CashBalance > 1000`. You need read-model projections (CQRS). |
| **Schema evolution** | Changing event shapes requires versioning and upcasting logic. |
| **Overkill for Stocky's scale** | A personal stock tracker with simple buy/sell operations may not benefit enough to justify the complexity. |
| **EF Core is not ideal** | EF Core is designed for CRUD. True event sourcing typically uses a dedicated event store (EventStoreDB, Marten, or a custom append-only table). |

### 3.4 Verdict

**You already have 70% of the audit benefit without full event sourcing.** Your `AssetTransactionModel` and `FundsTransactionModel` are already append-only event logs. The main gap is that:

1. They're not the *source of truth* - the mutable `PortfolioModel` is.
2. They don't capture enough detail (delete transactions lose financial data).
3. There's no mechanism to reconcile state from the log.

**Recommendation:** Don't adopt full event sourcing. Instead, adopt a **hybrid approach** that gives you the audit and correctness benefits without the complexity.

---

## 4. Recommended Hybrid Approach

### 4.1 Strategy: "Transaction Log as Source of Truth" (Lightweight Event Sourcing)

Keep your current EF Core / relational setup. Make these targeted changes:

#### Step 1: Make transaction logs the authoritative source

- Every state mutation (buy, sell, deposit, withdraw, delete) MUST create a transaction record first.
- The aggregate state in `PortfolioModel` and `StockHoldingModel` is a *derived cache* that can be rebuilt from the log.

#### Step 2: Add a reconciliation capability

- Add a `ReconcilePortfolio(Guid userId)` method that replays all `AssetTransactionModel` and `FundsTransactionModel` records and recomputes `CashBalance`, `InvestedAmount`, `TotalValue`, and all holdings.
- Run this on demand (admin tool) or on a schedule to detect drift.

#### Step 3: Enrich the transaction model

Add fields to capture the full picture of each event:

```csharp
public class AssetTransactionModel : BaseModel
{
    // Existing fields...

    // NEW: Capture the state AFTER the transaction
    public decimal? PortfolioCashBalanceAfter { get; init; }
    public decimal? PortfolioInvestedAmountAfter { get; init; }
    public decimal? HoldingSharesAfter { get; init; }
    public decimal? HoldingAverageCostAfter { get; init; }

    // NEW: Who initiated it
    public Guid InitiatedByUserId { get; init; }

    // NEW: Optional notes / reason
    public string? Notes { get; init; }
}
```

#### Step 4: Add concurrency protection

```csharp
public class PortfolioModel : BaseModel
{
    // Existing fields...

    [ConcurrencyCheck]
    public uint RowVersion { get; set; }
}
```

Or use `[Timestamp]` with PostgreSQL's `xmin` system column.

### 4.2 What You Do NOT Need to Change

- **No need to remove your current architecture.** The mutable `PortfolioModel` stays as a read-optimised cache.
- **No need for CQRS** or separate read/write databases.
- **No need for an event store** (EventStoreDB, etc.). Your existing relational DB is fine.
- **No need for event bus / messaging.** This is not a distributed system.
- **EF Core continues to work** exactly as it does today.

### 4.3 Migration Path

| Phase | Change | Effort |
|-------|--------|--------|
| 1 | Fix nullable mismatch in AssetTransactionModel migration | Small |
| 2 | Add concurrency token to PortfolioModel | Small |
| 3 | Enrich AssetTransactionModel with post-state fields | Medium |
| 4 | Add reconciliation method | Medium |
| 5 | Ensure all repository mutations create a transaction record before mutating state | Medium |
| 6 | Add admin endpoint to trigger reconciliation | Small |

---

## 5. Additional Database Improvements

### 5.1 Model Improvements

| Issue | Current | Recommended |
|-------|---------|-------------|
| `DefaultCurrency.GDP` | Typo | Rename to `GBP` |
| `PriceAlertModel.Condition` | Magic string (`"above"`, `"below"`) | Create `AlertCondition` enum |
| `FundsTransactionModel` lacks balance snapshot | No post-state | Add `CashBalanceAfter`, `TotalValueAfter` |
| No soft delete | Hard delete on holdings and users | Add `IsDeleted` flag to `BaseModel` or use EF global query filters |
| `OrderType` enum | Unused | Remove or implement limit/stop order support |
| Cascade delete everywhere | Deleting a user cascades to portfolio, holdings, transactions | Consider `Restrict` on transaction tables (preserve financial history) |

### 5.2 Index Improvements

| Table | Suggested Index | Reason |
|-------|-----------------|--------|
| `FundsTransactions` | `(PortfolioId, CreatedAt)` | Transaction history queries are always by portfolio + time |
| `Transactions` | `(PortfolioId, Ticker)` | Finding all transactions for a specific holding |
| `Users` | `(Email)` already exists | Good |
| `Watchlist` | `(UserId)` | Listing all watchlist items for a user |

### 5.3 Naming Consistency

| Current | Suggested | Reason |
|---------|-----------|--------|
| `StockHoldings` table with `Ticker` column | — | Fine, but `Symbol` is used in Watchlist/PriceAlert. Pick one consistently. |
| `Transactions` table | `AssetTransactions` | Distinguish from `FundsTransactions` |
| `Funds` navigation property on Portfolio | `FundsTransactions` | More descriptive |

---

## 6. Full Event Sourcing - If You Want to Go All the Way

If you decide in the future that you want full event sourcing, here's what it would look like.

### 6.1 Event Store Table

```csharp
public class DomainEvent
{
    public long SequenceNumber { get; set; }        // Auto-increment, global order
    public Guid AggregateId { get; set; }            // e.g. PortfolioId
    public string AggregateType { get; set; }        // "Portfolio"
    public int AggregateVersion { get; set; }        // Per-aggregate version
    public string EventType { get; set; }            // "StockBought", "FundsDeposited"
    public string EventData { get; set; }            // JSON payload
    public DateTime OccurredAt { get; set; }
    public Guid InitiatedBy { get; set; }
}
```

### 6.2 Event Types

```csharp
public record FundsDeposited(decimal Amount);
public record FundsWithdrawn(decimal Amount);
public record StockBought(string Symbol, decimal Quantity, decimal Price);
public record StockSold(string Symbol, decimal Quantity, decimal Price);
public record HoldingDeleted(string Symbol, decimal Shares, decimal AverageCost, bool Reimbursed);
```

### 6.3 Aggregate

```csharp
public class PortfolioAggregate
{
    public decimal CashBalance { get; private set; }
    public decimal InvestedAmount { get; private set; }
    public Dictionary<string, (decimal Shares, decimal AvgCost)> Holdings { get; } = new();

    public void Apply(FundsDeposited e) => CashBalance += e.Amount;
    public void Apply(StockBought e) { /* update cash, holdings, invested */ }
    // etc.
}
```

### 6.4 What You'd Remove

- `PortfolioModel.CashBalance`, `InvestedAmount`, `TotalValue` would become read-model projections
- `StockHoldingModel` would become a projection
- `AssetTransactionModel` and `FundsTransactionModel` would be replaced by `DomainEvent`

### 6.5 Libraries for .NET

- **Marten** (PostgreSQL-based, excellent with EF Core, supports both event store + document store)
- **EventStoreDB** (dedicated event store, gRPC protocol, more operational overhead)
- **Custom** (append-only table in your existing DB, simplest but you build everything yourself)

**If you go this route, Marten is the best fit** since you're already on PostgreSQL.

---

## 7. Summary

| Approach | Complexity | Audit Quality | Recommended? |
|----------|------------|---------------|--------------|
| Current (CRUD + log) | Low | Basic | No - has drift risk |
| **Hybrid (enriched log + reconciliation)** | **Medium** | **Good** | **Yes** |
| Full Event Sourcing | High | Excellent | No - overkill for current scale |

**Start with the hybrid approach.** It gives you robust auditability, drift detection, and state reconstruction without requiring a fundamental architectural change. If Stocky grows into a production trading platform with regulatory requirements, you can migrate to full event sourcing later using Marten.


Adding in bitemporality

Double accounting method