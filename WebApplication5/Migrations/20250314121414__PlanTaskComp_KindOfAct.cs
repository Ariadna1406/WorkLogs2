using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication5.Migrations
{
    public partial class _PlanTaskComp_KindOfAct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KindOfActId",
                table: "PlanTaskComp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Percent",
                table: "PlanTaskComp",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PlanTaskComp_KindOfActId",
                table: "PlanTaskComp",
                column: "KindOfActId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlanTaskComp_KindOfAct_KindOfActId",
                table: "PlanTaskComp",
                column: "KindOfActId",
                principalTable: "KindOfAct",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlanTaskComp_KindOfAct_KindOfActId",
                table: "PlanTaskComp");

            migrationBuilder.DropIndex(
                name: "IX_PlanTaskComp_KindOfActId",
                table: "PlanTaskComp");

            migrationBuilder.DropColumn(
                name: "KindOfActId",
                table: "PlanTaskComp");

            migrationBuilder.DropColumn(
                name: "Percent",
                table: "PlanTaskComp");
        }
    }
}
