namespace stockymodels.Events;

public record UserPasswordChangedStockyEvent : StockyEventPayload
{
    public required DateTimeOffset OccurredAt { get; init; }
    public required Guid RequestId { get; init; }
}
