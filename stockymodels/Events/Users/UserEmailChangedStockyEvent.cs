using stockymodels.models;

namespace stockymodels.Events.Users;

public record UserEmailChangedStockyEvent : UserEvent
{
    public required string EmailBefore { get; init; }
    public required string EmailAfter { get; init; }
    public required DateTimeOffset OccurredAt { get; init; }
    public required Guid RequestId { get; init; }

    public override void Apply(UserAggregate user)
    {
        user.Email = EmailAfter;
    }
}
