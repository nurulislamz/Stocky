# Stocky - Master TODO List

Scanned from inline `TODO`, `FIXME`, and `[Obsolete]` markers across **stockyapi** and **stockymodels**.

Last updated: February 2026

---

## Priority Legend

| Priority | Meaning |
|----------|---------|
| P0 | **Critical** - Bugs, data integrity, security |
| P1 | **High** - Blocks feature completeness or quality |
| P2 | **Medium** - Code hygiene, maintainability |
| P3 | **Low** - Nice-to-have, cosmetic |

---

## 1. Error Handling & Logging

### 1.1 Repository exception handling is raw and inconsistent
**Priority:** P0
**Files:**
- `stockyapi/Repository/Funds/FundsRepository.cs` (line 26)
- `stockyapi/Repository/PortfolioRepository/PortfolioRepository.cs` (multiple locations)

**Problem:** Repositories throw raw `Exception` or `NullReferenceException` with ad-hoc messages. No structured logging. No correlation IDs. Callers cannot distinguish "portfolio not found" from a database failure.

**Action items:**
- [x] Replace `throw new Exception(...)` with domain-specific exceptions or return `Result<T>.Fail(...)` so failures flow through the existing failure pipeline
- [x] Add structured `ILogger` calls with event IDs at the repository level (partially done in PortfolioRepository but not FundsRepository)
- [~] Consider a `PortfolioNotFoundException` that maps to 404 at the API layer
- [x] Remove `await _dbContext.SaveChangesAsync()` from `GetFundsAsync` (read-only query should not call SaveChanges)

### 1.2 BaseController ProblemDetails is incomplete
**Priority:** P1
**File:** `stockyapi/Controllers/Helpers/BaseController.cs` (lines 14, 25)

**Problem:** `ProcessFailure` does not populate the RFC 9457 `type` field. A `ProblemTypes` dictionary exists but is unused.

**Action items:**
- [ ] Wire `ProblemTypes` into `ProcessFailure` so each `Failure` subclass maps to a problem type URI
- [ ] Expand `ProblemTypes` to cover all Failure types (400, 401, 403, 409, 422, 500, 503, 504)
- [ ] Consider hosting a `/problems/{type}` endpoint that describes each error type (or link to docs)

---

## 2. Validation & Security

### 2.1 Email validation missing in UserContext
**Priority:** P1
**File:** `stockyapi/Middleware/UserContext.cs` (line 43)

**Problem:** The email claim is read from the JWT but no format validation is performed before it's used as the user's identity.

**Action items:**
- [x] Add email format validation (regex or `MailAddress.TryCreate`)
- [x] Log a warning and reject the request if the claim is present but malformed

### 2.2 Add additional claims to IUserContext
**Priority:** P2
**File:** `stockyapi/Middleware/IUserContext.cs` (line 9)

**Action items:**
- [ ] Add `Role`, `FirstName`, `Surname` properties to `IUserContext`
- [ ] Populate them in `UserContext` from JWT claims
- [ ] Use `Role` for authorization checks where needed (admin endpoints, premium features)

---

## 3. Repository & Data Access

### 3.1 Use HashSet for holding ID lookups
**Priority:** P2
**File:** `stockyapi/Repository/PortfolioRepository/PortfolioRepository.cs` (line 55)

**Action items:**
- [ ] Convert `Guid[] requestedIds` to `HashSet<Guid>` for O(1) lookups in `ValidateHoldingsExist`
- [ ] Same change for `string[] requestedTickers`

### 3.2 Implement reimbursement on holding deletion
**Priority:** P1
**File:** `stockyapi/Repository/PortfolioRepository/PortfolioRepository.cs` (line 199)

**Problem:** `DeleteHoldingsAsync` removes holdings but does not reimburse the portfolio's cash balance. This is a financial correctness issue.

**Action items:**
- [ ] Add an optional `reimburse` parameter (bool or enum: `Reimburse` / `WriteOff`)
- [ ] When reimbursing: calculate value from `Shares * AverageCost`, add to `CashBalance`, subtract from `InvestedAmount`
- [ ] Record the reimbursement in an `AssetTransactionModel` with a new `TransactionType.Reimburse`

### 3.3 UserRepository CreateUser separation
**Priority:** P2
**File:** `stockyapi/Repository/User/UserRepository.cs` (line 44)

**Problem:** User creation has commented-out code for initializing portfolio and preferences inline. This should be separated.

**Action items:**
- [ ] Create user in `UserRepository.CreateUserAsync`
- [ ] Create portfolio and preferences in a dedicated setup service or orchestrator (e.g. `UserSetupService`)
- [ ] Wrap in a transaction to ensure atomicity

### 3.4 UserRepository UpdateUser is too broad
**Priority:** P2
**File:** `stockyapi/Repository/User/UserRepository.cs` (line 83)

**Action items:**
- [ ] Replace `UpdateUserAsync(UserModel)` with targeted methods: `UpdateEmailAsync`, `UpdatePasswordAsync`, `UpdateProfileAsync`
- [ ] Use EF `Attach` + mark specific properties as modified to avoid overwriting unrelated fields

---

## 4. Caching

### 4.1 Implement FusionCache
**Priority:** P2
**File:** `stockyapi/Services/BaseApiServiceClient.cs` (line 53)

**Problem:** Currently using `IMemoryCache` directly. FusionCache would add cache stampede protection, distributed caching support, and fail-safe stale data.

**Action items:**
- [ ] Add `ZiggyCreatures.FusionCache` NuGet package
- [ ] Replace `IMemoryCache` with `IFusionCache` in `BaseApiServiceClient`
- [ ] Configure TTL, fail-safe, and optional Redis backplane for multi-instance deployments

---

## 5. Feature Implementation

### 5.1 Fund transaction history endpoint
**Priority:** P1
**File:** `stockyapi/Application/Funds/FundsController.cs` (line 69)

**Action items:**
- [ ] Add `GET /api/funds/history` endpoint
- [ ] Query `FundsTransactions` by user's portfolio, paginated, sorted by `CreatedAt` descending
- [ ] Return `FundsTransactionModel` list with total count for pagination

### 5.2 SQLite dev mode
**Priority:** P3
**File:** `stockyapi/Program.cs` (line 67)

**Problem:** Comment says "Add SQLite" but the code already supports SQLite via a dev flag. Verify this TODO is stale and remove it, or finish the implementation if it's incomplete.

**Action items:**
- [ ] Verify SQLite dev mode works end-to-end
- [ ] Remove the TODO if it's already complete

---

## 6. Code Organisation & Naming

### 6.1 Fix namespacing everywhere
**Priority:** P3
**File:** `stockyapi/Application/Portfolio/ZHelperTypes/OrderCommand.cs` (line 5)

**Action items:**
- [ ] Move `OrderCommand.cs` out of `ZHelperTypes` (the Z-prefix is a sort hack)
- [ ] Rename to `stockyapi.Application.Portfolio.Commands`
- [ ] Audit all namespaces for consistency (`stockyapi.Application.X`, `stockyapi.Repository.X`, `stockyapi.Services.X`)

### 6.2 Rename CustomClaimTypes
**Priority:** P3
**File:** `stockyapi/Services/TokenService.cs` (line 17)

**Problem:** The TODO says "Change name to `public static class CustomClaimTypes`" but the class is already named that. Likely the TODO is stale.

**Action items:**
- [ ] Verify the name is correct and remove the TODO
- [ ] Consider moving `CustomClaimTypes` to the `Middleware` or `Options` namespace since it's used by both TokenService and UserContext

---

## 7. Deprecated Code Cleanup

### 7.1 Remove obsolete Quote and QuoteSummary endpoints
**Priority:** P2
**Files:**
- `stockyapi/Application/MarketPricing/MarketPricingController.cs` (lines 90, 108)
- `stockyapi/Application/MarketPricing/MarketPricingApi.cs` (lines 45, 53)
- `stockyapi/Application/MarketPricing/IMarketPricingApi.cs`
- `stockyapi/Services/YahooFinance/IYahooFinanceService.cs` (both methods marked `[Obsolete("...", true)]`)

**Problem:** `GetQuotes` and `GetQuoteSummary` are marked obsolete because Yahoo Finance removed these endpoints. The code compiles (controller has `[Obsolete]` without `error: true`) but will fail at runtime.

**Action items:**
- [ ] Remove `GetQuotes` and `GetQuoteSummary` from controller, API, interface, and service
- [ ] Remove corresponding types: `QuoteResponseArray`, `QuoteSummaryResult` (if unused elsewhere)
- [ ] Remove corresponding test cases in `MarketPricingControllerTests`
- [ ] Remove mock JSON fixtures if any

---

## 8. stockymodels Issues (Found During Review)

### 8.1 Migration / model nullability mismatch
**Priority:** P1

**Problem:** `AssetTransactionModel` declares `Quantity`, `Price`, and `NewAverageCost` as `decimal?` (nullable) but the migration creates them as `NOT NULL`. Delete transactions use `null` for these fields, which will fail against the DB.

**Action items:**
- [ ] Generate a new migration that makes these columns nullable
- [ ] Or change the model to non-nullable and use `0` for delete transactions (less clean)

### 8.2 Unused `OrderType` enum
**Priority:** P3
**File:** `stockymodels/Models/Enums/OrderType.cs`

**Action items:**
- [ ] If limit/stop orders are planned, keep it and document the plan
- [ ] Otherwise remove it to reduce dead code

### 8.3 `DefaultCurrency` enum has `GDP` instead of `GBP`
**Priority:** P0
**File:** `stockymodels/Models/Enums/DefaultCurrency.cs`

**Action items:**
- [ ] Rename `GDP` to `GBP` (British Pounds)
- [ ] Generate a migration to update existing rows
- [ ] Add more currencies (EUR, JPY, etc.) as needed

---

## Summary by Priority

| Priority | Count | Key Items |
|----------|-------|-----------|
| P0 | 2 | Exception handling, GDP→GBP typo |
| P1 | 5 | Holding deletion reimbursement, ProblemDetails, email validation, fund history, nullability mismatch |
| P2 | 6 | HashSet, UpdateUser, FusionCache, obsolete cleanup, user creation separation, IUserContext claims |
| P3 | 4 | Namespacing, SQLite TODO cleanup, OrderType, CustomClaimTypes |

---

## Workflow

1. Start with **P0** items (data integrity and correctness)
2. Then **P1** (feature completeness and quality)
3. **P2** items can be done as part of regular refactoring sprints
4. **P3** items can be picked up when touching nearby code
