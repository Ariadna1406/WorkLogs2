using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication5.Migrations
{
    public partial class _task_comp20230209 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Taskrsrc_id",
                table: "WorkLogs",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "Task_id",
                table: "WorkLogs",
                newName: "TaskComp_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "WorkLogs",
                newName: "Taskrsrc_id");

            migrationBuilder.RenameColumn(
                name: "TaskComp_id",
                table: "WorkLogs",
                newName: "Task_id");
        }
    }
}
