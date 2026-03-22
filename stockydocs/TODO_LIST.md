# Stocky - Master TODO List

Scanned from inline `TODO`, `FIXME`, and `[Obsolete]` markers across **stockyapi** and **stockymodels**.

Last updated: March 2026

---

## Priority Legend

| Priority | Meaning |
|----------|---------|
| P0 | **Critical** - Bugs, data integrity, security |
| P1 | **High** - Blocks feature completeness or quality |
| P2 | **Medium** - Code hygiene, maintainability |
| P3 | **Low** - Nice-to-have, cosmetic |

---

## Event Store Improvements (Current Focus)

### E1. Fix WithRowVersioningMulti – add proper version validation
**Priority:** P0
**File:** `stockymodels/EventStore/EventStore.cs`

**Problem:** `WithRowVersioningMulti` does not perform row version validation. It behaves like optimistic concurrency (relies on unique constraint) instead of explicit version checking. The single-event `WithRowVersioning` correctly:
1. Reads expected next sequence **before** transaction
2. Re-reads **inside** transaction
3. Compares and throws if they differ
4. Uses the pre-read expected sequence for the insert

`WithRowVersioningMulti` skips steps 1–3 entirely.

**Action items:**
- [ ] Before starting the transaction: for each distinct (AggregateType, AggregateId), query `MAX(AggregateSequenceId)` and build a map of expected next sequences
- [ ] Inside the transaction: re-query max sequences for all distinct aggregates
- [ ] Compare expected vs current; if any differ, throw `InvalidOperationException` with a clear message (e.g. "Row version conflict for aggregate X")
- [ ] Use the expected sequences (not the re-read ones) when inserting, so semantics match single-event row versioning
- [ ] Add integration tests for `RegisterMultipleEventsAsync` with `ConcurrencyLevel.RowVersion` (multi-aggregate, single-aggregate multi-event, and conflict scenarios)

---

### E2. Add optional expectedVersion parameter to RegisterEventAsync (RowVersion mode)
**Priority:** P1
**File:** `stockymodels/EventStore/EventStore.cs`

**Problem:** The tests in `EventStoreRowVersioningTests.cs` expect `RegisterEventAsync(cmd, evt, context, expectedVersion: 0)` and `GetStreamVersionAsync("FundId", fundId)`, but the current API has no `expectedVersion` parameter or `GetStreamVersionAsync`. The current `WithRowVersioning` uses an implicit “read before tx” as the expected version, which works but does not allow the caller to explicitly assert the version they expect (e.g. from a prior read).

**Action items:**
- [ ] Add optional `int? expectedVersion` parameter to `RegisterEventAsync` (and `RegisterMultipleEventsAsync` if needed)
- [ ] When `ConcurrencyLevel.RowVersion` and `expectedVersion` is not null: validate that `expectedVersion == currentMaxSeq` before inserting; throw with a clear message if mismatch
- [ ] Add `GetStreamVersionAsync(string aggregateType, Guid aggregateId)` (or equivalent) that returns `MAX(AggregateSequenceId)` for the stream
- [ ] Align enum: tests use `ConcurrencyLevel.RowVersioning` but code has `RowVersion` – standardise (e.g. add `RowVersioning` as alias or rename to match tests)
- [ ] Update or fix `EventStoreRowVersioningTests` so they compile and pass against the updated API

---

### E3. Sequence indexer – pros and cons analysis
**Priority:** P2 (decision/documentation)

**Context:** A “sequence indexer” typically means a **global** monotonically increasing sequence number across all events (in addition to the per-aggregate `AggregateSequenceId`).

**Pros:**
- **Event log subscriptions:** Consumers can subscribe from “event N” and process in global order
- **Replication / CDC:** Easier to track “last processed position” for outbox-style replication
- **Debugging / auditing:** Single number to reference any event across the system
- **Time-ordered queries:** Can order events across aggregates by insertion order

**Cons:**
- **Schema change:** New column (e.g. `GlobalSequenceId` BIGSERIAL or similar), migration, backfill
- **Single writer:** Global sequence usually implies a single writer or coordination; with multiple appenders, you need a sequence source (e.g. PostgreSQL `nextval` on a sequence)
- **Per-aggregate ordering:** `AggregateSequenceId` already gives correct order per stream; global sequence adds little for pure aggregate replay
- **Complexity:** More moving parts, potential bottlenecks if the sequence is a hotspot

**Recommendation:** Document the decision. If you plan event subscriptions or cross-aggregate ordering, add a global sequence. If you only need per-stream replay, keep the current design. Defer implementation until there is a concrete use case.

**Action items:**
- [ ] Document the decision (add to this TODO or a design doc): adopt global sequence index or not, and why
- [ ] If adopting: design column name, type (BIGINT/BIGSERIAL), and migration strategy
- [ ] If adopting: ensure Dapper inserts use the sequence (e.g. `INSERT ... RETURNING` or `nextval`)

---

### E4. Keep all logic in Dapper (no PostgreSQL functions for inserts)
**Priority:** P2 (policy)

**Context:** The migration `003InsertCommandAndEventFunction.sql` defines `insert_command_and_event`. The decision is to keep all insert logic in C# with Dapper.

**Action items:**
- [ ] Do not call `insert_command_and_event` from the event store; keep using parameterised `INSERT` via Dapper
- [ ] Optionally: add a revert migration or comment that the function is unused by the app (or remove it if not needed for other tooling)
- [ ] Ensure all sequencing, locking, and validation remain in C# for consistency and testability

---

## Summary – Event Store TODO

| ID  | Priority | Description |
|-----|----------|-------------|
| E1  | P0       | Fix WithRowVersioningMulti – add version validation |
| E2  | P1       | Add expectedVersion + GetStreamVersionAsync, align tests |
| E3  | P2       | Document sequence indexer decision (pros/cons) |
| E4  | P2       | Confirm policy: all logic in Dapper, no PG functions for inserts |

---

## Other Areas (Deferred / Non–Event Store)

### 1. Error Handling & Logging
- **1.1** Repository exception handling – mostly done (marked complete in prior list)
- **1.2** BaseController ProblemDetails – wire `ProblemTypes`, expand coverage (P1)

### 2. Validation & Security
- **2.1** Email validation – done
- **2.2** IUserContext claims – done

### 3. Repository & Data Access
- **3.1** HashSet for ID lookups – done
- **3.2** Reimbursement on holding deletion (P1)
- **3.3** UserRepository CreateUser separation (P2)
- **3.4** UserRepository UpdateUser – targeted methods (P2)

### 4. Caching
- **4.1** FusionCache (P2)

### 5. Feature Implementation
- **5.1** Fund transaction history endpoint (P1)
- **5.2** SQLite dev mode – verify/remove stale TODO (P3)

### 6. Code Organisation
- **6.1** Fix namespacing (P3)
- **6.2** CustomClaimTypes – verify/remove stale TODO (P3)

### 7. Deprecated Code
- **7.1** Remove obsolete Quote/QuoteSummary (P2)

### 8. stockymodels (Non–Event Store)
- **8.1** Migration/model nullability mismatch (P1)
- **8.2** Unused OrderType enum (P3)
- **8.3** DefaultCurrency GDP→GBP typo (P0)

---

## Execution Plan – Event Store Focus

### Phase 1: Fix RowVersionMulti (E1)
1. Implement expected-sequence map for all distinct aggregates before transaction
2. Inside transaction: re-query max sequences, compare, throw on mismatch
3. Use expected sequences for inserts (consistent with single-event behaviour)
4. Add integration tests for multi-event row versioning (success and conflict)

### Phase 2: expectedVersion API (E2)
1. Add `expectedVersion` parameter to `RegisterEventAsync` (optional)
2. Add `GetStreamVersionAsync(aggregateType, aggregateId)`
3. When RowVersion + expectedVersion provided: validate before insert
4. Fix enum naming (RowVersion vs RowVersioning) and update tests
5. Ensure `EventStoreRowVersioningTests` compile and pass

### Phase 3: Documentation & Policy (E3, E4)
1. Document sequence indexer decision (add or not, rationale)
2. Confirm Dapper-only policy; document or remove unused PG function

---

## Workflow

1. Start with **E1** (RowVersionMulti fix) – highest impact for correctness
2. Then **E2** (expectedVersion API) – improves API and test alignment
3. **E3** and **E4** – documentation and policy, no code change required immediately
