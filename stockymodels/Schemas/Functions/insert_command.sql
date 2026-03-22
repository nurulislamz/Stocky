SET search_path TO stockydb;

CREATE OR REPLACE FUNCTION insert_command(
    p stockydb.command_insert
)
RETURNS TABLE (
    command_id uuid,
    user_id uuid,
    trace_id uuid,
    db_stored_at_time timestamptz
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_db_stored_at_time timestamptz;
BEGIN
    v_db_stored_at_time := pg_catalog.clock_timestamp();

    INSERT INTO stockydb."Commands" (
        "CommandId",
        "UserId",
        "CommandType",
        "CommandPayloadJson",
        "TtStart",
        "TtEnd",
        "TraceId",
        "DbStoredAtTime"
    ) VALUES (
        p.command_id,
        p.user_id,
        p.command_type,
        p.command_payload_json,
        p.tt_start,
        p.tt_end,
        p.trace_id,
        v_db_stored_at_time
    );

    RETURN QUERY SELECT p.command_id, p.user_id, p.trace_id, v_db_stored_at_time;
END;
$$;