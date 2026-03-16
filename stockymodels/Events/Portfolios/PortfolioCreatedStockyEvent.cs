using stockymodels.models;

namespace stockymodels.Events.Portfolios;

public record PortfolioCreatedStockyEvent : PortfolioEvent
{
    public required decimal CashBalance { get; init; }
    public required decimal TotalValue { get; init; }
    public required decimal InvestedAmount { get; init; }
    public required DateTimeOffset OccurredAt { get; init; }
    public required Guid RequestId { get; init; }

    public override void Apply(PortfolioAggregate portfolio) { } // Already applied via Create
}
