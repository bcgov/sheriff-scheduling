using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CAS.DB.Migrations
{
    public partial class LocationDeleteSetNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LookupCode_Location_LocationId",
                table: "LookupCode");

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

            migrationBuilder.AddForeignKey(
                name: "FK_LookupCode_Location_LocationId",
                table: "LookupCode",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LookupCode_Location_LocationId",
                table: "LookupCode");

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 5, 19, 37, 40, 650, DateTimeKind.Utc).AddTicks(841));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 5, 19, 37, 40, 650, DateTimeKind.Utc).AddTicks(1969));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 5, 19, 37, 40, 650, DateTimeKind.Utc).AddTicks(2002));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 5, 19, 37, 40, 650, DateTimeKind.Utc).AddTicks(2004));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 5, 19, 37, 40, 650, DateTimeKind.Utc).AddTicks(2006));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 5, 19, 37, 40, 650, DateTimeKind.Utc).AddTicks(2007));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 5, 19, 37, 40, 650, DateTimeKind.Utc).AddTicks(2008));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 5, 19, 37, 40, 653, DateTimeKind.Utc).AddTicks(1086));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 5, 19, 37, 40, 655, DateTimeKind.Utc).AddTicks(7819));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 5, 19, 37, 40, 655, DateTimeKind.Utc).AddTicks(8690));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 5, 19, 37, 40, 655, DateTimeKind.Utc).AddTicks(8711));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                column: "CreatedOn",
                value: new DateTime(2020, 10, 5, 12, 37, 40, 663, DateTimeKind.Local).AddTicks(4121));

            migrationBuilder.AddForeignKey(
                name: "FK_LookupCode_Location_LocationId",
                table: "LookupCode",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
