using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using stockyapi.Middleware;
using stockyapi.Services;

namespace stockyapi.Controllers;

public sealed class HttpUserContext : IUserContext
{
    private readonly IHttpContextAccessor _accessor;

    public HttpUserContext(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public bool IsAuthenticated =>
        _accessor.HttpContext?.User?.Identity?.IsAuthenticated == true;

    public Guid UserId
    {
        get
        {
            var user = _accessor.HttpContext?.User;

            // If you put [Authorize] on endpoints, this should always be present.
            var raw =
                user?.FindFirstValue(CustomClaimTypes.UserId);

            if (!Guid.TryParse(raw, out var id))
                throw new InvalidOperationException("Authenticated user is missing a valid user id claim.");

            return id;
        }
    }
    
    public string Email
    {
        get
        {
            var user = _accessor.HttpContext?.User;

            var email = user?.FindFirstValue(CustomClaimTypes.Email);
            
            // TODO : add validation that the email is valid
            if (email == null)
            {
                throw new InvalidOperationException("Authenticated user is missing a valid email claim.");
            }
            
            return email;
        }
    }
}