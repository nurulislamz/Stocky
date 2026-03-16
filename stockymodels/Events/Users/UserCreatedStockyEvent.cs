using stockymodels.models;

namespace stockymodels.Events.Users;

public record UserCreatedStockyEvent : UserEvent
{
    public required string FirstName { get; init; }
    public required string Surname { get; init; }
    public required string Email { get; init; }
    public required DateTimeOffset OccurredAt { get; init; }
    public required Guid RequestId { get; init; }

    public override void Apply(UserAggregate user) { } // Already applied via Create
}
