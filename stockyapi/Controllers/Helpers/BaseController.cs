using System.Net;
using Microsoft.AspNetCore.Mvc;
using stockyapi.Middleware;

namespace stockyapi.Controllers.Helpers;

public abstract class BaseController : ControllerBase
{
    protected ActionResult<T> ProcessSuccess<T>(HttpStatusCode statusCode, T value)
    {
        return StatusCode((int)statusCode, value);
    }
    
    // TODO: Complete the problem details
    protected ActionResult ProcessFailure(Failure failure)
    {
        {
            //var type  create a list of problems which links to the type of problem faced. 
            var problemDetails = ProblemDetailsFactory.CreateProblemDetails(
                HttpContext,
                statusCode: (int)failure.StatusCode,
                title: failure.Title,
                detail: failure.Detail,
                instance: HttpContext.Request.Path.Value
                // TODO: implement type endpoint and types here
            );

            return StatusCode(problemDetails.Status ?? 500, problemDetails);
        }
    }
    
    private static readonly IReadOnlyDictionary<Type, string> ProblemTypes =
        new Dictionary<Type, string>
        {
            { typeof(NotFoundFailure404), "https://api.yourcompany.com/problems/not-found" },
            { typeof(ConflictFailure409), "https://api.yourcompany.com/problems/conflict" },
            { typeof(ValidationFailure422), "https://api.yourcompany.com/problems/validation-failed" }
        };
}
    