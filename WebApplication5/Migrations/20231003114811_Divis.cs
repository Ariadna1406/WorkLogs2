using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication5.Migrations
{
    public partial class Divis : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Division",
                table: "TaskComps",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DivisionForReport",
                table: "TaskComps",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Division",
                table: "TaskComps");

            migrationBuilder.DropColumn(
                name: "DivisionForReport",
                table: "TaskComps");
        }
    }
}
