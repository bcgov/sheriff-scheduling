using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace CAS.DB.Migrations
{
    public partial class LocationLookupSystemUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Location_User_CreatedById",
                table: "Location");

            migrationBuilder.DropForeignKey(
                name: "FK_Location_Region_RegionId",
                table: "Location");

            migrationBuilder.DropForeignKey(
                name: "FK_Location_User_UpdatedById",
                table: "Location");

            migrationBuilder.DropForeignKey(
                name: "FK_LookupCode_User_CreatedById",
                table: "LookupCode");

            migrationBuilder.DropForeignKey(
                name: "FK_LookupCode_User_UpdatedById",
                table: "LookupCode");

            migrationBuilder.DropForeignKey(
                name: "FK_LookupSortOrder_User_CreatedById",
                table: "LookupSortOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_LookupSortOrder_User_UpdatedById",
                table: "LookupSortOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_Permission_User_CreatedById",
                table: "Permission");

            migrationBuilder.DropForeignKey(
                name: "FK_Permission_User_UpdatedById",
                table: "Permission");

            migrationBuilder.DropForeignKey(
                name: "FK_Region_User_CreatedById",
                table: "Region");

            migrationBuilder.DropForeignKey(
                name: "FK_Region_User_UpdatedById",
                table: "Region");

            migrationBuilder.DropForeignKey(
                name: "FK_Role_User_CreatedById",
                table: "Role");

            migrationBuilder.DropForeignKey(
                name: "FK_Role_User_UpdatedById",
                table: "Role");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermission_User_CreatedById",
                table: "RolePermission");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermission_User_UpdatedById",
                table: "RolePermission");

            migrationBuilder.DropForeignKey(
                name: "FK_SheriffAwayLocation_User_CreatedById",
                table: "SheriffAwayLocation");

            migrationBuilder.DropForeignKey(
                name: "FK_SheriffAwayLocation_User_UpdatedById",
                table: "SheriffAwayLocation");

            migrationBuilder.DropForeignKey(
                name: "FK_User_User_CreatedById",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Location_HomeLocationId",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_User_User_UpdatedById",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_User_CreatedById",
                table: "UserRole");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_User_UpdatedById",
                table: "UserRole");

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
                table: "RolePermission",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:IdentitySequenceOptions", "'100', '1', '', '', 'False', '1'")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "Role",
                nullable: true);

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

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "CreatedById", "CreatedOn", "Discriminator", "Email", "FirstName", "HomeLocationId", "IdirId", "IdirName", "IsEnabled", "KeyCloakId", "LastLogin", "LastName", "UpdatedById", "UpdatedOn" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), null, new DateTime(2020, 10, 5, 12, 37, 40, 663, DateTimeKind.Local).AddTicks(4121), "User", null, "SYSTEM", null, null, null, false, null, null, "SYSTEM", null, null });

            migrationBuilder.CreateIndex(
                name: "IX_LookupCode_Type_Code_LocationId",
                table: "LookupCode",
                columns: new[] { "Type", "Code", "LocationId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Location_User_CreatedById",
                table: "Location",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Location_Region_RegionId",
                table: "Location",
                column: "RegionId",
                principalTable: "Region",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Location_User_UpdatedById",
                table: "Location",
                column: "UpdatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_LookupCode_User_CreatedById",
                table: "LookupCode",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_LookupCode_User_UpdatedById",
                table: "LookupCode",
                column: "UpdatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_LookupSortOrder_User_CreatedById",
                table: "LookupSortOrder",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_LookupSortOrder_User_UpdatedById",
                table: "LookupSortOrder",
                column: "UpdatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Permission_User_CreatedById",
                table: "Permission",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Permission_User_UpdatedById",
                table: "Permission",
                column: "UpdatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Region_User_CreatedById",
                table: "Region",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Region_User_UpdatedById",
                table: "Region",
                column: "UpdatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Role_User_CreatedById",
                table: "Role",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Role_User_UpdatedById",
                table: "Role",
                column: "UpdatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermission_User_CreatedById",
                table: "RolePermission",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermission_User_UpdatedById",
                table: "RolePermission",
                column: "UpdatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_SheriffAwayLocation_User_CreatedById",
                table: "SheriffAwayLocation",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_SheriffAwayLocation_User_UpdatedById",
                table: "SheriffAwayLocation",
                column: "UpdatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_User_User_CreatedById",
                table: "User",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Location_HomeLocationId",
                table: "User",
                column: "HomeLocationId",
                principalTable: "Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_User_User_UpdatedById",
                table: "User",
                column: "UpdatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRole_User_CreatedById",
                table: "UserRole",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRole_User_UpdatedById",
                table: "UserRole",
                column: "UpdatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Location_User_CreatedById",
                table: "Location");

            migrationBuilder.DropForeignKey(
                name: "FK_Location_Region_RegionId",
                table: "Location");

            migrationBuilder.DropForeignKey(
                name: "FK_Location_User_UpdatedById",
                table: "Location");

            migrationBuilder.DropForeignKey(
                name: "FK_LookupCode_User_CreatedById",
                table: "LookupCode");

            migrationBuilder.DropForeignKey(
                name: "FK_LookupCode_User_UpdatedById",
                table: "LookupCode");

            migrationBuilder.DropForeignKey(
                name: "FK_LookupSortOrder_User_CreatedById",
                table: "LookupSortOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_LookupSortOrder_User_UpdatedById",
                table: "LookupSortOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_Permission_User_CreatedById",
                table: "Permission");

            migrationBuilder.DropForeignKey(
                name: "FK_Permission_User_UpdatedById",
                table: "Permission");

            migrationBuilder.DropForeignKey(
                name: "FK_Region_User_CreatedById",
                table: "Region");

            migrationBuilder.DropForeignKey(
                name: "FK_Region_User_UpdatedById",
                table: "Region");

            migrationBuilder.DropForeignKey(
                name: "FK_Role_User_CreatedById",
                table: "Role");

            migrationBuilder.DropForeignKey(
                name: "FK_Role_User_UpdatedById",
                table: "Role");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermission_User_CreatedById",
                table: "RolePermission");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermission_User_UpdatedById",
                table: "RolePermission");

            migrationBuilder.DropForeignKey(
                name: "FK_SheriffAwayLocation_User_CreatedById",
                table: "SheriffAwayLocation");

            migrationBuilder.DropForeignKey(
                name: "FK_SheriffAwayLocation_User_UpdatedById",
                table: "SheriffAwayLocation");

            migrationBuilder.DropForeignKey(
                name: "FK_User_User_CreatedById",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Location_HomeLocationId",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_User_User_UpdatedById",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_User_CreatedById",
                table: "UserRole");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_User_UpdatedById",
                table: "UserRole");

            migrationBuilder.DropIndex(
                name: "IX_LookupCode_Type_Code_LocationId",
                table: "LookupCode");

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"));

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "Role");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "RolePermission",
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
                    { -1, "FAKE", null, new DateTime(2020, 10, 3, 15, 43, 31, 687, DateTimeKind.Utc).AddTicks(4793), null, null, "Dummy Location", null, null, null, null },
                    { -2, "FAKE2", null, new DateTime(2020, 10, 3, 15, 43, 31, 687, DateTimeKind.Utc).AddTicks(5995), null, null, "Dummy Location2", null, null, null, null }
                });

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

            migrationBuilder.AddForeignKey(
                name: "FK_Location_User_CreatedById",
                table: "Location",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Location_Region_RegionId",
                table: "Location",
                column: "RegionId",
                principalTable: "Region",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Location_User_UpdatedById",
                table: "Location",
                column: "UpdatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LookupCode_User_CreatedById",
                table: "LookupCode",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LookupCode_User_UpdatedById",
                table: "LookupCode",
                column: "UpdatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LookupSortOrder_User_CreatedById",
                table: "LookupSortOrder",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LookupSortOrder_User_UpdatedById",
                table: "LookupSortOrder",
                column: "UpdatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Permission_User_CreatedById",
                table: "Permission",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Permission_User_UpdatedById",
                table: "Permission",
                column: "UpdatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Region_User_CreatedById",
                table: "Region",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Region_User_UpdatedById",
                table: "Region",
                column: "UpdatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Role_User_CreatedById",
                table: "Role",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Role_User_UpdatedById",
                table: "Role",
                column: "UpdatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermission_User_CreatedById",
                table: "RolePermission",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermission_User_UpdatedById",
                table: "RolePermission",
                column: "UpdatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SheriffAwayLocation_User_CreatedById",
                table: "SheriffAwayLocation",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SheriffAwayLocation_User_UpdatedById",
                table: "SheriffAwayLocation",
                column: "UpdatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_User_User_CreatedById",
                table: "User",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Location_HomeLocationId",
                table: "User",
                column: "HomeLocationId",
                principalTable: "Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_User_User_UpdatedById",
                table: "User",
                column: "UpdatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRole_User_CreatedById",
                table: "UserRole",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRole_User_UpdatedById",
                table: "UserRole",
                column: "UpdatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
