using Microsoft.Extensions.Logging;

namespace stockyapi.Middleware;

/// <summary>
/// Centralized event IDs for structured logging across the application.
/// Use these when logging so sinks can filter or alert by event ID.
/// </summary>
public static class LoggingEventIds
{
    /// <summary>Portfolio not found for the given user (PortfolioRepository, FundsRepository).</summary>
    public static readonly EventId PortfolioNotFound = new(123, "Portfolio.NotFound");

    /// <summary>Funds: portfolio not found for the given user.</summary>
    public static readonly EventId FundsPortfolioNotFound = new(201, "Funds.PortfolioNotFound");

    /// <summary>Funds: deposit would violate balance invariant (e.g. decrease cash).</summary>
    public static readonly EventId FundsDepositInvariantViolation = new(202, "Funds.DepositInvariantViolation");

    /// <summary>Funds: withdraw would violate balance invariant (e.g. increase cash).</summary>
    public static readonly EventId FundsWithdrawInvariantViolation = new(203, "Funds.WithdrawInvariantViolation");

    /// <summary>User not found by id or email (UserRepository).</summary>
    public static readonly EventId UserNotFound = new(301, "User.NotFound");

    /// <summary>User or portfolio persistence failure (e.g. SaveChangesAsync).</summary>
    public static readonly EventId RepositorySaveFailure = new(310, "Repository.SaveFailure");

    /// <summary>Authenticated user is missing user id claim (UserContext).</summary>
    public static readonly EventId AuthMissingUserIdClaim = new(401, "Auth.MissingUserIdClaim");

    /// <summary>Authenticated user has malformed user id claim - present but not a valid GUID (UserContext).</summary>
    public static readonly EventId AuthMalformedUserIdClaim = new(4011, "Auth.MalformedUserIdClaim");

    /// <summary>Authenticated user is missing email claim (UserContext).</summary>
    public static readonly EventId AuthMissingEmailClaim = new(402, "Auth.MissingEmailClaim");

    /// <summary>Authenticated user has malformed email claim - present but not a valid email (UserContext).</summary>
    public static readonly EventId AuthMalformedEmailClaim = new(4021, "Auth.MalformedEmailClaim");

    /// <summary>Authenticated user is missing a profile claim - FirstName, Surname, or Role (UserContext).</summary>
    public static readonly EventId AuthMissingProfileClaim = new(403, "Auth.MissingProfileClaim");
}
