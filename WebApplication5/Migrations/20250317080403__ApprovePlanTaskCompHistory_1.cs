using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication5.Migrations
{
    public partial class _ApprovePlanTaskCompHistory_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApprovePlanTaskCompStatusHistories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ApprovePlanTaskCompId = table.Column<int>(nullable: true),
                    ChangedStatusDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    UserChangedStatusId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovePlanTaskCompStatusHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovePlanTaskCompStatusHistories_ApprovePlanTaskComp_ApprovePlanTaskCompId",
                        column: x => x.ApprovePlanTaskCompId,
                        principalTable: "ApprovePlanTaskComp",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApprovePlanTaskCompStatusHistories_Users_UserChangedStatusId",
                        column: x => x.UserChangedStatusId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovePlanTaskCompStatusHistories_ApprovePlanTaskCompId",
                table: "ApprovePlanTaskCompStatusHistories",
                column: "ApprovePlanTaskCompId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovePlanTaskCompStatusHistories_UserChangedStatusId",
                table: "ApprovePlanTaskCompStatusHistories",
                column: "UserChangedStatusId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApprovePlanTaskCompStatusHistories");
        }
    }
}
