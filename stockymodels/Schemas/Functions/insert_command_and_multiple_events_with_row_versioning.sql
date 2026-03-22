-- Row-version append: each array element pairs event_insert with expected_next_sequence for that row.
-- Before each insert, get_max + 1 must match expected_next_sequence (prior rows in this batch are visible).
-- No advisory lock (optimistic / compare-and-append).
SET search_path TO stockydb;

CREATE OR REPLACE FUNCTION insert_command_and_multiple_events_with_row_versioning(
    p_command stockydb.command_insert,
    p_appends stockydb.event_insert_with_seq_id[]
)
RETURNS stockydb.command_and_event_result[]
LANGUAGE plpgsql
AS $$
DECLARE
    v_max_now INTEGER;
    v_next_now INTEGER;
    v_row stockydb.event_insert_with_seq_id;
    v_event stockydb.event_insert;
    v_exp INTEGER;
    v_i INTEGER;
    v_n INTEGER;
    v_command_result RECORD;
    v_event_result RECORD;
    v_results stockydb.command_and_event_result[];
BEGIN
    IF p_appends IS NULL OR COALESCE(array_length(p_appends, 1), 0) < 1 THEN
        RAISE EXCEPTION 'p_appends must contain at least one row'
            USING ERRCODE = '22023';
    END IF;

    v_n := array_length(p_appends, 1);

    v_command_result := insert_command(p_command);

    v_results := ARRAY[]::stockydb.command_and_event_result[];

    FOR v_i IN 1..v_n LOOP
        v_row := p_appends[v_i];
        v_event := v_row.event;
        v_exp := v_row.expected_next_sequence;

        if p_command.command_id != v_event.command_id then
            raise exception 'Command and event command IDs do not match';
        end if;

        if p_command.trace_id != v_event.trace_id then
            raise exception 'Command and event trace IDs do not match';
        end if;

        IF v_exp < 1 THEN
            RAISE EXCEPTION 'expected_next_sequence must be >= 1'
                USING ERRCODE = '22023';
        END IF;

        v_max_now := get_max_aggregate_sequence(v_event.aggregate_type, v_event.aggregate_id);
        v_next_now := v_max_now + 1;

        IF v_next_now IS DISTINCT FROM v_exp THEN
            RAISE EXCEPTION 'row version conflict: expected next sequence %, actual next % (aggregate % / %)',
                v_exp, v_next_now, v_event.aggregate_type, v_event.aggregate_id
                USING ERRCODE = 'P0001';
        END IF;

        v_event_result := insert_event_and_update_stream_version(v_event, v_next_now);

        v_results := array_append(
            v_results,
            ROW(
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
                v_event_result.db_stored_at_time
            )::stockydb.command_and_event_result
        );
    END LOOP;

    RETURN v_results;
END;
$$;
