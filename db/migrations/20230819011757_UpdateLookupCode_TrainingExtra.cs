using Microsoft.EntityFrameworkCore.Migrations;

namespace CAS.DB.Migrations
{
    public partial class UpdateLookupCode_TrainingExtra : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdvanceNotice",
                table: "LookupCode",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "LookupCode",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Mandatory",
                table: "LookupCode",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ValidityPeriod",
                table: "LookupCode",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdvanceNotice",
                table: "LookupCode");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "LookupCode");

            migrationBuilder.DropColumn(
                name: "Mandatory",
                table: "LookupCode");

            migrationBuilder.DropColumn(
                name: "ValidityPeriod",
                table: "LookupCode");
        }
    }
}
