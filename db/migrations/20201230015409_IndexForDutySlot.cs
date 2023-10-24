using Microsoft.EntityFrameworkCore.Migrations;

namespace CAS.DB.Migrations
{
    public partial class IndexForDutySlot : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_DutySlot_StartDate_EndDate",
                table: "DutySlot",
                columns: new[] { "StartDate", "EndDate" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DutySlot_StartDate_EndDate",
                table: "DutySlot");
        }
    }
}
