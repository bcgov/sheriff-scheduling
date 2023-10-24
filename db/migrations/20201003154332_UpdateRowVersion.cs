using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CAS.DB.Migrations
{
    public partial class UpdateRowVersion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "UserRole");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "User");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "SheriffTraining");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "SheriffLeave");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "SheriffAwayLocation");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "RolePermission");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Region");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Permission");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "LookupSortOrder");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "LookupCode");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Location");

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "UserRole",
                type: "xid",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "User",
                type: "xid",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "SheriffTraining",
                type: "xid",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "SheriffLeave",
                type: "xid",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "SheriffAwayLocation",
                type: "xid",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "RolePermission",
                type: "xid",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Role",
                type: "xid",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Region",
                type: "xid",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Permission",
                type: "xid",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "LookupSortOrder",
                type: "xid",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "LookupCode",
                type: "xid",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Location",
                type: "xid",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.UpdateData(
                table: "Location",
                keyColumn: "Id",
                keyValue: -2,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 3, 15, 43, 31, 687, DateTimeKind.Utc).AddTicks(5995));

            migrationBuilder.UpdateData(
                table: "Location",
                keyColumn: "Id",
                keyValue: -1,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 3, 15, 43, 31, 687, DateTimeKind.Utc).AddTicks(4793));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 3, 15, 43, 31, 700, DateTimeKind.Utc).AddTicks(378));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 3, 15, 43, 31, 700, DateTimeKind.Utc).AddTicks(1361));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 3, 15, 43, 31, 700, DateTimeKind.Utc).AddTicks(1383));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 3, 15, 43, 31, 700, DateTimeKind.Utc).AddTicks(1385));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 3, 15, 43, 31, 700, DateTimeKind.Utc).AddTicks(1386));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 3, 15, 43, 31, 700, DateTimeKind.Utc).AddTicks(1387));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 3, 15, 43, 31, 700, DateTimeKind.Utc).AddTicks(1389));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 3, 15, 43, 31, 702, DateTimeKind.Utc).AddTicks(9060));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 3, 15, 43, 31, 705, DateTimeKind.Utc).AddTicks(5052));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 3, 15, 43, 31, 705, DateTimeKind.Utc).AddTicks(6001));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 3, 15, 43, 31, 705, DateTimeKind.Utc).AddTicks(6042));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "xmin",
                table: "UserRole");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "User");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "SheriffTraining");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "SheriffLeave");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "SheriffAwayLocation");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "RolePermission");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Region");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Permission");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "LookupSortOrder");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "LookupCode");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Location");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "UserRole",
                type: "bytea",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "User",
                type: "bytea",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "SheriffTraining",
                type: "bytea",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "SheriffLeave",
                type: "bytea",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "SheriffAwayLocation",
                type: "bytea",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "RolePermission",
                type: "bytea",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Role",
                type: "bytea",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Region",
                type: "bytea",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Permission",
                type: "bytea",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "LookupSortOrder",
                type: "bytea",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "LookupCode",
                type: "bytea",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Location",
                type: "bytea",
                rowVersion: true,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Location",
                keyColumn: "Id",
                keyValue: -2,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 2, 20, 24, 27, 795, DateTimeKind.Utc).AddTicks(9545));

            migrationBuilder.UpdateData(
                table: "Location",
                keyColumn: "Id",
                keyValue: -1,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 2, 20, 24, 27, 795, DateTimeKind.Utc).AddTicks(8309));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 2, 20, 24, 27, 808, DateTimeKind.Utc).AddTicks(1158));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 2, 20, 24, 27, 808, DateTimeKind.Utc).AddTicks(2071));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 2, 20, 24, 27, 808, DateTimeKind.Utc).AddTicks(2095));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 2, 20, 24, 27, 808, DateTimeKind.Utc).AddTicks(2096));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 2, 20, 24, 27, 808, DateTimeKind.Utc).AddTicks(2097));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 2, 20, 24, 27, 808, DateTimeKind.Utc).AddTicks(2099));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 2, 20, 24, 27, 808, DateTimeKind.Utc).AddTicks(2100));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 2, 20, 24, 27, 810, DateTimeKind.Utc).AddTicks(2302));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 2, 20, 24, 27, 812, DateTimeKind.Utc).AddTicks(4271));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 2, 20, 24, 27, 812, DateTimeKind.Utc).AddTicks(5210));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 2, 20, 24, 27, 812, DateTimeKind.Utc).AddTicks(5242));
        }
    }
}
