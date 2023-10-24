using Microsoft.EntityFrameworkCore.Migrations;

namespace CAS.DB.Migrations
{
    public partial class TrainingPermission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Edit Past Training", "EditPastTraining" });

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Remove Past Training", "RemovePastTraining" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Edit Training", "EditTraining" });

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Remove Training", "RemoveTraining" });
        }
    }
}
