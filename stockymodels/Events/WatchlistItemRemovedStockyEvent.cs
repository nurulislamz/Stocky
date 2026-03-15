namespace stockymodels.Events;

public record WatchlistItemRemovedStockyEvent : StockyEventPayload
{
    public required string Symbol { get; init; }
    public required DateTimeOffset OccurredAt { get; init; }
    public required Guid RequestId { get; init; }
}
