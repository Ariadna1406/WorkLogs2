using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication5.Migrations
{
    public partial class _ApproveTaskComp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApprovePlanTaskComp",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserCreatedRequestId = table.Column<int>(nullable: true),
                    PlanMonth = table.Column<int>(nullable: false),
                    PlanYear = table.Column<int>(nullable: false),
                    RejectComment = table.Column<string>(nullable: true),
                    RejectedUser = table.Column<string>(nullable: true),
                    PlanTaskCompStatus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovePlanTaskComp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovePlanTaskComp_Users_UserCreatedRequestId",
                        column: x => x.UserCreatedRequestId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovePlanTaskComp_UserCreatedRequestId",
                table: "ApprovePlanTaskComp",
                column: "UserCreatedRequestId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApprovePlanTaskComp");
        }
    }
}
