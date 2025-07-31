using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SS.Db.Migrations
{
    public partial class AddSherifStandardTraining : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SheriffStandardTraining",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'200', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TrainingTypeId = table.Column<int>(type: "integer", nullable: false),
                    ExpiryInYears = table.Column<int>(type: "integer", nullable: true),
                    ExpiryMonth = table.Column<int>(type: "integer", nullable: true),
                    ExpiryDate = table.Column<int>(type: "integer", nullable: true),
                    ConditionOn = table.Column<string>(type: "text", nullable: true),
                    ConditionOperator = table.Column<string>(type: "text", nullable: true),
                    ConditionValue = table.Column<int>(type: "integer", nullable: true),
                    ConditionExpiryInYears = table.Column<int>(type: "integer", nullable: true),
                    ConditionExpiryMonth = table.Column<int>(type: "integer", nullable: true),
                    ConditionExpiryDate = table.Column<int>(type: "integer", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SheriffStandardTraining", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SheriffStandardTraining_LookupCode_TrainingTypeId",
                        column: x => x.TrainingTypeId,
                        principalTable: "LookupCode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SheriffStandardTraining_User_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SheriffStandardTraining_User_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SheriffStandardTraining_CreatedById",
                table: "SheriffStandardTraining",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SheriffStandardTraining_TrainingTypeId",
                table: "SheriffStandardTraining",
                column: "TrainingTypeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SheriffStandardTraining_UpdatedById",
                table: "SheriffStandardTraining",
                column: "UpdatedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SheriffStandardTraining");
        }
    }
}
