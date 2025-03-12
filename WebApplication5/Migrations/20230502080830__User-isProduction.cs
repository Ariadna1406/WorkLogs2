﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication5.Migrations
{
    public partial class _UserisProduction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsProduction",
                table: "Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsProduction",
                table: "Users");
        }
    }
}
