namespace stockymodels.Sql;

/// <summary>
/// Schema-qualified Postgres routine names for <see cref="System.Data.CommandType.StoredProcedure"/> (or equivalent) calls.
/// </summary>
public static class StockySqlFunctions
{
	public const string GetMaxAggregateSequenceFromEventTable = "stockydb.get_max_aggregate_sequence_from_events";

	public const string GetAggregateVersion = "stockydb.get_aggregate_version";

	public const string InsertEventAndUpdateStreamVersion = "stockydb.insert_event_and_update_stream_version";

	public const string InsertCommandAndEvent = "stockydb.insert_command_and_event";

	public const string InsertCommandAndEventWithAdvisoryLock = "stockydb.insert_command_and_event_with_advisory_lock";

	public const string InsertCommandAndEventWithRowVersioning = "stockydb.insert_command_and_event_with_row_versioning";

	public const string InsertCommandAndMultipleEvents = "stockydb.insert_command_and_multiple_events";

	public const string InsertCommandAndMultipleEventsWithAdvisoryLocks = "stockydb.insert_command_and_multiple_events_with_advisory_locks";

	public const string InsertCommandAndMultipleEventsWithRowVersioning = "stockydb.insert_command_and_multiple_events_with_row_versioning";

}

