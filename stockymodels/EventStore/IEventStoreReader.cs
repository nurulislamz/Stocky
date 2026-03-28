using System.Data;
using stockymodels.Events;

namespace stockymodels.EventStore;

/// <summary>
/// Read operations for the event store. Implemented by <see cref="PostgresEventStoreReader"/> (standalone)
/// and used internally by <see cref="EventStore"/> (writer passes its connection for transactional consistency).
/// </summary>
public interface IEventStoreReader
{
	Task<T[]?> QueryAllAggregatedEventsAsync<T>(int aggregateType, Guid aggregateId, IDbConnection? connection = null, CancellationToken ct = default) where T : StockyEventPayload;
	Task<StockyEventPayload?> QuerySingleEventAsync(int aggregateType, Guid aggregateId, int aggregateSequenceId, IDbConnection? connection = null, CancellationToken ct = default);
	Task<int> GetStreamVersionAsync(string aggregateType, Guid aggregateId, IDbConnection? connection = null, CancellationToken ct = default);
}
