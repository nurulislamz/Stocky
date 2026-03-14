namespace stockymodels.Events;

public record UserPasswordChangedStockyEventPayload : StockyEventPayload
{
    public required DateTimeOffset OccurredAt { get; init; }
    public required Guid RequestId { get; init; }
}
