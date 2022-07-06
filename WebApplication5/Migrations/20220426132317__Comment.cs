using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication5.Migrations
{
    public partial class _Comment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "WorkLogs",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfSendingReport",
                table: "WorkLogs",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "WorkLogs");

            migrationBuilder.DropColumn(
                name: "DateOfSendingReport",
                table: "WorkLogs");
        }
    }
}
