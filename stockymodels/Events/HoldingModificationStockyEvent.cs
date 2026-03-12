namespace stockymodels.Events;

public record HoldingModificationStockyEvent : StockyEvent
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
}
