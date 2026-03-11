namespace stockymodels.Events;

public record WatchlistItemAddedEvent : Event
{
    public required string Symbol { get; init; }
    public required DateTimeOffset OccurredAt { get; init; }
    public required Guid RequestId { get; init; }
}
