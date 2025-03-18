using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace WebApplication5.Models
{
    public class AppDbContext : DbContext //IdentityDbContext<IdentityUser> 
    {

        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) { }

        public DbSet<Corrections> Cors { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Project> ProjectSet { get; set; }
        public DbSet<ProjectLinks> ProjectLinks { get; set; }

        public DbSet<Response> ResponseSet { get; set; }

        public DbSet<Department> Departments { get; set; }

        public DbSet<KindOfAct> KindOfAct { get; set; }
        public DbSet<WorkLogs> WorkLogs { get; set; }

        public DbSet<AvevaElemAmount> AvevaElemAmounts { get; set; }
        public DbSet<TeklaElemAmount> TeklaElemAmounts { get; set; }

        public DbSet<Licence> AvevaLicences { get; set; }
        public DbSet<Role> Roles { get; set; }

        public DbSet<AvevaPipeLength> avevaPipeLengths { get; set; }

        public DbSet<TaskComp> TaskComps { get; set; }

        public DbSet<DayOff> DayOffs { get; set; }

        public DbSet<TaskCompRequest> TaskCompRequests { get; set; }

        public DbSet<Absence> Absences { get; set; }

        public DbSet<AbsenceReason> AbsenceReasons { get; set; }
        public DbSet<UserSubs> UsersSubs { get; set; }

        public DbSet<TaskCompPercentHistory> TaskCompPercentHistories { get; set; }

        public DbSet<PlanTaskComp> PlanTaskComp { get; set; }

        public DbSet<ApprovePlanTaskComp> ApprovePlanTaskComp { get; set; }

        public DbSet<ApprovePlanTaskCompStatusHistory> ApprovePlanTaskCompStatusHistories { get; set; }

        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);
        //    builder.Entity<Corrections>().HasData(new Corrections)
        //}
    }
}
