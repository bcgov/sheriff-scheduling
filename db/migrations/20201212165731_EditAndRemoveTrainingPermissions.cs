using Microsoft.EntityFrameworkCore.Migrations;

namespace CAS.DB.Migrations
{
    public partial class EditAndRemoveTrainingPermissions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Permission",
                columns: new[] { "Id", "CreatedById", "Description", "Name", "UpdatedById", "UpdatedOn" },
                values: new object[,]
                {
                    { 38, null, "Edit Training", "EditTraining", null, null },
                    { 39, null, "Remove Training", "RemoveTraining", null, null }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 39);
        }
    }
}
