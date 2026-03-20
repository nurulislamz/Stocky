-- Plain: insert_command once, then one insert_event per row. Each event gets the next sequence
-- for its own (aggregate_type, aggregate_id), matching PostgresEventStore.InsertMultipleEventsWithSequencing.
SET search_path TO stockydb;

CREATE OR REPLACE FUNCTION insert_command_and_multiple_events(
    p_command stockydb.command_insert,
    p_events stockydb.event_insert[]
)
RETURNS INTEGER
LANGUAGE plpgsql
AS $$
DECLARE
    v_next_seq INTEGER;
    v_event stockydb.event_insert;
BEGIN
    IF p_events IS NULL OR COALESCE(array_length(p_events, 1), 0) < 1 THEN
        RAISE EXCEPTION 'p_events must contain at least one event'
            USING ERRCODE = '22023';
    END IF;

    PERFORM insert_command(p_command);

    FOREACH v_event IN ARRAY p_events
    LOOP
        v_next_seq := get_max_aggregate_sequence(v_event.aggregate_type, v_event.aggregate_id) + 1;
        v_next_seq := insert_event(v_event, v_next_seq);
    END LOOP;

    RETURN v_next_seq;
END;
$$;
