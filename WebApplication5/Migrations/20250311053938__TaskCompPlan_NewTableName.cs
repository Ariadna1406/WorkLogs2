using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication5.Migrations
{
    public partial class _TaskCompPlan_NewTableName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlanTaskComp_Users_ExecuterId",
                table: "PlanTaskComp");

            migrationBuilder.DropForeignKey(
                name: "FK_PlanTaskComp_TaskComps_TaskCompId",
                table: "PlanTaskComp");

            migrationBuilder.DropForeignKey(
                name: "FK_PlanTaskComp_Users_UserId",
                table: "PlanTaskComp");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlanTaskComp",
                table: "PlanTaskComp");

            migrationBuilder.RenameTable(
                name: "PlanTaskComp",
                newName: "TaskCompPlan");

            migrationBuilder.RenameIndex(
                name: "IX_PlanTaskComp_UserId",
                table: "TaskCompPlan",
                newName: "IX_TaskCompPlan_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PlanTaskComp_TaskCompId",
                table: "TaskCompPlan",
                newName: "IX_TaskCompPlan_TaskCompId");

            migrationBuilder.RenameIndex(
                name: "IX_PlanTaskComp_ExecuterId",
                table: "TaskCompPlan",
                newName: "IX_TaskCompPlan_ExecuterId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskCompPlan",
                table: "TaskCompPlan",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskCompPlan_Users_ExecuterId",
                table: "TaskCompPlan",
                column: "ExecuterId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskCompPlan_TaskComps_TaskCompId",
                table: "TaskCompPlan",
                column: "TaskCompId",
                principalTable: "TaskComps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskCompPlan_Users_UserId",
                table: "TaskCompPlan",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskCompPlan_Users_ExecuterId",
                table: "TaskCompPlan");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskCompPlan_TaskComps_TaskCompId",
                table: "TaskCompPlan");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskCompPlan_Users_UserId",
                table: "TaskCompPlan");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskCompPlan",
                table: "TaskCompPlan");

            migrationBuilder.RenameTable(
                name: "TaskCompPlan",
                newName: "PlanTaskComp");

            migrationBuilder.RenameIndex(
                name: "IX_TaskCompPlan_UserId",
                table: "PlanTaskComp",
                newName: "IX_PlanTaskComp_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskCompPlan_TaskCompId",
                table: "PlanTaskComp",
                newName: "IX_PlanTaskComp_TaskCompId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskCompPlan_ExecuterId",
                table: "PlanTaskComp",
                newName: "IX_PlanTaskComp_ExecuterId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlanTaskComp",
                table: "PlanTaskComp",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PlanTaskComp_Users_ExecuterId",
                table: "PlanTaskComp",
                column: "ExecuterId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PlanTaskComp_TaskComps_TaskCompId",
                table: "PlanTaskComp",
                column: "TaskCompId",
                principalTable: "TaskComps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PlanTaskComp_Users_UserId",
                table: "PlanTaskComp",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
