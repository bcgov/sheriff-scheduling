using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CAS.DB.Migrations
{
    public partial class FK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RolePermission_Permission_PermissionId",
                table: "RolePermission");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermission_Role_RoleId",
                table: "RolePermission");

            migrationBuilder.DropForeignKey(
                name: "FK_SheriffAwayLocation_User_SheriffId",
                table: "SheriffAwayLocation");

            migrationBuilder.DropForeignKey(
                name: "FK_SheriffLeave_User_SheriffId",
                table: "SheriffLeave");

            migrationBuilder.DropForeignKey(
                name: "FK_SheriffTraining_User_SheriffId",
                table: "SheriffTraining");

            migrationBuilder.AlterColumn<Guid>(
                name: "SheriffId",
                table: "SheriffTraining",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "SheriffId",
                table: "SheriffLeave",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "SheriffId",
                table: "SheriffAwayLocation",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RoleId",
                table: "RolePermission",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PermissionId",
                table: "RolePermission",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "Region",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Region",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Region",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedById",
                table: "Region",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "Region",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ParentLocationId",
                table: "Location",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "JustinId",
                table: "Location",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_Region_CreatedById",
                table: "Region",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Region_UpdatedById",
                table: "Region",
                column: "UpdatedById");

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
                name: "FK_RolePermission_Permission_PermissionId",
                table: "RolePermission",
                column: "PermissionId",
                principalTable: "Permission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermission_Role_RoleId",
                table: "RolePermission",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SheriffAwayLocation_User_SheriffId",
                table: "SheriffAwayLocation",
                column: "SheriffId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SheriffLeave_User_SheriffId",
                table: "SheriffLeave",
                column: "SheriffId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SheriffTraining_User_SheriffId",
                table: "SheriffTraining",
                column: "SheriffId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Region_User_CreatedById",
                table: "Region");

            migrationBuilder.DropForeignKey(
                name: "FK_Region_User_UpdatedById",
                table: "Region");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermission_Permission_PermissionId",
                table: "RolePermission");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermission_Role_RoleId",
                table: "RolePermission");

            migrationBuilder.DropForeignKey(
                name: "FK_SheriffAwayLocation_User_SheriffId",
                table: "SheriffAwayLocation");

            migrationBuilder.DropForeignKey(
                name: "FK_SheriffLeave_User_SheriffId",
                table: "SheriffLeave");

            migrationBuilder.DropForeignKey(
                name: "FK_SheriffTraining_User_SheriffId",
                table: "SheriffTraining");

            migrationBuilder.DropIndex(
                name: "IX_Region_CreatedById",
                table: "Region");

            migrationBuilder.DropIndex(
                name: "IX_Region_UpdatedById",
                table: "Region");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Region");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Region");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Region");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "Region");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "Region");

            migrationBuilder.AlterColumn<Guid>(
                name: "SheriffId",
                table: "SheriffTraining",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<Guid>(
                name: "SheriffId",
                table: "SheriffLeave",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<Guid>(
                name: "SheriffId",
                table: "SheriffAwayLocation",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<int>(
                name: "RoleId",
                table: "RolePermission",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "PermissionId",
                table: "RolePermission",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "ParentLocationId",
                table: "Location",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "JustinId",
                table: "Location",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermission_Permission_PermissionId",
                table: "RolePermission",
                column: "PermissionId",
                principalTable: "Permission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermission_Role_RoleId",
                table: "RolePermission",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SheriffAwayLocation_User_SheriffId",
                table: "SheriffAwayLocation",
                column: "SheriffId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SheriffLeave_User_SheriffId",
                table: "SheriffLeave",
                column: "SheriffId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SheriffTraining_User_SheriffId",
                table: "SheriffTraining",
                column: "SheriffId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
