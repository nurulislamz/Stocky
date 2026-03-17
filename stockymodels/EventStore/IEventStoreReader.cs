using stockymodels.Events;

namespace stockymodels.EventStore;

/// <summary>
/// Read operations for the event store. Implemented by <see cref="PostgresEventStoreReader"/> (standalone)
/// and used internally by <see cref="PostgresEventStore"/> (same connection as writes via <see cref="IDbConnectionFactory"/>).
/// </summary>
public interface IEventStoreReader
{
	Task<T[]?> QueryAllAggregatedEventsAsync<T>(int aggregateType, Guid aggregateId, CancellationToken ct = default) where T : StockyEventPayload;
	Task<StockyEventPayload?> QuerySingleEventAsync(int aggregateType, Guid aggregateId, int aggregateSequenceId, CancellationToken ct = default);
}
