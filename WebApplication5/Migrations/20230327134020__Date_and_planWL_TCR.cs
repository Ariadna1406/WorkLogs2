using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication5.Migrations
{
    public partial class _Date_and_planWL_TCR : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FinishDate",
                table: "TaskCompRequests",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "PlanWorkLog",
                table: "TaskCompRequests",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "TaskCompRequests",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinishDate",
                table: "TaskCompRequests");

            migrationBuilder.DropColumn(
                name: "PlanWorkLog",
                table: "TaskCompRequests");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "TaskCompRequests");
        }
    }
}
