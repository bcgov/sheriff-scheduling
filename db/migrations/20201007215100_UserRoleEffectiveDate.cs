using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CAS.DB.Migrations
{
    public partial class UserRoleEffectiveDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "EffectiveDate",
                table: "UserRole",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ExpiryDate",
                table: "UserRole",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 7, 21, 50, 59, 303, DateTimeKind.Utc).AddTicks(8575));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 7, 21, 50, 59, 303, DateTimeKind.Utc).AddTicks(9810));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 7, 21, 50, 59, 303, DateTimeKind.Utc).AddTicks(9842));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 7, 21, 50, 59, 303, DateTimeKind.Utc).AddTicks(9843));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 7, 21, 50, 59, 303, DateTimeKind.Utc).AddTicks(9844));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 7, 21, 50, 59, 303, DateTimeKind.Utc).AddTicks(9846));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 7, 21, 50, 59, 303, DateTimeKind.Utc).AddTicks(9847));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 7, 21, 50, 59, 307, DateTimeKind.Utc).AddTicks(300));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 7, 21, 50, 59, 309, DateTimeKind.Utc).AddTicks(3233));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 7, 21, 50, 59, 309, DateTimeKind.Utc).AddTicks(4185));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 7, 21, 50, 59, 309, DateTimeKind.Utc).AddTicks(4205));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                column: "CreatedOn",
                value: new DateTime(2020, 10, 7, 14, 50, 59, 317, DateTimeKind.Local).AddTicks(1196));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EffectiveDate",
                table: "UserRole");

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "UserRole");

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 6, 21, 56, 55, 976, DateTimeKind.Utc).AddTicks(8752));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 6, 21, 56, 55, 977, DateTimeKind.Utc).AddTicks(3));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 6, 21, 56, 55, 977, DateTimeKind.Utc).AddTicks(34));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 6, 21, 56, 55, 977, DateTimeKind.Utc).AddTicks(36));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 6, 21, 56, 55, 977, DateTimeKind.Utc).AddTicks(37));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 6, 21, 56, 55, 977, DateTimeKind.Utc).AddTicks(38));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 6, 21, 56, 55, 977, DateTimeKind.Utc).AddTicks(40));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 6, 21, 56, 55, 979, DateTimeKind.Utc).AddTicks(6787));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 6, 21, 56, 55, 981, DateTimeKind.Utc).AddTicks(8209));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 6, 21, 56, 55, 981, DateTimeKind.Utc).AddTicks(9159));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 6, 21, 56, 55, 981, DateTimeKind.Utc).AddTicks(9181));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                column: "CreatedOn",
                value: new DateTime(2020, 10, 6, 14, 56, 55, 989, DateTimeKind.Local).AddTicks(3226));
        }
    }
}
