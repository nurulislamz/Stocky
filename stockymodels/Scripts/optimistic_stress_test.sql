-- Optimistic locking stress test: retries on unique_violation (23505)
-- Each attempt runs in its own transaction; on conflict we ROLLBACK and retry.
-- Run: CALL stockydb.run_optimistic_stress();
SET search_path TO stockydb;

CREATE OR REPLACE PROCEDURE stockydb.run_optimistic_stress(
    p_iterations int DEFAULT 50,
    p_retry_cap int DEFAULT 10
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_aggregate_type int := 0;
    v_aggregate_id uuid := 'aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee'::uuid;
    i int;
    attempt int;
    cmd_id uuid;
    v_payload jsonb;
BEGIN
    FOR i IN 1..p_iterations LOOP
        attempt := 0;

        <<retry_loop>>
        LOOP
            BEGIN
                cmd_id := gen_random_uuid();
                v_payload := jsonb_build_object('bar', 2, 'attempt', i);

                CALL stockydb.insert_command_and_event_test(
                    cmd_id,
                    gen_random_uuid(),
                    'TestCommand'::varchar(128),
                    '{"foo": 1}'::jsonb,
                    NOW(),
                    '9999-12-31 23:59:59+00'::timestamptz,
                    gen_random_uuid(),
                    NULL::uuid,
                    v_aggregate_type,
                    'User'::varchar(32),
                    v_aggregate_id,
                    0::integer,
                    v_payload,
                    NOW(),
                    '9999-12-31 23:59:59+00'::timestamptz
                );

                EXIT retry_loop;
            EXCEPTION
                WHEN SQLSTATE '23505' THEN
                    attempt := attempt + 1;
                    IF attempt >= p_retry_cap THEN
                        RAISE EXCEPTION 'Gave up on iteration % after % retries', i, attempt;
                    END IF;
                    PERFORM pg_sleep(0.05 * attempt);
            END;
        END LOOP retry_loop;
        COMMIT;
    END LOOP;
END;
$$;

-- Run the test
CALL stockydb.run_optimistic_stress(50, 10);

-- Verify no duplicate sequence IDs (empty result = success)
SELECT aggregate_sequence_id, COUNT(*) AS cnt
FROM stockydb.events
WHERE aggregate_type = 0
  AND aggregate_id = 'aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee'::uuid
GROUP BY aggregate_sequence_id
HAVING COUNT(*) > 1;
