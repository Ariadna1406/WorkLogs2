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
    [Route ("PlanTaskComp")]
    public class PlanTaskCompController : Controller
    {
        AppDbContext context;
        //List<PlanTaskCompJson> planTaskCompJsonList = new List<PlanTaskCompJson>();
        public PlanTaskCompController(AppDbContext appDbContext, IHostingEnvironment appEnv)
        {
            context = appDbContext;
        }

        [HttpGet("")]
        public IActionResult Index(int month)
        {
            if (month == 0) month = DateTime.Now.Month;
            var curUser = WebApplication5.Models.User.GetUser(context, HttpContext);
            var taskComps = TaskComp.GetAllTasksForMyDepartmentCurMonth(curUser, context);            
            var webApiTasks = taskComps.Select(x => (WebApiTask)x);           
            //var planTaskCompJsonList = PlanTaskComp.GetPlanTaskCompCurUser(curUser, DateTime.Now.Month, context);
            //var planTaskComp = PlanTaskComp.GetPlanTaskCompCurUser(curUser, context);
            return View(webApiTasks);
        }

        [HttpGet("api/gantt/tasks")]
        public IActionResult GetTasks()
        {
            var curUser = WebApplication5.Models.User.GetUser(context, HttpContext);
            var taskComps = TaskComp.GetAllTasksForMyDepartmentCurMonth(curUser, context);
            var webApiTasks = taskComps.Select(x => (WebApiTask)x);
            return Json(new { data = webApiTasks });
        }

        [HttpGet("api/gantt/plantaskcomp")]
        public IActionResult GetPlanTaskComp(int planMonth, int planYear)
        {
            if (planMonth == 0) planMonth = DateTime.Now.Month;
            var curUser = WebApplication5.Models.User.GetUser(context, HttpContext);
            var planTaskCompJsonList = PlanTaskComp.GetPlanTaskCompCurUser(curUser, planMonth, context);   
            //planTaskCompList.Select(x=>x.)
            return Json(new { data = planTaskCompJsonList });
        }

        [HttpGet("api/gantt/getcurstatus")]
        public IActionResult GetCurStatus(int planMonth, int planYear)
        {
           var curStatus= ApprovePlanTaskComp.GetApprovePlanTaskCompStatus(planMonth, planYear, context);
            var statusStr = ApprovePlanTaskComp.GetStatusRus(curStatus);            
            return Json(new { data = statusStr });
        }

        // Сохранение задачи
        [HttpPost("SaveTasks")]
        public IActionResult SaveTasks([FromBody] List<PlanTaskCompJson> planTaskCompJsonList)
        {
            var result = PlanTaskComp.SavePlanTaskCompToDb(planTaskCompJsonList, context, out string errors);
            if (result) return Ok(new { success = true });
            else return StatusCode(500, new { success = false, message = "Ошибка сохранения задач", error = errors });
        }  
        
         // Сохранение задач и отправка на согласование
        [HttpPost("SendToApprove")]
        public IActionResult SendToApprove([FromBody] List<PlanTaskCompJson> planTaskCompJsonList)
        {
            var result = PlanTaskComp.SavePlanTaskCompToDb(planTaskCompJsonList, context, out string errors);           
            var result2 = ApprovePlanTaskComp.SendToApprove(planTaskCompJsonList, context, out string errorsSendToApprove);
            if (result && result2) return Ok(new { success = true });
            else return StatusCode(500, new { success = false, message = "Ошибка сохранения задач", error = errors });            
        }
           
        // Получение списка сотрудников отдела текущего пользователя
        [HttpGet("api/gantt/resources")]
        public IActionResult GetUsersFromMyDepartment()
        {
            var users = WebApplication5.Models.User.GetUsersFromCurrentDepartment(context, HttpContext);
            var resourceList = users.Select(u => new { key = u.Id, label = u.FullName }).ToList();
            return Json(new { data = resourceList });
        }

        // Получение списка работ
        [HttpGet("api/gantt/kindofact")]
        public IActionResult GetKindOfActs()
        {
            var kindOfActs = WebApplication5.Models.KindOfAct.GetAllKindOfAct(context);
            var kindOfActList = kindOfActs.Select(ka => new { key = ka.Id, label = ka.Name }).ToList();
            return Json(new { data = kindOfActList });
        }

        // Получение id текущего пользователя
        [HttpGet("api/currentuserid")]
        public IActionResult GetCurrentUserId()
        {
            int userId = WebApplication5.Models.User.GetCurrentUserId(context, HttpContext);
            return Json(new { userId = userId });
        }
    }
}