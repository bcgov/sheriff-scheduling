using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace CAS.DB.Migrations
{
    public partial class UpdateUserConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Location",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Location",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "UserRole",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:IdentitySequenceOptions", "'100', '1', '', '', 'False', '1'")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.InsertData(
                table: "Location",
                columns: new[] { "Id", "AgencyId", "CreatedById", "CreatedOn", "ExpiryDate", "JustinCode", "Name", "ParentLocationId", "RegionId", "UpdatedById", "UpdatedOn" },
                values: new object[,]
                {
                    { -1, "FAKE", null, new DateTime(2020, 10, 2, 20, 24, 27, 795, DateTimeKind.Utc).AddTicks(8309), null, null, "Dummy Location", null, null, null, null },
                    { -2, "FAKE2", null, new DateTime(2020, 10, 2, 20, 24, 27, 795, DateTimeKind.Utc).AddTicks(9545), null, null, "Dummy Location2", null, null, null, null }
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Location",
                keyColumn: "Id",
                keyValue: -2);

            migrationBuilder.DeleteData(
                table: "Location",
                keyColumn: "Id",
                keyValue: -1);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "UserRole",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:IdentitySequenceOptions", "'100', '1', '', '', 'False', '1'")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.InsertData(
                table: "Location",
                columns: new[] { "Id", "AgencyId", "CreatedById", "CreatedOn", "ExpiryDate", "JustinCode", "Name", "ParentLocationId", "RegionId", "UpdatedById", "UpdatedOn" },
                values: new object[,]
                {
                    { 1, "FAKE", null, new DateTime(2020, 10, 2, 3, 30, 43, 924, DateTimeKind.Utc).AddTicks(7930), null, null, "Dummy Location", null, null, null, null },
                    { 2, "FAKE2", null, new DateTime(2020, 10, 2, 3, 30, 43, 924, DateTimeKind.Utc).AddTicks(9186), null, null, "Dummy Location2", null, null, null, null }
                });

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 2, 3, 30, 43, 937, DateTimeKind.Utc).AddTicks(6582));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 2, 3, 30, 43, 937, DateTimeKind.Utc).AddTicks(7471));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 2, 3, 30, 43, 937, DateTimeKind.Utc).AddTicks(7493));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 2, 3, 30, 43, 937, DateTimeKind.Utc).AddTicks(7494));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 2, 3, 30, 43, 937, DateTimeKind.Utc).AddTicks(7496));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 2, 3, 30, 43, 937, DateTimeKind.Utc).AddTicks(7497));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 2, 3, 30, 43, 937, DateTimeKind.Utc).AddTicks(7498));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 2, 3, 30, 43, 939, DateTimeKind.Utc).AddTicks(9870));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 2, 3, 30, 43, 942, DateTimeKind.Utc).AddTicks(2756));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 2, 3, 30, 43, 942, DateTimeKind.Utc).AddTicks(3683));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 2, 3, 30, 43, 942, DateTimeKind.Utc).AddTicks(3716));
        }
    }
}
