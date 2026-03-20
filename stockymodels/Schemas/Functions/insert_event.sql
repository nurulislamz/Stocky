SET search_path TO stockydb;

-- Assigns next AggregateSequenceId (transaction advisory lock + max sequence).
CREATE OR REPLACE FUNCTION insert_event(p stockydb.event_insert, v_next_seq int)
RETURNS INTEGER
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO stockydb."Events" (
        "EventId", "UserId", "AggregateType", "AggregateId",
        "AggregateSequenceId", "EventType", "EventPayloadJson",
        "TtStart", "TtEnd", "ValidFrom", "ValidTo", "CommandId", "TraceId"
    ) VALUES (
        p.event_id, p.user_id, p.aggregate_type, p.aggregate_id,
        v_next_seq, p.event_type, p.event_payload_json,
        p.tt_start, p.tt_end, p.valid_from, p.valid_to, p.command_id, p.trace_id
    );

    RETURN v_next_seq;
END;
$$;
