using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CAS.DB.Migrations
{
    public partial class SheriffEventIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SheriffLeave_User_CreatedById",
                table: "SheriffLeave");

            migrationBuilder.DropForeignKey(
                name: "FK_SheriffLeave_User_UpdatedById",
                table: "SheriffLeave");

            migrationBuilder.DropForeignKey(
                name: "FK_SheriffTraining_User_CreatedById",
                table: "SheriffTraining");

            migrationBuilder.DropForeignKey(
                name: "FK_SheriffTraining_User_UpdatedById",
                table: "SheriffTraining");

            migrationBuilder.AlterColumn<Guid>(
                name: "SheriffId",
                table: "Shift",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedOn",
                table: "SheriffTraining",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedOn",
                table: "SheriffLeave",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.CreateIndex(
                name: "IX_SheriffTraining_StartDate_EndDate",
                table: "SheriffTraining",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_SheriffLeave_StartDate_EndDate",
                table: "SheriffLeave",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_SheriffAwayLocation_StartDate_EndDate",
                table: "SheriffAwayLocation",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.AddForeignKey(
                name: "FK_SheriffLeave_User_CreatedById",
                table: "SheriffLeave",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_SheriffLeave_User_UpdatedById",
                table: "SheriffLeave",
                column: "UpdatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_SheriffTraining_User_CreatedById",
                table: "SheriffTraining",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_SheriffTraining_User_UpdatedById",
                table: "SheriffTraining",
                column: "UpdatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SheriffLeave_User_CreatedById",
                table: "SheriffLeave");

            migrationBuilder.DropForeignKey(
                name: "FK_SheriffLeave_User_UpdatedById",
                table: "SheriffLeave");

            migrationBuilder.DropForeignKey(
                name: "FK_SheriffTraining_User_CreatedById",
                table: "SheriffTraining");

            migrationBuilder.DropForeignKey(
                name: "FK_SheriffTraining_User_UpdatedById",
                table: "SheriffTraining");

            migrationBuilder.DropIndex(
                name: "IX_SheriffTraining_StartDate_EndDate",
                table: "SheriffTraining");

            migrationBuilder.DropIndex(
                name: "IX_SheriffLeave_StartDate_EndDate",
                table: "SheriffLeave");

            migrationBuilder.DropIndex(
                name: "IX_SheriffAwayLocation_StartDate_EndDate",
                table: "SheriffAwayLocation");

            migrationBuilder.AlterColumn<Guid>(
                name: "SheriffId",
                table: "Shift",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedOn",
                table: "SheriffTraining",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedOn",
                table: "SheriffLeave",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AddForeignKey(
                name: "FK_SheriffLeave_User_CreatedById",
                table: "SheriffLeave",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SheriffLeave_User_UpdatedById",
                table: "SheriffLeave",
                column: "UpdatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SheriffTraining_User_CreatedById",
                table: "SheriffTraining",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SheriffTraining_User_UpdatedById",
                table: "SheriffTraining",
                column: "UpdatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
