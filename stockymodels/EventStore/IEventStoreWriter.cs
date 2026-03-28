using stockymodels.Events;
using stockymodels.models;

namespace stockymodels.EventStore;

/// <summary>
/// Write operations for the event store.
/// Implemented by <see cref="EventStore"/>.
/// </summary>
public interface IEventStoreWriter
{
	/// <summary>Appends a command and one event in a single transaction.</summary>
	Task<CommandAndEventResult> RegisterEventAsync(
		Command command,
		StockyEvent @event,
		Guid userId,
		Guid traceId,
		CancellationToken ct = default);

	/// <summary>Appends a command and multiple event in a single transaction.</summary>
	Task<CommandAndEventResult[]> RegisterMultipleEventsAsync(
		Command command,
		StockyEvent[] events,
		Guid userId,
		Guid traceId,
		CancellationToken ct = default);
}
