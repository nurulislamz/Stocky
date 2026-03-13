namespace stockyapi.Application.Commands.Portfolio;

/// <summary>Command for PortfolioCreate event. Used when creating a new portfolio (e.g. on user registration).</summary>
public record PortfolioCreationCommand(
    Guid PortfolioId,
    Guid UserId,
    decimal CashBalance = 0m,
    decimal TotalValue = 0m,
    decimal InvestedAmount = 0m) : Command;
