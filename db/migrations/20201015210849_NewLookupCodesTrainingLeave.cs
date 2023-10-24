using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace CAS.DB.Migrations
{
    public partial class NewLookupCodesTrainingLeave : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "LookupCode",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:IdentitySequenceOptions", "'1000', '1', '', '', 'False', '1'")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:IdentitySequenceOptions", "'200', '1', '', '', 'False', '1'")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedById", "CreatedOn" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 559, DateTimeKind.Unspecified).AddTicks(6701), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedById", "CreatedOn" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 559, DateTimeKind.Unspecified).AddTicks(9825), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedById", "CreatedOn" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 559, DateTimeKind.Unspecified).AddTicks(9884), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedById", "CreatedOn" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 559, DateTimeKind.Unspecified).AddTicks(9887), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedById", "CreatedOn" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 559, DateTimeKind.Unspecified).AddTicks(9890), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedById", "CreatedOn" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 559, DateTimeKind.Unspecified).AddTicks(9892), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedById", "CreatedOn" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 559, DateTimeKind.Unspecified).AddTicks(9894), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.InsertData(
                table: "LookupCode",
                columns: new[] { "Id", "Code", "CreatedById", "CreatedOn", "Description", "EffectiveDate", "ExpiryDate", "LocationId", "SortOrder", "SubCode", "Type", "UpdatedById", "UpdatedOn" },
                values: new object[,]
                {
                    { 20, null, new Guid("00000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 559, DateTimeKind.Unspecified).AddTicks(9921), new TimeSpan(0, 0, 0, 0, 0)), "Special", null, null, null, null, null, 5, null, null },
                    { 19, null, new Guid("00000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 559, DateTimeKind.Unspecified).AddTicks(9919), new TimeSpan(0, 0, 0, 0, 0)), "Illness", null, null, null, null, null, 5, null, null },
                    { 18, null, new Guid("00000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 559, DateTimeKind.Unspecified).AddTicks(9917), new TimeSpan(0, 0, 0, 0, 0)), "Annual", null, null, null, null, null, 5, null, null },
                    { 8, null, new Guid("00000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 559, DateTimeKind.Unspecified).AddTicks(9896), new TimeSpan(0, 0, 0, 0, 0)), "CEW (Taser)", null, null, null, null, null, 6, null, null },
                    { 16, null, new Guid("00000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 559, DateTimeKind.Unspecified).AddTicks(9913), new TimeSpan(0, 0, 0, 0, 0)), "Other", null, null, null, null, null, 6, null, null },
                    { 12, null, new Guid("00000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 559, DateTimeKind.Unspecified).AddTicks(9904), new TimeSpan(0, 0, 0, 0, 0)), "First Aid", null, null, null, null, null, 6, null, null },
                    { 11, null, new Guid("00000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 559, DateTimeKind.Unspecified).AddTicks(9902), new TimeSpan(0, 0, 0, 0, 0)), "Fire Arm", null, null, null, null, null, 6, null, null },
                    { 10, null, new Guid("00000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 559, DateTimeKind.Unspecified).AddTicks(9900), new TimeSpan(0, 0, 0, 0, 0)), "FRO", null, null, null, null, null, 6, null, null },
                    { 9, null, new Guid("00000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 559, DateTimeKind.Unspecified).AddTicks(9898), new TimeSpan(0, 0, 0, 0, 0)), "DNA", null, null, null, null, null, 6, null, null },
                    { 14, null, new Guid("00000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 559, DateTimeKind.Unspecified).AddTicks(9909), new TimeSpan(0, 0, 0, 0, 0)), "Extenuating Circumstances SPC (EXSPC)", null, null, null, null, null, 6, null, null },
                    { 13, null, new Guid("00000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 559, DateTimeKind.Unspecified).AddTicks(9906), new TimeSpan(0, 0, 0, 0, 0)), "Advanced Escort SPC (AESOC)", null, null, null, null, null, 6, null, null },
                    { 17, null, new Guid("00000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 559, DateTimeKind.Unspecified).AddTicks(9915), new TimeSpan(0, 0, 0, 0, 0)), "STIP", null, null, null, null, null, 5, null, null },
                    { 15, null, new Guid("00000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 559, DateTimeKind.Unspecified).AddTicks(9911), new TimeSpan(0, 0, 0, 0, 0)), "Search Gate", null, null, null, null, null, 6, null, null }
                });

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 564, DateTimeKind.Unspecified).AddTicks(9200), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 565, DateTimeKind.Unspecified).AddTicks(603), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 565, DateTimeKind.Unspecified).AddTicks(639), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 565, DateTimeKind.Unspecified).AddTicks(641), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 565, DateTimeKind.Unspecified).AddTicks(643), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 565, DateTimeKind.Unspecified).AddTicks(645), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 565, DateTimeKind.Unspecified).AddTicks(647), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 565, DateTimeKind.Unspecified).AddTicks(649), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 565, DateTimeKind.Unspecified).AddTicks(651), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 565, DateTimeKind.Unspecified).AddTicks(653), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 565, DateTimeKind.Unspecified).AddTicks(655), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 565, DateTimeKind.Unspecified).AddTicks(658), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 565, DateTimeKind.Unspecified).AddTicks(660), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 565, DateTimeKind.Unspecified).AddTicks(662), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 565, DateTimeKind.Unspecified).AddTicks(664), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 16,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 565, DateTimeKind.Unspecified).AddTicks(666), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 17,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 565, DateTimeKind.Unspecified).AddTicks(668), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 18,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 565, DateTimeKind.Unspecified).AddTicks(670), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 19,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 565, DateTimeKind.Unspecified).AddTicks(672), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 20,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 565, DateTimeKind.Unspecified).AddTicks(674), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 21,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 565, DateTimeKind.Unspecified).AddTicks(676), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 22,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 565, DateTimeKind.Unspecified).AddTicks(678), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 23,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 565, DateTimeKind.Unspecified).AddTicks(681), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 24,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 565, DateTimeKind.Unspecified).AddTicks(683), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 25,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 565, DateTimeKind.Unspecified).AddTicks(685), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 26,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 565, DateTimeKind.Unspecified).AddTicks(687), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 568, DateTimeKind.Unspecified).AddTicks(8728), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 569, DateTimeKind.Unspecified).AddTicks(374), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 569, DateTimeKind.Unspecified).AddTicks(454), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 21, 8, 48, 583, DateTimeKind.Unspecified).AddTicks(3793), new TimeSpan(0, 0, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "LookupCode",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:IdentitySequenceOptions", "'200', '1', '', '', 'False', '1'")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:IdentitySequenceOptions", "'1000', '1', '', '', 'False', '1'")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedById", "CreatedOn" },
                values: new object[] { null, new DateTimeOffset(new DateTime(2020, 10, 15, 1, 21, 58, 506, DateTimeKind.Unspecified).AddTicks(4665), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedById", "CreatedOn" },
                values: new object[] { null, new DateTimeOffset(new DateTime(2020, 10, 15, 1, 21, 58, 506, DateTimeKind.Unspecified).AddTicks(5891), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedById", "CreatedOn" },
                values: new object[] { null, new DateTimeOffset(new DateTime(2020, 10, 15, 1, 21, 58, 506, DateTimeKind.Unspecified).AddTicks(5926), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedById", "CreatedOn" },
                values: new object[] { null, new DateTimeOffset(new DateTime(2020, 10, 15, 1, 21, 58, 506, DateTimeKind.Unspecified).AddTicks(5928), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedById", "CreatedOn" },
                values: new object[] { null, new DateTimeOffset(new DateTime(2020, 10, 15, 1, 21, 58, 506, DateTimeKind.Unspecified).AddTicks(5930), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedById", "CreatedOn" },
                values: new object[] { null, new DateTimeOffset(new DateTime(2020, 10, 15, 1, 21, 58, 506, DateTimeKind.Unspecified).AddTicks(5931), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "LookupCode",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedById", "CreatedOn" },
                values: new object[] { null, new DateTimeOffset(new DateTime(2020, 10, 15, 1, 21, 58, 506, DateTimeKind.Unspecified).AddTicks(5933), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 1, 21, 58, 509, DateTimeKind.Unspecified).AddTicks(7394), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 1, 21, 58, 509, DateTimeKind.Unspecified).AddTicks(8222), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 1, 21, 58, 509, DateTimeKind.Unspecified).AddTicks(8244), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 1, 21, 58, 509, DateTimeKind.Unspecified).AddTicks(8246), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 1, 21, 58, 509, DateTimeKind.Unspecified).AddTicks(8247), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 1, 21, 58, 509, DateTimeKind.Unspecified).AddTicks(8249), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 1, 21, 58, 509, DateTimeKind.Unspecified).AddTicks(8250), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 1, 21, 58, 509, DateTimeKind.Unspecified).AddTicks(8252), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 1, 21, 58, 509, DateTimeKind.Unspecified).AddTicks(8253), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 1, 21, 58, 509, DateTimeKind.Unspecified).AddTicks(8255), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 1, 21, 58, 509, DateTimeKind.Unspecified).AddTicks(8256), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 1, 21, 58, 509, DateTimeKind.Unspecified).AddTicks(8258), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 1, 21, 58, 509, DateTimeKind.Unspecified).AddTicks(8259), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 1, 21, 58, 509, DateTimeKind.Unspecified).AddTicks(8261), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 1, 21, 58, 509, DateTimeKind.Unspecified).AddTicks(8262), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 16,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 1, 21, 58, 509, DateTimeKind.Unspecified).AddTicks(8264), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 17,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 1, 21, 58, 509, DateTimeKind.Unspecified).AddTicks(8265), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 18,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 1, 21, 58, 509, DateTimeKind.Unspecified).AddTicks(8267), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 19,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 1, 21, 58, 509, DateTimeKind.Unspecified).AddTicks(8268), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 20,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 1, 21, 58, 509, DateTimeKind.Unspecified).AddTicks(8270), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 21,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 1, 21, 58, 509, DateTimeKind.Unspecified).AddTicks(8271), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 22,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 1, 21, 58, 509, DateTimeKind.Unspecified).AddTicks(8273), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 23,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 1, 21, 58, 509, DateTimeKind.Unspecified).AddTicks(8274), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 24,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 1, 21, 58, 509, DateTimeKind.Unspecified).AddTicks(8276), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 25,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 1, 21, 58, 509, DateTimeKind.Unspecified).AddTicks(8277), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 26,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 1, 21, 58, 509, DateTimeKind.Unspecified).AddTicks(8279), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 1, 21, 58, 512, DateTimeKind.Unspecified).AddTicks(985), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 1, 21, 58, 512, DateTimeKind.Unspecified).AddTicks(2966), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 1, 21, 58, 512, DateTimeKind.Unspecified).AddTicks(3030), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                column: "CreatedOn",
                value: new DateTimeOffset(new DateTime(2020, 10, 15, 1, 21, 58, 521, DateTimeKind.Unspecified).AddTicks(3571), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
