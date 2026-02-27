# Stocky Testing Plan

This document outlines the current test coverage, gaps, and a prioritized plan for adding tests using industry best practices.

---

## 1. Current State Summary

### 1.1 Test Stack
- **Framework:** NUnit 4.x
- **Mocking:** Moq
- **Coverage:** coverlet.collector
- **Integration DB:** SQLite in-memory via `SqliteTestSession` + EF Core

### 1.2 What Is Tested

| Area | Type | Status |
|------|------|--------|
| **Controllers** | Unit (mocked API) | Auth, Funds, MarketPricing, Portfolio, AccountSettings |
| **HoldingDetailsController** | — | **Not tested** |
| **Application / API layer** | Integration | PortfolioApi, FundsApi, AuthenticationApi (via integration tests) |
| **MarketPricingApi** | — | Only via controller mocks; no direct API tests |
| **Yahoo Finance service** | Mock HTTP + Live | Chart, Historical, Fundamentals, Insights, Options, Recommendations, Screener, Search, TrendingSymbols |
| **Repositories** | Integration only | Used inside Portfolio/Funds/Auth integration tests; no isolated repo tests |
| **Middleware / Helpers** | — | **Not tested** (Failures, Results, BaseController, ModelBinder) |
| **Finviz ScraperService** | — | Empty implementation; **no tests** |

### 1.3 Test Layout
- `stockytests/Controllers/` – controller unit tests (mock API layer)
- `stockytests/Integration/` – Auth, Portfolio, Funds integration tests
- `stockytests/Integration/YahooMockTests/` – Yahoo service with mocked HTTP (static JSON)
- `stockytests/Integration/LiveServiceTests/` – Yahoo live API (environment-dependent)
- `stockytests/Helpers/` – `ControllerTestHelpers`, `SqliteTestSession`, `TestUserContext`
- `stockytests/Mocks/` – `MockHttpMessageHandler`, `MockCallLoader`, `MockCallFile`

### 1.4 Conventions in Use (Good)
- **Arrange–Act–Assert** in tests
- **One logical assertion focus** per test in many places
- **Integration tests** use real SQLite + real application/API classes
- **Controller tests** mock the API boundary (thin controllers, test delegation + status/failure mapping)
- **Mock HTTP** for Yahoo with reusable `MockCallLoader` + JSON fixtures

---

## 2. Gaps and Issues

### 2.1 Missing Test Coverage

1. **HoldingDetailsController**  
   - Endpoints: `GetFundamentalsTimeSeries`, `GetInsights`, `GetOptions`, `GetRecommendations`.  
   - Add controller unit tests (mock `IYahooFinanceService`), success and failure paths.

2. **Application layer (API classes) in isolation**  
   - **MarketPricingApi**: Delegation to Yahoo is tested only via controller mocks. Consider 1–2 tests with mocked `IYahooFinanceService` to lock in mapping (e.g. GetCurrentPrice → GetChart with correct range/interval).  
   - **PortfolioApi**: Rich logic (validation, conflict, 422) is covered by integration tests; optional extra: unit tests with mocked `IPortfolioRepository` for specific failure branches.  
   - **FundsApi**: Same as Portfolio – integration covers it; optional unit tests for edge cases.

3. **Repositories in isolation**  
   - No tests that target `PortfolioRepository`, `FundsRepository`, or `UserRepository` directly.  
   - Adding repository-only tests (with SQLite) would make it easier to pinpoint DB/query bugs vs application logic.

4. **Middleware and shared types**  
   - **Result&lt;T&gt;** / **Failure** hierarchy: no unit tests (e.g. `Result.Success`, `Result.Fail`, implicit conversions).  
   - **BaseController.ProcessFailure**: No test that a given `Failure` type produces the expected status code and ProblemDetails (could be a small unit test with a concrete controller or a test double).  
   - **CommaDelimitedArrayModelBinder**: No tests for empty, whitespace, over-length, valid comma-separated list, and model state errors.

5. **Endpoint builders / Yahoo types**  
   - No tests for `YahooEndpointBuilder`, `YahooRange`, `YahooInterval`, etc. If they contain logic (e.g. URL building), add unit tests; if they are pure DTOs/enums, optional.

6. **Finviz ScraperService**  
   - Currently empty. When implemented, add unit tests (e.g. with mocked HTTP or static HTML) and optionally integration tests.

7. **TokenService**  
   - Used in auth; covered indirectly via `AuthIntegrationTests`. Optional: unit tests with mocked options to assert token shape/expiry.

### 2.2 Edge Cases and Negative Paths

- **Controllers:** Add tests for invalid/edge inputs where the API returns 4xx (e.g. null/empty symbol, invalid GUIDs for holdings).  
- **MarketPricing:** Empty or null symbol/list; behaviour may be undefined today – define it and add tests.  
- **Portfolio:** Sell more than holding, buy with insufficient funds – already covered in integration; ensure controller tests also verify 409/422 mapping when API returns those failures.  
- **Auth:** Invalid/duplicate email, short password – add or extend tests to lock validation behaviour.

### 2.3 Consistency and Hygiene

- **Namespace:** Controller test files use `stockyunittests` (e.g. `stockyunittests.Controllers`, `stockyunittests.Helpers`) while the project is `stockytests`. Prefer a single namespace (e.g. `stockytests`) for consistency and to match `RootNamespace` in the csproj.  
- **Test categories:** Consider `[Category("Unit")]`, `[Category("Integration")]`, `[Category("Live")]` so you can run only unit/integration in CI and exclude live Yahoo tests when no credentials.  
- **Coverage:** Run coverlet and track coverage for the API project; add tests for uncovered branches (especially failure paths and validation).

---

## 3. Recommended Test Additions (Prioritized)

### Priority 1 – High value, low effort

1. **HoldingDetailsController unit tests**  
   - Mock `IYahooFinanceService`.  
   - For each action: one success (200 + response body) and one failure (non-200 + ProblemDetails).  
   - Covers the only controller currently without tests.

2. **CommaDelimitedArrayModelBinder unit tests**  
   - Cases: `ValueProviderResult.None`, null/empty/whitespace string, string length &gt; 500, valid comma-separated list, multiple commas, leading/trailing spaces.  
   - Assert `ModelState` errors and `ModelBindingResult.Success`/`Failed`.

3. **Result&lt;T&gt; / Failure unit tests**  
   - `Result.Success`, `Result.Fail`, `IsSuccess`/`IsFailure`, implicit from T and from Failure.  
   - Optionally one test that `ProcessFailure` returns correct status and ProblemDetails for 400, 404, 409, 422, 500.

4. **Fix namespaces**  
   - Use `stockytests` (and `stockytests.Helpers`, `stockytests.Controllers`) everywhere so it matches the test project and avoids confusion.

### Priority 2 – Important for stability

5. **MarketPricingApi unit tests**  
   - Mock `IYahooFinanceService`.  
   - At least: GetCurrentPrice calls GetChart with expected range/interval; one success and one failure per public method if time permits.

6. **Controller edge/validation tests**  
   - MarketPricing: empty symbol, null symbols array (if allowed by route).  
   - Portfolio: invalid GUID format for GetHoldingsById/Delete; empty list.  
   - Auth: duplicate email, invalid password format.  
   - Define expected status (400/422) and add tests.

7. **Test categories and CI**  
   - Tag unit vs integration vs live.  
   - In CI, run unit + integration by default; run live only when explicitly requested or in a separate job with credentials.

### Priority 3 – Deeper coverage

8. **Repository unit tests**  
   - `PortfolioRepository`: GetHoldingsById with missing IDs, Buy/Sell persistence, ListAllHoldings ordering/count.  
   - `FundsRepository`: Deposit/Withdraw balance updates, concurrent updates if applicable.  
   - `UserRepository`: GetByEmail, duplicate insert.  
   - Use `SqliteTestSession` or equivalent in-memory DB.

9. **BaseController / ProcessFailure test**  
   - Small test (e.g. custom test controller that returns `ProcessFailure`) to assert status code and ProblemDetails for 2–3 Failure types.

10. **Yahoo endpoint builder / URL building**  
    - If `YahooEndpointBuilder` (or similar) has non-trivial logic, add unit tests for URL construction and query params.

### Priority 4 – When implementing new features

11. **Finviz ScraperService**  
    - When you add implementation: unit tests with mocked HTTP or static HTML; integration test only if you have a stable test source.

12. **TokenService**  
    - Unit tests with fixed `JwtSettings`: token not empty, contains expected claims, expiry in the future.

13. **PortfolioApi / FundsApi unit tests**  
    - Mock repository; test specific branches (e.g. 409 insufficient funds, 422 missing tickers) without hitting the DB.

---

## 4. Best Practices to Follow

- **Naming:** `MethodName_Scenario_ExpectedResult` (you already use this in many places).  
- **Arrange–Act–Assert:** Keep sections clear; one test per scenario.  
- **No logic in tests:** Avoid conditionals/loops; use parameterized tests (`[TestCase]` / `[TestCaseSource]`) for multiple inputs.  
- **Isolate tests:** No shared mutable state between tests; integration tests can share a session but reset data per test (as with `InitialiseTestAsync`).  
- **Mock only boundaries:** Controllers mock API; API can mock repository or Yahoo; repositories use real SQLite.  
- **Fast feedback:** Unit tests must be fast; keep integration tests in a separate category so they can be run less frequently if needed.  
- **Determinism:** No flaky tests; avoid fixed sleeps; for time-dependent logic use injectable clock or fixed time in tests.  
- **Coverage:** Aim for high coverage on business and failure paths; don’t chase 100% on trivial DTOs.  
- **Readability:** Prefer clear test names and short tests over fewer, large tests.

---

## 5. Suggested File Layout (Optional)

- `stockytests/Unit/` – pure unit tests (no DB, no HTTP)
  - `ResultTests.cs`, `FailureTests.cs`
  - `CommaDelimitedArrayModelBinderTests.cs`
  - `MarketPricingApiTests.cs`
  - `PortfolioApiTests.cs` / `FundsApiTests.cs` (if added)
- `stockytests/Unit/Controllers/` – controller tests (current `Controllers/` content)
  - Add `HoldingDetailsControllerTests.cs`
- `stockytests/Unit/Middleware/` or `Unit/Infrastructure/`
  - `BaseControllerProcessFailureTests.cs` (if you add it)
- `stockytests/Integration/` – existing integration tests
- `stockytests/Integration/YahooMockTests/` – existing
- `stockytests/Integration/LiveServiceTests/` – existing
- `stockytests/Integration/Repositories/` (optional)
  - `PortfolioRepositoryTests.cs`, `FundsRepositoryTests.cs`, `UserRepositoryTests.cs`

---

## 6. Quick Wins Checklist

- [x] Add `HoldingDetailsControllerTests` (mock `IYahooFinanceService`).  
- [x] Add `CommaDelimitedArrayModelBinderTests`.  
- [x] Add `ResultTests` (and optionally `FailureTests` / ProcessFailure).  
- [x] Unify namespaces to `stockytests`.  
- [x] Add `[Category("Unit")]` / `[Category("Integration")]` / `[Category("Live")]`.  
- [x] Add 1–2 edge-case tests per controller (null/empty/invalid).  
- [x] Add MarketPricingApi unit tests with mocked Yahoo service.  
- [ ] When Finviz is implemented, add ScraperService tests.

This plan should bring coverage and structure in line with best practices and make regressions and refactors safer.
