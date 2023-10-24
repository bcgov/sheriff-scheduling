using Microsoft.EntityFrameworkCore.Migrations;

namespace CAS.DB.Migrations
{
    public partial class RemoveShiftDutySlotAssociation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DutySlot_Shift_ShiftId",
                table: "DutySlot");

            migrationBuilder.DropIndex(
                name: "IX_DutySlot_ShiftId",
                table: "DutySlot");

            migrationBuilder.DropColumn(
                name: "ShiftId",
                table: "DutySlot");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShiftId",
                table: "DutySlot",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DutySlot_ShiftId",
                table: "DutySlot",
                column: "ShiftId");

            migrationBuilder.AddForeignKey(
                name: "FK_DutySlot_Shift_ShiftId",
                table: "DutySlot",
                column: "ShiftId",
                principalTable: "Shift",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
