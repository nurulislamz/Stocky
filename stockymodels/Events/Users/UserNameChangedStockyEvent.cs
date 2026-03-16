using stockymodels.models;

namespace stockymodels.Events.Users;

public record UserNameChangedStockyEvent : UserEvent
{
    public required string FirstNameBefore { get; init; }
    public required string FirstNameAfter { get; init; }
    public required string SurnameBefore { get; init; }
    public required string SurnameAfter { get; init; }
    public required DateTimeOffset OccurredAt { get; init; }
    public required Guid RequestId { get; init; }

    public override void Apply(UserAggregate user)
    {
        user.FirstName = FirstNameAfter;
        user.Surname = SurnameAfter;
    }
}
