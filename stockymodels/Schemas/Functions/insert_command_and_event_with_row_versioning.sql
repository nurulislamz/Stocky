-- Row-version append: caller passes the next sequence they computed *before* this transaction
-- (same as get_max_aggregate_sequence(...) + 1 at read time). Inside the txn we recompute
-- get_max + 1; if it differs, the stream advanced between the client's read and this txn → conflict.
-- No advisory lock (optimistic / compare-and-append).
SET search_path TO stockydb;

CREATE OR REPLACE FUNCTION insert_command_and_event_with_row_versioning(
    p_command stockydb.command_insert,
    p_event stockydb.event_insert_with_seq_id
)
RETURNS INTEGER
LANGUAGE plpgsql
AS $$
DECLARE
    v_max_now INTEGER;
    v_next_now INTEGER;
BEGIN
    IF p_event.expected_next_sequence < 1 THEN
        RAISE EXCEPTION 'p_expected_next_sequence must be >= 1'
            USING ERRCODE = '22023';
    END IF;

    v_max_now := get_max_aggregate_sequence((p_event).event.aggregate_type, (p_event).event.aggregate_id);
    v_next_now := v_max_now + 1;

    IF v_next_now IS DISTINCT FROM p_event.expected_next_sequence THEN
        RAISE EXCEPTION 'row version conflict: expected next sequence %, actual next % (aggregate % / %)',
            p_event.expected_next_sequence, v_next_now, (p_event).event.aggregate_type, (p_event).event.aggregate_id
            USING ERRCODE = 'P0001';
    END IF;

    PERFORM insert_command(p_command);
    RETURN insert_event((p_event).event, v_next_now);
END;
$$;
