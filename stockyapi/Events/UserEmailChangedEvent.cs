namespace stockymodels.Events;

public record UserEmailChangedEvent : Event
{
    public required string EmailBefore { get; init; }
    public required string EmailAfter { get; init; }
    public required DateTimeOffset OccurredAt { get; init; }
    public required Guid RequestId { get; init; }
}
