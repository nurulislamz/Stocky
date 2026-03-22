SET search_path TO stockydb;

DROP TYPE IF EXISTS event_insert_with_seq_id CASCADE;
DROP TYPE IF EXISTS event_insert CASCADE;

CREATE TYPE event_insert AS (
    event_id uuid,
    user_id uuid,
    aggregate_type varchar(32),
    aggregate_id text,
    aggregate_sequence_id int,
    event_type varchar(32),
    event_payload_json jsonb,
    tt_start timestamptz,
    tt_end timestamptz,
    valid_from timestamptz,
    valid_to timestamptz,
    command_id uuid,
    trace_id uuid
);

CREATE TYPE event_insert_with_seq_id AS (
    event stockydb.event_insert,
    expected_next_sequence integer
);

DROP TYPE IF EXISTS command_insert CASCADE;

CREATE TYPE command_insert AS (
    command_id uuid,
    user_id uuid,
    command_type varchar(32),
    command_payload_json jsonb,
    tt_start timestamptz,
    tt_end timestamptz,
    request_id uuid,
    trace_id uuid
);

CREATE TYPE command_and_event_result AS (
    command_id uuid,
    command_user_id uuid,
    command_trace_id uuid,
    command_db_stored_at_time timestamptz,
    event_id uuid,
    event_user_id uuid,
    event_aggregate_id uuid,
    event_aggregate_sequence_id integer,
    event_command_id uuid,
    event_trace_id uuid,
    event_db_stored_at_time timestamptz
);
