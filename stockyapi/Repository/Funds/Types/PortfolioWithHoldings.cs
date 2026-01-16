using stockymodels.models;

namespace stockyapi.Repository.Portfolio;

public record PortfolioWithHoldings(
    decimal CashBalance,
    decimal TotalValue,
    decimal InvestedAmount,
    IEnumerable<StockHoldingModel> Holdings);