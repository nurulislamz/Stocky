-- Row-version append: caller passes the next sequence they computed *before* this transaction
-- (same as get_aggregate_version(...) + 1 at read time). Inside the txn we recompute
-- version + 1; if it differs, the stream advanced between the client's read and this txn → conflict.
-- No advisory lock (optimistic / compare-and-append).
SET search_path TO stockydb;

CREATE OR REPLACE FUNCTION insert_command_and_event_with_row_versioning(
    p_command stockydb.command_insert,
    p_event stockydb.event_insert_with_seq_id
)
RETURNS SETOF stockydb.command_and_event_result
LANGUAGE plpgsql
AS $$
DECLARE
    v_max_now INTEGER;
    v_next_now INTEGER;
    v_command_result RECORD;
    v_event_result RECORD;
BEGIN
    IF p_event.expected_next_sequence < 1 THEN
        RAISE EXCEPTION 'p_expected_next_sequence must be >= 1'
            USING ERRCODE = '22023';
    END IF;

    if p_command.command_id != p_event.command_id then
        raise exception 'Command and event command IDs do not match';
    end if;

    if p_command.trace_id != p_event.trace_id then
        raise exception 'Command and event trace IDs do not match';
    end if;

    v_max_now := get_aggregate_version((p_event).event.aggregate_type, (p_event).event.aggregate_id);
    v_next_now := v_max_now + 1;

    IF v_next_now IS DISTINCT FROM p_event.expected_next_sequence THEN
        RAISE EXCEPTION 'row version conflict: expected next sequence %, actual next % (aggregate % / %)',
            p_event.expected_next_sequence, v_next_now, (p_event).event.aggregate_type, (p_event).event.aggregate_id
            USING ERRCODE = 'P0001';
    END IF;

    v_command_result := insert_command(p_command);
    v_event_result := insert_event_and_update_stream_version((p_event).event, v_next_now);

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
