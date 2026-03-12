namespace stockymodels.Events;

public record UserNameChangedStockyEvent : StockyEvent
{
    public required string FirstNameBefore { get; init; }
    public required string FirstNameAfter { get; init; }
    public required string SurnameBefore { get; init; }
    public required string SurnameAfter { get; init; }
    public required DateTimeOffset OccurredAt { get; init; }
    public required Guid RequestId { get; init; }
}
