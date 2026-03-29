CREATE SCHEMA stockydb;
SET search_path TO stockydb;

-- Commands must exist before Events (Events references Commands)
CREATE TABLE commands (
    command_id UUID PRIMARY KEY,
    user_id UUID NOT NULL,
    command_type VARCHAR(32) NOT NULL,
    command_payload_json JSONB NOT NULL,
    tt_start TIMESTAMPTZ NOT NULL,
    tt_end TIMESTAMPTZ NOT NULL,
    trace_id UUID NULL,
    db_stored_at_time TIMESTAMPTZ NOT NULL
);

CREATE INDEX ix_commands_trace_id ON commands (trace_id);

CREATE TABLE events (
    event_id UUID PRIMARY KEY,
    user_id UUID NOT NULL,
    aggregate_type VARCHAR(32) NOT NULL,
    aggregate_id VARCHAR(32) NOT NULL,
    aggregate_sequence_id INTEGER NOT NULL,
    event_type VARCHAR(32) NOT NULL,
    event_payload_json JSONB NOT NULL,
    tt_start TIMESTAMPTZ NOT NULL,
    tt_end TIMESTAMPTZ NOT NULL,
    valid_from TIMESTAMPTZ NOT NULL,
    valid_to TIMESTAMPTZ NOT NULL,
    command_id UUID NOT NULL,
    trace_id UUID NOT NULL,
    db_stored_at_time TIMESTAMPTZ NOT NULL,
    CONSTRAINT uq_events_aggregate_id
        UNIQUE (aggregate_id),
    CONSTRAINT uq_events_aggregate_type_id_sequence
        UNIQUE (aggregate_type, aggregate_id, aggregate_sequence_id),
    CONSTRAINT fk_events_commands_command_id
        FOREIGN KEY (command_id) REFERENCES commands (command_id)
        ON DELETE restrict,
    CHECK (aggregate_sequence_id > 0),
	CHECK (tt_start <= tt_end),
	CHECK (valid_from <= valid_to)
);

CREATE INDEX ix_events_aggregate_id ON events (aggregate_id);
CREATE INDEX ix_events_user_id ON events (user_id);

ALTER TABLE events ENABLE ROW LEVEL SECURITY;

CREATE TABLE aggregate_version (
    aggregate_type VARCHAR(32) NOT NULL,
    aggregate_id VARCHAR(32) NOT NULL,
    current_seq_id INTEGER NOT NULL DEFAULT 0,
    updated_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    CONSTRAINT pk_aggregate_version PRIMARY KEY (aggregate_type, aggregate_id),
    CONSTRAINT ck_aggregate_version_non_negative CHECK (current_seq_id > 0)
);

CREATE POLICY event_user_policy ON events
    FOR ALL
    USING (user_id = current_setting('app.user_id', true)::uuid);