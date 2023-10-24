using System.IO;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CAS.DB.Migrations
{
    public partial class UpdatePermissions_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var dbPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "migrations/20201209182004_UpdatePermissions_2.sql");
            migrationBuilder.Sql(File.ReadAllText(dbPath));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
