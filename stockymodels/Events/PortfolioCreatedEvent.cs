namespace stockymodels.Events;

public record PortfolioCreatedEvent : EventBase
{
    public required decimal CashBalance { get; init; }
    public required decimal TotalValue { get; init; }
    public required decimal InvestedAmount { get; init; }
    public required DateTimeOffset OccurredAt { get; init; }
    public required Guid RequestId { get; init; }
}
