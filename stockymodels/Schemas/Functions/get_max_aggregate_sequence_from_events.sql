SET search_path TO stockydb;

CREATE OR REPLACE FUNCTION get_max_aggregate_sequence_from_events(
    p_aggregate_type VARCHAR(32),
    p_aggregate_id uuid
)
RETURNS INTEGER
LANGUAGE sql
STABLE
AS $$
    SELECT COALESCE(MAX(e.aggregate_sequence_id), 0)::integer
    FROM stockydb.events e
    WHERE e.aggregate_type = p_aggregate_type
      AND e.aggregate_id = p_aggregate_id;
$$;
