using stockymodels.models;

namespace stockyapi.Repository.Funds.Types;

public record PortfolioWithHoldings(
    decimal CashBalance,
    decimal TotalValue,
    decimal InvestedAmount,
    IEnumerable<StockHoldingModel> Holdings);