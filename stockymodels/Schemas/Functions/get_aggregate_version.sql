SET search_path TO stockydb;

CREATE OR REPLACE FUNCTION get_aggregate_version(
    p_aggregate_type VARCHAR(32),
    p_aggregate_id TEXT
)
RETURNS INTEGER
LANGUAGE sql
STABLE
AS $$
    SELECT v."CurrentSeqId"
    FROM stockydb."AggregateVersion" v
    WHERE v."AggregateType" = p_aggregate_type
      AND v."AggregateId" = p_aggregate_id;
$$;
