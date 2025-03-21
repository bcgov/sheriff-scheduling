using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SS.Db.Migrations
{
    public partial class AddLocationRegion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "JustinId",
                table: "Region",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.InsertData(
                table: "Permission",
                columns: new[] { "Id", "CreatedById", "Description", "Name", "UpdatedById", "UpdatedOn" },
                values: new object[] { 45, null, "Exempt from Training", "ExemptFromTraining", null, null });

            migrationBuilder.InsertData(
                table: "Region",
                columns: new[] { "Id", "Code", "CreatedById", "ExpiryDate", "JustinId", "Name", "UpdatedById", "UpdatedOn" },
                values: new object[,]
                {
                    { 100, null, new Guid("00000000-0000-0000-0000-000000000001"), null, null, "Central Programs", null, null },
                    { 101, null, new Guid("00000000-0000-0000-0000-000000000001"), null, null, "Office of the Chief Sheriff", null, null }
                });

            migrationBuilder.UpdateData(
                table: "Location",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Name", "RegionId" },
                values: new object[] { "Integrated Threat Assessment Unit", 100 });

            migrationBuilder.UpdateData(
                table: "Location",
                keyColumn: "Id",
                keyValue: 5,
                column: "RegionId",
                value: 101);

            migrationBuilder.InsertData(
                table: "Location",
                columns: new[] { "Id", "AgencyId", "CreatedById", "ExpiryDate", "JustinCode", "Name", "ParentLocationId", "RegionId", "Timezone", "UpdatedById", "UpdatedOn" },
                values: new object[,]
                {
                    { 7, "SS7", new Guid("00000000-0000-0000-0000-000000000001"), null, null, "Training Section", null, 100, "America/Vancouver", null, null },
                    { 9, "SS9", new Guid("00000000-0000-0000-0000-000000000001"), null, null, "Recruitment Office", null, 100, "America/Vancouver", null, null },
                    { 10, "SS10", new Guid("00000000-0000-0000-0000-000000000001"), null, null, "Provincial Programs", null, 100, "America/Vancouver", null, null }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Location",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Location",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Location",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "Region",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Region",
                keyColumn: "Id",
                keyValue: 100);

            migrationBuilder.AlterColumn<int>(
                name: "JustinId",
                table: "Region",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Location",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Name", "RegionId" },
                values: new object[] { "ITAU", null });

            migrationBuilder.UpdateData(
                table: "Location",
                keyColumn: "Id",
                keyValue: 5,
                column: "RegionId",
                value: null);
        }
    }
}
