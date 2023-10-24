using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace CAS.DB.Migrations
{
    public partial class Data : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SheriffAwayLocation_Location_LocationId",
                table: "SheriffAwayLocation");

            migrationBuilder.DropColumn(
                name: "IsDisabled",
                table: "User");

            migrationBuilder.DropColumn(
                name: "PreferredUsername",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "JustinId",
                table: "Location");

            migrationBuilder.AddColumn<string>(
                name: "IdirName",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "User",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Role",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:IdentitySequenceOptions", "'50', '1', '', '', 'False', '1'")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "Region",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "JustinId",
                table: "Region",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Permission",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:IdentitySequenceOptions", "'50', '1', '', '', 'False', '1'")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "LookupCode",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:IdentitySequenceOptions", "'200', '1', '', '', 'False', '1'")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Location",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:IdentitySequenceOptions", "'200', '1', '', '', 'False', '1'")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "AgencyId",
                table: "Location",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "Location",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LookupSortOrder",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedById = table.Column<Guid>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedById = table.Column<Guid>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    LookupCodeId = table.Column<int>(nullable: true),
                    LocationId = table.Column<int>(nullable: true),
                    LookupType = table.Column<int>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LookupSortOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LookupSortOrder_User_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LookupSortOrder_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LookupSortOrder_LookupCode_LookupCodeId",
                        column: x => x.LookupCodeId,
                        principalTable: "LookupCode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LookupSortOrder_User_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Location",
                columns: new[] { "Id", "AgencyId", "CreatedById", "CreatedOn", "ExpiryDate", "JustinCode", "Name", "ParentLocationId", "RegionId", "UpdatedById", "UpdatedOn" },
                values: new object[] { 1, "FAKE", null, new DateTime(2020, 9, 30, 19, 1, 26, 237, DateTimeKind.Utc).AddTicks(8500), null, null, "Dummy Location", null, null, null, null });

            migrationBuilder.InsertData(
                table: "LookupCode",
                columns: new[] { "Id", "Code", "CreatedById", "CreatedOn", "Description", "EffectiveDate", "ExpiryDate", "LocationId", "SortOrder", "SubCode", "Type", "UpdatedById", "UpdatedOn" },
                values: new object[,]
                {
                    { 1, null, null, new DateTime(2020, 9, 30, 19, 1, 26, 243, DateTimeKind.Utc).AddTicks(8473), "Chief Sheriff", null, null, null, null, null, 7, null, null },
                    { 2, null, null, new DateTime(2020, 9, 30, 19, 1, 26, 243, DateTimeKind.Utc).AddTicks(9345), "Superintendent", null, null, null, null, null, 7, null, null },
                    { 3, null, null, new DateTime(2020, 9, 30, 19, 1, 26, 243, DateTimeKind.Utc).AddTicks(9369), "Staff Inspector", null, null, null, null, null, 7, null, null },
                    { 4, null, null, new DateTime(2020, 9, 30, 19, 1, 26, 243, DateTimeKind.Utc).AddTicks(9371), "Inspector", null, null, null, null, null, 7, null, null },
                    { 5, null, null, new DateTime(2020, 9, 30, 19, 1, 26, 243, DateTimeKind.Utc).AddTicks(9372), "Staff Sergeant", null, null, null, null, null, 7, null, null },
                    { 6, null, null, new DateTime(2020, 9, 30, 19, 1, 26, 243, DateTimeKind.Utc).AddTicks(9406), "Sergeant", null, null, null, null, null, 7, null, null },
                    { 7, null, null, new DateTime(2020, 9, 30, 19, 1, 26, 243, DateTimeKind.Utc).AddTicks(9408), "Deputy Sheriff", null, null, null, null, null, 7, null, null }
                });

            migrationBuilder.InsertData(
                table: "Permission",
                columns: new[] { "Id", "CreatedById", "CreatedOn", "Description", "Name", "UpdatedById", "UpdatedOn", "UserId" },
                values: new object[] { 1, null, new DateTime(2020, 9, 30, 19, 1, 26, 245, DateTimeKind.Utc).AddTicks(9203), "Permission to login to the application", "Login", null, null, null });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Id", "CreatedById", "CreatedOn", "Description", "Name", "UpdatedById", "UpdatedOn" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2020, 9, 30, 19, 1, 26, 250, DateTimeKind.Utc).AddTicks(5647), "System Administrator", "SystemAdministrator", null, null },
                    { 2, null, new DateTime(2020, 9, 30, 19, 1, 26, 250, DateTimeKind.Utc).AddTicks(6516), "Administrator", "Administrator", null, null },
                    { 3, null, new DateTime(2020, 9, 30, 19, 1, 26, 250, DateTimeKind.Utc).AddTicks(6537), "Sheriff", "Sheriff", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Region_JustinId",
                table: "Region",
                column: "JustinId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Location_AgencyId",
                table: "Location",
                column: "AgencyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LookupSortOrder_CreatedById",
                table: "LookupSortOrder",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_LookupSortOrder_LocationId",
                table: "LookupSortOrder",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_LookupSortOrder_LookupCodeId",
                table: "LookupSortOrder",
                column: "LookupCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_LookupSortOrder_LookupType",
                table: "LookupSortOrder",
                column: "LookupType");

            migrationBuilder.CreateIndex(
                name: "IX_LookupSortOrder_UpdatedById",
                table: "LookupSortOrder",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_SheriffAwayLocation_Location_LocationId",
                table: "SheriffAwayLocation",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SheriffAwayLocation_Location_LocationId",
                table: "SheriffAwayLocation");

            migrationBuilder.DropTable(
                name: "LookupSortOrder");

            migrationBuilder.DropIndex(
                name: "IX_Region_JustinId",
                table: "Region");

            migrationBuilder.DropIndex(
                name: "IX_Location_AgencyId",
                table: "Location");

            migrationBuilder.DeleteData(
                table: "Location",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "IdirName",
                table: "User");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "Region");

            migrationBuilder.DropColumn(
                name: "JustinId",
                table: "Region");

            migrationBuilder.DropColumn(
                name: "AgencyId",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "Location");

            migrationBuilder.AddColumn<bool>(
                name: "IsDisabled",
                table: "User",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PreferredUsername",
                table: "User",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Role",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:IdentitySequenceOptions", "'50', '1', '', '', 'False', '1'")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Permission",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:IdentitySequenceOptions", "'50', '1', '', '', 'False', '1'")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "LookupCode",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:IdentitySequenceOptions", "'200', '1', '', '', 'False', '1'")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Location",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:IdentitySequenceOptions", "'200', '1', '', '', 'False', '1'")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Location",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "JustinId",
                table: "Location",
                type: "integer",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SheriffAwayLocation_Location_LocationId",
                table: "SheriffAwayLocation",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
