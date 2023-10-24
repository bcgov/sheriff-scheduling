using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CAS.DB.Migrations
{
    public partial class SheriffActingRankChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SheriffActingRank_User_CreatedById",
                table: "SheriffActingRank");

            migrationBuilder.DropForeignKey(
                name: "FK_SheriffActingRank_User_UpdatedById",
                table: "SheriffActingRank");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedOn",
                table: "SheriffActingRank",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.CreateIndex(
                name: "IX_SheriffActingRank_StartDate_EndDate",
                table: "SheriffActingRank",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.AddForeignKey(
                name: "FK_SheriffActingRank_User_CreatedById",
                table: "SheriffActingRank",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_SheriffActingRank_User_UpdatedById",
                table: "SheriffActingRank",
                column: "UpdatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SheriffActingRank_User_CreatedById",
                table: "SheriffActingRank");

            migrationBuilder.DropForeignKey(
                name: "FK_SheriffActingRank_User_UpdatedById",
                table: "SheriffActingRank");

            migrationBuilder.DropIndex(
                name: "IX_SheriffActingRank_StartDate_EndDate",
                table: "SheriffActingRank");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedOn",
                table: "SheriffActingRank",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AddForeignKey(
                name: "FK_SheriffActingRank_User_CreatedById",
                table: "SheriffActingRank",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SheriffActingRank_User_UpdatedById",
                table: "SheriffActingRank",
                column: "UpdatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
