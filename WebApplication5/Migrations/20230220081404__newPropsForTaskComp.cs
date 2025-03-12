using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication5.Migrations
{
    public partial class _newPropsForTaskComp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApproveFactDate",
                table: "TaskComps",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContractStatus",
                table: "TaskComps",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentSendTask",
                table: "TaskComps",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FactDateSendTask",
                table: "TaskComps",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FisnishContractDate",
                table: "TaskComps",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GIPAcronym",
                table: "TaskComps",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OperationName",
                table: "TaskComps",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PlanDateSendTask",
                table: "TaskComps",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProjectShortName",
                table: "TaskComps",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProjectStage",
                table: "TaskComps",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApproveFactDate",
                table: "TaskComps");

            migrationBuilder.DropColumn(
                name: "ContractStatus",
                table: "TaskComps");

            migrationBuilder.DropColumn(
                name: "DepartmentSendTask",
                table: "TaskComps");

            migrationBuilder.DropColumn(
                name: "FactDateSendTask",
                table: "TaskComps");

            migrationBuilder.DropColumn(
                name: "FisnishContractDate",
                table: "TaskComps");

            migrationBuilder.DropColumn(
                name: "GIPAcronym",
                table: "TaskComps");

            migrationBuilder.DropColumn(
                name: "OperationName",
                table: "TaskComps");

            migrationBuilder.DropColumn(
                name: "PlanDateSendTask",
                table: "TaskComps");

            migrationBuilder.DropColumn(
                name: "ProjectShortName",
                table: "TaskComps");

            migrationBuilder.DropColumn(
                name: "ProjectStage",
                table: "TaskComps");
        }
    }
}
