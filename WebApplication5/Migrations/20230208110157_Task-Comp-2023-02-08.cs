using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication5.Migrations
{
    public partial class TaskComp20230208 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaskComps",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProjectNumber = table.Column<string>(nullable: true),
                    TaskCompName = table.Column<string>(nullable: true),
                    Department = table.Column<string>(nullable: true),
                    Executers = table.Column<string>(nullable: true),
                    StartPlanDate = table.Column<DateTime>(nullable: true),
                    FinishPlanDate = table.Column<DateTime>(nullable: true),
                    PlanWorkLog = table.Column<double>(nullable: true),
                    StartFactDate = table.Column<DateTime>(nullable: true),
                    FinishFactDate = table.Column<DateTime>(nullable: true),
                    FactWorkLog = table.Column<double>(nullable: true),
                    CompletePercent = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskComps", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskComps");
        }
    }
}
