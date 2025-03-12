using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication5.Migrations
{
    public partial class _TaskCompPlan_TaskCompPlanBackToPlanTaskComp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskCompPlan");

            migrationBuilder.CreateTable(
                name: "PlanTaskComp",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TaskCompId = table.Column<int>(nullable: true),
                    StartPlanDate = table.Column<DateTime>(nullable: false),
                    FinishPlanDate = table.Column<DateTime>(nullable: false),
                    Intencity = table.Column<double>(nullable: false),
                    ExecuterId = table.Column<int>(nullable: true),
                    AuthorId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanTaskComp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanTaskComp_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlanTaskComp_Users_ExecuterId",
                        column: x => x.ExecuterId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlanTaskComp_TaskComps_TaskCompId",
                        column: x => x.TaskCompId,
                        principalTable: "TaskComps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlanTaskComp_AuthorId",
                table: "PlanTaskComp",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanTaskComp_ExecuterId",
                table: "PlanTaskComp",
                column: "ExecuterId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanTaskComp_TaskCompId",
                table: "PlanTaskComp",
                column: "TaskCompId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlanTaskComp");

            migrationBuilder.CreateTable(
                name: "TaskCompPlan",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AuthorId = table.Column<int>(nullable: true),
                    ExecuterId = table.Column<int>(nullable: true),
                    FinishPlanDate = table.Column<DateTime>(nullable: false),
                    Intencity = table.Column<double>(nullable: false),
                    StartPlanDate = table.Column<DateTime>(nullable: false),
                    TaskCompId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskCompPlan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskCompPlan_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskCompPlan_Users_ExecuterId",
                        column: x => x.ExecuterId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskCompPlan_TaskComps_TaskCompId",
                        column: x => x.TaskCompId,
                        principalTable: "TaskComps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskCompPlan_AuthorId",
                table: "TaskCompPlan",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskCompPlan_ExecuterId",
                table: "TaskCompPlan",
                column: "ExecuterId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskCompPlan_TaskCompId",
                table: "TaskCompPlan",
                column: "TaskCompId");
        }
    }
}
