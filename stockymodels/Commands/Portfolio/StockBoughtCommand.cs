namespace stockyapi.Application.Commands.Portfolio;

/// <summary>Command for StockBought event. Maps from BuyTickerRequest. PortfolioId is set by the handler from context.</summary>
public record StockBoughtCommand(
    string Symbol,
    decimal Quantity,
    decimal Price);
