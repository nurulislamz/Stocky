# Stocky UI - Code Evaluation & Best Practices Review

---

## 1. Tech Stack Summary

| Layer | Technology | Version |
|-------|-----------|---------|
| Framework | React (CRA) | 18.3.1 |
| Language | TypeScript | 4.9.5 |
| UI Library | Material UI (MUI) v7 | 7.x |
| Charting | MUI X Charts | 8.1.0 |
| Data Grid | MUI X Data Grid Pro | 8.1.0 |
| Routing | React Router | 7.5.2 |
| HTTP | Axios | 1.9.0 |
| API Client | NSwag (auto-generated) | 14.4.0 |
| State | React hooks + local state | — |
| Date | Day.js | 1.11.13 |
| Animation | React Spring | 9.7.5 |

---

## 2. Architecture Evaluation

### 2.1 What's Working Well

**Auto-generated API client:** Using NSwag to generate `stockyapi.ts` from the Swagger spec is excellent. It keeps frontend types in sync with the backend and eliminates manual type duplication.

**Service layer pattern:** `BaseService` → `AuthService` / `PortfolioService` provides a clean abstraction over the generated API client with interceptors and error handling.

**Custom hooks:** `useAuth` and `usePortfolio` encapsulate business logic and state management in reusable hooks, which is idiomatic React.

**MUI theming:** The `shared-theme` directory with component-level customisations is well-organised and allows consistent design changes from a single place.

**Protected routes:** The `ProtectedRoute` component cleanly separates public from authenticated routes.

### 2.2 Structural Issues

#### Issue 1: Service instances are recreated on every render

**Problem:** Both `useAuth` and `usePortfolio` instantiate services inside the hook body:

```typescript
// useAuth.ts
export const useAuth = () => {
  const authService = new AuthService(); // NEW instance every render
```

```typescript
// usePortfolio.ts
export const usePortfolio = () => {
  const portfolioService = new PortfolioService(); // NEW instance every render
```

Each `new BaseService()` creates a new `axios.create()` instance, new interceptors, and a new `StockyApi` client. This is wasteful and can cause bugs (e.g., stale interceptor closures).

**Fix:** Create service instances once, outside the hook. Use `useMemo`, a module-level singleton, or React Context.

```typescript
// Singleton approach
const authService = new AuthService();

export const useAuth = () => {
  // use authService directly
};
```

Or better, provide services via Context:

```typescript
const ServiceContext = createContext<{ auth: AuthService; portfolio: PortfolioService }>(null!);
export const useServices = () => useContext(ServiceContext);
```

#### Issue 2: No global state management

**Problem:** `usePortfolio` is called independently in multiple components (`PortfolioTable`, `BuyTradeModal`, `PortfolioPage`). Each call creates its own state and fetches data independently. This means:

- Multiple identical API calls on page load
- State is not shared between components
- A buy in `BuyTradeModal` triggers a refetch, but `PortfolioTable`'s separate `usePortfolio` instance doesn't know about it

**Fix:** Lift portfolio state to a shared provider:

```typescript
const PortfolioContext = createContext<ReturnType<typeof usePortfolio>>(null!);

export const PortfolioProvider = ({ children }) => {
  const portfolio = usePortfolioInternal(); // the actual hook logic
  return <PortfolioContext.Provider value={portfolio}>{children}</PortfolioContext.Provider>;
};

export const usePortfolio = () => useContext(PortfolioContext);
```

Wrap the portfolio page (or the entire protected layout) in `<PortfolioProvider>` so all child components share one state.

#### Issue 3: ProtectedRoute has debug logging in production

**Problem:** `ProtectedRoute.tsx` contains `console.log` statements:

```typescript
console.log('ProtectedRoute - Auth check:', { ... });
console.log('ProtectedRoute - Not authenticated, forcing logout...');
```

These leak internal state to the browser console in production.

**Fix:** Remove all `console.log` from production code. Use a proper logger with environment-based log levels, or strip console calls via a build plugin.

#### Issue 4: `useAuth` creates `AuthService` but `ProtectedRoute` creates its own

**Problem:** `ProtectedRoute` creates a separate `AuthService` instance to call `isAuthenticated()`, duplicating logic that `useAuth` already provides. The two can disagree.

```typescript
// ProtectedRoute.tsx
const { isAuthenticated, logout } = useAuth();
const authService = new AuthService(); // second check!
const isReallyAuthenticated = authService.isAuthenticated();
```

**Fix:** Trust `useAuth` as the single source of truth. Remove the duplicate `AuthService` instantiation.

---

## 3. Component-Level Issues

### 3.1 PortfolioTable

**Hardcoded currency symbol:**

```typescript
averageBuyPrice: `£${item.averageBuyPrice || 0}`,
```

The `£` symbol is hardcoded. This should use the user's currency preference from `UserPreferencesModel`.

**Console.log in production:**

```typescript
console.log(items); // line 53
```

Remove.

**Inline styles in renderCell:**

The action buttons use inline `style={{...}}` instead of MUI's `sx` prop or theme overrides. This is inconsistent with the rest of the codebase.

**Edit button is a no-op:**

```typescript
onClick={() => console.log('Edit clicked for id:', params.row.id)}
```

Either implement the feature or remove the button.

### 3.2 BuyTradeModal

**Hardcoded colours:**

```typescript
background: 'linear-gradient(135deg, #2c3e50 0%, #34495e 100%)',
backgroundColor: '#667eea',
```

These bypass the MUI theme system. If the user switches to dark mode, these colours won't adapt. Use `theme.palette` values instead.

**Mixed currency symbols:**

The modal uses `$` for the total:

```typescript
${total}
```

But `PortfolioTable` uses `£`. These should be consistent and derived from user settings.

**No form validation library:**

Validation is done inline with `if (!symbolToUse || !quantityNum || !priceNum)`. For forms with more fields or complex rules, consider a library like `react-hook-form` + `zod` or `yup`.

### 3.3 Layout

**Potential runtime error:**

```typescript
backgroundColor: theme
  ? `rgba(${theme.palette.background.default} / 1)`
  : alpha(theme, 1),
```

`theme.palette.background.default` returns a colour string like `#fff`, not RGB values. The `rgba()` CSS function won't work with hex. Also, the falsy branch calls `alpha(theme, 1)` where `theme` is `undefined`, which would crash.

---

## 4. Service Layer Issues

### 4.1 Error handling swallows details

**Problem:** Every service method catches errors and returns a fabricated response:

```typescript
async getPortfolio() {
  try {
    return await this.api.portfolio();
  } catch (error) {
    console.error('Error fetching portfolio:', error);
    return {
      success: false,
      statusCode: 500,
      message: 'Failed to fetch portfolio'
    } as StockyApi.UserPortfolioResponse;
  }
}
```

This casts a generic object `as StockyApi.UserPortfolioResponse`, hiding the actual error. The component never sees the real HTTP status code or the server's `ProblemDetails` response.

**Fix:** Let errors propagate to the hook/component. Handle API errors in one place (the Axios interceptor or a shared error handler). Extract the `ProblemDetails` response body:

```typescript
async getPortfolio(): Promise<StockyApi.UserPortfolioResponse> {
  return this.api.portfolio();
}
```

Then handle errors in the hook:

```typescript
try {
  const response = await portfolioService.getPortfolio();
  // ...
} catch (error) {
  const message = extractErrorMessage(error); // read ProblemDetails.detail
  setError(message);
}
```

### 4.2 Token stored in localStorage

**Problem:** JWT tokens in `localStorage` are accessible to any JavaScript on the page (XSS vulnerability).

**Fix options (in order of security):**
1. **HttpOnly cookie** (most secure - token never accessible to JS)
2. **SessionStorage** (clears on tab close, still XSS-vulnerable but limits window)
3. **In-memory + refresh token in HttpOnly cookie** (best balance of UX and security)

At minimum, if staying with localStorage:
- Ensure CSP headers prevent inline scripts
- Sanitise all user inputs to prevent XSS

### 4.3 401 handler does a hard redirect

```typescript
if (error.response?.status === 401) {
  localStorage.removeItem('token');
  window.location.href = '/login';
}
```

`window.location.href` causes a full page reload, losing all React state. Use React Router's `navigate('/login')` instead. This requires making the interceptor aware of the router, which can be done via a callback or event.

---

## 5. Type System Issues

### 5.1 Duplicate type definitions

The `types/` directory defines `UserModel`, `StockHolding`, `PortfolioTransaction`, etc. that overlap with NSwag-generated types in `services/generated/stockyapi.ts`. Two sources of truth for the same data.

**Fix:** Use the NSwag-generated types as the single source of truth. Delete or minimise `types/*.ts` to only contain frontend-specific types (e.g., UI state, form state) that don't come from the API.

### 5.2 `DecodedToken` interface doesn't match JWT structure

```typescript
interface DecodedToken {
  issuer: string;
  audience: string;
  claims: string;
  expires: number;
  signingCredentials: string;
}
```

Standard JWT payload uses `iss`, `aud`, `exp`, `iat`, etc. The backend's custom claims use `userId`, `email`, etc. This interface doesn't match either format. The `decodeToken` method will parse the JSON but the field names won't align.

**Fix:** Define `DecodedToken` to match the actual JWT payload shape from your backend:

```typescript
interface DecodedToken {
  userId: string;
  email: string;
  firstName: string;
  surname: string;
  role: string;
  exp: number;
  iss: string;
  aud: string;
}
```

---

## 6. Testing

### 6.1 Current State

The project has `setupTests.ts` and testing library dependencies installed, but **no test files exist**. Zero frontend test coverage.

### 6.2 Recommended Test Plan

| Layer | What to Test | Tool |
|-------|-------------|------|
| Hooks | `useAuth` (login, logout, token expiry) | React Testing Library + `renderHook` |
| Hooks | `usePortfolio` (fetch, buy, sell, error states) | React Testing Library + MSW |
| Services | `AuthService` (token decode, expiry check) | Jest |
| Components | `PortfolioTable` (renders rows, delete flow) | React Testing Library |
| Components | `BuyTradeModal` (validation, submission) | React Testing Library |
| Routing | Protected routes redirect when unauthenticated | React Testing Library + MemoryRouter |
| Integration | Full page flows (login → portfolio → buy) | Cypress or Playwright |

### 6.3 Quick Win: Test AuthService

```typescript
describe('AuthService', () => {
  it('should return null when no token in localStorage', () => {
    const service = new AuthService();
    expect(service.getToken()).toBeNull();
  });

  it('should detect expired tokens', () => {
    const expiredToken = createJwtWithExp(Date.now() / 1000 - 3600);
    localStorage.setItem('token', expiredToken);
    const service = new AuthService();
    expect(service.isAuthenticated()).toBe(false);
  });
});
```

---

## 7. Performance Issues

### 7.1 No request deduplication or caching

`usePortfolio` fetches on mount and after every mutation. If multiple components mount simultaneously (PortfolioTable, PortfolioChart, StatCards), each triggers its own fetch. No SWR or React Query to deduplicate.

**Fix:** Consider adopting **TanStack Query (React Query)** or **SWR**:

- Automatic deduplication of identical requests
- Background refetching
- Cache invalidation after mutations
- Loading/error states built-in
- Stale-while-revalidate pattern

This would replace the manual `useState` + `useEffect` + `fetchPortfolio` pattern in `usePortfolio`.

### 7.2 No code splitting

All pages are imported eagerly in `App.tsx`:

```typescript
import HomePage from './pages/Home/HomePage';
import SearchPage from "./pages/Search/SearchPage";
// ... all imported at once
```

**Fix:** Use `React.lazy` + `Suspense`:

```typescript
const HomePage = React.lazy(() => import('./pages/Home/HomePage'));
const SearchPage = React.lazy(() => import('./pages/Search/SearchPage'));
```

### 7.3 CRA is outdated

Create React App is no longer maintained. Consider migrating to **Vite** for faster builds, HMR, and better long-term support.

---

## 8. Best Practices Checklist

### Applied
- [x] TypeScript with strict mode
- [x] Component-based architecture
- [x] Custom hooks for reusable logic
- [x] API client auto-generated from OpenAPI spec
- [x] Service layer abstraction over HTTP
- [x] Protected routes for authentication
- [x] MUI theme system with customisations
- [x] Responsive layout (mobile nav, desktop sidebar)

### Not Applied
- [ ] **Error Boundary** - No `ErrorBoundary` component. An unhandled error in any component crashes the entire app.
- [ ] **React Context for shared state** - Portfolio/auth state duplicated across component instances.
- [ ] **Centralised error handling** - Errors swallowed in services, inconsistent display in components.
- [ ] **Environment variable validation** - `REACT_APP_API_URL` falls back silently; no startup check.
- [ ] **Loading skeletons** - Loading states use plain `<div>Loading...</div>` instead of MUI Skeleton components.
- [ ] **Accessibility** - No ARIA labels, no keyboard navigation testing, no focus management.
- [ ] **i18n** - Currency symbols and text hardcoded in English / mixed `£` and `$`.
- [ ] **Code splitting** - All pages loaded eagerly.
- [ ] **Frontend tests** - Zero test files.
- [ ] **Remove console.log** - Debug logging present in production code.

---

## 9. Recommended Action Plan (Prioritised)

### Phase 1: Quick Fixes (1-2 days)

1. Remove all `console.log` statements from production code
2. Fix the `Layout.tsx` runtime error with `rgba` / `alpha`
3. Fix mixed currency symbols (`£` vs `$`) - use a shared constant or user preference
4. Remove the duplicate `AuthService` instantiation in `ProtectedRoute`
5. Fix `DecodedToken` interface to match actual JWT payload

### Phase 2: Architecture Improvements (3-5 days)

6. Make service instances singletons (module-level or Context-provided)
7. Create `PortfolioContext` so all portfolio components share one state
8. Add an `ErrorBoundary` at the app and page level
9. Let API errors propagate properly (remove try-catch-swallow in services)
10. Add `React.lazy` code splitting for route-level pages

### Phase 3: Quality & DX (1 week)

11. Add React Testing Library tests for `useAuth`, `usePortfolio`, and key components
12. Replace CRA with Vite
13. Adopt TanStack Query for API data fetching (replaces manual useState/useEffect/fetch pattern)
14. Remove duplicate types in `types/` - use NSwag-generated types everywhere
15. Add MUI Skeleton loading states

### Phase 4: Security & Production Readiness

16. Move JWT from localStorage to HttpOnly cookie (or in-memory + refresh token)
17. Replace `window.location.href` redirect with React Router navigation
18. Add environment variable validation at startup
19. Add CSP headers and input sanitisation
20. Add accessibility audit (axe-core or similar)

---

## 10. Summary

The Stocky UI has a solid foundation with TypeScript, MUI, auto-generated API types, and a clean service layer pattern. The main areas for improvement are:

1. **Shared state** - Portfolio data is fetched independently by every component that uses `usePortfolio`
2. **Error handling** - Services swallow errors; no error boundary; inconsistent UX
3. **Service lifecycle** - New service instances on every render is wasteful and bug-prone
4. **Production readiness** - Console logs, hardcoded colours, mixed currencies, no tests
5. **Security** - JWT in localStorage, hard page reload on 401

Addressing Phase 1 and 2 would have the highest impact on code quality and user experience.
