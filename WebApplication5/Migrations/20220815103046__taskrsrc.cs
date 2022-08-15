using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication5.Migrations
{
    public partial class _taskrsrc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Task_id",
                table: "WorkLogs",
                newName: "Taskrsrc_id");
            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {            

            migrationBuilder.RenameColumn(
                name: "Taskrsrc_id",
                table: "WorkLogs",
                newName: "Task_id");
        }
    }
}
