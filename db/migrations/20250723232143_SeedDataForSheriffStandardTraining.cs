using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SS.Db.Migrations
{
    public partial class SeedDataForSheriffStandardTraining : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "SheriffStandardTraining",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:IdentitySequenceOptions", "'2000', '1', '', '', 'False', '1'")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:IdentitySequenceOptions", "'200', '1', '', '', 'False', '1'")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.InsertData(
                table: "SheriffStandardTraining",
                columns: new[] { "Id", "ConditionExpiryDate", "ConditionExpiryInYears", "ConditionExpiryMonth", "ConditionOn", "ConditionOperator", "ConditionValue", "CreatedById", "ExpiryDate", "ExpiryInYears", "ExpiryMonth", "TrainingTypeId", "UpdatedById", "UpdatedOn" },
                values: new object[,]
                {
                    { 1, null, null, null, null, null, null, new Guid("00000000-0000-0000-0000-000000000001"), 31, 100, 12, 1718748, null, null },
                    { 2, null, null, null, null, null, null, new Guid("00000000-0000-0000-0000-000000000001"), 31, 0, 12, 1716961, null, null },
                    { 3, null, null, null, null, null, null, new Guid("00000000-0000-0000-0000-000000000001"), 31, 0, 12, 1718646, null, null },
                    { 4, 31, 1, 5, "Month", ">", 5, new Guid("00000000-0000-0000-0000-000000000001"), 31, 0, 5, 1716948, null, null },
                    { 5, null, null, null, null, null, null, new Guid("00000000-0000-0000-0000-000000000001"), 31, 1, 12, 1716960, null, null },
                    { 6, null, null, null, null, null, null, new Guid("00000000-0000-0000-0000-000000000001"), 31, 1, 12, 1716951, null, null },
                    { 7, null, null, null, null, null, null, new Guid("00000000-0000-0000-0000-000000000001"), 31, 2, 12, 1716955, null, null },
                    { 8, null, null, null, null, null, null, new Guid("00000000-0000-0000-0000-000000000001"), 31, 2, 12, 1718650, null, null },
                    { 9, null, null, null, null, null, null, new Guid("00000000-0000-0000-0000-000000000001"), 31, 2, 12, 1716956, null, null },
                    { 10, null, null, null, null, null, null, new Guid("00000000-0000-0000-0000-000000000001"), 31, 1, 12, 1716958, null, null },
                    { 11, null, null, null, null, null, null, new Guid("00000000-0000-0000-0000-000000000001"), 31, 100, 12, 1716954, null, null },
                    { 12, null, null, null, null, null, null, new Guid("00000000-0000-0000-0000-000000000001"), 31, 100, 12, 1716959, null, null }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SheriffStandardTraining",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "SheriffStandardTraining",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "SheriffStandardTraining",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "SheriffStandardTraining",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "SheriffStandardTraining",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "SheriffStandardTraining",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "SheriffStandardTraining",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "SheriffStandardTraining",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "SheriffStandardTraining",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "SheriffStandardTraining",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "SheriffStandardTraining",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "SheriffStandardTraining",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "SheriffStandardTraining",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:IdentitySequenceOptions", "'200', '1', '', '', 'False', '1'")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:IdentitySequenceOptions", "'2000', '1', '', '', 'False', '1'")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }
    }
}
