-- Matches PostgresEventStore.WithAdvisoryLockMulti: insert_command first, then one advisory lock
-- per distinct (aggregate_type, aggregate_id) in sorted order, then insert_event per row with
-- per-stream next sequence (get_max + 1 after prior inserts in this txn).
SET search_path TO stockydb;

CREATE OR REPLACE FUNCTION insert_command_and_multiple_events_with_advisory_locks(
    p_command stockydb.command_insert,
    p_events stockydb.event_insert[]
)
RETURNS INTEGER
LANGUAGE plpgsql
AS $$
DECLARE
    v_next_seq INTEGER;
    v_event stockydb.event_insert;
    rec RECORD;
BEGIN
    IF p_events IS NULL OR COALESCE(array_length(p_events, 1), 0) < 1 THEN
        RAISE EXCEPTION 'p_events must contain at least one event'
            USING ERRCODE = '22023';
    END IF;

    PERFORM insert_command(p_command);

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
        v_next_seq := insert_event(v_event, v_next_seq);
    END LOOP;

    RETURN v_next_seq;
END;
$$;
