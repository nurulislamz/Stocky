namespace stockymodels.Events;

public record UserCreatedEvent : Event
{
    public required string FirstName { get; init; }
    public required string Surname { get; init; }
    public required string Email { get; init; }
    public required DateTimeOffset OccurredAt { get; init; }
    public required Guid RequestId { get; init; }
}
