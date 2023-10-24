using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CAS.DB.Migrations
{
    public partial class UserRoleUnique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserRole_RoleId",
                table: "UserRole");

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 9, 18, 55, 56, 449, DateTimeKind.Utc).AddTicks(2728));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 9, 18, 55, 56, 449, DateTimeKind.Utc).AddTicks(3895));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 9, 18, 55, 56, 449, DateTimeKind.Utc).AddTicks(3932));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 9, 18, 55, 56, 449, DateTimeKind.Utc).AddTicks(3933));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 9, 18, 55, 56, 449, DateTimeKind.Utc).AddTicks(3935));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 9, 18, 55, 56, 449, DateTimeKind.Utc).AddTicks(3936));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 9, 18, 55, 56, 449, DateTimeKind.Utc).AddTicks(3937));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 9, 18, 55, 56, 452, DateTimeKind.Utc).AddTicks(1707));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 9, 18, 55, 56, 454, DateTimeKind.Utc).AddTicks(4010));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 9, 18, 55, 56, 454, DateTimeKind.Utc).AddTicks(4886));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 9, 18, 55, 56, 454, DateTimeKind.Utc).AddTicks(4908));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                column: "CreatedOn",
                value: new DateTime(2020, 10, 9, 11, 55, 56, 462, DateTimeKind.Local).AddTicks(7401));

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_RoleId_UserId",
                table: "UserRole",
                columns: new[] { "RoleId", "UserId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserRole_RoleId_UserId",
                table: "UserRole");

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 9, 17, 22, 35, 471, DateTimeKind.Utc).AddTicks(3543));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 9, 17, 22, 35, 471, DateTimeKind.Utc).AddTicks(4786));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 9, 17, 22, 35, 471, DateTimeKind.Utc).AddTicks(4821));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 9, 17, 22, 35, 471, DateTimeKind.Utc).AddTicks(4823));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 9, 17, 22, 35, 471, DateTimeKind.Utc).AddTicks(4824));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 9, 17, 22, 35, 471, DateTimeKind.Utc).AddTicks(4826));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 9, 17, 22, 35, 471, DateTimeKind.Utc).AddTicks(4827));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 9, 17, 22, 35, 474, DateTimeKind.Utc).AddTicks(4235));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 9, 17, 22, 35, 476, DateTimeKind.Utc).AddTicks(6378));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 9, 17, 22, 35, 476, DateTimeKind.Utc).AddTicks(7294));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 9, 17, 22, 35, 476, DateTimeKind.Utc).AddTicks(7315));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                column: "CreatedOn",
                value: new DateTime(2020, 10, 9, 10, 22, 35, 484, DateTimeKind.Local).AddTicks(8490));

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_RoleId",
                table: "UserRole",
                column: "RoleId");
        }
    }
}
