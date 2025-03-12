using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication5.Migrations
{
    public partial class __taskCompStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CommnetToPrice",
                table: "TaskComps",
                newName: "TaskCompStatus");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TaskCompStatus",
                table: "TaskComps",
                newName: "CommnetToPrice");
        }
    }
}
