using System.Data;
using System.Text.Json;
using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;
using stockymodels.Events;
using stockymodels.models;
using stockymodels.Models.Enums;
using stockymodels.Sql;

namespace stockymodels.EventStore;

/// <summary>
/// Dapper-based append-only event store. Inserts commands and events per 001CreateEventStore.sql.
/// Use for high-throughput event appends; read models are updated separately (two-prong flow).
/// </summary>
public class PostgresEventStore : IDisposable, IAsyncDisposable
{
	private readonly NpgsqlConnection _connection;
	private readonly int _commandTimeout;
	private readonly int _retryAttempts;
	private readonly ConcurrencyLevel _concurrencyLevel;
	private readonly ILogger<PostgresEventStore> _logger;


	public PostgresEventStore(string connectionString, ILogger<PostgresEventStore> logger, int retryAttempts = 5, int commandTimeout = 30, ConcurrencyLevel? concurrencyLevel = null)
	{
		_logger = logger;
		_commandTimeout = commandTimeout;
		_retryAttempts = retryAttempts;
		_concurrencyLevel = concurrencyLevel ?? ConcurrencyLevel.OptimisticConcurrency;
		_connection = new NpgsqlConnection(connectionString);
		_connection.Open();
		var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);

		// Map the Command composite type
		dataSourceBuilder.MapComposite<CommandAggregate>("stockydb.command_insert");

		// Map the Event composite type
		dataSourceBuilder.MapComposite<InsertEventAggregate>("stockydb.event_insert");

		// Map the Event composite type with expected next sequence
		dataSourceBuilder.MapComposite<InsertEventAggregateWithExpectedNextSequence>("stockydb.event_insert_with_seq_id");

		// Map the CommandAndEventResult composite type
		dataSourceBuilder.MapComposite<CommandAndEventResult>("stockydb.command_and_event_result");

		using var dataSource = dataSourceBuilder.Build();
	}

	/// <summary>Appends a command and one event in a single transaction.</summary>
	public async Task<CommandAndEventResult> RegisterEventAsync(
		Command command,
		StockyEvent @event,
		Guid userId,
		Guid traceId,
		CancellationToken ct = default)
	{
		var commandId = Guid.CreateVersion7();
		var eventId = Guid.CreateVersion7();
		var now = DateTimeOffset.UtcNow;
		var ttEnd = DateTimeOffset.MaxValue;

		var insertCommand = CreateCommandAggregate(command, commandId, userId, traceId, now, ttEnd);
		var insertEvent = CreateInsertEventAggregate(@event, eventId, commandId, userId, traceId, now, ttEnd);

		return _concurrencyLevel switch
		{
			ConcurrencyLevel.OptimisticConcurrency => await WithOptimisticRetry(insertCommand, insertEvent, ct),
			ConcurrencyLevel.LockOnAggregate => await WithAdvisoryLock(insertCommand, insertEvent, ct),
			ConcurrencyLevel.RowVersion => await WithRowVersioning(insertCommand, insertEvent, ct),
			_ => throw new ArgumentOutOfRangeException()
		};
	}


	private async Task<CommandAndEventResult> WithOptimisticRetry(CommandAggregate insertCommand, InsertEventAggregate insertEvent, CancellationToken ct)
	{
		for (int attempt = 0; attempt < _retryAttempts; attempt++)
		{
			try
			{
				var result = await _connection.QuerySingleAsync<CommandAndEventResult>(StockySqlFunctions.InsertCommandAndEvent,
					new { p_command = insertCommand, p_event = insertEvent }, commandType: System.Data.CommandType.StoredProcedure);
				_logger.LogInformation("Command and event inserted successfully. Result: {Result}", result);
				return result;
			}
			catch (PostgresException ex) when (ex.SqlState == "23505")
			{
				_logger.LogWarning("Sequence conflict, attempt {Attempt}/{Max}",
					attempt + 1, _retryAttempts);
			}
		}

		throw new InvalidOperationException(
			$"Unable to register event after {_retryAttempts} retries");
	}

	private async Task<CommandAndEventResult> WithAdvisoryLock(
		CommandAggregate insertCommand, InsertEventAggregate insertEvent, CancellationToken ct)
	{
		var result = await _connection.QuerySingleAsync<CommandAndEventResult>(
			StockySqlFunctions.InsertCommandAndEventWithAdvisoryLock,
			new { p_command = insertCommand, p_event = insertEvent },
			commandType: System.Data.CommandType.StoredProcedure);

		_logger.LogInformation("Command and event inserted successfully. Result: {Result}", result);
		return result;
	}

	private async Task<CommandAndEventResult> WithRowVersioning(CommandAggregate insertCommand, InsertEventAggregate insertEvent, CancellationToken ct)
	{
		var currentVersion = await _connection.QuerySingleAsync<int>(
			StockySqlFunctions.GetAggregateVersion,
			new { p_aggregate_type = insertEvent.AggregateType, p_aggregate_id = insertEvent.AggregateId },
			commandType: System.Data.CommandType.StoredProcedure);

		var insertEventWithSeqId = new InsertEventAggregateWithExpectedNextSequence
		{
			Event = insertEvent,
			ExpectedNextSequence = currentVersion + 1,
		};

		var result = await _connection.QuerySingleAsync<CommandAndEventResult>(
			StockySqlFunctions.InsertCommandAndEventWithRowVersioning,
			new { p_command = insertCommand, p_event = insertEventWithSeqId },
			commandType: System.Data.CommandType.StoredProcedure);

		_logger.LogInformation("Command and event inserted successfully. Result: {Result}", result);
		return result;
	}

	/// <summary>Appends a command and multiple event in a single transaction.</summary>
	public async Task<CommandAndEventResult[]> RegisterMultipleEventsAsync(
		Command command,
		StockyEvent[] events,
		Guid userId,
		Guid traceId,
		CancellationToken ct = default)
	{
		var commandId = Guid.CreateVersion7();
		var eventId = Guid.CreateVersion7();
		var now = DateTimeOffset.UtcNow;
		var ttEnd = DateTimeOffset.MaxValue;

		var commandModel = CreateCommandAggregate(command, commandId, userId, traceId, now, ttEnd);
		var eventModels = events.Select(e => CreateInsertEventAggregate(e, eventId, commandId, userId, traceId, now, ttEnd)).ToArray();

		return _concurrencyLevel switch
		{
			ConcurrencyLevel.OptimisticConcurrency => await WithOptimisticRetryMulti(commandModel, eventModels, ct),
			ConcurrencyLevel.LockOnAggregate => await WithAdvisoryLockMulti(commandModel, eventModels, ct),
			ConcurrencyLevel.RowVersion => await WithRowVersioningMulti(commandModel, eventModels, ct),
			_ => throw new ArgumentOutOfRangeException()
		};
	}

	private async Task<CommandAndEventResult[]> WithOptimisticRetryMulti(
		CommandAggregate commandModel, InsertEventAggregate[] eventModels, CancellationToken ct)
	{
		for (int attempt = 0; attempt < _retryAttempts; attempt++)
		{
			try
			{
				var result = await _connection.QuerySingleAsync<CommandAndEventResult[]>(
					StockySqlFunctions.InsertCommandAndMultipleEvents,
					new { p_command = commandModel, p_events = eventModels },
					commandType: System.Data.CommandType.StoredProcedure);
				return result;
			}
			catch (PostgresException ex) when (ex.SqlState == "23505")
			{
				_logger.LogWarning("Sequence conflict, attempt {Attempt}/{Max}",
					attempt + 1, _retryAttempts);
			}
		}

		throw new InvalidOperationException(
			$"Unable to register event after {_retryAttempts} retries");
	}

	private async Task<CommandAndEventResult[]> WithAdvisoryLockMulti(
		CommandAggregate commandModel, InsertEventAggregate[] eventModels, CancellationToken ct)
	{
		var result = await _connection.QuerySingleAsync<CommandAndEventResult[]>(
			StockySqlFunctions.InsertCommandAndMultipleEventsWithAdvisoryLocks,
			new { p_command = commandModel, p_events = eventModels },
			commandType: System.Data.CommandType.StoredProcedure);
		return result;
	}


	private async Task<CommandAndEventResult[]> WithRowVersioningMulti(CommandAggregate commandModel, InsertEventAggregate[] eventModels, CancellationToken ct)
	{
		var nextByAggregate = new Dictionary<(string AggregateType, Guid AggregateId), int>();
		var appends = new InsertEventAggregateWithExpectedNextSequence[eventModels.Length];

		for (int i = 0; i < eventModels.Length; i++)
		{
			var eventModel = eventModels[i];
			var key = (eventModel.AggregateType, eventModel.AggregateId);

			if (!nextByAggregate.TryGetValue(key, out var previousExpected))
			{
				var currentVersion = await _connection.QuerySingleAsync<int>(
					StockySqlFunctions.GetAggregateVersion,
					new { p_aggregate_type = eventModel.AggregateType, p_aggregate_id = eventModel.AggregateId },
					commandType: CommandType.StoredProcedure);

				previousExpected = currentVersion;
			}

			var expectedNext = previousExpected + 1;
			nextByAggregate[key] = expectedNext;

			appends[i] = new InsertEventAggregateWithExpectedNextSequence
			{
				Event = eventModel,
				ExpectedNextSequence = expectedNext,
			};
		}

		var result = await _connection.QuerySingleAsync<CommandAndEventResult[]>(
			StockySqlFunctions.InsertCommandAndMultipleEventsWithRowVersioning,
			new { p_command = commandModel, p_appends = appends },
			commandType: CommandType.StoredProcedure);

		return result;
	}

	private async Task<int> QueryMaxSequenceAsync(EventAggregate model, IDbTransaction? tx, CancellationToken ct)
	{
		return await _connection.ExecuteScalarAsync<int>(new CommandDefinition(
			StockySqlFunctions.GetMaxAggregateSequenceFromEventTable,
			new { p_aggregate_type = model.AggregateType, p_aggregate_id = model.AggregateId },
			tx,
			commandType: CommandType.StoredProcedure,
			commandTimeout: _commandTimeout,
			cancellationToken: ct));
	}

	public async Task<T[]?> QueryAllAggregatedEventsAsync<T>(int aggregateType, Guid aggregateId, CancellationToken ct = default) where T : StockyEventPayload
	{
		const string sql = """
		                   SELECT e."EventType", e."EventPayloadJson" FROM stockydb."Events" e
		                   WHERE e."AggregateType" = @aggregateType
		                   AND e."AggregateId" = @aggregateId
		                   ORDER BY e."AggregateSequenceId"
		                   """;

		var rows = await _connection.QueryAsync<(string, string)>(new CommandDefinition(sql, new { aggregateType, aggregateId }, commandTimeout: 10, cancellationToken: ct));
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

	public async Task<StockyEventPayload?> QuerySingleEventAsync(int aggregateType, Guid aggregateId, int aggregateSequenceId, CancellationToken ct = default)
	{
		const string sql = """
		                   SELECT e."EventPayloadJson" FROM stockydb."Events" e
		                   WHERE e."AggregateType" = @aggregateType
		                   AND e."AggregateId" = @aggregateId
		                   AND e."AggregateSequenceId" = @aggregateSequenceId
		                   """;

		var json = await _connection.QuerySingleOrDefaultAsync<string>(new CommandDefinition(sql,
			new { aggregateType, aggregateId, aggregateSequenceId },
			commandTimeout: _commandTimeout,
			cancellationToken: ct));
		return string.IsNullOrEmpty(json) ? null : JsonSerializer.Deserialize<StockyEventPayload>(json);
	}

	private static CommandAggregate CreateCommandAggregate(Command command, Guid commandId, Guid userId, Guid traceId, DateTimeOffset ttStart, DateTimeOffset ttEnd)
		=> new CommandAggregate
		{
			CommandId = commandId,
			UserId = userId,
			CommandType = command.GetType().Name,
			CommandPayloadJson = JsonSerializer.SerializeToDocument(command),
			TtStart = ttStart,
			TtEnd = ttEnd,
			RequestId = Guid.NewGuid(),
			TraceId = traceId,
		};

	private static InsertEventAggregate CreateInsertEventAggregate(
		StockyEvent @event,
		Guid eventId,
		Guid commandId,
		Guid userId,
		Guid traceId,
		DateTimeOffset validFrom,
		DateTimeOffset validTo)
	{
		return new InsertEventAggregate
		{
			EventId = eventId,
			UserId = userId,
			AggregateType = @event.AggregateType,
			AggregateId = @event.AggregateId,
			EventType = @event.GetType().Name,
			EventPayloadJson = JsonSerializer.SerializeToDocument(@event.Payload),
			TtStart = validFrom,
			TtEnd = validTo,
			ValidFrom = validFrom,
			ValidTo = validTo,
			CommandId = commandId,
			TraceId = traceId,
		};
	}

	public void Dispose()
	{
		_connection.Dispose();
	}

	public async ValueTask DisposeAsync()
	{
		await _connection.DisposeAsync();
	}
}

public enum ConcurrencyLevel
{
	OptimisticConcurrency,
	LockOnAggregate,
	RowVersion
}