namespace stockymodels.Events;

public record UserEmailChangedEvent : EventBase
{
    public required string EmailBefore { get; init; }
    public required string EmailAfter { get; init; }
    public required DateTimeOffset OccurredAt { get; init; }
    public required Guid RequestId { get; init; }
}
