using System.Data;
using System.Text.Json;
using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;
using stockymodels.Events;
using stockymodels.models;
using stockymodels.Sql;

namespace stockymodels.EventStore;

/// <summary>
/// Read-only access to the event store. Uses <see cref="IDbConnectionFactory"/> so the same
/// scoped connection is shared with the write side when both are resolved in the same scope.
/// </summary>
public class PostgresEventStoreReader : IEventStoreReader
{
	private readonly IDbConnectionFactory _factory;
	private readonly int _commandTimeout;
	private readonly ILogger<PostgresEventStore> _logger;

	public PostgresEventStoreReader(IDbConnectionFactory factory, ILogger<PostgresEventStore> logger, int commandTimeout = 30)
	{
		_factory = factory ?? throw new ArgumentNullException(nameof(factory));
		_logger = logger;
		_commandTimeout = commandTimeout;
	}

	public async Task<T[]?> QueryAllAggregatedEventsAsync<T>(int aggregateType, Guid aggregateId, IDbConnection? connection = null, CancellationToken ct = default) where T : StockyEventPayload
	{
		connection ??= await _factory.GetConnectionAsync(ct);

		const string sql = """
		                   SELECT e."EventType", e."EventPayloadJson" FROM stockydb."Events" e
		                   WHERE e."AggregateType" = @aggregateType
		                   AND e."AggregateId" = @aggregateId
		                   ORDER BY e."AggregateSequenceId"
		                   """;
		var rows = await connection.QueryAsync<(string, string)>(new CommandDefinition(sql, new { aggregateType, aggregateId }, commandTimeout: _commandTimeout, cancellationToken: ct));
		var list = new List<T>();
		foreach (var (eventType, json) in rows)
		{
			if (string.IsNullOrEmpty(json))
			{
				_logger.LogWarning("Empty payload, this shouldn't happen");
				throw new InvalidOperationException("Empty payload, this shouldn't happen");
			}

			try
			{
				var type = Type.GetType(eventType);
				if (type == null || !typeof(StockyEventPayload).IsAssignableFrom(type))
				{
					throw new InvalidOperationException($"Invalid event payload type '{eventType}'");
				}

				if (JsonSerializer.Deserialize(json, type) is T payload) list.Add(payload);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to deserialize payload for event type {EventType}", eventType);
				throw;
			}
		}
		return list.Count > 0 ? list.ToArray() : null;
	}

	public async Task<StockyEventPayload?> QuerySingleEventAsync(int aggregateType, Guid aggregateId, int aggregateSequenceId, IDbConnection? connection = null, CancellationToken ct = default)
	{
		connection ??= await _factory.GetConnectionAsync(ct);

		const string sql = """
		                   SELECT e."EventPayloadJson" FROM stockydb."Events" e
		                   WHERE e."AggregateType" = @aggregateType
		                   AND e."AggregateId" = @aggregateId
		                   AND e."AggregateSequenceId" = @aggregateSequenceId
		                   """;

		var json = await connection.QuerySingleOrDefaultAsync<string>(new CommandDefinition(sql,
			new { aggregateType, aggregateId, aggregateSequenceId },
			commandTimeout: _commandTimeout,
			cancellationToken: ct));
		return string.IsNullOrEmpty(json) ? null : JsonSerializer.Deserialize<StockyEventPayload>(json);
	}

	public async Task<int> GetStreamVersionAsync(string aggregateType, Guid aggregateId, IDbConnection? connection = null, CancellationToken ct = default)
	{
		connection ??= await _factory.GetConnectionAsync(ct);

		return await connection.QuerySingleAsync<int>(new CommandDefinition(
			StockySqlFunctions.GetAggregateVersion,
			new { p_aggregate_type = aggregateType, p_aggregate_id = aggregateId },
			commandType: CommandType.StoredProcedure,
			commandTimeout: _commandTimeout,
			cancellationToken: ct));
	}
}
