using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CAS.DB.Migrations
{
    public partial class CourtAdminEventIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourtAdminLeave_User_CreatedById",
                table: "CourtAdminLeave");

            migrationBuilder.DropForeignKey(
                name: "FK_CourtAdminLeave_User_UpdatedById",
                table: "CourtAdminLeave");

            migrationBuilder.DropForeignKey(
                name: "FK_CourtAdminTraining_User_CreatedById",
                table: "CourtAdminTraining");

            migrationBuilder.DropForeignKey(
                name: "FK_CourtAdminTraining_User_UpdatedById",
                table: "CourtAdminTraining");

            migrationBuilder.AlterColumn<Guid>(
                name: "CourtAdminId",
                table: "Shift",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedOn",
                table: "CourtAdminTraining",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedOn",
                table: "CourtAdminLeave",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.CreateIndex(
                name: "IX_CourtAdminTraining_StartDate_EndDate",
                table: "CourtAdminTraining",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CourtAdminLeave_StartDate_EndDate",
                table: "CourtAdminLeave",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CourtAdminAwayLocation_StartDate_EndDate",
                table: "CourtAdminAwayLocation",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.AddForeignKey(
                name: "FK_CourtAdminLeave_User_CreatedById",
                table: "CourtAdminLeave",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CourtAdminLeave_User_UpdatedById",
                table: "CourtAdminLeave",
                column: "UpdatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CourtAdminTraining_User_CreatedById",
                table: "CourtAdminTraining",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CourtAdminTraining_User_UpdatedById",
                table: "CourtAdminTraining",
                column: "UpdatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourtAdminLeave_User_CreatedById",
                table: "CourtAdminLeave");

            migrationBuilder.DropForeignKey(
                name: "FK_CourtAdminLeave_User_UpdatedById",
                table: "CourtAdminLeave");

            migrationBuilder.DropForeignKey(
                name: "FK_CourtAdminTraining_User_CreatedById",
                table: "CourtAdminTraining");

            migrationBuilder.DropForeignKey(
                name: "FK_CourtAdminTraining_User_UpdatedById",
                table: "CourtAdminTraining");

            migrationBuilder.DropIndex(
                name: "IX_CourtAdminTraining_StartDate_EndDate",
                table: "CourtAdminTraining");

            migrationBuilder.DropIndex(
                name: "IX_CourtAdminLeave_StartDate_EndDate",
                table: "CourtAdminLeave");

            migrationBuilder.DropIndex(
                name: "IX_CourtAdminAwayLocation_StartDate_EndDate",
                table: "CourtAdminAwayLocation");

            migrationBuilder.AlterColumn<Guid>(
                name: "CourtAdminId",
                table: "Shift",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedOn",
                table: "CourtAdminTraining",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedOn",
                table: "CourtAdminLeave",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AddForeignKey(
                name: "FK_CourtAdminLeave_User_CreatedById",
                table: "CourtAdminLeave",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CourtAdminLeave_User_UpdatedById",
                table: "CourtAdminLeave",
                column: "UpdatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CourtAdminTraining_User_CreatedById",
                table: "CourtAdminTraining",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CourtAdminTraining_User_UpdatedById",
                table: "CourtAdminTraining",
                column: "UpdatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
