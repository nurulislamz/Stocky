using System.Reflection;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace stockymodels.Migrations
{
    public partial class _002CreateProjections : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sql = ReadEmbeddedSql("002CreateProjections.sql");
            migrationBuilder.Sql(sql);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var sql = ReadEmbeddedSql("002CreateProjectionsRevert.sql");
            migrationBuilder.Sql(sql);
        }

        private static string ReadEmbeddedSql(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"stockymodels.Migrations.{fileName}";
            using var stream = assembly.GetManifestResourceStream(resourceName)
                ?? throw new InvalidOperationException($"Embedded resource '{resourceName}' not found.");
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
