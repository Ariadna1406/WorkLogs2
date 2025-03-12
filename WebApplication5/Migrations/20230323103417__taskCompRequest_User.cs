using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication5.Migrations
{
    public partial class _taskCompRequest_User : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfRequest",
                table: "TaskCompRequests",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "TaskCompRequests",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskCompRequests_UserId",
                table: "TaskCompRequests",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskCompRequests_Users_UserId",
                table: "TaskCompRequests",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskCompRequests_Users_UserId",
                table: "TaskCompRequests");

            migrationBuilder.DropIndex(
                name: "IX_TaskCompRequests_UserId",
                table: "TaskCompRequests");

            migrationBuilder.DropColumn(
                name: "DateOfRequest",
                table: "TaskCompRequests");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TaskCompRequests");
        }
    }
}
