using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace stockymodels.Migrations
{
    /// <summary>
    /// Adds a PostgreSQL trigger so the Events table is append-only at the database level.
    /// UPDATE and DELETE on "Events" raise an error. Only runs when using Npgsql; SQLite relies on app-level enforcement.
    /// </summary>
    public partial class EventsAppendOnlyTrigger : Migration
    {
        private const string NpgsqlProvider = "Npgsql.EntityFrameworkCore.PostgreSQL";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder.ActiveProvider != NpgsqlProvider)
                return;

            migrationBuilder.Sql(@"
CREATE OR REPLACE FUNCTION prevent_events_update_delete()
RETURNS TRIGGER AS $func$
BEGIN
  RAISE EXCEPTION 'Events table is append-only: updates and deletes are not allowed';
END;
$func$ LANGUAGE plpgsql;
");

            migrationBuilder.Sql(@"
DO $mig$
BEGIN
  IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'Events') THEN
    DROP TRIGGER IF EXISTS events_append_only_trigger ON ""Events"";
    CREATE TRIGGER events_append_only_trigger
    BEFORE UPDATE OR DELETE ON ""Events""
    FOR EACH ROW EXECUTE PROCEDURE prevent_events_update_delete();
  END IF;
END $mig$;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder.ActiveProvider != NpgsqlProvider)
                return;

            migrationBuilder.Sql(@"
DO $mig$
BEGIN
  IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'Events') THEN
    DROP TRIGGER IF EXISTS events_append_only_trigger ON ""Events"";
  END IF;
END $mig$;
DROP FUNCTION IF EXISTS prevent_events_update_delete();
");
        }
    }
}
