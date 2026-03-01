using System.Security.Claims;
using stockyapi.Services;

namespace stockyapi.Middleware;

public class DevBypassAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DevBypassAuthMiddleware> _logger;
    private readonly ITokenService _tokenService;

    public DevBypassAuthMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<DevBypassAuthMiddleware> logger, ITokenService tokenService)
    {
        _next = next;
        _configuration = configuration;
        _logger = logger;
        this._tokenService = tokenService;
    }

    public async Task Invoke(HttpContext context)
    {
        if (_configuration.GetValue<string>("Env") == "Development" &&
            _configuration.GetValue<bool>("Authentication:BypassAuth"))
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "11111111-1111-1111-1111-111111111111"),
                new Claim(ClaimTypes.Name, "Test User"),
                new Claim(ClaimTypes.Email, "test@example.com"),
                new Claim(ClaimTypes.GivenName, "Test"),
                new Claim(ClaimTypes.Surname, "User"),
                new Claim(ClaimTypes.Role, "User")
            };

            var identity = new ClaimsIdentity(claims, "Development");
            var principal = new ClaimsPrincipal(identity);

            // Create token
            _logger.LogInformation("new login");
            var token = _tokenService.CreateDevelopmentToken(claims);

            context.User = principal;
            context.Request.Headers["Authorization"] = $"Bearer {token}";
        }

        await _next(context);
    }
}