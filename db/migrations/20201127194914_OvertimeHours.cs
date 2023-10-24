using Microsoft.EntityFrameworkCore.Migrations;

namespace CAS.DB.Migrations
{
    public partial class OvertimeHours : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOvertime",
                table: "Shift");

            migrationBuilder.AddColumn<double>(
                name: "OvertimeHours",
                table: "Shift",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OvertimeHours",
                table: "Shift");

            migrationBuilder.AddColumn<bool>(
                name: "IsOvertime",
                table: "Shift",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
