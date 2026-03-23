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
	private readonly IEventStoreReader _reader;


	public PostgresEventStore(string connectionString, IEventStoreReader reader, ILogger<PostgresEventStore> logger, int retryAttempts = 5, int commandTimeout = 30, ConcurrencyLevel? concurrencyLevel = null)
	{
		_reader = reader ?? throw new ArgumentNullException(nameof(reader));
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
		var currentVersion = await _reader.GetStreamVersionAsync(insertEvent.AggregateType, insertEvent.AggregateId, _connection, ct);

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
				var currentVersion = await _reader.GetStreamVersionAsync(eventModel.AggregateType, eventModel.AggregateId, _connection, ct);
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
			commandType: System.Data.CommandType.StoredProcedure);

		return result;
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