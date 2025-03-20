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
using System.Drawing;

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
            if (curUser != null)
            {
                ViewData["IsHOD"] = curUser.isHeadOfDepartment(context);
                ViewData["IsAdmin"] = curUser.IsAdmin(context);
                ViewData["IsGIP"] = curUser.IsGIP;
                ViewData["IsKSP"] = curUser.IsKSP(context);
            }
            var taskComps = TaskComp.GetAllTasksForMyDepartmentCurMonth(curUser, context);            
            var webApiTasks = taskComps.Select(x => (WebApiTask)x);           
            //var planTaskCompJsonList = PlanTaskComp.GetPlanTaskCompCurUser(curUser, DateTime.Now.Month, context);
            //var planTaskComp = PlanTaskComp.GetPlanTaskCompCurUser(curUser, context);
            return View(webApiTasks);
        }

        [HttpGet("ApprovePlanTaskComp")]
        public IActionResult ApprovePlanTaskComp(int? month, int? year, string status)
        {
            if (!month.HasValue) month = DateTime.Now.Month;
            if (!year.HasValue) year = DateTime.Now.Year;

            var curUser = WebApplication5.Models.User.GetUser(context, HttpContext);

            // Получение задач по выбранному месяцу и году
            var aptcList = Models.ApprovePlanTaskComp.GetAllWithStatusSendToApprove(context).
                Where(t => t.PlanMonth == month.Value && t.PlanYear == year.Value)
                .ToList();        
            
            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                ApprovePlanTaskComp.Status selectedStatus = Enum.Parse<ApprovePlanTaskComp.Status>(status);
                aptcList = aptcList.Where(t => t.PlanTaskCompStatus == selectedStatus).ToList();
            }

            foreach (var task in aptcList)
            {
                task.UserCreatedRequest.Department = task.UserCreatedRequest.GetDepartment(context);  // Получение отдела
            }

            return View(aptcList);
        }

        [HttpPost("UpdateStatus")]
        public IActionResult UpdateStatus([FromBody] UpdateStatusRequest request)
        {
            var curUser = WebApplication5.Models.User.GetUser(context, HttpContext);
            Models.ApprovePlanTaskComp.Status appliedStatus=Models.ApprovePlanTaskComp.GetStatusFromStr(request.Status);

            var result = new { success = false, message = "Что-то пошло не так(" };

            try
            {
                foreach (int aptcId in request.AptcIds)
                {
                    ApprovePlanTaskComp aptc = Models.ApprovePlanTaskComp.FindById(aptcId, context, out string errors);
                    if (aptc != null)
                    {
                        aptc.ChangeStatus(curUser, appliedStatus, context, out string errorsChangeStatus);
                    }
                }
                context.SaveChanges();

                result = new { success = true, message = "Статус успешно обновлен" };
            }
            catch (Exception ex)
            {
                result = new { success = false, message = ex.Message };
            }

            return Json(result);
        }

        [HttpPost("UploadPlanTaskCompExcel")]
        public IActionResult UploadPlanTaskCompExcel([FromBody] int planMonth, int planYear, Models.User author)
        {

            return Ok(new { success = true });
        }


        public class UpdateStatusRequest
        {
            public List<int> AptcIds { get; set; }
            public string Status { get; set; }
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
           var curStatus= Models.ApprovePlanTaskComp.GetApprovePlanTaskCompStatus(planMonth, planYear, context);
            var statusStr = Models.ApprovePlanTaskComp.GetStatusRus(curStatus);            
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

        [HttpPost("DeleteTask")]
        public IActionResult DeleteTask([FromBody] PlanTaskCompJson planTaskCompJson)
        {
            var result = PlanTaskComp.Delete(planTaskCompJson, context);
            if (result) return Ok(new { success = true });
            else return StatusCode(500, new { success = false, message = "Ошибка удаления задач"});
        }

        // Сохранение задач и отправка на согласование
        [HttpPost("SendToApprove")]
        public IActionResult SendToApprove([FromBody] List<PlanTaskCompJson> planTaskCompJsonList)
        {
            var curUser = WebApplication5.Models.User.GetUser(context, HttpContext);
            var result = PlanTaskComp.SavePlanTaskCompToDb(planTaskCompJsonList, context, out string errors);           
            var result2 = Models.ApprovePlanTaskComp.SendToApprove(planTaskCompJsonList, context, out string errorsSendToApprove);
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