﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using CAS.DB.models;

namespace CAS.DB.Migrations
{
    [DbContext(typeof(CourtAdminDbContext))]
    [Migration("20201006215656_LocationDeleteSetNull")]
    partial class LocationDeleteSetNull
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("CAS.API.Models.DB.Location", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:IdentitySequenceOptions", "'200', '1', '', '', 'False', '1'")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("AgencyId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<uint>("ConcurrencyToken")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("xmin")
                        .HasColumnType("xid");

                    b.Property<Guid?>("CreatedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("ExpiryDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("JustinCode")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int?>("ParentLocationId")
                        .HasColumnType("integer");

                    b.Property<int?>("RegionId")
                        .HasColumnType("integer");

                    b.Property<Guid?>("UpdatedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("AgencyId")
                        .IsUnique();

                    b.HasIndex("CreatedById");

                    b.HasIndex("RegionId");

                    b.HasIndex("UpdatedById");

                    b.ToTable("Location");
                });

            modelBuilder.Entity("CAS.DB.models.auth.Permission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:IdentitySequenceOptions", "'50', '1', '', '', 'False', '1'")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<uint>("ConcurrencyToken")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("xmin")
                        .HasColumnType("xid");

                    b.Property<Guid?>("CreatedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<Guid?>("UpdatedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("UpdatedById");

                    b.ToTable("Permission");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            ConcurrencyToken = 0u,
                            CreatedOn = new DateTime(2020, 10, 6, 21, 56, 55, 979, DateTimeKind.Utc).AddTicks(6787),
                            Description = "Permission to login to the application",
                            Name = "Login"
                        });
                });

            modelBuilder.Entity("CAS.DB.models.auth.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:IdentitySequenceOptions", "'50', '1', '', '', 'False', '1'")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<uint>("ConcurrencyToken")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("xmin")
                        .HasColumnType("xid");

                    b.Property<Guid?>("CreatedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<DateTime?>("ExpiryDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<Guid?>("UpdatedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("UpdatedById");

                    b.ToTable("Role");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            ConcurrencyToken = 0u,
                            CreatedOn = new DateTime(2020, 10, 6, 21, 56, 55, 981, DateTimeKind.Utc).AddTicks(8209),
                            Description = "System Administrator",
                            Name = "System Administrator"
                        },
                        new
                        {
                            Id = 2,
                            ConcurrencyToken = 0u,
                            CreatedOn = new DateTime(2020, 10, 6, 21, 56, 55, 981, DateTimeKind.Utc).AddTicks(9159),
                            Description = "Administrator",
                            Name = "Administrator"
                        },
                        new
                        {
                            Id = 3,
                            ConcurrencyToken = 0u,
                            CreatedOn = new DateTime(2020, 10, 6, 21, 56, 55, 981, DateTimeKind.Utc).AddTicks(9181),
                            Description = "Sheriff",
                            Name = "Sheriff"
                        });
                });

            modelBuilder.Entity("CAS.DB.models.auth.RolePermission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:IdentitySequenceOptions", "'100', '1', '', '', 'False', '1'")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<uint>("ConcurrencyToken")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("xmin")
                        .HasColumnType("xid");

                    b.Property<Guid?>("CreatedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("PermissionId")
                        .HasColumnType("integer");

                    b.Property<int>("RoleId")
                        .HasColumnType("integer");

                    b.Property<Guid?>("UpdatedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("PermissionId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UpdatedById");

                    b.ToTable("RolePermission");
                });

            modelBuilder.Entity("CAS.DB.models.auth.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasAnnotation("Npgsql:IdentitySequenceOptions", "'200', '1', '', '', 'False', '1'");

                    b.Property<uint>("ConcurrencyToken")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("xmin")
                        .HasColumnType("xid");

                    b.Property<Guid?>("CreatedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<int?>("HomeLocationId")
                        .HasColumnType("integer");

                    b.Property<Guid?>("IdirId")
                        .HasColumnType("uuid");

                    b.Property<string>("IdirName")
                        .HasColumnType("text");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("boolean");

                    b.Property<Guid?>("KeyCloakId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("LastLogin")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<Guid?>("UpdatedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("HomeLocationId");

                    b.HasIndex("UpdatedById");

                    b.ToTable("User");

                    b.HasDiscriminator<string>("Discriminator").HasValue("User");

                    b.HasData(
                        new
                        {
                            Id = new Guid("00000000-0000-0000-0000-000000000001"),
                            ConcurrencyToken = 0u,
                            CreatedOn = new DateTime(2020, 10, 6, 14, 56, 55, 989, DateTimeKind.Local).AddTicks(3226),
                            FirstName = "SYSTEM",
                            IsEnabled = false,
                            LastName = "SYSTEM"
                        });
                });

            modelBuilder.Entity("CAS.DB.models.auth.UserRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:IdentitySequenceOptions", "'100', '1', '', '', 'False', '1'")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<uint>("ConcurrencyToken")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("xmin")
                        .HasColumnType("xid");

                    b.Property<Guid?>("CreatedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("RoleId")
                        .HasColumnType("integer");

                    b.Property<Guid?>("UpdatedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("RoleId");

                    b.HasIndex("UpdatedById");

                    b.HasIndex("UserId");

                    b.ToTable("UserRole");
                });

            modelBuilder.Entity("CAS.DB.models.lookupcodes.LookupSortOrder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<uint>("ConcurrencyToken")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("xmin")
                        .HasColumnType("xid");

                    b.Property<Guid?>("CreatedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("LocationId")
                        .HasColumnType("integer");

                    b.Property<int?>("LookupCodeId")
                        .HasColumnType("integer");

                    b.Property<int>("LookupType")
                        .HasColumnType("integer");

                    b.Property<int>("SortOrder")
                        .HasColumnType("integer");

                    b.Property<Guid?>("UpdatedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("LocationId");

                    b.HasIndex("LookupCodeId");

                    b.HasIndex("LookupType");

                    b.HasIndex("UpdatedById");

                    b.ToTable("LookupSortOrder");
                });

            modelBuilder.Entity("CAS.DB.models.sheriff.SheriffAwayLocation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<uint>("ConcurrencyToken")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("xmin")
                        .HasColumnType("xid");

                    b.Property<Guid?>("CreatedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("ExpiryDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("IsFullDay")
                        .HasColumnType("boolean");

                    b.Property<int?>("LocationId")
                        .HasColumnType("integer");

                    b.Property<Guid>("SheriffId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("StartDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid?>("UpdatedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("LocationId");

                    b.HasIndex("SheriffId");

                    b.HasIndex("UpdatedById");

                    b.ToTable("SheriffAwayLocation");
                });

            modelBuilder.Entity("CAS.DB.models.sheriff.SheriffLeave", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<uint>("ConcurrencyToken")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("xmin")
                        .HasColumnType("xid");

                    b.Property<Guid?>("CreatedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("ExpiryDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("IsFullDay")
                        .HasColumnType("boolean");

                    b.Property<int?>("LeaveTypeId")
                        .HasColumnType("integer");

                    b.Property<Guid>("SheriffId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("StartDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid?>("UpdatedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("LeaveTypeId");

                    b.HasIndex("SheriffId");

                    b.HasIndex("UpdatedById");

                    b.ToTable("SheriffLeave");
                });

            modelBuilder.Entity("CAS.DB.models.sheriff.SheriffTraining", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<uint>("ConcurrencyToken")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("xmin")
                        .HasColumnType("xid");

                    b.Property<Guid?>("CreatedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("ExpiryDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("IsFullDay")
                        .HasColumnType("boolean");

                    b.Property<Guid>("SheriffId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("StartDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("TrainingTypeId")
                        .HasColumnType("integer");

                    b.Property<Guid?>("UpdatedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("SheriffId");

                    b.HasIndex("TrainingTypeId");

                    b.HasIndex("UpdatedById");

                    b.ToTable("SheriffTraining");
                });

            modelBuilder.Entity("db.models.Region", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Code")
                        .HasColumnType("text");

                    b.Property<uint>("ConcurrencyToken")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("xmin")
                        .HasColumnType("xid");

                    b.Property<Guid?>("CreatedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("ExpiryDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("JustinId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<Guid?>("UpdatedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("JustinId")
                        .IsUnique();

                    b.HasIndex("UpdatedById");

                    b.ToTable("Region");
                });

            modelBuilder.Entity("CAS.DB.models.LookupCode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:IdentitySequenceOptions", "'200', '1', '', '', 'False', '1'")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Code")
                        .HasColumnType("text");

                    b.Property<uint>("ConcurrencyToken")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("xmin")
                        .HasColumnType("xid");

                    b.Property<Guid?>("CreatedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<DateTime?>("EffectiveDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("ExpiryDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("LocationId")
                        .HasColumnType("integer");

                    b.Property<int?>("SortOrder")
                        .HasColumnType("integer");

                    b.Property<string>("SubCode")
                        .HasColumnType("text");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.Property<Guid?>("UpdatedById")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("LocationId");

                    b.HasIndex("UpdatedById");

                    b.HasIndex("Type", "Code", "LocationId")
                        .IsUnique();

                    b.ToTable("LookupCode");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            ConcurrencyToken = 0u,
                            CreatedOn = new DateTime(2020, 10, 6, 21, 56, 55, 976, DateTimeKind.Utc).AddTicks(8752),
                            Description = "Chief Sheriff",
                            Type = 7
                        },
                        new
                        {
                            Id = 2,
                            ConcurrencyToken = 0u,
                            CreatedOn = new DateTime(2020, 10, 6, 21, 56, 55, 977, DateTimeKind.Utc).AddTicks(3),
                            Description = "Superintendent",
                            Type = 7
                        },
                        new
                        {
                            Id = 3,
                            ConcurrencyToken = 0u,
                            CreatedOn = new DateTime(2020, 10, 6, 21, 56, 55, 977, DateTimeKind.Utc).AddTicks(34),
                            Description = "Staff Inspector",
                            Type = 7
                        },
                        new
                        {
                            Id = 4,
                            ConcurrencyToken = 0u,
                            CreatedOn = new DateTime(2020, 10, 6, 21, 56, 55, 977, DateTimeKind.Utc).AddTicks(36),
                            Description = "Inspector",
                            Type = 7
                        },
                        new
                        {
                            Id = 5,
                            ConcurrencyToken = 0u,
                            CreatedOn = new DateTime(2020, 10, 6, 21, 56, 55, 977, DateTimeKind.Utc).AddTicks(37),
                            Description = "Staff Sergeant",
                            Type = 7
                        },
                        new
                        {
                            Id = 6,
                            ConcurrencyToken = 0u,
                            CreatedOn = new DateTime(2020, 10, 6, 21, 56, 55, 977, DateTimeKind.Utc).AddTicks(38),
                            Description = "Sergeant",
                            Type = 7
                        },
                        new
                        {
                            Id = 7,
                            ConcurrencyToken = 0u,
                            CreatedOn = new DateTime(2020, 10, 6, 21, 56, 55, 977, DateTimeKind.Utc).AddTicks(40),
                            Description = "Deputy Sheriff",
                            Type = 7
                        });
                });

            modelBuilder.Entity("CAS.DB.models.sheriff.Sheriff", b =>
                {
                    b.HasBaseType("CAS.DB.models.auth.User");

                    b.Property<string>("BadgeNumber")
                        .HasColumnType("text");

                    b.Property<int>("Gender")
                        .HasColumnType("integer");

                    b.Property<byte[]>("Photo")
                        .HasColumnType("bytea");

                    b.Property<string>("Rank")
                        .HasColumnType("text");

                    b.HasDiscriminator().HasValue("Sheriff");
                });

            modelBuilder.Entity("CAS.API.Models.DB.Location", b =>
                {
                    b.HasOne("CAS.DB.models.auth.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("db.models.Region", "Region")
                        .WithMany()
                        .HasForeignKey("RegionId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("CAS.DB.models.auth.User", "UpdatedBy")
                        .WithMany()
                        .HasForeignKey("UpdatedById")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("CAS.DB.models.auth.Permission", b =>
                {
                    b.HasOne("CAS.DB.models.auth.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("CAS.DB.models.auth.User", "UpdatedBy")
                        .WithMany()
                        .HasForeignKey("UpdatedById")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("CAS.DB.models.auth.Role", b =>
                {
                    b.HasOne("CAS.DB.models.auth.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("CAS.DB.models.auth.User", "UpdatedBy")
                        .WithMany()
                        .HasForeignKey("UpdatedById")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("CAS.DB.models.auth.RolePermission", b =>
                {
                    b.HasOne("CAS.DB.models.auth.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("CAS.DB.models.auth.Permission", "Permission")
                        .WithMany()
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CAS.DB.models.auth.Role", "Role")
                        .WithMany("RolePermissions")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CAS.DB.models.auth.User", "UpdatedBy")
                        .WithMany()
                        .HasForeignKey("UpdatedById")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("CAS.DB.models.auth.User", b =>
                {
                    b.HasOne("CAS.DB.models.auth.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("CAS.API.Models.DB.Location", "HomeLocation")
                        .WithMany()
                        .HasForeignKey("HomeLocationId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("CAS.DB.models.auth.User", "UpdatedBy")
                        .WithMany()
                        .HasForeignKey("UpdatedById")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("CAS.DB.models.auth.UserRole", b =>
                {
                    b.HasOne("CAS.DB.models.auth.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("CAS.DB.models.auth.Role", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.HasOne("CAS.DB.models.auth.User", "UpdatedBy")
                        .WithMany()
                        .HasForeignKey("UpdatedById")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("CAS.DB.models.auth.User", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CAS.DB.models.lookupcodes.LookupSortOrder", b =>
                {
                    b.HasOne("CAS.DB.models.auth.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("CAS.API.Models.DB.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CAS.DB.models.LookupCode", "LookupCode")
                        .WithMany()
                        .HasForeignKey("LookupCodeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CAS.DB.models.auth.User", "UpdatedBy")
                        .WithMany()
                        .HasForeignKey("UpdatedById")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("CAS.DB.models.sheriff.SheriffAwayLocation", b =>
                {
                    b.HasOne("CAS.DB.models.auth.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("CAS.API.Models.DB.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CAS.DB.models.sheriff.Sheriff", "Sheriff")
                        .WithMany("AwayLocation")
                        .HasForeignKey("SheriffId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CAS.DB.models.auth.User", "UpdatedBy")
                        .WithMany()
                        .HasForeignKey("UpdatedById")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("CAS.DB.models.sheriff.SheriffLeave", b =>
                {
                    b.HasOne("CAS.DB.models.auth.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById");

                    b.HasOne("CAS.DB.models.LookupCode", "LeaveType")
                        .WithMany()
                        .HasForeignKey("LeaveTypeId");

                    b.HasOne("CAS.DB.models.sheriff.Sheriff", "Sheriff")
                        .WithMany("Leave")
                        .HasForeignKey("SheriffId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CAS.DB.models.auth.User", "UpdatedBy")
                        .WithMany()
                        .HasForeignKey("UpdatedById");
                });

            modelBuilder.Entity("CAS.DB.models.sheriff.SheriffTraining", b =>
                {
                    b.HasOne("CAS.DB.models.auth.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById");

                    b.HasOne("CAS.DB.models.sheriff.Sheriff", "Sheriff")
                        .WithMany("Training")
                        .HasForeignKey("SheriffId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CAS.DB.models.LookupCode", "TrainingType")
                        .WithMany()
                        .HasForeignKey("TrainingTypeId");

                    b.HasOne("CAS.DB.models.auth.User", "UpdatedBy")
                        .WithMany()
                        .HasForeignKey("UpdatedById");
                });

            modelBuilder.Entity("db.models.Region", b =>
                {
                    b.HasOne("CAS.DB.models.auth.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("CAS.DB.models.auth.User", "UpdatedBy")
                        .WithMany()
                        .HasForeignKey("UpdatedById")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("CAS.DB.models.LookupCode", b =>
                {
                    b.HasOne("CAS.DB.models.auth.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("CAS.API.Models.DB.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("CAS.DB.models.auth.User", "UpdatedBy")
                        .WithMany()
                        .HasForeignKey("UpdatedById")
                        .OnDelete(DeleteBehavior.SetNull);
                });
#pragma warning restore 612, 618
        }
    }
}
