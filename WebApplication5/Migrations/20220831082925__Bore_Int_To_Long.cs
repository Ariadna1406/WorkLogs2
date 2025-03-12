using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication5.Migrations
{
    public partial class _Bore_Int_To_Long : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "PipeLineLength",
                table: "avevaPipeLengths",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PipeLineLength",
                table: "avevaPipeLengths",
                nullable: false,
                oldClrType: typeof(long));
        }
    }
}
