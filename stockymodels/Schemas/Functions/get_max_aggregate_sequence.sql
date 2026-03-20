SET search_path TO stockydb;

CREATE OR REPLACE FUNCTION get_max_aggregate_sequence(
    p_aggregate_type VARCHAR(32),
    p_aggregate_id TEXT
)
RETURNS INTEGER
LANGUAGE sql
STABLE
AS $$
    SELECT COALESCE(MAX(e."AggregateSequenceId"), 0)::integer
    FROM stockydb."Events" e
    WHERE e."AggregateType" = p_aggregate_type
      AND e."AggregateId" = p_aggregate_id;
$$;
