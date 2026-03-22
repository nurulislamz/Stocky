-- Plain: insert_command once, then one insert_event per row. Each event gets the next sequence
-- for its own (aggregate_type, aggregate_id), matching PostgresEventStore.InsertMultipleEventsWithSequencing.
SET search_path TO stockydb;

CREATE OR REPLACE FUNCTION insert_command_and_multiple_events(
    p_command stockydb.command_insert,
    p_events stockydb.event_insert[]
)
RETURNS stockydb.command_and_event_result[]
LANGUAGE plpgsql
AS $$
DECLARE
    v_next_seq INTEGER;
    v_event stockydb.event_insert;
    v_command_result RECORD;
    v_event_result RECORD;
    v_results stockydb.command_and_event_result[];
BEGIN
    IF p_events IS NULL OR COALESCE(array_length(p_events, 1), 0) < 1 THEN
        RAISE EXCEPTION 'p_events must contain at least one event'
            USING ERRCODE = '22023';
    END IF;

    v_command_result := insert_command(p_command);

    v_results := ARRAY[]::stockydb.command_and_event_result[];

    FOREACH v_event IN ARRAY p_events
    LOOP
        v_next_seq := get_aggregate_version(v_event.aggregate_type, v_event.aggregate_id) + 1;

        v_event_result := insert_event_and_update_stream_version(v_event, v_next_seq);

        v_results := array_append(
            v_results,
            ROW(
                v_command_result.command_id,
                v_command_result.user_id,
                v_command_result.trace_id,
                v_command_result.db_stored_at_time,
                v_event_result.event_id,
                v_event_result.user_id,
                v_event_result.aggregate_id,
                v_event_result.aggregate_sequence_id,
                v_event_result.command_id,
                v_event_result.trace_id,
                v_event_result.db_stored_at_time
            )::stockydb.command_and_event_result
        );
    END LOOP;

    RETURN v_results;
END;
$$;
