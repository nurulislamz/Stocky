using stockymodels.models;

namespace stockymodels.Events;

public record HoldingCreationStockyEvent : PortfolioEvent
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

    public override void Apply(PortfolioModel portfolio)
    {
        portfolio.StockHoldings.Add(new StockHoldingModel
        {
            Id = HoldingId,
            PortfolioId = portfolio.Id,
            Ticker = Symbol,
            Shares = Quantity,
            AverageCost = AverageCost,
            CreatedAt = OccurredAt.UtcDateTime,
            UpdatedAt = OccurredAt.UtcDateTime
        });
        portfolio.CashBalance = CashBalanceAfter;
        portfolio.TotalValue = PortfolioTotalValueAfter;
        portfolio.InvestedAmount = PortfolioInvestedAmountAfter;
        portfolio.UpdatedAt = OccurredAt.UtcDateTime;
    }
}
