using stockymodels.models;

namespace stockyapi.Application.Portfolio;
// TODO: Fix namespacing everywhere

public abstract record BaseOrderCommand(
    Guid PortfolioId,
    string Ticker,
    decimal Quantity,
    decimal Price,
    TransactionType TransactionType,
    decimal TotalAmount);

public record BuyOrderCommand(
    Guid PortfolioId,
    string Ticker,
    decimal Quantity,
    decimal Price) : BaseOrderCommand(PortfolioId, Ticker, Quantity, Price, TransactionType.Buy, Quantity * Price);

public record SellOrderCommand(
    Guid PortfolioId,
    string Ticker,
    decimal Quantity,
    decimal Price) : BaseOrderCommand(PortfolioId, Ticker, Quantity, Price, TransactionType.Sell, Quantity * Price);
