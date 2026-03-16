using stockymodels.models;

namespace stockymodels.Events.Users;

public record UserDeletedStockyEvent : UserEvent
{
    public string? LastKnownEmail { get; init; }
    public string? LastKnownFirstName { get; init; }
    public string? LastKnownSurname { get; init; }
    public required DateTimeOffset OccurredAt { get; init; }
    public required Guid RequestId { get; init; }

    public override void Apply(UserAggregate user)
    {
        user.IsActive = false;
    }
}
