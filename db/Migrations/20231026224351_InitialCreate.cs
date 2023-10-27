using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CAS.DB.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataProtectionKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FriendlyName = table.Column<string>(type: "text", nullable: true),
                    Xml = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataProtectionKeys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JcSynchronization",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LastSynchronization = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JcSynchronization", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Assignment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'200', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LookupCodeId = table.Column<int>(type: "integer", nullable: false),
                    AdhocStartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    AdhocEndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Start = table.Column<TimeSpan>(type: "interval", nullable: false),
                    End = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Timezone = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Monday = table.Column<bool>(type: "boolean", nullable: false),
                    Tuesday = table.Column<bool>(type: "boolean", nullable: false),
                    Wednesday = table.Column<bool>(type: "boolean", nullable: false),
                    Thursday = table.Column<bool>(type: "boolean", nullable: false),
                    Friday = table.Column<bool>(type: "boolean", nullable: false),
                    Saturday = table.Column<bool>(type: "boolean", nullable: false),
                    Sunday = table.Column<bool>(type: "boolean", nullable: false),
                    LocationId = table.Column<int>(type: "integer", nullable: false),
                    ExpiryDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ExpiryReason = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Audit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TableName = table.Column<string>(type: "text", nullable: true),
                    KeyValues = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    OldValues = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    NewValues = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CourtAdminActingRank",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Rank = table.Column<string>(type: "text", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ExpiryDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ExpiryReason = table.Column<string>(type: "text", nullable: true),
                    CourtAdminId = table.Column<Guid>(type: "uuid", nullable: false),
                    Comment = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Timezone = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourtAdminActingRank", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CourtAdminAwayLocation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LocationId = table.Column<int>(type: "integer", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ExpiryDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ExpiryReason = table.Column<string>(type: "text", nullable: true),
                    CourtAdminId = table.Column<Guid>(type: "uuid", nullable: false),
                    Comment = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Timezone = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourtAdminAwayLocation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CourtAdminLeave",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LeaveTypeId = table.Column<int>(type: "integer", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ExpiryDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ExpiryReason = table.Column<string>(type: "text", nullable: true),
                    CourtAdminId = table.Column<Guid>(type: "uuid", nullable: false),
                    Comment = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Timezone = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourtAdminLeave", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CourtAdminTraining",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TrainingTypeId = table.Column<int>(type: "integer", nullable: true),
                    TrainingCertificationExpiry = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Note = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    FirstNotice = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ExpiryDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ExpiryReason = table.Column<string>(type: "text", nullable: true),
                    CourtAdminId = table.Column<Guid>(type: "uuid", nullable: false),
                    Comment = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Timezone = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourtAdminTraining", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Duty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'200', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LocationId = table.Column<int>(type: "integer", nullable: false),
                    ExpiryDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    AssignmentId = table.Column<int>(type: "integer", nullable: true),
                    Timezone = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Duty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Duty_Assignment_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "Assignment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "DutySlot",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'200', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ExpiryDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DutyId = table.Column<int>(type: "integer", nullable: false),
                    CourtAdminId = table.Column<Guid>(type: "uuid", nullable: true),
                    LocationId = table.Column<int>(type: "integer", nullable: false),
                    Timezone = table.Column<string>(type: "text", nullable: true),
                    IsNotRequired = table.Column<bool>(type: "boolean", nullable: false),
                    IsNotAvailable = table.Column<bool>(type: "boolean", nullable: false),
                    IsOvertime = table.Column<bool>(type: "boolean", nullable: false),
                    IsClosed = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DutySlot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DutySlot_Duty_DutyId",
                        column: x => x.DutyId,
                        principalTable: "Duty",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'200', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AgencyId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    JustinCode = table.Column<string>(type: "text", nullable: true),
                    ParentLocationId = table.Column<int>(type: "integer", nullable: true),
                    ExpiryDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RegionId = table.Column<int>(type: "integer", nullable: true),
                    Timezone = table.Column<string>(type: "text", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdirName = table.Column<string>(type: "text", nullable: true),
                    IdirId = table.Column<Guid>(type: "uuid", nullable: true),
                    KeyCloakId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    HomeLocationId = table.Column<int>(type: "integer", nullable: true),
                    LastLogin = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Discriminator = table.Column<string>(type: "text", nullable: false),
                    Gender = table.Column<int>(type: "integer", nullable: true),
                    BadgeNumber = table.Column<string>(type: "text", nullable: true),
                    Rank = table.Column<string>(type: "text", nullable: true),
                    Photo = table.Column<byte[]>(type: "bytea", nullable: true),
                    LastPhotoUpdate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Location_HomeLocationId",
                        column: x => x.HomeLocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_User_User_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_User_User_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "LookupCode",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'1000', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    SubCode = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    EffectiveDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ExpiryDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LocationId = table.Column<int>(type: "integer", nullable: true),
                    Mandatory = table.Column<bool>(type: "boolean", nullable: false),
                    ValidityPeriod = table.Column<int>(type: "integer", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: true),
                    AdvanceNotice = table.Column<int>(type: "integer", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LookupCode", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LookupCode_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_LookupCode_User_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_LookupCode_User_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Permission",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'200', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Permission_User_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Permission_User_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Region",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JustinId = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    ExpiryDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Region", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Region_User_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Region_User_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'50', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Role_User_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Role_User_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Shift",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'200', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CourtAdminId = table.Column<Guid>(type: "uuid", nullable: false),
                    AnticipatedAssignmentId = table.Column<int>(type: "integer", nullable: true),
                    LocationId = table.Column<int>(type: "integer", nullable: false),
                    ExpiryDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Timezone = table.Column<string>(type: "text", nullable: true),
                    OvertimeHours = table.Column<double>(type: "double precision", nullable: false),
                    Comment = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shift", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shift_Assignment_AnticipatedAssignmentId",
                        column: x => x.AnticipatedAssignmentId,
                        principalTable: "Assignment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Shift_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Shift_User_CourtAdminId",
                        column: x => x.CourtAdminId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Shift_User_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Shift_User_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "LookupSortOrder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'1000', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LookupCodeId = table.Column<int>(type: "integer", nullable: false),
                    LocationId = table.Column<int>(type: "integer", nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LookupSortOrder", x => x.Id);
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
                        name: "FK_LookupSortOrder_User_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_LookupSortOrder_User_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "RolePermission",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'100', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    PermissionId = table.Column<int>(type: "integer", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermission_Permission_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermission_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermission_User_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RolePermission_User_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'5000', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    EffectiveDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ExpiryDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ExpiryReason = table.Column<string>(type: "text", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRole_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRole_User_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_UserRole_User_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_UserRole_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Permission",
                columns: new[] { "Id", "CreatedById", "Description", "Name", "UpdatedById", "UpdatedOn" },
                values: new object[,]
                {
                    { 1, null, "Allows the user to login.", "Login", null, null },
                    { 5, null, "Create Profile (User)", "CreateUsers", null, null },
                    { 6, null, "Expire Profile (User)", "ExpireUsers", null, null },
                    { 7, null, "Edit Profile (User)", "EditUsers", null, null },
                    { 8, null, "View all Roles", "ViewRoles", null, null },
                    { 9, null, "Create and Assign Roles", "CreateAndAssignRoles", null, null },
                    { 10, null, "Expire Roles", "ExpireRoles", null, null },
                    { 11, null, "Edit Roles", "EditRoles", null, null },
                    { 13, null, "Create Types", "CreateTypes", null, null },
                    { 14, null, "Edit Types", "EditTypes", null, null },
                    { 15, null, "Expire Types", "ExpireTypes", null, null },
                    { 16, null, "View shifts", "ViewShifts", null, null },
                    { 19, null, "Create and Assign Shifts", "CreateAndAssignShifts", null, null },
                    { 20, null, "Expire Shifts", "ExpireShifts", null, null },
                    { 21, null, "Edit Shifts", "EditShifts", null, null },
                    { 22, null, "View Distribute Schedule", "ViewDistributeSchedule", null, null },
                    { 23, null, "View Assigned Location", "ViewAssignedLocation", null, null },
                    { 24, null, "View Region (all locations within region)", "ViewRegion", null, null },
                    { 25, null, "View Province (all regions, all locations)", "ViewProvince", null, null },
                    { 27, null, "View Home Location", "ViewHomeLocation", null, null },
                    { 28, null, "Import Shifts", "ImportShifts", null, null },
                    { 30, null, "Create Assignments", "CreateAssignments", null, null },
                    { 31, null, "Edit Assignments", "EditAssignments", null, null },
                    { 32, null, "Expire Assignments", "ExpireAssignments", null, null },
                    { 33, null, "View Duties", "ViewDutyRoster", null, null },
                    { 34, null, "Create Duties", "CreateAndAssignDuties", null, null },
                    { 35, null, "Edit Duties", "EditDuties", null, null },
                    { 36, null, "Expire Duties", "ExpireDuties", null, null },
                    { 37, null, "Edit Idir", "EditIdir", null, null },
                    { 38, null, "Edit Past Training", "EditPastTraining", null, null },
                    { 39, null, "Remove Past Training", "RemovePastTraining", null, null },
                    { 40, null, "View DutyRoster in the future", "ViewDutyRosterInFuture", null, null },
                    { 41, null, "View Shifts in the future (not time constrained)", "ViewAllFutureShifts", null, null },
                    { 42, null, "View other profiles (beside their own)", "ViewOtherProfiles", null, null },
                    { 43, null, "Generate Reports based on CourtAdmin's activity", "GenerateReports", null, null }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "CreatedById", "Discriminator", "Email", "FirstName", "HomeLocationId", "IdirId", "IdirName", "IsEnabled", "KeyCloakId", "LastLogin", "LastName", "UpdatedById", "UpdatedOn" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), null, "User", null, "SYSTEM", null, null, null, false, null, null, "SYSTEM", null, null });

            migrationBuilder.InsertData(
                table: "Location",
                columns: new[] { "Id", "AgencyId", "CreatedById", "ExpiryDate", "JustinCode", "Name", "ParentLocationId", "RegionId", "Timezone", "UpdatedById", "UpdatedOn" },
                values: new object[,]
                {
                    { 1, "SS1", new Guid("00000000-0000-0000-0000-000000000001"), null, null, "Office of Professional Standards", null, null, "America/Vancouver", null, null },
                    { 2, "SS2", new Guid("00000000-0000-0000-0000-000000000001"), null, null, "CourtAdmin Provincial Operation Centre", null, null, "America/Vancouver", null, null },
                    { 3, "SS3", new Guid("00000000-0000-0000-0000-000000000001"), null, null, "Central Float Pool", null, null, "America/Vancouver", null, null },
                    { 4, "SS4", new Guid("00000000-0000-0000-0000-000000000001"), null, null, "ITAU", null, null, "America/Vancouver", null, null },
                    { 5, "SS5", new Guid("00000000-0000-0000-0000-000000000001"), null, null, "Office of the Chief CourtAdmin", null, null, "America/Vancouver", null, null },
                    { 6, "SS6", new Guid("00000000-0000-0000-0000-000000000001"), null, "4882", "South Okanagan Escort Centre", null, null, "America/Vancouver", null, null }
                });

            migrationBuilder.InsertData(
                table: "LookupCode",
                columns: new[] { "Id", "AdvanceNotice", "Category", "Code", "CreatedById", "Description", "EffectiveDate", "ExpiryDate", "LocationId", "Mandatory", "SubCode", "Type", "UpdatedById", "UpdatedOn", "ValidityPeriod" },
                values: new object[,]
                {
                    { 1, 0, null, "Chief CourtAdmin", new Guid("00000000-0000-0000-0000-000000000001"), "Chief CourtAdmin", null, null, null, false, null, 7, null, null, 0 },
                    { 2, 0, null, "Superintendent", new Guid("00000000-0000-0000-0000-000000000001"), "Superintendent", null, null, null, false, null, 7, null, null, 0 },
                    { 3, 0, null, "Staff Inspector", new Guid("00000000-0000-0000-0000-000000000001"), "Staff Inspector", null, null, null, false, null, 7, null, null, 0 },
                    { 4, 0, null, "Inspector", new Guid("00000000-0000-0000-0000-000000000001"), "Inspector", null, null, null, false, null, 7, null, null, 0 },
                    { 5, 0, null, "Staff Sergeant", new Guid("00000000-0000-0000-0000-000000000001"), "Staff Sergeant", null, null, null, false, null, 7, null, null, 0 },
                    { 6, 0, null, "Sergeant", new Guid("00000000-0000-0000-0000-000000000001"), "Sergeant", null, null, null, false, null, 7, null, null, 0 },
                    { 7, 0, null, "Deputy CourtAdmin", new Guid("00000000-0000-0000-0000-000000000001"), "Deputy CourtAdmin", null, null, null, false, null, 7, null, null, 0 },
                    { 8, 0, null, "CEW (Taser)", new Guid("00000000-0000-0000-0000-000000000001"), "CEW (Taser)", null, null, null, false, null, 6, null, null, 0 },
                    { 9, 0, null, "DNA", new Guid("00000000-0000-0000-0000-000000000001"), "DNA", null, null, null, false, null, 6, null, null, 0 },
                    { 10, 0, null, "FRO", new Guid("00000000-0000-0000-0000-000000000001"), "FRO", null, null, null, false, null, 6, null, null, 0 },
                    { 11, 0, null, "Fire Arm", new Guid("00000000-0000-0000-0000-000000000001"), "Fire Arm", null, null, null, false, null, 6, null, null, 0 },
                    { 12, 0, null, "First Aid", new Guid("00000000-0000-0000-0000-000000000001"), "First Aid", null, null, null, false, null, 6, null, null, 0 },
                    { 13, 0, null, "Advanced Escort SPC (AESOC)", new Guid("00000000-0000-0000-0000-000000000001"), "Advanced Escort SPC (AESOC)", null, null, null, false, null, 6, null, null, 0 },
                    { 14, 0, null, "Extenuating Circumstances SPC (EXSPC)", new Guid("00000000-0000-0000-0000-000000000001"), "Extenuating Circumstances SPC (EXSPC)", null, null, null, false, null, 6, null, null, 0 },
                    { 15, 0, null, "Search Gate", new Guid("00000000-0000-0000-0000-000000000001"), "Search Gate", null, null, null, false, null, 6, null, null, 0 },
                    { 16, 0, null, "Other", new Guid("00000000-0000-0000-0000-000000000001"), "Other", null, null, null, false, null, 6, null, null, 0 },
                    { 17, 0, null, "STIP", new Guid("00000000-0000-0000-0000-000000000001"), "STIP", null, null, null, false, null, 5, null, null, 0 },
                    { 18, 0, null, "Annual", new Guid("00000000-0000-0000-0000-000000000001"), "Annual", null, null, null, false, null, 5, null, null, 0 },
                    { 19, 0, null, "Illness", new Guid("00000000-0000-0000-0000-000000000001"), "Illness", null, null, null, false, null, 5, null, null, 0 },
                    { 20, 0, null, "Special", new Guid("00000000-0000-0000-0000-000000000001"), "Special", null, null, null, false, null, 5, null, null, 0 }
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Id", "CreatedById", "Description", "Name", "UpdatedById", "UpdatedOn" },
                values: new object[,]
                {
                    { 1, new Guid("00000000-0000-0000-0000-000000000001"), "Administrator", "Administrator", null, null },
                    { 2, new Guid("00000000-0000-0000-0000-000000000001"), "Manager", "Manager", null, null },
                    { 3, new Guid("00000000-0000-0000-0000-000000000001"), "CourtAdmin", "CourtAdmin", null, null }
                });

            migrationBuilder.InsertData(
                table: "LookupSortOrder",
                columns: new[] { "Id", "CreatedById", "LocationId", "LookupCodeId", "SortOrder", "UpdatedById", "UpdatedOn" },
                values: new object[,]
                {
                    { 1, new Guid("00000000-0000-0000-0000-000000000001"), null, 1, 1, null, null },
                    { 2, new Guid("00000000-0000-0000-0000-000000000001"), null, 2, 2, null, null },
                    { 3, new Guid("00000000-0000-0000-0000-000000000001"), null, 3, 3, null, null },
                    { 4, new Guid("00000000-0000-0000-0000-000000000001"), null, 4, 4, null, null },
                    { 5, new Guid("00000000-0000-0000-0000-000000000001"), null, 5, 5, null, null },
                    { 6, new Guid("00000000-0000-0000-0000-000000000001"), null, 6, 6, null, null },
                    { 7, new Guid("00000000-0000-0000-0000-000000000001"), null, 7, 7, null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_CreatedById",
                table: "Assignment",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_LocationId",
                table: "Assignment",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_LookupCodeId",
                table: "Assignment",
                column: "LookupCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_UpdatedById",
                table: "Assignment",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Audit_CreatedById",
                table: "Audit",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Audit_KeyValues",
                table: "Audit",
                column: "KeyValues");

            migrationBuilder.CreateIndex(
                name: "IX_CourtAdminActingRank_CourtAdminId",
                table: "CourtAdminActingRank",
                column: "CourtAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_CourtAdminActingRank_CreatedById",
                table: "CourtAdminActingRank",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CourtAdminActingRank_StartDate_EndDate",
                table: "CourtAdminActingRank",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CourtAdminActingRank_UpdatedById",
                table: "CourtAdminActingRank",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CourtAdminAwayLocation_CourtAdminId",
                table: "CourtAdminAwayLocation",
                column: "CourtAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_CourtAdminAwayLocation_CreatedById",
                table: "CourtAdminAwayLocation",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CourtAdminAwayLocation_LocationId",
                table: "CourtAdminAwayLocation",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CourtAdminAwayLocation_StartDate_EndDate",
                table: "CourtAdminAwayLocation",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CourtAdminAwayLocation_UpdatedById",
                table: "CourtAdminAwayLocation",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CourtAdminLeave_CourtAdminId",
                table: "CourtAdminLeave",
                column: "CourtAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_CourtAdminLeave_CreatedById",
                table: "CourtAdminLeave",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CourtAdminLeave_LeaveTypeId",
                table: "CourtAdminLeave",
                column: "LeaveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CourtAdminLeave_StartDate_EndDate",
                table: "CourtAdminLeave",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CourtAdminLeave_UpdatedById",
                table: "CourtAdminLeave",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CourtAdminTraining_CourtAdminId",
                table: "CourtAdminTraining",
                column: "CourtAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_CourtAdminTraining_CreatedById",
                table: "CourtAdminTraining",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CourtAdminTraining_StartDate_EndDate",
                table: "CourtAdminTraining",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CourtAdminTraining_TrainingTypeId",
                table: "CourtAdminTraining",
                column: "TrainingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CourtAdminTraining_UpdatedById",
                table: "CourtAdminTraining",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Duty_AssignmentId",
                table: "Duty",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Duty_CreatedById",
                table: "Duty",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Duty_LocationId",
                table: "Duty",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Duty_StartDate_EndDate",
                table: "Duty",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Duty_UpdatedById",
                table: "Duty",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_DutySlot_CourtAdminId",
                table: "DutySlot",
                column: "CourtAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_DutySlot_CreatedById",
                table: "DutySlot",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_DutySlot_DutyId",
                table: "DutySlot",
                column: "DutyId");

            migrationBuilder.CreateIndex(
                name: "IX_DutySlot_LocationId",
                table: "DutySlot",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_DutySlot_StartDate_EndDate",
                table: "DutySlot",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_DutySlot_UpdatedById",
                table: "DutySlot",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Location_AgencyId",
                table: "Location",
                column: "AgencyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Location_CreatedById",
                table: "Location",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Location_RegionId",
                table: "Location",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Location_UpdatedById",
                table: "Location",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_LookupCode_CreatedById",
                table: "LookupCode",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_LookupCode_LocationId",
                table: "LookupCode",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_LookupCode_Type_Code_LocationId",
                table: "LookupCode",
                columns: new[] { "Type", "Code", "LocationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LookupCode_UpdatedById",
                table: "LookupCode",
                column: "UpdatedById");

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
                name: "IX_LookupSortOrder_LookupCodeId_LocationId",
                table: "LookupSortOrder",
                columns: new[] { "LookupCodeId", "LocationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LookupSortOrder_UpdatedById",
                table: "LookupSortOrder",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Permission_CreatedById",
                table: "Permission",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Permission_UpdatedById",
                table: "Permission",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Region_CreatedById",
                table: "Region",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Region_JustinId",
                table: "Region",
                column: "JustinId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Region_UpdatedById",
                table: "Region",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Role_CreatedById",
                table: "Role",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Role_UpdatedById",
                table: "Role",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_CreatedById",
                table: "RolePermission",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_PermissionId",
                table: "RolePermission",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_RoleId",
                table: "RolePermission",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_UpdatedById",
                table: "RolePermission",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Shift_AnticipatedAssignmentId",
                table: "Shift",
                column: "AnticipatedAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Shift_CourtAdminId",
                table: "Shift",
                column: "CourtAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Shift_CreatedById",
                table: "Shift",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Shift_LocationId",
                table: "Shift",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Shift_StartDate_EndDate",
                table: "Shift",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Shift_UpdatedById",
                table: "Shift",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_User_BadgeNumber",
                table: "User",
                column: "BadgeNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_CreatedById",
                table: "User",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_User_HomeLocationId",
                table: "User",
                column: "HomeLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_User_UpdatedById",
                table: "User",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_CreatedById",
                table: "UserRole",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_RoleId_UserId",
                table: "UserRole",
                columns: new[] { "RoleId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_UpdatedById",
                table: "UserRole",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_UserId",
                table: "UserRole",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignment_Location_LocationId",
                table: "Assignment",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Assignment_LookupCode_LookupCodeId",
                table: "Assignment",
                column: "LookupCodeId",
                principalTable: "LookupCode",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Assignment_User_CreatedById",
                table: "Assignment",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Assignment_User_UpdatedById",
                table: "Assignment",
                column: "UpdatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Audit_User_CreatedById",
                table: "Audit",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CourtAdminActingRank_User_CourtAdminId",
                table: "CourtAdminActingRank",
                column: "CourtAdminId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourtAdminActingRank_User_CreatedById",
                table: "CourtAdminActingRank",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CourtAdminActingRank_User_UpdatedById",
                table: "CourtAdminActingRank",
                column: "UpdatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CourtAdminAwayLocation_Location_LocationId",
                table: "CourtAdminAwayLocation",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourtAdminAwayLocation_User_CourtAdminId",
                table: "CourtAdminAwayLocation",
                column: "CourtAdminId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourtAdminAwayLocation_User_CreatedById",
                table: "CourtAdminAwayLocation",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CourtAdminAwayLocation_User_UpdatedById",
                table: "CourtAdminAwayLocation",
                column: "UpdatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CourtAdminLeave_LookupCode_LeaveTypeId",
                table: "CourtAdminLeave",
                column: "LeaveTypeId",
                principalTable: "LookupCode",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourtAdminLeave_User_CourtAdminId",
                table: "CourtAdminLeave",
                column: "CourtAdminId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourtAdminLeave_User_CreatedById",
                table: "CourtAdminLeave",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CourtAdminLeave_User_UpdatedById",
                table: "CourtAdminLeave",
                column: "UpdatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CourtAdminTraining_LookupCode_TrainingTypeId",
                table: "CourtAdminTraining",
                column: "TrainingTypeId",
                principalTable: "LookupCode",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourtAdminTraining_User_CourtAdminId",
                table: "CourtAdminTraining",
                column: "CourtAdminId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourtAdminTraining_User_CreatedById",
                table: "CourtAdminTraining",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CourtAdminTraining_User_UpdatedById",
                table: "CourtAdminTraining",
                column: "UpdatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Duty_Location_LocationId",
                table: "Duty",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Duty_User_CreatedById",
                table: "Duty",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Duty_User_UpdatedById",
                table: "Duty",
                column: "UpdatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_DutySlot_Location_LocationId",
                table: "DutySlot",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_DutySlot_User_CourtAdminId",
                table: "DutySlot",
                column: "CourtAdminId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_DutySlot_User_CreatedById",
                table: "DutySlot",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_DutySlot_User_UpdatedById",
                table: "DutySlot",
                column: "UpdatedById",
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
                name: "FK_Location_User_CreatedById",
                table: "Location",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Location_User_UpdatedById",
                table: "Location",
                column: "UpdatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Location_HomeLocationId",
                table: "User");

            migrationBuilder.DropTable(
                name: "Audit");

            migrationBuilder.DropTable(
                name: "CourtAdminActingRank");

            migrationBuilder.DropTable(
                name: "CourtAdminAwayLocation");

            migrationBuilder.DropTable(
                name: "CourtAdminLeave");

            migrationBuilder.DropTable(
                name: "CourtAdminTraining");

            migrationBuilder.DropTable(
                name: "DataProtectionKeys");

            migrationBuilder.DropTable(
                name: "DutySlot");

            migrationBuilder.DropTable(
                name: "JcSynchronization");

            migrationBuilder.DropTable(
                name: "LookupSortOrder");

            migrationBuilder.DropTable(
                name: "RolePermission");

            migrationBuilder.DropTable(
                name: "Shift");

            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropTable(
                name: "Duty");

            migrationBuilder.DropTable(
                name: "Permission");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Assignment");

            migrationBuilder.DropTable(
                name: "LookupCode");

            migrationBuilder.DropTable(
                name: "Location");

            migrationBuilder.DropTable(
                name: "Region");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
