namespace stockymodels.Events;

public record UserDeletedStockyEvent : StockyEvent
{
    public string? LastKnownEmail { get; init; }
    public string? LastKnownFirstName { get; init; }
    public string? LastKnownSurname { get; init; }
    public required DateTimeOffset OccurredAt { get; init; }
    public required Guid RequestId { get; init; }
}
