SET search_path TO stockydb;

CREATE OR REPLACE FUNCTION insert_command(p stockydb.command_insert)
RETURNS VOID
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO stockydb."Commands" (
        "CommandId", "UserId", "CommandType", "CommandPayloadJson",
        "TtStart", "TtEnd", "RequestId", "TraceId"
    ) VALUES (
        p.command_id, p.user_id, p.command_type, p.command_payload_json,
        p.tt_start, p.tt_end, p.request_id, p.trace_id
    );
END;
$$;
