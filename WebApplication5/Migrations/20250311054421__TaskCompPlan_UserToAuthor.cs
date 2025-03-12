using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication5.Migrations
{
    public partial class _TaskCompPlan_UserToAuthor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskCompPlan_Users_UserId",
                table: "TaskCompPlan");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "TaskCompPlan",
                newName: "AuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskCompPlan_UserId",
                table: "TaskCompPlan",
                newName: "IX_TaskCompPlan_AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskCompPlan_Users_AuthorId",
                table: "TaskCompPlan",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskCompPlan_Users_AuthorId",
                table: "TaskCompPlan");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "TaskCompPlan",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskCompPlan_AuthorId",
                table: "TaskCompPlan",
                newName: "IX_TaskCompPlan_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskCompPlan_Users_UserId",
                table: "TaskCompPlan",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
