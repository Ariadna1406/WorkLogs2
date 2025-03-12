using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication5.Migrations
{
    public partial class _Absence_Percent_Change : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReasonId",
                table: "Absences",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Absences",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Absences_ReasonId",
                table: "Absences",
                column: "ReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_Absences_UserId",
                table: "Absences",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Absences_AbsenceReasons_ReasonId",
                table: "Absences",
                column: "ReasonId",
                principalTable: "AbsenceReasons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Absences_Users_UserId",
                table: "Absences",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Absences_AbsenceReasons_ReasonId",
                table: "Absences");

            migrationBuilder.DropForeignKey(
                name: "FK_Absences_Users_UserId",
                table: "Absences");

            migrationBuilder.DropIndex(
                name: "IX_Absences_ReasonId",
                table: "Absences");

            migrationBuilder.DropIndex(
                name: "IX_Absences_UserId",
                table: "Absences");

            migrationBuilder.DropColumn(
                name: "ReasonId",
                table: "Absences");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Absences");
        }
    }
}
