using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication5.Migrations
{
    public partial class _User_PercentHist : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "TaskCompPercentHistories",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskCompPercentHistories_UserId",
                table: "TaskCompPercentHistories",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskCompPercentHistories_Users_UserId",
                table: "TaskCompPercentHistories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskCompPercentHistories_Users_UserId",
                table: "TaskCompPercentHistories");

            migrationBuilder.DropIndex(
                name: "IX_TaskCompPercentHistories_UserId",
                table: "TaskCompPercentHistories");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TaskCompPercentHistories");
        }
    }
}
