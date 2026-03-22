SET search_path TO stockydb;

-- Inserts one event row; returns the stored event id and assigned aggregate sequence.
CREATE OR REPLACE FUNCTION insert_or_update_aggregate_version(aggregate_type text, aggregate_id uuid, current_seq_id integer)
RETURNS VOID
LANGUAGE plpgsql
AS $$
DECLARE
    v_db_stored_at_time timestamptz;
BEGIN
    v_db_stored_at_time := pg_catalog.clock_timestamp();

    INSERT INTO stockydb."AggregateVersion" (
        "AggregateType", "AggregateId", "CurrentSeqId", "UpdatedAt"
    ) VALUES (
        aggregate_type, aggregate_id, current_seq_id, v_db_stored_at_time
    )
    ON CONFLICT ("AggregateType", "AggregateId") DO UPDATE
    SET "CurrentSeqId" = EXCLUDED."CurrentSeqId",
        "UpdatedAt" = v_db_stored_at_time;

    RETURN;
END;
$$;
