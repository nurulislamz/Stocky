using System.Data;
using System.Text.Json;
using Dapper;
using Microsoft.Testing.Platform.Logging;
using Npgsql;
using stockymodels.Events;
using stockymodels.EventStore;
using stockymodels.models;

namespace stockymodels.Data;

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
	                                  "CommandId", "UserId", "CommandType", "CommandPayloadJson", "TtStart", "TtEnd", "RequestId", "TraceId")
	                                  VALUES (
	                                  @CommandId, @UserId, @CommandType, @CommandPayloadJson, @TtStart, @TtEnd, @RequestId, @TraceId);
	                                  """;

	private const string EventSql = """
	                                INSERT INTO stockydb."Events" (
	                                	"EventId", "UserId", "AggregateType", "AggregateTypeDesc", "AggregateId",
	                                	"AggregateSequenceId", "EventType", "EventPayloadJson", "TtStart", "TtEnd",
	                                	"ValidFrom", "ValidTo", "CommandId", "TraceId")
	                                VALUES (
	                                	@EventId, @UserId, @AggregateType, @AggregateTypeDesc, @AggregateId,
	                                	@AggregateSequenceId, @EventType, @EventPayloadJson, @TtStart, @TtEnd,
	                                	@ValidFrom, @ValidTo, @CommandId, @TraceId);
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
		var eventId = Guid.NewGuid();
		var now = DateTimeOffset.UtcNow;
		var ttEnd = DateTimeOffset.MaxValue;

		// check if aggregate of that type already exists
		var commandModel = CreateCommandModel(command, commandId, context, now, ttEnd);
		var eventModel = CreateEventModel(@event, eventId, commandId, context, now, ttEnd);

		return _concurrencyLevel switch
		{
			ConcurrencyLevel.OptimisticConcurrency => await RegisterEventOptimisticLock(commandModel, eventModel, @event, ct),
			ConcurrencyLevel.LockOnAggregate => await RegisterEventWithLockOnAggregate(commandModel, eventModel, @event, ct),
			_ => throw new ArgumentOutOfRangeException()
		};
	}

	/// <summary>Appends a command and one event in a single transaction.</summary>
	private async Task<(CommandModel, EventModel)> RegisterEventOptimisticLock(
		CommandModel commandModel,
		EventModel eventModel,
		StockyEvent @event,
		CancellationToken ct = default)
	{
		int retryAttempts = 0;
		while (retryAttempts < _retryAttempts)
		{
			await using var transaction = await _connection.BeginTransactionAsync(ct);
			try
			{
				var nextAggregateSequenceId =
					await QueryMaxAggregateSequenceIdAsync((int)@event.AggregateType, @event.AggregateId, transaction, ct) + 1;

				await _connection.ExecuteAsync(new CommandDefinition(CommandSql, commandModel, transaction,
					commandTimeout: _commandTimeout, cancellationToken: ct));
				await _connection.ExecuteAsync(new CommandDefinition(EventSql,
					eventModel with { AggregateSequenceId = nextAggregateSequenceId }, transaction,
					commandTimeout: _commandTimeout, cancellationToken: ct));
				await transaction.CommitAsync(ct);
			}
			catch (PostgresException ex) when (ex.SqlState == "23505")
			{
				retryAttempts++;
				await _logger.LogWarningAsync($"Failed to registerEvent, {retryAttempts}/{_retryAttempts} retrying..."); 
			}
			catch
			{
				await transaction.RollbackAsync(ct);
				throw;
			}
			
			return (commandModel, eventModel);
		}

		throw new InvalidOperationException($"Unable to registerEvents after {_retryAttempts}");
	}
	
	private async Task<(CommandModel, EventModel)> RegisterEventWithLockOnAggregate(CommandModel commandModel, EventModel eventModel, StockyEvent @event, CancellationToken ct)
	{
		await using var transaction = await _connection.BeginTransactionAsync(ct);
		try
		{
			const string sqlLock =
				"""select * from pg_advisory_xact_lock(@aggregateType, hashtext(@aggregateId::text));""";
			
			await _connection.ExecuteAsync(new CommandDefinition(sqlLock, new {eventModel.AggregateType, eventModel.AggregateId}, transaction,
				commandTimeout: _commandTimeout, cancellationToken: ct));
			var nextAggregateSequenceId =
				await QueryMaxAggregateSequenceIdAsync((int)@event.AggregateType, @event.AggregateId, transaction, ct) + 1;
			await _connection.ExecuteAsync(new CommandDefinition(CommandSql, commandModel, transaction,
				commandTimeout: _commandTimeout, cancellationToken: ct));
			await _connection.ExecuteAsync(new CommandDefinition(EventSql,
				eventModel with { AggregateSequenceId = nextAggregateSequenceId }, transaction,
				commandTimeout: _commandTimeout, cancellationToken: ct));
			await transaction.CommitAsync(ct);
		}
		catch
		{
			await transaction.RollbackAsync(ct);
			throw;
		}
			
		return (commandModel, eventModel);
	}

	public async Task<StockyEventPayload[]?> QueryAllAggregatedEventsAsync(int aggregateType, Guid aggregateId, CancellationToken ct = default)
	{
		const string sql = """
		                   SELECT e."EventPayloadJson" FROM stockydb."Events" e
		                   WHERE e."AggregateType" = @aggregateType
		                   AND e."AggregateId" = @aggregateId
		                   ORDER BY e."AggregateSequenceId"
		                   """;

		var events = await _connection.ExecuteScalarAsync<StockyEventPayload[]>(new CommandDefinition(sql, new {aggregateType, aggregateId}, commandTimeout: 10, cancellationToken: ct));
		return events;
	}

	public async Task<StockyEventPayload?> QuerySingleEventAsync(int aggregateType, Guid aggregateId, int aggregateSequenceId, CancellationToken ct = default)
	{
		const string sql = """
		                   SELECT e."EventPayloadJson" FROM STOCKYDB."EVENTS" E
		                   WHERE E."AggregateType" = @aggregateType
		                   AND E."AggregateId" = @aggregateId
		                   AND E."AggregateSequenceId" = @aggregateSequenceId;
		                   """;

		var @event = await _connection.ExecuteScalarAsync<StockyEventPayload>(new CommandDefinition(sql,
			new {aggregateType,aggregateId,aggregateSequenceId},
			commandTimeout: _commandTimeout,
			cancellationToken: ct));
		return @event;
	}

	public async Task<int> QueryMaxAggregateSequenceIdAsync(int aggregateType, Guid aggregateId, IDbTransaction? transaction = null,
		CancellationToken ct = default)
	{
		const string sql = """
		                   SELECT COALESCE(MAX("AggregateSequenceId"), 0) FROM STOCKYDB."EVENTS" E
		                   WHERE E."AggregateType" = @aggregateType
		                   AND E."AggregateId" = @aggregateId
		                   """;

		var maxSequenceId = await _connection.ExecuteScalarAsync<int>(new CommandDefinition(sql,
			new {aggregateType,aggregateId},
			transaction: transaction,
			commandTimeout: _commandTimeout,
			cancellationToken: ct));
		return maxSequenceId;
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
			AggregateType = (int)eventPayload.AggregateType,
			AggregateTypeDesc = eventPayload.AggregateType.ToString(),
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
};

public enum ConcurrencyLevel
{
	OptimisticConcurrency,
	LockOnAggregate
}