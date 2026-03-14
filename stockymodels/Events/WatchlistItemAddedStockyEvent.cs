namespace stockymodels.Events;

public record WatchlistItemAddedStockyEventPayload : StockyEventPayload
{
    public required string Symbol { get; init; }
    public required DateTimeOffset OccurredAt { get; init; }
    public required Guid RequestId { get; init; }
}
