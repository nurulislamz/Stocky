namespace stockymodels.Events;

public record UserPasswordChangedEvent : EventBase
{
    public required DateTimeOffset OccurredAt { get; init; }
    public required Guid RequestId { get; init; }
}
