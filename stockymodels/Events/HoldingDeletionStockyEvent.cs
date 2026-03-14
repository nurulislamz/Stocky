namespace stockymodels.Events;

public record HoldingDeletionStockyEventPayload : StockyEventPayload
{
    public required Guid HoldingId { get; init; }
    public required string Symbol { get; init; }

    public required decimal SharesRemoved { get; init; }
    public required decimal AverageCostAtRemoval { get; init; }
    public decimal? PricePerShareAtSale { get; init; }
    public decimal? ProceedsFromSale { get; init; }
    public required decimal CostBasisRemoved { get; init; }
    public required bool WasSold { get; init; }

    public required decimal CashBalanceBefore { get; init; }
    public required decimal CashBalanceAfter { get; init; }
    public required decimal PortfolioTotalValueBefore { get; init; }
    public required decimal PortfolioTotalValueAfter { get; init; }
    public required decimal PortfolioInvestedAmountBefore { get; init; }
    public required decimal PortfolioInvestedAmountAfter { get; init; }

    public required DateTimeOffset OccurredAt { get; init; }
    public required Guid RequestId { get; init; }
}
