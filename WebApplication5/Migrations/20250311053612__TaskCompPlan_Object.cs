using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication5.Migrations
{
    public partial class _TaskCompPlan_Object : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkLoadAmount",
                table: "PlanTaskComp");

            migrationBuilder.RenameColumn(
                name: "WorkLoadDate",
                table: "PlanTaskComp",
                newName: "StartPlanDate");

            migrationBuilder.AddColumn<int>(
                name: "ExecuterId",
                table: "PlanTaskComp",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FinishPlanDate",
                table: "PlanTaskComp",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<double>(
                name: "Intencity",
                table: "PlanTaskComp",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_PlanTaskComp_ExecuterId",
                table: "PlanTaskComp",
                column: "ExecuterId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlanTaskComp_Users_ExecuterId",
                table: "PlanTaskComp",
                column: "ExecuterId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlanTaskComp_Users_ExecuterId",
                table: "PlanTaskComp");

            migrationBuilder.DropIndex(
                name: "IX_PlanTaskComp_ExecuterId",
                table: "PlanTaskComp");

            migrationBuilder.DropColumn(
                name: "ExecuterId",
                table: "PlanTaskComp");

            migrationBuilder.DropColumn(
                name: "FinishPlanDate",
                table: "PlanTaskComp");

            migrationBuilder.DropColumn(
                name: "Intencity",
                table: "PlanTaskComp");

            migrationBuilder.RenameColumn(
                name: "StartPlanDate",
                table: "PlanTaskComp",
                newName: "WorkLoadDate");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "WorkLoadAmount",
                table: "PlanTaskComp",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }
    }
}
