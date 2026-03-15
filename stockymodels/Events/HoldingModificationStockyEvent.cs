using stockymodels.models;

namespace stockymodels.Events;

public record HoldingModificationStockyEvent : PortfolioEvent
{
    public required Guid HoldingId { get; init; }
    public required string Symbol { get; init; }

    public required decimal QuantityDelta { get; init; }
    public required decimal SharesBefore { get; init; }
    public required decimal SharesAfter { get; init; }
    public required decimal AverageCostBefore { get; init; }
    public required decimal AverageCostAfter { get; init; }
    public required decimal PricePerShare { get; init; }
    public required decimal TotalValueOfTrade { get; init; }
    public required bool IsBuy { get; init; }

    public required decimal CashBalanceBefore { get; init; }
    public required decimal CashBalanceAfter { get; init; }
    public required decimal PortfolioTotalValueBefore { get; init; }
    public required decimal PortfolioTotalValueAfter { get; init; }
    public required decimal PortfolioInvestedAmountBefore { get; init; }
    public required decimal PortfolioInvestedAmountAfter { get; init; }

    public required DateTimeOffset OccurredAt { get; init; }
    public required Guid RequestId { get; init; }

    public override void Apply(PortfolioModel portfolio)
    {
        portfolio.CashBalance = CashBalanceAfter;
        portfolio.TotalValue = PortfolioTotalValueAfter;
        portfolio.InvestedAmount = PortfolioInvestedAmountAfter;
        portfolio.UpdatedAt = OccurredAt.UtcDateTime;
        var holding = portfolio.StockHoldings.FirstOrDefault(h => h.Id == HoldingId);
        if (holding is not null)
        {
            holding.Shares = SharesAfter;
            holding.AverageCost = AverageCostAfter;
            holding.UpdatedAt = OccurredAt.UtcDateTime;
        }
    }
}
