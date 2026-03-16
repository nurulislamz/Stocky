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
	public async Task<(CommandAggregate, EventAggregate)> RegisterEventAsync(
		Command command,
		StockyEvent @event,
		AppendContext context,
		CancellationToken ct = default)
	{
		var commandId = Guid.NewGuid();
		var now = DateTimeOffset.UtcNow;
		var ttEnd = DateTimeOffset.MaxValue;

		var commandModel = CreateCommandAggregate(command, commandId, context, now, ttEnd);
		var eventModel = CreateEventAggregate(@event, Guid.NewGuid(), commandId, context, now, ttEnd);

		return _concurrencyLevel switch
		{
			ConcurrencyLevel.OptimisticConcurrency => await WithOptimisticRetry(commandModel, eventModel, ct),
			ConcurrencyLevel.LockOnAggregate => await WithAdvisoryLock(commandModel, eventModel, ct),
			_ => throw new ArgumentOutOfRangeException()
		};
	}

	private async Task<(CommandAggregate, EventAggregate)> WithOptimisticRetry(
		CommandAggregate commandModel, EventAggregate eventModel, CancellationToken ct)
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

	private async Task<(CommandAggregate, EventAggregate)> WithAdvisoryLock(
		CommandAggregate commandModel, EventAggregate eventModel, CancellationToken ct)
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
	public async Task<(CommandAggregate, EventAggregate[])> RegisterMultipleEventsAsync(
		Command command,
		StockyEvent[] events,
		AppendContext context,
		CancellationToken ct = default)
	{
		var commandId = Guid.NewGuid();
		var now = DateTimeOffset.UtcNow;
		var ttEnd = DateTimeOffset.MaxValue;

		var commandModel = CreateCommandAggregate(command, commandId, context, now, ttEnd);
		var eventModel = events.Select(e => CreateEventAggregate(e, Guid.NewGuid(), commandId, context, now, ttEnd)).ToArray();

		return _concurrencyLevel switch
		{
			ConcurrencyLevel.OptimisticConcurrency => await WithOptimisticRetryMulti(commandModel, eventModel, ct),
			ConcurrencyLevel.LockOnAggregate => await WithAdvisoryLockMulti(commandModel, eventModel, ct),
			_ => throw new ArgumentOutOfRangeException()
		};
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

	private async Task InsertCommandAsync(CommandAggregate model, IDbTransaction tx, CancellationToken ct)
	{
		await _connection.ExecuteAsync(new CommandDefinition(
			CommandSql, model, tx,
			commandTimeout: _commandTimeout, cancellationToken: ct));
	}

	private async Task<EventAggregate> InsertEventAsync(EventAggregate model, int sequenceId, IDbTransaction tx, CancellationToken ct)
	{
		var toInsert = model with { AggregateSequenceId = sequenceId };
		await _connection.ExecuteAsync(new CommandDefinition(
			EventSql, toInsert, tx,
			commandTimeout: _commandTimeout, cancellationToken: ct));
		return toInsert;
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

	private async Task<int> QueryMaxSequenceAsync(EventAggregate model, IDbTransaction tx, CancellationToken ct)
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

	private static CommandAggregate CreateCommandAggregate(Command command, Guid commandId, AppendContext ctx, DateTimeOffset ttStart, DateTimeOffset ttEnd)
		=> new CommandAggregate
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

	private static EventAggregate CreateEventAggregate(
		StockyEvent eventPayload,
		Guid eventId,
		Guid commandId,
		AppendContext ctx,
		DateTimeOffset validFrom,
		DateTimeOffset validTo)
	{
		return new EventAggregate
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
