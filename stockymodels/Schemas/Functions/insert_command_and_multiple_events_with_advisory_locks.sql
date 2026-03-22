-- Matches PostgresEventStore.WithAdvisoryLockMulti: insert_command first, then one advisory lock
-- per distinct (aggregate_type, aggregate_id) in sorted order, then insert_event per row with
-- per-stream next sequence (get_max + 1 after prior inserts in this txn).
SET search_path TO stockydb;

CREATE OR REPLACE FUNCTION insert_command_and_multiple_events_with_advisory_locks(
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
    rec RECORD;
BEGIN
    IF p_events IS NULL OR COALESCE(array_length(p_events, 1), 0) < 1 THEN
        RAISE EXCEPTION 'p_events must contain at least one event'
            USING ERRCODE = '22023';
    END IF;

    v_command_result := insert_command(p_command);

    v_results := ARRAY[]::stockydb.command_and_event_result[];

    FOR rec IN
        SELECT DISTINCT e.aggregate_type, e.aggregate_id
        FROM unnest(p_events) AS e
        ORDER BY 1, 2
    LOOP
        PERFORM pg_advisory_xact_lock(
            hashtext(rec.aggregate_type::text),
            hashtext(rec.aggregate_id::text)
        );
    END LOOP;

    FOREACH v_event IN ARRAY p_events
    LOOP
        v_next_seq := get_max_aggregate_sequence(v_event.aggregate_type, v_event.aggregate_id) + 1;

        if p_command.command_id != v_event.command_id then
            raise exception 'Command and event command IDs do not match';
        end if;

        if p_command.trace_id != v_event.trace_id then
            raise exception 'Command and event trace IDs do not match';
        end if;

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
