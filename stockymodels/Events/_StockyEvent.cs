namespace stockymodels.Events;

public record StockyEvent
{
    public required Guid AggregateId { get; init; }
}
