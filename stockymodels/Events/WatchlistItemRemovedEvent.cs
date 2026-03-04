namespace stockymodels.Events;

public record WatchlistItemRemovedEvent : EventBase
{
    public required string Symbol { get; init; }
    public required DateTimeOffset OccurredAt { get; init; }
    public required Guid RequestId { get; init; }
}
