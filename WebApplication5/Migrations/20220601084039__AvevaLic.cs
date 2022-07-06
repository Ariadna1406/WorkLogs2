using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication5.Migrations
{
    public partial class _AvevaLic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AvevaElemAmounts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProjectId = table.Column<int>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    PipeLineAmount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvevaElemAmounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AvevaElemAmounts_ProjectSet_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "ProjectSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AvevaLicences",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    HostName = table.Column<string>(nullable: true),
                    LicApplyDate = table.Column<DateTime>(nullable: false),
                    LicStatus = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvevaLicences", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AvevaElemAmounts_ProjectId",
                table: "AvevaElemAmounts",
                column: "ProjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AvevaElemAmounts");

            migrationBuilder.DropTable(
                name: "AvevaLicences");
        }
    }
}
