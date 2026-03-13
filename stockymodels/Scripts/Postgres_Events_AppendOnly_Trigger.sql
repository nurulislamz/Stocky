-- Append-only enforcement for the Events table (PostgreSQL).
-- Run this manually if the migration was skipped, or to re-apply after dropping.
-- Requires the "Events" table to exist.

CREATE OR REPLACE FUNCTION prevent_events_update_delete()
RETURNS TRIGGER AS $func$
BEGIN
  RAISE EXCEPTION 'Events table is append-only: updates and deletes are not allowed';
END;
$func$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS events_append_only_trigger ON "Events";
CREATE TRIGGER events_append_only_trigger
BEFORE UPDATE OR DELETE ON "Events"
FOR EACH ROW EXECUTE PROCEDURE prevent_events_update_delete();
