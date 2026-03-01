using System.Net.Mail;
using System.Security.Claims;

namespace stockyapi.Middleware;

public sealed class HttpUserContext : IUserContext
{
    private readonly IHttpContextAccessor _accessor;
    private readonly ILogger<HttpUserContext> _logger;

    public HttpUserContext(IHttpContextAccessor accessor, ILogger<HttpUserContext> logger)
    {
        _accessor = accessor;
        _logger = logger;
    }

    public bool IsAuthenticated =>
        _accessor.HttpContext?.User.Identity?.IsAuthenticated == true;

    public Guid UserId
    {
        get
        {
            var user = _accessor.HttpContext?.User;
            var raw = user?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(raw))
            {
                _logger.LogError(LoggingEventIds.AuthMissingUserIdClaim,
                    "Authenticated user is missing user id claim.");
                throw new InvalidOperationException("Authenticated user is missing a valid user id claim.");
            }

            if (!Guid.TryParse(raw, out var id))
            {
                _logger.LogWarning(LoggingEventIds.AuthMalformedUserIdClaim,
                    "User id claim is present but malformed (not a valid GUID). Raw value: {RawUserId}", raw);
                throw new InvalidOperationException("Authenticated user is missing a valid user id claim.");
            }

            return id;
        }
    }
    
    public string Email
    {
        get
        {
            var user = _accessor.HttpContext?.User;
            var email = user?.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(email))
            {
                _logger.LogError(LoggingEventIds.AuthMissingEmailClaim,
                    "Authenticated user is missing email claim. UserId: {UserId}",
                    user?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "(unknown)");
                throw new InvalidOperationException("Authenticated user is missing a valid email claim.");
            }

            if (!MailAddress.TryCreate(email, out _))
            {
                _logger.LogWarning(LoggingEventIds.AuthMalformedEmailClaim,
                    "Email claim is present but malformed (not a valid email). UserId: {UserId}, Raw email: {RawEmail}",
                    user?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "(unknown)", email);
                throw new InvalidOperationException("Authenticated user is missing a valid email claim.");
            }

            return email;
        }
    }

    public string FirstName
    {
        get
        {
            var value = _accessor.HttpContext?.User?.FindFirstValue(ClaimTypes.GivenName);
            if (string.IsNullOrEmpty(value))
            {
                _logger.LogError(LoggingEventIds.AuthMissingProfileClaim,
                    "Authenticated user is missing first name claim. UserId: {UserId}",
                    _accessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "(unknown)");
                throw new InvalidOperationException("Authenticated user is missing a valid first name claim.");
            }
            return value;
        }
    }

    public string Surname
    {
        get
        {
            var value = _accessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Surname);
            if (string.IsNullOrEmpty(value))
            {
                _logger.LogError(LoggingEventIds.AuthMissingProfileClaim,
                    "Authenticated user is missing surname claim. UserId: {UserId}",
                    _accessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "(unknown)");
                throw new InvalidOperationException("Authenticated user is missing a valid surname claim.");
            }
            return value;
        }
    }

    public string Role
    {
        get
        {
            var value = _accessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);
            if (string.IsNullOrEmpty(value))
            {
                _logger.LogError(LoggingEventIds.AuthMissingProfileClaim,
                    "Authenticated user is missing role claim. UserId: {UserId}",
                    _accessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "(unknown)");
                throw new InvalidOperationException("Authenticated user is missing a valid role claim.");
            }
            return value;
        }
    }
}