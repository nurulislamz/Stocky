-- Plain: no extra wrapper lock or prefetch; relies on insert_command + insert_event only.
-- insert_event still uses pg_advisory_xact_lock internally for sequence assignment.
SET search_path TO stockydb;

CREATE OR REPLACE FUNCTION insert_command_and_event(
    p_command stockydb.command_insert,
    p_event stockydb.event_insert
)
RETURNS INTEGER
LANGUAGE plpgsql
AS $$
DECLARE
    v_max_seq INTEGER;
BEGIN
    v_max_seq := get_max_aggregate_sequence(p_event.aggregate_type, p_event.aggregate_id);

    PERFORM insert_command(p_command);
    RETURN insert_event(p_event, v_max_seq + 1);
END;
$$;
