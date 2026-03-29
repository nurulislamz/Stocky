SET search_path TO stockydb;

CREATE OR REPLACE FUNCTION get_aggregate_version(
    p_aggregate_type VARCHAR(32),
    p_aggregate_id uuid
)
RETURNS INTEGER
LANGUAGE sql
STABLE
AS $$
    SELECT COALESCE(
        (SELECT v.current_seq_id
         FROM stockydb.aggregate_version v
         WHERE v.aggregate_type = p_aggregate_type
           AND v.aggregate_id = p_aggregate_id),
        0
    )::integer;
$$;
