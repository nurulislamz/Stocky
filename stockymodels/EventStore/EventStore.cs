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
		using var dataSource = dataSourceBuilder.Build();
	}

	/// <summary>Appends a command and one event in a single transaction.</summary>
	public async Task<(CommandAggregate, InsertEventAggregate)> RegisterEventAsync(
		Command command,
		StockyEvent @event,
		Guid userId,
		Guid traceId,
		CancellationToken ct = default)
	{
		var commandId = Guid.NewGuid();
		var now = DateTimeOffset.UtcNow;
		var ttEnd = DateTimeOffset.MaxValue;

		var commandModel = CreateCommandAggregate(command, commandId, userId, traceId, now, ttEnd);
		var eventModel = CreateInsertEventAggregate(@event, Guid.NewGuid(), commandId, userId, traceId, now, ttEnd);

		return _concurrencyLevel switch
		{
			ConcurrencyLevel.OptimisticConcurrency => await WithOptimisticRetry(commandModel, eventModel, ct),
			ConcurrencyLevel.LockOnAggregate => await WithAdvisoryLock(commandModel, eventModel, ct),
			ConcurrencyLevel.RowVersion => await WithRowVersioning(commandModel, eventModel, ct),
			_ => throw new ArgumentOutOfRangeException()
		};
	}

	private async Task<(CommandAggregate, InsertEventAggregate)> WithRowVersioning(CommandAggregate commandModel, InsertEventAggregate eventModel, CancellationToken ct)
	{

		await _connection.ExecuteAsync(StockySqlFunctions.GetMaxAggregateSequence,
			(eventModel.AggregateType, eventModel.AggregateId));
		await _connection.ExecuteAsync(StockySqlFunctions.InsertCommandAndEventWithRowVersioning,
			(commandModel, eventModel));

		return (commandModel, eventModel);
	}

	private async Task<(CommandAggregate, InsertEventAggregate)> WithOptimisticRetry(CommandAggregate commandModel, InsertEventAggregate eventModel, CancellationToken ct)
	{
		for (int attempt = 0; attempt < _retryAttempts; attempt++)
		{
			await using var tx = await _connection.BeginTransactionAsync(ct);
			try
			{
				await _connection.ExecuteAsync(StockySqlFunctions.InsertCommandAndEvent,
					(commandModel, eventModel), commandType: System.Data.CommandType.StoredProcedure);
				return (commandModel, eventModel);
			}
			catch (PostgresException ex) when (ex.SqlState == "23505")
			{
				_logger.LogWarning("Sequence conflict, attempt {Attempt}/{Max}",
					attempt + 1, _retryAttempts);
			}
			catch
			{
				await tx.RollbackAsync(ct);
				throw;
			}
		}

		throw new InvalidOperationException(
			$"Unable to register event after {_retryAttempts} retries");
	}

	private async Task<(CommandAggregate, EventAggregate)> WithAdvisoryLock(
		CommandAggregate commandModel, InsertEventAggregate eventModel, CancellationToken ct)
	{
		await _connection.ExecuteAsync(StockySqlFunctions.InsertCommandAndEventWithAdvisoryLock,
			(commandModel, eventModel), commandType: System.Data.CommandType.StoredProcedure);
		return (commandModel, eventModel);
	}

	/// <summary>Appends a command and multiple event in a single transaction.</summary>
	public async Task<(CommandAggregate, EventAggregate[])> RegisterMultipleEventsAsync(
		Command command,
		StockyEvent[] events,
		Guid userId,
		Guid traceId,
		CancellationToken ct = default)
	{
		var commandId = Guid.NewGuid();
		var now = DateTimeOffset.UtcNow;
		var ttEnd = DateTimeOffset.MaxValue;

		var commandModel = CreateCommandAggregate(command, commandId, context, now, ttEnd);

		var dataSourceBuilder = new NpgsqlDataSourceBuilder("Your_Connection_String");
		dataSourceBuilder.MapComposite<EventModel>("stockydb.event_insert");
		var dataSource = dataSourceBuilder.Build();
		var eventModels = events.Select(e => CreateInsertEventAggregate(e, Guid.NewGuid(), commandId, context, now, ttEnd)).ToArray();

		return _concurrencyLevel switch
		{
			ConcurrencyLevel.OptimisticConcurrency => await WithOptimisticRetryMulti(commandModel, eventModels, ct),
			ConcurrencyLevel.LockOnAggregate => await WithAdvisoryLockMulti(commandModel, eventModels, ct),
			ConcurrencyLevel.RowVersion => await WithRowVersioningMulti(commandModel, eventModels, ct),
			_ => throw new ArgumentOutOfRangeException()
		};
	}

	private async Task<(CommandAggregate, EventAggregate[])> WithRowVersioningMulti(CommandAggregate commandModel, EventAggregate[] eventModels, CancellationToken ct)
	{
		await _connection.ExecuteScalarAsync(StockySqlFunctions.InsertCommandAndEventWithRowVersioning, (commandModel, eventModels))
		await using var tx = await _connection.BeginTransactionAsync(ct);
		try
		{
			await InsertCommandAsync(commandModel, tx, ct);
			var inserted = await InsertMultipleEventsWithSequencing(eventModels, tx, ct);
			await tx.CommitAsync(ct);
			return (commandModel, inserted);
		}
		catch
		{
			await tx.RollbackAsync(ct);
			throw;
		}
	}

	private async Task<(CommandAggregate, EventAggregate[])> WithOptimisticRetryMulti(
		CommandAggregate commandModel, EventAggregate[] eventModel, CancellationToken ct)
	{
		for (int attempt = 0; attempt < _retryAttempts; attempt++)
		{
			await using var tx = await _connection.BeginTransactionAsync(ct);
			try
			{
				await InsertCommandAsync(commandModel, tx, ct);
				var inserted = await InsertMultipleEventsWithSequencing(eventModel, tx, ct);
				await tx.CommitAsync(ct);
				return (commandModel, inserted);
			}
			catch (PostgresException ex) when (ex.SqlState == "23505")
			{
				_logger.LogWarning("Sequence conflict, attempt {Attempt}/{Max}",
					attempt + 1, _retryAttempts);
			}
			catch
			{
				await tx.RollbackAsync(ct);
				throw;
			}
		}

		throw new InvalidOperationException(
			$"Unable to register event after {_retryAttempts} retries");
	}

	private async Task<(CommandAggregate, EventAggregate[])> WithAdvisoryLockMulti(
		CommandAggregate commandModel, EventAggregate[] eventModels, CancellationToken ct)
	{
		await using var tx = await _connection.BeginTransactionAsync(ct);
		try
		{
			var distinctAggregates = eventModels
				.Select(e => (e.AggregateType, e.AggregateId))
				.Distinct()
				.OrderBy(a => a.AggregateType)
				.ThenBy(a => a.AggregateId);

			await InsertCommandAsync(commandModel, tx, ct);
			foreach (var (aggType, aggId) in distinctAggregates)
			{
				await AcquireAdvisoryLockAsync(
					eventModels.First(e => e.AggregateType == aggType && e.AggregateId == aggId),
					tx, ct);
			}
			var inserted = await InsertMultipleEventsWithSequencing(eventModels, tx, ct);
			await tx.CommitAsync(ct);
			return (commandModel, inserted);
		}
		catch
		{
			await tx.RollbackAsync(ct);
			throw;
		}
	}

	private async Task<EventAggregate[]> InsertMultipleEventsWithSequencing(EventAggregate[] eventModels, IDbTransaction tx, CancellationToken ct)
	{
		var sequenceTracker = new Dictionary<(string, Guid), int>();
		var insertList = new List<EventAggregate>();

		foreach (var model in eventModels)
		{
			var key = (model.AggregateType, model.AggregateId);
			if (!sequenceTracker.TryGetValue(key, out var nextSeq))
			{
				nextSeq = await QueryMaxSequenceAsync(model, tx, ct) + 1;
			}
			var insert = await InsertEventAsync(model, nextSeq, tx, ct);
			insertList.Add(insert);
			sequenceTracker[key] = nextSeq + 1;
		}
		return insertList.ToArray();
	}

	private async Task<int> QueryMaxSequenceAsync(EventAggregate model, IDbTransaction? tx, CancellationToken ct)
	{
		return await _connection.ExecuteScalarAsync<int>(new CommandDefinition(
			MaxSeqSql,
			new { aggregateType = model.AggregateType, aggregateId = model.AggregateId },
			tx, commandTimeout: _commandTimeout, cancellationToken: ct));
	}

	private async Task AcquireAdvisoryLockAsync(EventAggregate model, IDbTransaction tx, CancellationToken ct)
	{
		await _connection.ExecuteAsync(new CommandDefinition(
			AdvisoryLockSql,
			new { aggregateType = model.AggregateType, aggregateId = model.AggregateId },
			tx, commandTimeout: _commandTimeout, cancellationToken: ct));
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