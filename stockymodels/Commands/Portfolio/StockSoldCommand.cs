namespace stockyapi.Application.Commands.Portfolio;

/// <summary>Command for StockSold event. Maps from SellTickerRequest. PortfolioId is set by the handler from context.</summary>
public record StockSoldCommand(
    string Symbol,
    decimal Quantity,
    decimal Price) : Command;
