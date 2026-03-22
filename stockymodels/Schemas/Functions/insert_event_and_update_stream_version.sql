SET search_path TO stockydb;

-- Inserts one event row; returns the stored event id and assigned aggregate sequence.
CREATE OR REPLACE FUNCTION insert_event_and_update_stream_version(p stockydb.event_insert, v_next_seq int)
RETURNS TABLE (
    event_id uuid,
    user_id uuid,
    aggregate_id uuid,
    aggregate_sequence_id integer,
    command_id uuid,
    trace_id uuid,
    db_stored_at_time timestamptz
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_db_stored_at_time timestamptz;
BEGIN
    v_db_stored_at_time := pg_catalog.clock_timestamp();


    INSERT INTO stockydb."Events" (
        "EventId", "UserId", "AggregateType", "AggregateId",
        "AggregateSequenceId", "EventType", "EventPayloadJson",
        "TtStart", "TtEnd", "ValidFrom", "ValidTo", "CommandId", "TraceId", "DbStoredAtTime"
    ) VALUES (
        p.event_id, p.user_id, p.aggregate_type, p.aggregate_id,
        v_next_seq, p.event_type, p.event_payload_json,
        p.tt_start, p.tt_end, p.valid_from, p.valid_to, p.command_id, p.trace_id, v_db_stored_at_time
    );
    PERFORM insert_or_update_aggregate_version(p.aggregate_type, p.aggregate_id, v_next_seq);

    RETURN QUERY SELECT p.event_id, p.user_id, p.aggregate_id, v_next_seq, p.command_id, p.trace_id, v_db_stored_at_time;
END;
$$;
