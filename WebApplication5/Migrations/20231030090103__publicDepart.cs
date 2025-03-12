using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication5.Migrations
{
    public partial class _publicDepart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlanTaskComp_Users_TaskCompId",
                table: "PlanTaskComp");

            migrationBuilder.AddColumn<string>(
                name: "PublicDepart",
                table: "Users",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PlanTaskComp_TaskComps_TaskCompId",
                table: "PlanTaskComp",
                column: "TaskCompId",
                principalTable: "TaskComps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlanTaskComp_TaskComps_TaskCompId",
                table: "PlanTaskComp");

            migrationBuilder.DropColumn(
                name: "PublicDepart",
                table: "Users");

            migrationBuilder.AddForeignKey(
                name: "FK_PlanTaskComp_Users_TaskCompId",
                table: "PlanTaskComp",
                column: "TaskCompId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
