using System.Text.Json;
using Dapper;
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

	public PostgresEventStore(string connectionString)
	{
		_connection = new NpgsqlConnection(connectionString);
		_connection.Open();
	}

	/// <summary>Appends a command and one event in a single transaction.</summary>
	public async Task<(CommandModel, EventModel)> InsertEventAsync(
		Command command,
		StockyEvent @event,
		AppendContext context,
		int sequenceId,
		CancellationToken ct = default)
	{
		var commandId = Guid.NewGuid();
		var eventId = Guid.NewGuid();
		var now = DateTimeOffset.UtcNow;
		var ttEnd = DateTimeOffset.MaxValue;

		var commandModel = CreateCommandModel(command, commandId, context, now, ttEnd);
		var eventModel = CreateEventModel(@event, eventId, commandId, context, 0, 0, now, ttEnd);
		// need to edit aggregate version logic

		const string commandSql = """
			INSERT INTO stockydb."Commands" (
				"CommandId", "UserId", "CommandType", "CommandPayloadJson", "TtStart", "TtEnd", "RequestId", "TraceId")
			VALUES (
				@CommandId, @UserId, @CommandType, @CommandPayloadJson, @TtStart, @TtEnd, @RequestId, @TraceId);
			""";

		const string eventSql = """
			INSERT INTO stockydb."Events" (
				"EventId", "UserId", "AggregateType", "AggregateTypeDesc", "AggregateId", "SequenceId",
				"AggregateVersion", "EventType", "EventPayloadJson", "TtStart", "TtEnd",
				"ValidFrom", "ValidTo", "CommandId", "TraceId")
			VALUES (
				@EventId, @UserId, @AggregateType, @AggregateTypeDesc, @AggregateId, @SequenceId,
				@AggregateVersion, @EventType, @EventPayloadJson, @TtStart, @TtEnd,
				@ValidFrom, @ValidTo, @CommandId, @TraceId);
			""";

		using var transaction = await _connection.BeginTransactionAsync(ct);
		try
		{
			await _connection.ExecuteAsync(new CommandDefinition(commandSql, commandModel, transaction, cancellationToken: ct));
			await _connection.ExecuteAsync(new CommandDefinition(eventSql, eventModel, transaction, cancellationToken: ct));
			await transaction.CommitAsync(ct);
		}
		catch
		{
			await transaction.RollbackAsync(ct);
			throw;
		}

		return (commandModel, eventModel);
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
		StockyEvent @event,
		Guid eventId,
		Guid commandId,
		AppendContext ctx,
		int sequenceId,
		int aggregateVersion,
		DateTimeOffset validFrom,
		DateTimeOffset validTo)
	{
		var aggregateTypeDesc = @event.GetType().Name;

		return new EventModel
		{
			EventId = eventId,
			UserId = ctx.UserId,
			AggregateType = aggregateTypeDesc,
			AggregateTypeDesc = aggregateTypeDesc,
			AggregateId = @event.AggregateId,
			SequenceId = sequenceId,
			AggregateVersion = aggregateVersion,
			EventType = @event.GetType().Name,
			EventPayloadJson = JsonSerializer.Serialize(@event),
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
