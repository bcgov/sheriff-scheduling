using Microsoft.EntityFrameworkCore.Migrations;

namespace CAS.DB.Migrations
{
    public partial class Indexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Shift_StartDate_EndDate",
                table: "Shift",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Duty_StartDate_EndDate",
                table: "Duty",
                columns: new[] { "StartDate", "EndDate" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Shift_StartDate_EndDate",
                table: "Shift");

            migrationBuilder.DropIndex(
                name: "IX_Duty_StartDate_EndDate",
                table: "Duty");
        }
    }
}
