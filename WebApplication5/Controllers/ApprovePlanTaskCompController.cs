using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication5.Models;
using System.DirectoryServices;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Aspose.Cells;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.Versioning;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
//using static WebApplication5.Models.PlanTaskComp;
using System.Reflection.Emit;
using System.Data.OleDb;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;
using DHX.Gantt.Models;

namespace WebApplication5.Controllers
{
    [Route ("ApprovePlanTaskComp")]
    public class ApprovePlanTaskCompController : Controller
    {
        AppDbContext context;
        //List<PlanTaskCompJson> planTaskCompJsonList = new List<PlanTaskCompJson>();
        public ApprovePlanTaskCompController(AppDbContext appDbContext, IHostingEnvironment appEnv)
        {
            context = appDbContext;
        }

        [HttpGet("")]
        public IActionResult Index(int month)
        {
            if (month == 0) month = DateTime.Now.Month;
            var curUser = WebApplication5.Models.User.GetUser(context, HttpContext);
            //var taskComps = ApprovePlanTaskComp.GetAllTasksForMyDepartmentCurMonth(curUser, context);            
            //var webApiTasks = taskComps.Select(x => (WebApiTask)x);           
            //var planTaskCompJsonList = PlanTaskComp.GetPlanTaskCompCurUser(curUser, DateTime.Now.Month, context);
            //var planTaskComp = PlanTaskComp.GetPlanTaskCompCurUser(curUser, context);
            return View();
        }
        
    }
}