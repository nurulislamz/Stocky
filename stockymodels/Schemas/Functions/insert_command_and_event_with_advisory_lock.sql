-- Wrapper: advisory lock + prefetch max sequence, then insert_command + insert_event.
-- insert_event still takes the same aggregate lock and recomputes max before insert.
SET search_path TO stockydb;

CREATE OR REPLACE FUNCTION insert_command_and_event_with_advisory_lock(
    p_command stockydb.command_insert,
    p_event stockydb.event_insert
)
RETURNS INTEGER
LANGUAGE plpgsql
AS $$
DECLARE
    v_max_seq INTEGER;
BEGIN
    PERFORM pg_advisory_xact_lock(
        hashtext(p_event.aggregate_type::text),
        hashtext(p_event.aggregate_id::text)
    );
    v_max_seq := get_max_aggregate_sequence(p_event.aggregate_type, p_event.aggregate_id);

    PERFORM insert_command(p_command);
    RETURN insert_event(p_event, v_max_seq + 1);
END;
$$;
