-- Plain: no extra wrapper lock or prefetch; relies on insert_command + insert_event only.
-- insert_event still uses pg_advisory_xact_lock internally for sequence assignment.
SET search_path TO stockydb;

CREATE OR REPLACE FUNCTION insert_command_and_event(
    p_command stockydb.command_insert,
    p_event stockydb.event_insert
)
RETURNS SETOF stockydb.command_and_event_result
LANGUAGE plpgsql
AS $$
DECLARE
    v_max_seq INTEGER;
    v_command_result RECORD;
    v_event_result RECORD;
BEGIN
    v_max_seq := get_max_aggregate_sequence(p_event.aggregate_type, p_event.aggregate_id);

    if p_command.command_id != p_event.command_id then
        raise exception 'Command and event command IDs do not match';
    end if;

    if p_command.trace_id != p_event.trace_id then
        raise exception 'Command and event trace IDs do not match';
    end if;

    v_command_result := insert_command(p_command);
    v_event_result := insert_event(p_event, v_max_seq + 1);

    RETURN QUERY SELECT
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
        v_event_result.db_stored_at_time;
END;
$$;
