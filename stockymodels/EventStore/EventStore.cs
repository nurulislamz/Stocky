using System.Data;
using System.Text.Json;
using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;
using stockymodels.Events;
using stockymodels.models;

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

	private const string CommandSql = """
	                                  INSERT INTO stockydb."Commands" (
	                                      "CommandId", "UserId", "CommandType", "CommandPayloadJson",
	                                      "TtStart", "TtEnd", "RequestId", "TraceId")
	                                  VALUES (
	                                      @CommandId, @UserId, @CommandType, @CommandPayloadJson,
	                                      @TtStart, @TtEnd, @RequestId, @TraceId);
	                                  """;

	private const string EventSql = """
	                                INSERT INTO stockydb."Events" (
	                                    "EventId", "UserId", "AggregateType", "AggregateId",
	                                    "AggregateSequenceId", "EventType", "EventPayloadJson",
	                                    "TtStart", "TtEnd", "ValidFrom", "ValidTo", "CommandId", "TraceId")
	                                VALUES (
	                                    @EventId, @UserId, @AggregateType, @AggregateId,
	                                    @AggregateSequenceId, @EventType, @EventPayloadJson,
	                                    @TtStart, @TtEnd, @ValidFrom, @ValidTo, @CommandId, @TraceId);
	                                """;

	private const string MaxSeqSql = """
	                                 SELECT COALESCE(MAX("AggregateSequenceId"), 0)
	                                 FROM stockydb."Events"
	                                 WHERE "AggregateType" = @aggregateType
	                                   AND "AggregateId" = @aggregateId
	                                 """;

	private const string AdvisoryLockSql = """
	                                       SELECT pg_advisory_xact_lock(@aggregateType, hashtext(@aggregateId::text))
	                                       """;

	public PostgresEventStore(string connectionString, ILogger<PostgresEventStore> logger, int retryAttempts = 5, int commandTimeout = 30, ConcurrencyLevel? concurrencyLevel = null)
	{
		_logger = logger;
		_commandTimeout = commandTimeout;
		_retryAttempts = retryAttempts;
		_concurrencyLevel = concurrencyLevel ?? ConcurrencyLevel.OptimisticConcurrency;
		_connection = new NpgsqlConnection(connectionString);
		_connection.Open();
	}

	/// <summary>Appends a command and one event in a single transaction.</summary>
	public async Task<(CommandModel, EventModel)> RegisterEventAsync(
		Command command,
		StockyEvent @event,
		AppendContext context,
		CancellationToken ct = default)
	{
		var commandId = Guid.NewGuid();
		var now = DateTimeOffset.UtcNow;
		var ttEnd = DateTimeOffset.MaxValue;

		var commandModel = CreateCommandModel(command, commandId, context, now, ttEnd);
		var eventModel = CreateEventModel(@event, Guid.NewGuid(), commandId, context, now, ttEnd);

		return _concurrencyLevel switch
		{
			ConcurrencyLevel.OptimisticConcurrency => await WithOptimisticRetry(commandModel, eventModel, ct),
			ConcurrencyLevel.LockOnAggregate => await WithAdvisoryLock(commandModel, eventModel, ct),
			_ => throw new ArgumentOutOfRangeException()
		};
	}

	private async Task<(CommandModel, EventModel)> WithOptimisticRetry(
		CommandModel commandModel, EventModel eventModel, CancellationToken ct)
	{
		for (int attempt = 0; attempt < _retryAttempts; attempt++)
		{
			await using var tx = await _connection.BeginTransactionAsync(ct);
			try
			{
				var nextSeq = await QueryMaxSequenceAsync(eventModel, tx, ct) + 1;
				await InsertCommandAsync(commandModel, tx, ct);
				var inserted = await InsertEventAsync(eventModel, nextSeq, tx, ct);
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

	private async Task<(CommandModel, EventModel)> WithAdvisoryLock(
		CommandModel commandModel, EventModel eventModel, CancellationToken ct)
	{
		await using var tx = await _connection.BeginTransactionAsync(ct);
		try
		{
			await AcquireAdvisoryLockAsync(eventModel, tx, ct);
			var nextSeq = await QueryMaxSequenceAsync(eventModel, tx, ct) + 1;
			await InsertCommandAsync(commandModel, tx, ct);
			var inserted = await InsertEventAsync(eventModel, nextSeq, tx, ct);
			await tx.CommitAsync(ct);
			return (commandModel, inserted);
		}
		catch
		{
			await tx.RollbackAsync(ct);
			throw;
		}
	}
	
	/// <summary>Appends a command and multiple event in a single transaction.</summary>
	public async Task<(CommandModel, EventModel[])> RegisterMultipleEventsAsync(
		Command command,
		StockyEvent[] events,
		AppendContext context,
		CancellationToken ct = default)
	{
		var commandId = Guid.NewGuid();
		var now = DateTimeOffset.UtcNow;
		var ttEnd = DateTimeOffset.MaxValue;

		var commandModel = CreateCommandModel(command, commandId, context, now, ttEnd);
		var eventModel = events.Select(e => CreateEventModel(e, Guid.NewGuid(), commandId, context, now, ttEnd)).ToArray();

		return _concurrencyLevel switch
		{
			ConcurrencyLevel.OptimisticConcurrency => await WithOptimisticRetryMulti(commandModel, eventModel, ct),
			ConcurrencyLevel.LockOnAggregate => await WithAdvisoryLockMulti(commandModel, eventModel, ct),
			_ => throw new ArgumentOutOfRangeException()
		};
	}

	private async Task<(CommandModel, EventModel[])> WithOptimisticRetryMulti(
		CommandModel commandModel, EventModel[] eventModel, CancellationToken ct)
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

	private async Task<(CommandModel, EventModel[])> WithAdvisoryLockMulti(
		CommandModel commandModel, EventModel[] eventModels, CancellationToken ct)
	{
		await using var tx = await _connection.BeginTransactionAsync(ct);
		try
		{
			var distinctAggregates = eventModels
				.Select(e => (e.AggregateType, e.AggregateId))
				.Distinct()
				.OrderBy(a => a.AggregateType)
				.ThenBy(a => a.AggregateId);
			
			foreach (var (aggType, aggId) in distinctAggregates)
			{
				await AcquireAdvisoryLockAsync(
					eventModels.First(e => e.AggregateType == aggType && e.AggregateId == aggId),
					tx, ct);
			}
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

	private async Task InsertCommandAsync(CommandModel model, IDbTransaction tx, CancellationToken ct)
	{
		await _connection.ExecuteAsync(new CommandDefinition(
			CommandSql, model, tx,
			commandTimeout: _commandTimeout, cancellationToken: ct));
	}

	private async Task<EventModel> InsertEventAsync(EventModel model, int sequenceId, IDbTransaction tx, CancellationToken ct)
	{
		var toInsert = model with { AggregateSequenceId = sequenceId };
		await _connection.ExecuteAsync(new CommandDefinition(
			EventSql, toInsert, tx,
			commandTimeout: _commandTimeout, cancellationToken: ct));
		return toInsert;
	}
	
	private async Task<EventModel[]> InsertMultipleEventsWithSequencing(EventModel[] eventModels, IDbTransaction tx, CancellationToken ct)
	{
		var sequenceTracker = new Dictionary<(string, Guid), int>();
		var insertList = new List<EventModel>();
		
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

	private async Task<int> QueryMaxSequenceAsync(EventModel model, IDbTransaction tx, CancellationToken ct)
	{
		return await _connection.ExecuteScalarAsync<int>(new CommandDefinition(
			MaxSeqSql,
			new { aggregateType = model.AggregateType, aggregateId = model.AggregateId },
			tx, commandTimeout: _commandTimeout, cancellationToken: ct));
	}

	private async Task AcquireAdvisoryLockAsync(EventModel model, IDbTransaction tx, CancellationToken ct)
	{
		await _connection.ExecuteAsync(new CommandDefinition(
			AdvisoryLockSql,
			new { aggregateType = model.AggregateType, aggregateId = model.AggregateId },
			tx, commandTimeout: _commandTimeout, cancellationToken: ct));
	}

	public async Task<StockyEventPayload[]?> QueryAllAggregatedEventsAsync(int aggregateType, Guid aggregateId, CancellationToken ct = default)
	{
		const string sql = """
		                   SELECT e."EventType", e."EventPayloadJson" FROM stockydb."Events" e
		                   WHERE e."AggregateType" = @aggregateType
		                   AND e."AggregateId" = @aggregateId
		                   ORDER BY e."AggregateSequenceId"
		                   """;

		var rows = await _connection.QueryAsync<(string, string)>(new CommandDefinition(sql, new { aggregateType, aggregateId }, commandTimeout: 10, cancellationToken: ct));
		var list = new List<StockyEventPayload>();
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

				if (JsonSerializer.Deserialize(json, type) is StockyEventPayload payload) list.Add(payload);
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

	private static CommandModel CreateCommandModel(Command command, Guid commandId, AppendContext ctx, DateTimeOffset ttStart, DateTimeOffset ttEnd)
		=> new CommandModel
		{
			CommandId = commandId,
			UserId = ctx.UserId,
			CommandType = command.GetType().Name,
			CommandPayloadJson = JsonSerializer.Serialize(command),
			TtStart = ttStart,
			TtEnd = ttEnd,
			RequestId = Guid.NewGuid(),
			TraceId = ctx.TraceId,
		};

	private static EventModel CreateEventModel(
		StockyEvent eventPayload,
		Guid eventId,
		Guid commandId,
		AppendContext ctx,
		DateTimeOffset validFrom,
		DateTimeOffset validTo)
	{
		return new EventModel
		{
			EventId = eventId,
			UserId = ctx.UserId,
			AggregateType = eventPayload.AggregateType,
			AggregateId = eventPayload.AggregateId,
			EventType = eventPayload.GetType().Name,
			EventPayloadJson = JsonSerializer.Serialize(eventPayload),
			TtStart = validFrom,
			TtEnd = validTo,
			ValidFrom = validFrom,
			ValidTo = validTo,
			CommandId = commandId,
			TraceId = ctx.TraceId,
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
	LockOnAggregate
}
