using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication5.Migrations
{
    public partial class _init20220414 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KindOfAct",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KindOfAct", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResponseSet",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(nullable: false),
                    ImageLink = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResponseSet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    MiddleName = table.Column<string>(nullable: true),
                    AD_GUID = table.Column<string>(nullable: true),
                    Login = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    DepartId = table.Column<int>(nullable: true),
                    FullName = table.Column<string>(nullable: true),
                    NameFromAD = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    HeadOfDepartmentId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Departments_Users_HeadOfDepartmentId",
                        column: x => x.HeadOfDepartmentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectSet",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InternalNum = table.Column<string>(nullable: false),
                    ContractNumber = table.Column<string>(nullable: true),
                    ShortName = table.Column<string>(nullable: true),
                    FullName = table.Column<string>(nullable: false),
                    ManagerId = table.Column<int>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    ShowInMenuBar = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectSet_Users_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: true),
                    Proj_id = table.Column<string>(nullable: true),
                    KindOfActId = table.Column<int>(nullable: true),
                    DateOfReport = table.Column<DateTime>(nullable: false),
                    Task_id = table.Column<string>(nullable: true),
                    WorkTime = table.Column<TimeSpan>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkLogs_KindOfAct_KindOfActId",
                        column: x => x.KindOfActId,
                        principalTable: "KindOfAct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CorNumber = table.Column<int>(nullable: false),
                    CorTerm = table.Column<DateTime>(nullable: false),
                    CorBodyText = table.Column<string>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    ImageLink = table.Column<string>(nullable: true),
                    ExecutorId = table.Column<int>(nullable: false),
                    ProjectId = table.Column<int>(nullable: true),
                    ResponseId = table.Column<int>(nullable: true),
                    RecieveDate = table.Column<DateTime>(nullable: false),
                    ReopenTimes = table.Column<int>(nullable: false),
                    OriginalCorId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cors_Users_ExecutorId",
                        column: x => x.ExecutorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Cors_Cors_OriginalCorId",
                        column: x => x.OriginalCorId,
                        principalTable: "Cors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cors_ProjectSet_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "ProjectSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cors_ResponseSet_ResponseId",
                        column: x => x.ResponseId,
                        principalTable: "ResponseSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cors_ExecutorId",
                table: "Cors",
                column: "ExecutorId");

            migrationBuilder.CreateIndex(
                name: "IX_Cors_OriginalCorId",
                table: "Cors",
                column: "OriginalCorId");

            migrationBuilder.CreateIndex(
                name: "IX_Cors_ProjectId",
                table: "Cors",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Cors_ResponseId",
                table: "Cors",
                column: "ResponseId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_HeadOfDepartmentId",
                table: "Departments",
                column: "HeadOfDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSet_ManagerId",
                table: "ProjectSet",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DepartId",
                table: "Users",
                column: "DepartId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogs_KindOfActId",
                table: "WorkLogs",
                column: "KindOfActId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogs_UserId",
                table: "WorkLogs",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Departments_DepartId",
                table: "Users",
                column: "DepartId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Users_HeadOfDepartmentId",
                table: "Departments");

            migrationBuilder.DropTable(
                name: "Cors");

            migrationBuilder.DropTable(
                name: "WorkLogs");

            migrationBuilder.DropTable(
                name: "ProjectSet");

            migrationBuilder.DropTable(
                name: "ResponseSet");

            migrationBuilder.DropTable(
                name: "KindOfAct");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Departments");
        }
    }
}
