namespace stockymodels.Events;

public record UserCreatedStockyEvent : StockyEvent
{
    public required string FirstName { get; init; }
    public required string Surname { get; init; }
    public required string Email { get; init; }
    public required DateTimeOffset OccurredAt { get; init; }
    public required Guid RequestId { get; init; }
}
