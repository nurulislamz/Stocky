namespace stockymodels.Events;

public record UserPasswordChangedEvent : Event
{
    public required DateTimeOffset OccurredAt { get; init; }
    public required Guid RequestId { get; init; }
}
