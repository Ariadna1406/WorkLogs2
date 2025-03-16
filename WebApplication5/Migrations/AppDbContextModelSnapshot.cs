﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApplication5.Models;

namespace WebApplication5.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.14-servicing-32113")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("WebApplication5.Models.Absence", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("FinishDate");

                    b.Property<double?>("HourAmount");

                    b.Property<int?>("ReasonId");

                    b.Property<DateTime>("StartDate");

                    b.Property<int?>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("ReasonId");

                    b.HasIndex("UserId");

                    b.ToTable("Absences");
                });

            modelBuilder.Entity("WebApplication5.Models.AbsenceReason", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ReasonName");

                    b.HasKey("Id");

                    b.ToTable("AbsenceReasons");
                });

            modelBuilder.Entity("WebApplication5.Models.ApprovePlanTaskComp", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("PlanMonth");

                    b.Property<int>("PlanTaskCompStatus");

                    b.Property<int>("PlanYear");

                    b.Property<string>("RejectComment");

                    b.Property<string>("RejectedUser");

                    b.Property<int?>("UserCreatedRequestId");

                    b.HasKey("Id");

                    b.HasIndex("UserCreatedRequestId");

                    b.ToTable("ApprovePlanTaskComp");
                });

            modelBuilder.Entity("WebApplication5.Models.AvevaElemAmount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Date");

                    b.Property<int>("PipeLineAmount");

                    b.Property<string>("ProjectAcr");

                    b.Property<int?>("ProjectId");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("AvevaElemAmounts");
                });

            modelBuilder.Entity("WebApplication5.Models.AvevaPipeLength", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Date");

                    b.Property<int>("PipeLineBore");

                    b.Property<long>("PipeLineLength");

                    b.Property<string>("ProjectAcr");

                    b.Property<int?>("ProjectId");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("avevaPipeLengths");
                });

            modelBuilder.Entity("WebApplication5.Models.Corrections", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CorBodyText")
                        .IsRequired();

                    b.Property<int>("CorNumber");

                    b.Property<DateTime>("CorTerm");

                    b.Property<int>("ExecutorId");

                    b.Property<string>("ImageLink");

                    b.Property<int?>("OriginalCorId");

                    b.Property<int?>("ProjectId");

                    b.Property<DateTime>("RecieveDate");

                    b.Property<int>("ReopenTimes");

                    b.Property<int?>("ResponseId");

                    b.Property<int>("Status");

                    b.HasKey("Id");

                    b.HasIndex("ExecutorId");

                    b.HasIndex("OriginalCorId");

                    b.HasIndex("ProjectId");

                    b.HasIndex("ResponseId");

                    b.ToTable("Cors");
                });

            modelBuilder.Entity("WebApplication5.Models.DayOff", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Date");

                    b.HasKey("Id");

                    b.ToTable("DayOffs");
                });

            modelBuilder.Entity("WebApplication5.Models.Department", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Acronym");

                    b.Property<string>("City");

                    b.Property<int?>("HeadOfDepartmentId");

                    b.Property<bool?>("IgnoreInReport");

                    b.Property<string>("Name");

                    b.Property<bool?>("Production");

                    b.HasKey("Id");

                    b.HasIndex("HeadOfDepartmentId");

                    b.ToTable("Departments");
                });

            modelBuilder.Entity("WebApplication5.Models.KindOfAct", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("KindOfAct");
                });

            modelBuilder.Entity("WebApplication5.Models.Licence", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("AddUserDate");

                    b.Property<string>("HostName");

                    b.Property<DateTime>("LicApplyDate");

                    b.Property<bool>("Status");

                    b.HasKey("Id");

                    b.ToTable("AvevaLicences");
                });

            modelBuilder.Entity("WebApplication5.Models.PlanTaskComp", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AuthorId");

                    b.Property<int?>("ExecuterId");

                    b.Property<DateTime>("FinishPlanDate");

                    b.Property<double>("Intencity");

                    b.Property<int?>("KindOfActId");

                    b.Property<int>("Percent");

                    b.Property<DateTime>("StartPlanDate");

                    b.Property<int?>("TaskCompId");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("ExecuterId");

                    b.HasIndex("KindOfActId");

                    b.HasIndex("TaskCompId");

                    b.ToTable("PlanTaskComp");
                });

            modelBuilder.Entity("WebApplication5.Models.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AvevaAcronym");

                    b.Property<string>("ContractNumber");

                    b.Property<string>("FullName")
                        .IsRequired();

                    b.Property<string>("InternalNum")
                        .IsRequired();

                    b.Property<bool>("IsDeleted");

                    b.Property<int?>("ManagerId");

                    b.Property<string>("ShortName");

                    b.Property<bool>("ShowInMenuBar");

                    b.Property<int>("Status");

                    b.HasKey("Id");

                    b.HasIndex("ManagerId");

                    b.ToTable("ProjectSet");
                });

            modelBuilder.Entity("WebApplication5.Models.ProjectLinks", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("PDPortalLink");

                    b.Property<int?>("ProjectId");

                    b.Property<string>("ProjectPart");

                    b.Property<string>("RDPortalLink");

                    b.Property<string>("ZPIPortalLink");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("ProjectLinks");
                });

            modelBuilder.Entity("WebApplication5.Models.Response", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ImageLink");

                    b.Property<string>("Text")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("ResponseSet");
                });

            modelBuilder.Entity("WebApplication5.Models.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("WebApplication5.Models.TaskComp", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("ApproveFactDate");

                    b.Property<double?>("CompletePercent");

                    b.Property<string>("ContractStatus");

                    b.Property<DateTime?>("DateMoneyGet");

                    b.Property<string>("Department");

                    b.Property<string>("DepartmentSendTask");

                    b.Property<string>("Division");

                    b.Property<string>("DivisionForReport");

                    b.Property<string>("Executers");

                    b.Property<DateTime?>("FactDateSendTask");

                    b.Property<double?>("FactWorkLog");

                    b.Property<DateTime?>("FinishFactDate");

                    b.Property<DateTime?>("FinishPlanDate");

                    b.Property<DateTime?>("FisnishContractDate");

                    b.Property<string>("GIPAcronym");

                    b.Property<double?>("Moneyleft");

                    b.Property<double?>("MoneyleftNHP");

                    b.Property<string>("NumberOfUPD");

                    b.Property<string>("OperationName");

                    b.Property<DateTime?>("PlanDateSendTask");

                    b.Property<double?>("PlanWorkLog");

                    b.Property<double?>("Prepayment");

                    b.Property<double?>("Price");

                    b.Property<string>("ProjectNumber");

                    b.Property<string>("ProjectShortName");

                    b.Property<string>("ProjectStage");

                    b.Property<DateTime?>("StartFactDate");

                    b.Property<DateTime?>("StartPlanDate");

                    b.Property<double?>("SubContractorPart");

                    b.Property<DateTime?>("TaskCompCreationDate");

                    b.Property<string>("TaskCompName");

                    b.Property<string>("TaskCompStatus");

                    b.Property<string>("UID");

                    b.HasKey("Id");

                    b.ToTable("TaskComps");
                });

            modelBuilder.Entity("WebApplication5.Models.TaskCompPercentHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("ChangePercentDate");

                    b.Property<double>("Percent");

                    b.Property<int?>("TaskCompId");

                    b.Property<int?>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("TaskCompId");

                    b.HasIndex("UserId");

                    b.ToTable("TaskCompPercentHistories");
                });

            modelBuilder.Entity("WebApplication5.Models.TaskCompRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Comment");

                    b.Property<DateTime?>("DateOfRequest");

                    b.Property<string>("DenyComment");

                    b.Property<DateTime?>("FinishDate");

                    b.Property<double?>("PlanWorkLog");

                    b.Property<string>("ProjectNumber");

                    b.Property<DateTime?>("StartDate");

                    b.Property<string>("TaskCompName");

                    b.Property<int>("TaskCompStatus");

                    b.Property<int?>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("TaskCompRequests");
                });

            modelBuilder.Entity("WebApplication5.Models.TeklaElemAmount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Date");

                    b.Property<int>("ElemAmount");

                    b.Property<string>("ProjectAcr");

                    b.Property<int?>("ProjectId");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("TeklaElemAmounts");
                });

            modelBuilder.Entity("WebApplication5.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AD_GUID");

                    b.Property<string>("AdditionalDepartmentAcronym");

                    b.Property<DateTime?>("BlockDate");

                    b.Property<string>("City");

                    b.Property<int?>("DepartId");

                    b.Property<string>("Email");

                    b.Property<string>("FirstName");

                    b.Property<string>("FullName");

                    b.Property<bool?>("IgnoreUserInReport");

                    b.Property<DateTime?>("ImportDate");

                    b.Property<bool?>("IngnoreInReportShares");

                    b.Property<bool?>("IsActive");

                    b.Property<bool>("IsGIP");

                    b.Property<bool?>("IsHeadOfDepartment");

                    b.Property<bool?>("IsProduction");

                    b.Property<string>("LastName");

                    b.Property<string>("Login");

                    b.Property<string>("MiddleName");

                    b.Property<string>("NameFromAD");

                    b.Property<string>("Position");

                    b.Property<string>("PublicDepart");

                    b.Property<double?>("Rate");

                    b.Property<int?>("RoleId");

                    b.HasKey("Id");

                    b.HasIndex("DepartId");

                    b.HasIndex("RoleId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("WebApplication5.Models.UserSubs", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("ReplacedUserId");

                    b.Property<int?>("SubstiteUserId");

                    b.HasKey("Id");

                    b.HasIndex("ReplacedUserId");

                    b.HasIndex("SubstiteUserId");

                    b.ToTable("UsersSubs");
                });

            modelBuilder.Entity("WebApplication5.Models.WorkLogs", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Comment");

                    b.Property<DateTime>("DateOfReport");

                    b.Property<DateTime>("DateOfSendingReport");

                    b.Property<int?>("KindOfActId");

                    b.Property<string>("KindOfActStr");

                    b.Property<string>("Proj_id");

                    b.Property<string>("TaskComp_id");

                    b.Property<int?>("UserId");

                    b.Property<TimeSpan>("WorkTime");

                    b.HasKey("Id");

                    b.HasIndex("KindOfActId");

                    b.HasIndex("UserId");

                    b.ToTable("WorkLogs");
                });

            modelBuilder.Entity("WebApplication5.Models.Absence", b =>
                {
                    b.HasOne("WebApplication5.Models.AbsenceReason", "Reason")
                        .WithMany()
                        .HasForeignKey("ReasonId");

                    b.HasOne("WebApplication5.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("WebApplication5.Models.ApprovePlanTaskComp", b =>
                {
                    b.HasOne("WebApplication5.Models.User", "UserCreatedRequest")
                        .WithMany()
                        .HasForeignKey("UserCreatedRequestId");
                });

            modelBuilder.Entity("WebApplication5.Models.AvevaElemAmount", b =>
                {
                    b.HasOne("WebApplication5.Models.Project", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId");
                });

            modelBuilder.Entity("WebApplication5.Models.AvevaPipeLength", b =>
                {
                    b.HasOne("WebApplication5.Models.Project", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId");
                });

            modelBuilder.Entity("WebApplication5.Models.Corrections", b =>
                {
                    b.HasOne("WebApplication5.Models.User", "Executor")
                        .WithMany()
                        .HasForeignKey("ExecutorId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("WebApplication5.Models.Corrections", "OriginalCor")
                        .WithMany()
                        .HasForeignKey("OriginalCorId");

                    b.HasOne("WebApplication5.Models.Project", "Project")
                        .WithMany("Corrections")
                        .HasForeignKey("ProjectId");

                    b.HasOne("WebApplication5.Models.Response", "Response")
                        .WithMany()
                        .HasForeignKey("ResponseId");
                });

            modelBuilder.Entity("WebApplication5.Models.Department", b =>
                {
                    b.HasOne("WebApplication5.Models.User", "HeadOfDepartment")
                        .WithMany()
                        .HasForeignKey("HeadOfDepartmentId");
                });

            modelBuilder.Entity("WebApplication5.Models.PlanTaskComp", b =>
                {
                    b.HasOne("WebApplication5.Models.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId");

                    b.HasOne("WebApplication5.Models.User", "Executer")
                        .WithMany()
                        .HasForeignKey("ExecuterId");

                    b.HasOne("WebApplication5.Models.KindOfAct", "KindOfAct")
                        .WithMany()
                        .HasForeignKey("KindOfActId");

                    b.HasOne("WebApplication5.Models.TaskComp", "TaskComp")
                        .WithMany()
                        .HasForeignKey("TaskCompId");
                });

            modelBuilder.Entity("WebApplication5.Models.Project", b =>
                {
                    b.HasOne("WebApplication5.Models.User", "Manager")
                        .WithMany()
                        .HasForeignKey("ManagerId");
                });

            modelBuilder.Entity("WebApplication5.Models.ProjectLinks", b =>
                {
                    b.HasOne("WebApplication5.Models.Project", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId");
                });

            modelBuilder.Entity("WebApplication5.Models.TaskCompPercentHistory", b =>
                {
                    b.HasOne("WebApplication5.Models.TaskComp", "TaskComp")
                        .WithMany()
                        .HasForeignKey("TaskCompId");

                    b.HasOne("WebApplication5.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("WebApplication5.Models.TaskCompRequest", b =>
                {
                    b.HasOne("WebApplication5.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("WebApplication5.Models.TeklaElemAmount", b =>
                {
                    b.HasOne("WebApplication5.Models.Project", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId");
                });

            modelBuilder.Entity("WebApplication5.Models.User", b =>
                {
                    b.HasOne("WebApplication5.Models.Department", "Department")
                        .WithMany()
                        .HasForeignKey("DepartId");

                    b.HasOne("WebApplication5.Models.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId");
                });

            modelBuilder.Entity("WebApplication5.Models.UserSubs", b =>
                {
                    b.HasOne("WebApplication5.Models.User", "ReplacedUser")
                        .WithMany()
                        .HasForeignKey("ReplacedUserId");

                    b.HasOne("WebApplication5.Models.User", "SubstiteUser")
                        .WithMany()
                        .HasForeignKey("SubstiteUserId");
                });

            modelBuilder.Entity("WebApplication5.Models.WorkLogs", b =>
                {
                    b.HasOne("WebApplication5.Models.KindOfAct", "KindOfAct")
                        .WithMany()
                        .HasForeignKey("KindOfActId");

                    b.HasOne("WebApplication5.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });
#pragma warning restore 612, 618
        }
    }
}
