# Stocky - Authentication Middleware & TokenService Review

---

## 1. What You Currently Have

Your auth pipeline has four parts:

```
┌─────────────┐     ┌──────────────────┐     ┌──────────────────┐     ┌───────────────┐
│ TokenService │     │ AddJwtBearer     │     │ HttpUserContext  │     │ [Authorize]   │
│ (generation) │     │ (middleware)     │     │ (claim reader)   │     │ (enforcement) │
└──────┬───────┘     └────────┬─────────┘     └────────┬─────────┘     └───────┬───────┘
       │                      │                        │                       │
  Login/Register         Every request           After auth passes      Controller level
  creates a JWT          validates JWT           reads UserId/Email     blocks anonymous
                         sets HttpContext.User   from ClaimsPrincipal   users
```

### Component breakdown

| Component | Type | Purpose | Location |
|-----------|------|---------|----------|
| `TokenService` | Scoped service | **Creates** JWTs on login/register | `Services/TokenService.cs` |
| `AddJwtBearer` | Middleware (built-in) | **Validates** incoming JWTs, populates `HttpContext.User` | `Program.cs` line 126 |
| `HttpUserContext` | Scoped service | **Reads** claims from the validated `ClaimsPrincipal` | `Middleware/UserContext.cs` |
| `[Authorize]` | Filter attribute | **Enforces** that the request is authenticated | On each controller |
| `DevBypassAuthMiddleware` | Custom middleware | **Bypasses** auth in dev mode | `Middleware/DevBypassAuthMiddleware.cs` |

### Verdict: You ARE already using middleware for auth

The `AddJwtBearer` call in `Program.cs` registers Microsoft's built-in JWT Bearer authentication middleware. On every request, this middleware:

1. Reads the `Authorization: Bearer <token>` header
2. Validates the token signature, issuer, audience, and expiry
3. Deserialises the claims into a `ClaimsPrincipal`
4. Sets `HttpContext.User` to that principal

This is the standard, recommended approach. **You do not need to write custom middleware to replace it.**

`TokenService` is not middleware and should not become middleware. It's a service that *generates* tokens, which is a fundamentally different concern from *validating* tokens. Generation happens once (on login/register). Validation happens on every request (that's the middleware's job, and `AddJwtBearer` already does it).

---

## 2. Issues With the Current Setup

While the overall pattern is correct, there are several best-practice violations and bugs:

### 2.1 `DateTime.Now` instead of `DateTime.UtcNow` in token expiry

```csharp
// TokenService.cs line 57
expires: DateTime.Now.AddMinutes(_jwtSettings.ExpirationInMinutes),
```

`DateTime.Now` uses the server's local timezone. If the server is in UTC+5 and the JWT middleware validates with UTC (which it does by default), the token will appear to expire 5 hours early or late depending on direction.

**Fix:** Use `DateTime.UtcNow`.

### 2.2 DevBypassAuthMiddleware uses wrong claim types

```csharp
// DevBypassAuthMiddleware.cs line 27-31
var claims = new[]
{
    new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
    new Claim(ClaimTypes.Name, "Test User"),
    new Claim(ClaimTypes.Email, "test@example.com")
};
```

But `HttpUserContext` reads `CustomClaimTypes.UserId` (string `"userId"`), not `ClaimTypes.NameIdentifier`. So when dev bypass is active, `HttpUserContext.UserId` will throw `InvalidOperationException` because the claim key doesn't match.

**Fix:** Use `CustomClaimTypes` consistently:

```csharp
var claims = new[]
{
    new Claim(CustomClaimTypes.UserId, "00000000-0000-0000-0000-000000000001"),
    new Claim(CustomClaimTypes.Email, "dev@example.com"),
    new Claim(CustomClaimTypes.FirstName, "Dev"),
    new Claim(CustomClaimTypes.Surname, "User"),
    new Claim(CustomClaimTypes.Role, "Admin")
};
```

### 2.3 `TokenService` and `CustomClaimTypes` are in the same file

`CustomClaimTypes` is a static class used by both `TokenService` (writes claims) and `HttpUserContext` (reads claims). It doesn't belong in `Services/TokenService.cs`. Both consumers already reference it via `using stockyapi.Services`, which is misleading.

**Fix:** Move `CustomClaimTypes` to a shared location.

### 2.4 Token expiry not validated

The `AddJwtBearer` config in `Program.cs` does NOT explicitly set `ValidateLifetime = true`. While this defaults to `true`, it's a security-critical setting that should be explicit:

```csharp
opts.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidIssuer = configuration["Jwt:Issuer"],
    ValidateAudience = true,
    ValidAudience = configuration["Jwt:Audience"],
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(...),
    // MISSING: ValidateLifetime = true,
    // MISSING: ClockSkew = TimeSpan.Zero (or a small window)
};
```

By default, `ClockSkew` is 5 minutes, meaning tokens are valid for `ExpirationInMinutes + 5`. This is usually fine but should be documented.

### 2.5 No token refresh mechanism

When the JWT expires, the user must log in again. For a better UX:

- Issue a short-lived access token (15-30 min)
- Issue a long-lived refresh token (days/weeks) stored in an HttpOnly cookie
- Add a `POST /api/auth/refresh` endpoint that exchanges a valid refresh token for a new access token

### 2.6 `ITokenService` is injected as Scoped but is stateless

`TokenService` only reads from `IOptions<JwtSettings>` which is a singleton. There's no per-request state. It should be registered as **Singleton** for better performance:

```csharp
services.AddSingleton<ITokenService, TokenService>();
```

### 2.7 `HttpsRedirection` is after `UseAuthentication`

```csharp
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection(); // ← should be BEFORE auth
```

If a client sends credentials over HTTP, the auth middleware processes them on the insecure connection before the redirect happens. The standard order is:

```csharp
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowLocalReactApp");
app.UseAuthentication();
app.UseAuthorization();
```

---

## 3. Recommended Restructure

### 3.1 File organisation

**Before:**

```
Services/
  TokenService.cs          ← contains ITokenService, TokenService, AND CustomClaimTypes
Middleware/
  UserContext.cs            ← HttpUserContext reads claims
  IUserContext.cs
  DevBypassAuthMiddleware.cs
Options/
  JwtSettings.cs
```

**After:**

```
Auth/
  Claims/
    StockyClaims.cs         ← CustomClaimTypes renamed, shared by all auth code
  Token/
    ITokenService.cs        ← interface only
    JwtTokenService.cs      ← implementation (renamed for clarity)
  UserContext/
    IUserContext.cs
    HttpUserContext.cs
  DevBypassAuthMiddleware.cs
Options/
  JwtSettings.cs
```

Or, if you prefer to keep the current flat structure, at minimum move `CustomClaimTypes` out of `TokenService.cs`:

```
Middleware/
  IUserContext.cs
  HttpUserContext.cs
  StockyClaims.cs           ← moved here, renamed
  DevBypassAuthMiddleware.cs
Services/
  ITokenService.cs
  TokenService.cs
```

### 3.2 Rename `CustomClaimTypes` to `StockyClaims`

The current name is generic and the TODO to rename it is stale. Make it descriptive:

```csharp
namespace stockyapi.Auth;

public static class StockyClaims
{
    public const string UserId = "userId";
    public const string FirstName = "firstName";
    public const string Surname = "surname";
    public const string Role = "role";
    public const string Email = "email";
}
```

### 3.3 Fix the middleware order in `Program.cs`

```csharp
private static void ConfigureMiddleware(IApplicationBuilder app, IHostEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseSwaggerDocumentation();
    }

    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseCors("AllowLocalReactApp");
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
}
```

### 3.4 Fix `TokenService` expiry

```csharp
var token = new JwtSecurityToken(
    issuer: _jwtSettings.Issuer,
    audience: _jwtSettings.Audience,
    claims: claims,
    expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
    signingCredentials: creds
);
```

### 3.5 Make token validation parameters explicit

```csharp
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opts =>
{
    opts.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = configuration["Jwt:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("Jwt:Key is not configured"))),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(1)
    };
});
```

### 3.6 Enrich `IUserContext`

The interface currently only exposes `UserId` and `Email`. Add `Role` and `FirstName` so downstream code doesn't need to re-read claims:

```csharp
public interface IUserContext
{
    bool IsAuthenticated { get; }
    Guid UserId { get; }
    string Email { get; }
    string FirstName { get; }
    string Surname { get; }
    string Role { get; }
}
```

### 3.7 Fix DevBypassAuthMiddleware

```csharp
public async Task Invoke(HttpContext context)
{
    if (_configuration.GetValue<string>("Env") == "Development" &&
        _configuration.GetValue<bool>("Authentication:BypassAuth"))
    {
        var devUserId = Guid.Empty.ToString();
        var claims = new[]
        {
            new Claim(StockyClaims.UserId, devUserId),
            new Claim(StockyClaims.Email, "dev@example.com"),
            new Claim(StockyClaims.FirstName, "Dev"),
            new Claim(StockyClaims.Surname, "User"),
            new Claim(StockyClaims.Role, "Admin")
        };

        var identity = new ClaimsIdentity(claims, "Development");
        context.User = new ClaimsPrincipal(identity);
    }

    await _next(context);
}
```

Note: the dev middleware no longer needs `ITokenService` at all. It just sets `context.User` directly. The JWT token generation and header injection was unnecessary since the middleware runs *before* `UseAuthentication()`, and `context.User` is already set.

---

## 4. Should You Write Custom Auth Middleware?

**No.** ASP.NET Core's built-in JWT Bearer middleware (`AddJwtBearer`) is production-hardened, handles edge cases (clock skew, key rotation, token format errors, WWW-Authenticate headers), and is maintained by Microsoft. Writing your own would mean reimplementing all of that.

The only scenarios where custom auth middleware makes sense:

| Scenario | Do You Need It? |
|----------|-----------------|
| Custom token format (not JWT) | No - you use standard JWT |
| Multi-tenant key resolution | No - single issuer/key |
| Rate limiting per user | Use a rate-limiting middleware, not auth |
| Request logging with user ID | Use a separate logging middleware that reads `HttpContext.User` |
| Token blacklisting/revocation | See section 5 below |

---

## 5. Future: Token Refresh & Revocation

### 5.1 Refresh tokens

Currently, when a JWT expires, the user is logged out. A better flow:

```
Client                    Server
  │                         │
  ├─ POST /auth/login ─────►│  Returns: { accessToken (15 min), refreshToken (7 days) }
  │◄────────────────────────┤
  │                         │
  ├─ GET /api/portfolio ───►│  Authorization: Bearer <accessToken>
  │◄────────────────────────┤  200 OK
  │                         │
  │  ... 15 min later ...   │
  │                         │
  ├─ GET /api/portfolio ───►│  Authorization: Bearer <expired accessToken>
  │◄────────────────────────┤  401 Unauthorized
  │                         │
  ├─ POST /auth/refresh ───►│  Body: { refreshToken }
  │◄────────────────────────┤  Returns: { new accessToken, new refreshToken }
  │                         │
  ├─ GET /api/portfolio ───►│  Authorization: Bearer <new accessToken>
  │◄────────────────────────┤  200 OK
```

Implementation:

1. Add a `RefreshTokenModel` entity (token hash, userId, expiry, isRevoked)
2. On login: generate both access + refresh token, store refresh token hash in DB
3. Add `POST /api/auth/refresh` endpoint that validates the refresh token and issues new tokens
4. On logout: revoke the refresh token in DB
5. Store refresh token in an HttpOnly, Secure, SameSite cookie (not localStorage)

### 5.2 Token revocation (blacklisting)

JWTs are stateless - once issued, they're valid until expiry. If you need to immediately invalidate a token (e.g., user changes password, admin bans user), you need one of:

- **Short-lived tokens + refresh**: Access tokens expire in 15 min. Revoke the refresh token and the user is locked out within 15 min. Good enough for most cases.
- **Token version on user**: Store a `TokenVersion` on `UserModel`. Include it as a claim. On each request, middleware checks if the claim version matches the DB version. If not, reject. (Adds a DB query per request.)
- **Redis blacklist**: Store revoked token JTI (JWT ID) in Redis with TTL matching the token's remaining lifetime. Middleware checks Redis before accepting. (Fast but adds infrastructure.)

**Recommendation:** Start with short-lived access tokens (15 min) + refresh tokens. Only add a blacklist if you have a concrete requirement for instant revocation.

---

## 6. Summary of Changes

| # | Change | Effort | Impact |
|---|--------|--------|--------|
| 1 | Fix `DateTime.Now` → `DateTime.UtcNow` in TokenService | 1 line | Bug fix - tokens may expire at wrong time |
| 2 | Fix DevBypassAuthMiddleware claim types to use `CustomClaimTypes` | Small | Bug fix - dev mode broken |
| 3 | Move middleware order: `UseHttpsRedirection` before `UseAuthentication` | 1 line | Security |
| 4 | Add `ValidateLifetime = true` and `ClockSkew` to token validation | 2 lines | Security |
| 5 | Move `CustomClaimTypes` to its own file, rename to `StockyClaims` | Small | Code hygiene |
| 6 | Add Role/FirstName/Surname to `IUserContext` | Small | Feature |
| 7 | Register `TokenService` as Singleton | 1 line | Performance |
| 8 | Remove `ITokenService` injection from DevBypassAuthMiddleware | Small | Simplification |
| 9 | Implement refresh tokens | Medium | UX + Security |
| 10 | Move JWT from localStorage to HttpOnly cookie (frontend) | Medium | Security |

Items 1-8 are quick wins. Items 9-10 are a larger feature that can be planned separately.

---

## 7. Key Takeaway

**Your architecture is correct.** `AddJwtBearer` is the right middleware for JWT validation, and `TokenService` is the right abstraction for JWT generation. You don't need to replace either with custom middleware. The improvements above are about fixing bugs, following best practices, and strengthening security - not about changing the fundamental pattern.
