using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CAS.DB.Migrations
{
    public partial class KeyCloakIdAndMore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permission_User_UserId",
                table: "Permission");

            migrationBuilder.DropIndex(
                name: "IX_Permission_UserId",
                table: "Permission");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Permission");

            migrationBuilder.AlterColumn<Guid>(
                name: "IdirId",
                table: "User",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "KeyCloakId",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "CourtAdminTraining",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "CourtAdminLeave",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "CourtAdminAwayLocation",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Location",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 2, 3, 30, 43, 924, DateTimeKind.Utc).AddTicks(7930));

            migrationBuilder.InsertData(
                table: "Location",
                columns: new[] { "Id", "AgencyId", "CreatedById", "CreatedOn", "ExpiryDate", "JustinCode", "Name", "ParentLocationId", "RegionId", "UpdatedById", "UpdatedOn" },
                values: new object[] { 2, "FAKE2", null, new DateTime(2020, 10, 2, 3, 30, 43, 924, DateTimeKind.Utc).AddTicks(9186), null, null, "Dummy Location2", null, null, null, null });

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
                columns: new[] { "CreatedOn", "Name" },
                values: new object[] { new DateTime(2020, 10, 2, 3, 30, 43, 942, DateTimeKind.Utc).AddTicks(2756), "System Administrator" });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Location",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DropColumn(
                name: "KeyCloakId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "CourtAdminTraining");

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "CourtAdminLeave");

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "CourtAdminAwayLocation");

            migrationBuilder.AlterColumn<Guid>(
                name: "IdirId",
                table: "User",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Permission",
                type: "uuid",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Location",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2020, 9, 30, 19, 1, 26, 237, DateTimeKind.Utc).AddTicks(8500));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2020, 9, 30, 19, 1, 26, 243, DateTimeKind.Utc).AddTicks(8473));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2020, 9, 30, 19, 1, 26, 243, DateTimeKind.Utc).AddTicks(9345));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2020, 9, 30, 19, 1, 26, 243, DateTimeKind.Utc).AddTicks(9369));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2020, 9, 30, 19, 1, 26, 243, DateTimeKind.Utc).AddTicks(9371));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedOn",
                value: new DateTime(2020, 9, 30, 19, 1, 26, 243, DateTimeKind.Utc).AddTicks(9372));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedOn",
                value: new DateTime(2020, 9, 30, 19, 1, 26, 243, DateTimeKind.Utc).AddTicks(9406));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedOn",
                value: new DateTime(2020, 9, 30, 19, 1, 26, 243, DateTimeKind.Utc).AddTicks(9408));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2020, 9, 30, 19, 1, 26, 245, DateTimeKind.Utc).AddTicks(9203));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedOn", "Name" },
                values: new object[] { new DateTime(2020, 9, 30, 19, 1, 26, 250, DateTimeKind.Utc).AddTicks(5647), "SystemAdministrator" });

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2020, 9, 30, 19, 1, 26, 250, DateTimeKind.Utc).AddTicks(6516));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2020, 9, 30, 19, 1, 26, 250, DateTimeKind.Utc).AddTicks(6537));

            migrationBuilder.CreateIndex(
                name: "IX_Permission_UserId",
                table: "Permission",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Permission_User_UserId",
                table: "Permission",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
