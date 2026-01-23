using System.Net;

namespace stockyapi.Failures;
/// <summary>
/// Represents a failure that can be returned from an HTTP request.
/// </summary>
/// <param name="StatusCode">The HTTP status code associated with the failure.</param>
/// <param name="Title">A human-readable brief description of the failure.</param>
/// <param name="Detail">A human-readable description of the failure.</param>
public abstract record Failure(HttpStatusCode StatusCode, string Title, string Detail);

// CLIENT ERRORS - 4xx

/// <summary>
/// Placeholder for successes when no failure has occured. Used to avoid nulls
/// </summary>
public sealed record None(
    string Detail = "No failure has occured. If this is selected and a failure is returned, something is wrong.")
    : Failure(
        HttpStatusCode.NotImplemented,
        "No Failure",
        Detail);

/// <summary>
/// Represents a failure caused by an invalid client request 400
/// </summary>
public sealed record BadRequestFailure400(
    string Detail = "The request was invalid.")
    : Failure(
        HttpStatusCode.BadRequest,
        "Bad Request",
        Detail);

/// <summary>
/// Represents a failure where authentication is required or has failed 401
/// </summary>
public sealed record UnauthorizedFailure401(
    string Detail = "Authentication is required or has failed.")
    : Failure(
        HttpStatusCode.Unauthorized,
        "Unauthorized",
        Detail);

/// <summary>
/// Represents a failure where the authenticated user lacks permission 403
/// to access the requested resource.
/// </summary>
public sealed record ForbiddenFailure403(
    string Detail = "You do not have permission to access this resource.")
    : Failure(
        HttpStatusCode.Forbidden,
        "Forbidden",
        Detail);

/// <summary>
/// Represents a failure where the requested resource could not be found 404
/// </summary>
public sealed record NotFoundFailure404(
    string Detail = "The requested resource was not found.")
    : Failure(
        HttpStatusCode.NotFound,
        "Not Found",
        Detail);

/// <summary>
/// Represents a failure caused by a conflict with the current state 409
/// of the target resource.
/// </summary>
public sealed record ConflictFailure409(
    string Detail = "The request could not be completed due to a conflict.")
    : Failure(
        HttpStatusCode.Conflict,
        "Conflict",
        Detail);

/// <summary>
/// Represents a failure caused by validation errors in the request 422
/// </summary>
public sealed record ValidationFailure422(
    string Detail = "One or more validation errors occurred.")
    : Failure(
        HttpStatusCode.UnprocessableEntity,
        "Validation Failed",
        Detail);

// SERVER ERRORS 5xx

/// <summary>
/// Represents an unexpected server-side failure 500
/// </summary>
public sealed record InternalServerFailure500(
    string Detail = "An unexpected server error occurred.")
    : Failure(
        HttpStatusCode.InternalServerError,
        "Internal Server Error",
        Detail);

/// <summary>
/// Represents a failure where the service is temporarily unavailable 503
/// </summary>
public sealed record ServiceUnavailableFailure503(
    string Detail = "The service is currently unavailable.")
    : Failure(
        HttpStatusCode.ServiceUnavailable,
        "Service Unavailable",
        Detail);

/// <summary>
/// Represents a failure caused by a timeout when communicating 504
/// with an upstream service.
/// </summary>
public sealed record GatewayTimeoutFailure504(
    string Detail = "The upstream service did not respond in time.")
    : Failure(
        HttpStatusCode.GatewayTimeout,
        "Gateway Timeout",
        Detail);