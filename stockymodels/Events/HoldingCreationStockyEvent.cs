namespace stockymodels.Events;

public record HoldingCreationStockyEventPayload : StockyEventPayload
{
    public required Guid HoldingId { get; init; }
    public required string Symbol { get; init; }
    public required decimal Quantity { get; init; }
    public required decimal PricePerShare { get; init; }
    public required decimal TotalCost { get; init; }
    public required decimal AverageCost { get; init; }

    public required decimal CashBalanceBefore { get; init; }
    public required decimal CashBalanceAfter { get; init; }
    public required decimal PortfolioTotalValueBefore { get; init; }
    public required decimal PortfolioTotalValueAfter { get; init; }
    public required decimal PortfolioInvestedAmountBefore { get; init; }
    public required decimal PortfolioInvestedAmountAfter { get; init; }

    public required DateTimeOffset OccurredAt { get; init; }
    public required Guid RequestId { get; init; }
}
