using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CAS.DB.Migrations
{
    public partial class CourtAdminActingRankChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourtAdminActingRank_User_CreatedById",
                table: "CourtAdminActingRank");

            migrationBuilder.DropForeignKey(
                name: "FK_CourtAdminActingRank_User_UpdatedById",
                table: "CourtAdminActingRank");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedOn",
                table: "CourtAdminActingRank",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.CreateIndex(
                name: "IX_CourtAdminActingRank_StartDate_EndDate",
                table: "CourtAdminActingRank",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.AddForeignKey(
                name: "FK_CourtAdminActingRank_User_CreatedById",
                table: "CourtAdminActingRank",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CourtAdminActingRank_User_UpdatedById",
                table: "CourtAdminActingRank",
                column: "UpdatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourtAdminActingRank_User_CreatedById",
                table: "CourtAdminActingRank");

            migrationBuilder.DropForeignKey(
                name: "FK_CourtAdminActingRank_User_UpdatedById",
                table: "CourtAdminActingRank");

            migrationBuilder.DropIndex(
                name: "IX_CourtAdminActingRank_StartDate_EndDate",
                table: "CourtAdminActingRank");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedOn",
                table: "CourtAdminActingRank",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AddForeignKey(
                name: "FK_CourtAdminActingRank_User_CreatedById",
                table: "CourtAdminActingRank",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CourtAdminActingRank_User_UpdatedById",
                table: "CourtAdminActingRank",
                column: "UpdatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
