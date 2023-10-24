using Microsoft.EntityFrameworkCore.Migrations;

namespace CAS.DB.Migrations
{
    public partial class ExtraPermissions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Permission",
                columns: new[] { "Id", "CreatedById", "Description", "Name", "UpdatedById", "UpdatedOn" },
                values: new object[,]
                {
                    { 40, null, "View DutyRoster in the future", "ViewDutyRosterInFuture", null, null },
                    { 41, null, "View Shifts in the future (not time constrained)", "ViewAllFutureShifts", null, null },
                    { 42, null, "View other profiles (beside their own)", "ViewOtherProfiles", null, null }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 42);
        }
    }
}
