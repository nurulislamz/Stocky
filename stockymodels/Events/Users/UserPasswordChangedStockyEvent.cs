using stockymodels.models;

namespace stockymodels.Events.Users;

public record UserPasswordChangedStockyEvent : UserEvent
{
    public required DateTimeOffset OccurredAt { get; init; }
    public required Guid RequestId { get; init; }

    public override void Apply(UserAggregate user)
    {
        // Password is updated elsewhere (e.g. hash); aggregate may track "password changed at" only if needed.
    }
}
