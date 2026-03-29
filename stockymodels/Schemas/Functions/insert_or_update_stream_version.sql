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

    INSERT INTO stockydb.aggregate_version (
        aggregate_type, aggregate_id, current_seq_id, updated_at
    ) VALUES (
        aggregate_type, aggregate_id, current_seq_id, v_db_stored_at_time
    )
    ON CONFLICT (aggregate_type, aggregate_id) DO UPDATE
    SET current_seq_id = EXCLUDED.current_seq_id,
        updated_at = v_db_stored_at_time;

    RETURN;
END;
$$;
