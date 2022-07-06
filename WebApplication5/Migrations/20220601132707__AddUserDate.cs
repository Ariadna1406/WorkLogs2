using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication5.Migrations
{
    public partial class _AddUserDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LicStatus",
                table: "AvevaLicences");

            migrationBuilder.AddColumn<DateTime>(
                name: "AddUserDate",
                table: "AvevaLicences",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddUserDate",
                table: "AvevaLicences");

            migrationBuilder.AddColumn<string>(
                name: "LicStatus",
                table: "AvevaLicences",
                nullable: true);
        }
    }
}
