using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace CAS.DB.Migrations
{
    public partial class MorePermissions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Permission",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:IdentitySequenceOptions", "'200', '1', '', '', 'False', '1'")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:IdentitySequenceOptions", "'50', '1', '', '', 'False', '1'")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 9, 19, 51, 54, 651, DateTimeKind.Utc).AddTicks(8836));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 9, 19, 51, 54, 652, DateTimeKind.Utc).AddTicks(72));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 9, 19, 51, 54, 652, DateTimeKind.Utc).AddTicks(106));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 9, 19, 51, 54, 652, DateTimeKind.Utc).AddTicks(108));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 9, 19, 51, 54, 652, DateTimeKind.Utc).AddTicks(109));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 9, 19, 51, 54, 652, DateTimeKind.Utc).AddTicks(110));

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 9, 19, 51, 54, 652, DateTimeKind.Utc).AddTicks(112));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedOn", "Description" },
                values: new object[] { new DateTime(2020, 10, 9, 19, 51, 54, 655, DateTimeKind.Utc).AddTicks(1193), "Allows the user to login." });

            migrationBuilder.InsertData(
                table: "Permission",
                columns: new[] { "Id", "CreatedById", "CreatedOn", "Description", "Name", "UpdatedById", "UpdatedOn" },
                values: new object[,]
                {
                    { 26, null, new DateTime(2020, 10, 9, 19, 51, 54, 655, DateTimeKind.Utc).AddTicks(2109), "Expire Location", "ExpireLocation", null, null },
                    { 25, null, new DateTime(2020, 10, 9, 19, 51, 54, 655, DateTimeKind.Utc).AddTicks(2108), "View Province (all regions, all locations)", "ViewProvince", null, null },
                    { 24, null, new DateTime(2020, 10, 9, 19, 51, 54, 655, DateTimeKind.Utc).AddTicks(2106), "View Region (all locations within region)", "ViewRegion", null, null },
                    { 23, null, new DateTime(2020, 10, 9, 19, 51, 54, 655, DateTimeKind.Utc).AddTicks(2105), "View Home Location and Assigned Location", "ViewHomeLocationAndAssignedLocation", null, null },
                    { 22, null, new DateTime(2020, 10, 9, 19, 51, 54, 655, DateTimeKind.Utc).AddTicks(2104), "View Distribute Schedule", "ViewDistributeSchedule", null, null },
                    { 21, null, new DateTime(2020, 10, 9, 19, 51, 54, 655, DateTimeKind.Utc).AddTicks(2102), "Edit Shifts", "EditShifts", null, null },
                    { 19, null, new DateTime(2020, 10, 9, 19, 51, 54, 655, DateTimeKind.Utc).AddTicks(2100), "Create and Assign Shifts", "CreateAndAssignShifts", null, null },
                    { 18, null, new DateTime(2020, 10, 9, 19, 51, 54, 655, DateTimeKind.Utc).AddTicks(2099), "View all Shifts", "ViewAllShifts", null, null },
                    { 20, null, new DateTime(2020, 10, 9, 19, 51, 54, 655, DateTimeKind.Utc).AddTicks(2101), "Expire Shifts", "ExpireShifts", null, null },
                    { 16, null, new DateTime(2020, 10, 9, 19, 51, 54, 655, DateTimeKind.Utc).AddTicks(2096), "View their own shifts", "ViewMyShifts", null, null },
                    { 3, null, new DateTime(2020, 10, 9, 19, 51, 54, 655, DateTimeKind.Utc).AddTicks(2079), "View profiles in their own location", "ViewProfilesInOwnLocation", null, null },
                    { 4, null, new DateTime(2020, 10, 9, 19, 51, 54, 655, DateTimeKind.Utc).AddTicks(2081), "View profiles in all locations", "ViewProfilesInAllLocation", null, null },
                    { 5, null, new DateTime(2020, 10, 9, 19, 51, 54, 655, DateTimeKind.Utc).AddTicks(2082), "Create Profile (User)", "CreateUsers", null, null },
                    { 6, null, new DateTime(2020, 10, 9, 19, 51, 54, 655, DateTimeKind.Utc).AddTicks(2083), "Expire Profile (User)", "ExpireUsers", null, null },
                    { 7, null, new DateTime(2020, 10, 9, 19, 51, 54, 655, DateTimeKind.Utc).AddTicks(2084), "Edit Profile (User)", "EditUsers", null, null },
                    { 8, null, new DateTime(2020, 10, 9, 19, 51, 54, 655, DateTimeKind.Utc).AddTicks(2086), "View all Roles", "ViewRoles", null, null },
                    { 17, null, new DateTime(2020, 10, 9, 19, 51, 54, 655, DateTimeKind.Utc).AddTicks(2097), "View Shifts at their location", "ViewAllShiftsAtMyLocation", null, null },
                    { 2, null, new DateTime(2020, 10, 9, 19, 51, 54, 655, DateTimeKind.Utc).AddTicks(2059), "View their own profile", "ViewOwnProfile", null, null },
                    { 9, null, new DateTime(2020, 10, 9, 19, 51, 54, 655, DateTimeKind.Utc).AddTicks(2087), "Create and Assign Roles", "CreateAndAssignRoles", null, null },
                    { 10, null, new DateTime(2020, 10, 9, 19, 51, 54, 655, DateTimeKind.Utc).AddTicks(2088), "Expire Roles", "ExpireRoles", null, null },
                    { 11, null, new DateTime(2020, 10, 9, 19, 51, 54, 655, DateTimeKind.Utc).AddTicks(2089), "Edit Roles", "EditRoles", null, null },
                    { 12, null, new DateTime(2020, 10, 9, 19, 51, 54, 655, DateTimeKind.Utc).AddTicks(2091), "View Manage Types", "ViewManageTypes", null, null },
                    { 13, null, new DateTime(2020, 10, 9, 19, 51, 54, 655, DateTimeKind.Utc).AddTicks(2092), "Create Types", "CreateTypes", null, null },
                    { 14, null, new DateTime(2020, 10, 9, 19, 51, 54, 655, DateTimeKind.Utc).AddTicks(2093), "Edit Types", "EditTypes", null, null },
                    { 15, null, new DateTime(2020, 10, 9, 19, 51, 54, 655, DateTimeKind.Utc).AddTicks(2095), "Expire Types", "ExpireTypes", null, null }
                });

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedOn", "Description", "Name" },
                values: new object[] { new DateTime(2020, 10, 9, 19, 51, 54, 657, DateTimeKind.Utc).AddTicks(2903), "Administrator", "Administrator" });

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedOn", "Description", "Name" },
                values: new object[] { new DateTime(2020, 10, 9, 19, 51, 54, 657, DateTimeKind.Utc).AddTicks(3798), "Manager", "Manager" });

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2020, 10, 9, 19, 51, 54, 657, DateTimeKind.Utc).AddTicks(3830));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                column: "CreatedOn",
                value: new DateTime(2020, 10, 9, 12, 51, 54, 665, DateTimeKind.Local).AddTicks(1557));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Permission",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("Npgsql:IdentitySequenceOptions", "'50', '1', '', '', 'False', '1'")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:IdentitySequenceOptions", "'200', '1', '', '', 'False', '1'")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

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
                columns: new[] { "CreatedOn", "Description" },
                values: new object[] { new DateTime(2020, 10, 9, 18, 55, 56, 452, DateTimeKind.Utc).AddTicks(1707), "Permission to login to the application" });

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedOn", "Description", "Name" },
                values: new object[] { new DateTime(2020, 10, 9, 18, 55, 56, 454, DateTimeKind.Utc).AddTicks(4010), "System Administrator", "System Administrator" });

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedOn", "Description", "Name" },
                values: new object[] { new DateTime(2020, 10, 9, 18, 55, 56, 454, DateTimeKind.Utc).AddTicks(4886), "Administrator", "Administrator" });

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
        }
    }
}
