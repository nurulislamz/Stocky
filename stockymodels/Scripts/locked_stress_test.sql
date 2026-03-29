-- Advisory lock stress test: serializes writers per aggregate
-- Run: CALL stockydb.run_locked_stress();
SET search_path TO stockydb;

CREATE OR REPLACE PROCEDURE stockydb.run_locked_stress(p_iterations int DEFAULT 50)
LANGUAGE plpgsql
AS $$
DECLARE
    v_aggregate_type int := 0;
    v_aggregate_id uuid := 'aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee'::uuid;
    i int;
    cmd_id uuid;
    v_payload jsonb;
BEGIN
    FOR i IN 1..p_iterations LOOP
        PERFORM pg_advisory_xact_lock(v_aggregate_type, hashtext(v_aggregate_id));

        cmd_id := gen_random_uuid();
        v_payload := jsonb_build_object('bar', 2, 'attempt', i);

        CALL stockydb.insert_command_and_event_test(
            cmd_id,
            gen_random_uuid(),
            'TestCommand',
            '{"foo": 1}'::jsonb,
            NOW(),
            '9999-12-31 23:59:59+00'::timestamptz,
            gen_random_uuid(),
            NULL,
            v_aggregate_type,
            'User',
            v_aggregate_id,
            0,
            v_payload,
            NOW(),
            '9999-12-31 23:59:59+00'::timestamptz
        );

        COMMIT;
        PERFORM pg_sleep(0.02);
    END LOOP;
END;
$$;

-- Run the test
CALL stockydb.run_locked_stress(50);

-- Verify no duplicate sequence IDs (empty result = success)
SELECT aggregate_sequence_id, COUNT(*) AS cnt
FROM stockydb.events
WHERE aggregate_type = 0
  AND aggregate_id = 'aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee'::uuid
GROUP BY aggregate_sequence_id
HAVING COUNT(*) > 1;
