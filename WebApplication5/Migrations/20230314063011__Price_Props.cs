using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication5.Migrations
{
    public partial class _Price_Props : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CommnetToPrice",
                table: "TaskComps",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateMoneyGet",
                table: "TaskComps",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Moneyleft",
                table: "TaskComps",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "MoneyleftNHP",
                table: "TaskComps",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "NumberOfUPD",
                table: "TaskComps",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Prepayment",
                table: "TaskComps",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "TaskComps",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SubContractorPart",
                table: "TaskComps",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommnetToPrice",
                table: "TaskComps");

            migrationBuilder.DropColumn(
                name: "DateMoneyGet",
                table: "TaskComps");

            migrationBuilder.DropColumn(
                name: "Moneyleft",
                table: "TaskComps");

            migrationBuilder.DropColumn(
                name: "MoneyleftNHP",
                table: "TaskComps");

            migrationBuilder.DropColumn(
                name: "NumberOfUPD",
                table: "TaskComps");

            migrationBuilder.DropColumn(
                name: "Prepayment",
                table: "TaskComps");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "TaskComps");

            migrationBuilder.DropColumn(
                name: "SubContractorPart",
                table: "TaskComps");
        }
    }
}
