using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication5.Migrations
{
    public partial class _AvevaPipeLength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "avevaPipeLengths",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProjectId = table.Column<int>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    PipeLineLength = table.Column<int>(nullable: false),
                    PipeLineBore = table.Column<int>(nullable: false),
                    ProjectAcr = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_avevaPipeLengths", x => x.Id);
                    table.ForeignKey(
                        name: "FK_avevaPipeLengths_ProjectSet_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "ProjectSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_avevaPipeLengths_ProjectId",
                table: "avevaPipeLengths",
                column: "ProjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "avevaPipeLengths");
        }
    }
}
