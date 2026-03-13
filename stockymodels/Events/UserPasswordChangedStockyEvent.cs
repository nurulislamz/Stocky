namespace stockymodels.Events;

public record UserPasswordChangedStockyEvent : StockyEvent
{
    public required DateTimeOffset OccurredAt { get; init; }
    public required Guid RequestId { get; init; }
}
