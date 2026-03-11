namespace stockyapi.Application.Commands.Portfolio;

/// <summary>Command for DeleteHolding event. Provide either HoldingIds or TickerSymbols; handler validates one is non-empty.</summary>
public record DeleteHoldingCommand(
    Guid[]? HoldingIds,
    string[]? TickerSymbols) : Command;
