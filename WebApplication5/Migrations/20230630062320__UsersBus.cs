using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication5.Migrations
{
    public partial class _UsersBus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UsersSubs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SubstiteUserId = table.Column<int>(nullable: true),
                    ReplacedUserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersSubs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsersSubs_Users_ReplacedUserId",
                        column: x => x.ReplacedUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsersSubs_Users_SubstiteUserId",
                        column: x => x.SubstiteUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsersSubs_ReplacedUserId",
                table: "UsersSubs",
                column: "ReplacedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersSubs_SubstiteUserId",
                table: "UsersSubs",
                column: "SubstiteUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsersSubs");
        }
    }
}
