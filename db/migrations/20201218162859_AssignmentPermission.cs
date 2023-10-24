using Microsoft.EntityFrameworkCore.Migrations;

namespace CAS.DB.Migrations
{
    public partial class AssignmentPermission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 33,
                column: "Name",
                value: "ViewDutyRoster");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 33,
                column: "Name",
                value: "ViewDuties");

            migrationBuilder.InsertData(
                table: "Permission",
                columns: new[] { "Id", "CreatedById", "Description", "Name", "UpdatedById", "UpdatedOn" },
                values: new object[] { 29, null, "View Assignments", "ViewAssignments", null, null });
        }
    }
}
