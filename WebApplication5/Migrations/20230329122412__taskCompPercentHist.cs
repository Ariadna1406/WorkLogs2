using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication5.Migrations
{
    public partial class _taskCompPercentHist : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaskCompPercentHistories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TaskCompId = table.Column<int>(nullable: true),
                    ChangePercentDate = table.Column<DateTime>(nullable: false),
                    Percent = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskCompPercentHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskCompPercentHistories_TaskComps_TaskCompId",
                        column: x => x.TaskCompId,
                        principalTable: "TaskComps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskCompPercentHistories_TaskCompId",
                table: "TaskCompPercentHistories",
                column: "TaskCompId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskCompPercentHistories");
        }
    }
}
