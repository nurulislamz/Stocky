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
}
