CREATE SCHEMA stockydb;
SET search_path TO stockydb;

-- Commands must exist before Events (Events references Commands)
CREATE TABLE "Commands" (
    "CommandId" UUID PRIMARY KEY,
    "UserId" UUID NOT NULL,
    "CommandType" VARCHAR(32) NOT NULL,
    "CommandPayloadJson" JSONB NOT NULL,
    "TtStart" TIMESTAMPTZ NOT NULL,
    "TtEnd" TIMESTAMPTZ NOT NULL,
    "TraceId" UUID NULL,
    "DbStoredAtTime" TIMESTAMPTZ NOT NULL
);

CREATE INDEX "ix_commands_trace_id" ON "Commands" ("TraceId");

CREATE TABLE "Events" (
    "EventId" UUID PRIMARY KEY,
    "UserId" UUID NOT NULL,
    "AggregateType" VARCHAR(32) NOT NULL,
    "AggregateId" VARCHAR(32) NOT NULL,
    "AggregateSequenceId" INTEGER NOT NULL,
    "EventType" VARCHAR(32) NOT NULL,
    "EventPayloadJson" JSONB NOT NULL,
    "TtStart" TIMESTAMPTZ NOT NULL,
    "TtEnd" TIMESTAMPTZ NOT NULL,
    "ValidFrom" TIMESTAMPTZ NOT NULL,
    "ValidTo" TIMESTAMPTZ NOT NULL,
    "CommandId" UUID NOT NULL,
    "TraceId" UUID NOT NULL,
    "DbStoredAtTime" TIMESTAMPTZ NOT NULL,
    CONSTRAINT "uq_events_aggregate_id"
        UNIQUE ("AggregateId"),
    CONSTRAINT "uq_events_aggregate_type_id_sequence"
        UNIQUE ("AggregateType", "AggregateId", "AggregateSequenceId"),
    CONSTRAINT "fk_events_commands_command_id"
        FOREIGN KEY ("CommandId") REFERENCES "Commands" ("CommandId")
        ON DELETE restrict,
    CHECK ("AggregateSequenceId" > 0),
	CHECK ("TtStart" <= "TtEnd"),
	CHECK ("ValidFrom" <= "ValidTo")
);

CREATE INDEX "ix_events_aggregate_id" ON "Events" ("AggregateId");
CREATE INDEX "ix_events_user_id" ON "Events" ("UserId");

ALTER TABLE "Events" ENABLE ROW LEVEL SECURITY;

CREATE TABLE "AggregateVersion" (
    "AggregateType" VARCHAR(32) NOT NULL,
    "AggregateId" VARCHAR(32) NOT NULL,
    "CurrentSeqId" INTEGER NOT NULL DEFAULT 0,
    "UpdatedAt" TIMESTAMPTZ NOT NULL DEFAULT now(),
    CONSTRAINT "pk_aggregate_version" PRIMARY KEY ("AggregateType", "AggregateId"),
    CONSTRAINT "ck_aggregate_version_non_negative" CHECK ("CurrentSeqId" > 0)
);

CREATE POLICY event_user_policy ON "Events"
    FOR ALL
    USING ("UserId" = current_setting('app.user_id', true)::uuid);