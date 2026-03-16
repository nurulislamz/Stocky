using stockymodels.Events;
using stockymodels.models;

namespace stockymodels.Events.Users;

/// <summary>
/// Base for events on the user stream (AggregateType UserId, AggregateId = UserId).
/// Applies to UserAggregate only.
/// </summary>
public abstract record UserEvent : StockyEventPayload, IEventApplier<UserAggregate>
{
	public abstract void Apply(UserAggregate user);
}
