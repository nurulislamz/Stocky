using System.Text.Json;
using Dapper;
using Npgsql;
using stockymodels.models;
using stockymodels.Models.Enums;

namespace stockymodels.Data;

/// <summary>
/// Dapper-based append-only event store. Inserts events into the Events table per 001CreateEventStore.sql.
/// Use for high-throughput event appends; read models are updated separately (two-prong flow).
/// </summary>
public class PostgresEventStore : IDisposable, IAsyncDisposable
{
	private readonly string _connectionString;
	public readonly NpgsqlConnection Connection;

	public PostgresEventStore(string connectionString)
	{
		_connectionString = connectionString;
		Connection = new NpgsqlConnection(_connectionString);
		Connection.Open();
	}

	/// <summary>Appends an event to the Events table. EventId is auto-generated (BIGSERIAL).</summary>
	public async Task<long> InsertEventAsync(
		Guid userId,
		AggregateType aggregateType,
		Guid aggregateId,
		int sequenceId,
		int aggregateVersion,
		EventType eventType,
		object payload,
		DateTimeOffset ttStart,
		DateTimeOffset ttEnd,
		DateTimeOffset validFrom,
		DateTimeOffset validTo,
		Guid? commandId = null,
		Guid? traceId = null,
		CancellationToken ct = default)
	{
		var aggregateTypeDesc = aggregateType.ToString();
		var eventPayloadJson = JsonSerializer.Serialize(payload);

		const string sql = """
			INSERT INTO stockydb."Events" (
				"UserId", "AggregateType", "AggregateTypeDesc", "AggregateId", "SequenceId",
				"AggregateVersion", "EventType", "EventPayloadJson", "TtStart", "TtEnd",
				"ValidFrom", "ValidTo", "CommandId", "TraceId")
			VALUES (
				@UserId, @AggregateType, @AggregateTypeDesc, @AggregateId, @SequenceId,
				@AggregateVersion, @EventType, @EventPayloadJson, @TtStart, @TtEnd,
				@ValidFrom, @ValidTo, @CommandId, @TraceId)
			RETURNING "EventId";
			""";

		var eventId = await Connection.ExecuteScalarAsync<long>(new Dapper.CommandDefinition(sql, new
		{
			UserId = userId,
			AggregateType = (int)aggregateType,
			AggregateTypeDesc = aggregateTypeDesc,
			AggregateId = aggregateId,
			SequenceId = sequenceId,
			AggregateVersion = aggregateVersion,
			EventType = (int)eventType,
			EventPayloadJson = eventPayloadJson,
			TtStart = ttStart,
			TtEnd = ttEnd,
			ValidFrom = validFrom,
			ValidTo = validTo,
			CommandId = commandId,
			TraceId = traceId
		}, cancellationToken: ct));

		return eventId;
	}

	public void Dispose()
	{
		Connection.Dispose();
	}

	public async ValueTask DisposeAsync()
	{
		await Connection.DisposeAsync();
	}
}