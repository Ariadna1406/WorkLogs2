using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication5.Migrations
{
    public partial class __PlanTaskComp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlanTaskComp",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: true),
                    TaskCompId = table.Column<int>(nullable: true),
                    WorkLoadDate = table.Column<DateTime>(nullable: false),
                    WorkLoadAmount = table.Column<TimeSpan>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanTaskComp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanTaskComp_Users_TaskCompId",
                        column: x => x.TaskCompId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlanTaskComp_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlanTaskComp_TaskCompId",
                table: "PlanTaskComp",
                column: "TaskCompId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanTaskComp_UserId",
                table: "PlanTaskComp",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlanTaskComp");
        }
    }
}
