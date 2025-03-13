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
        public PlanTaskCompController(AppDbContext appDbContext, IHostingEnvironment appEnv)
        {
            context = appDbContext;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            var curUser = WebApplication5.Models.User.GetUser(context, HttpContext);
            var taskComps = TaskComp.GetAllTasksForMyDepartmentCurMonth(curUser, context);
            var webApiTasks = taskComps.Select(x => (WebApiTask)x);
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
        public IActionResult GetPlanTaskComp()
        {
            var curUser = WebApplication5.Models.User.GetUser(context, HttpContext);
            var planTaskCompList = PlanTaskComp.GetPlanTaskCompCurUser(curUser, context);            
            return Json(new { data = planTaskCompList });
        }

        // Добавление задачи
        [HttpPost("SaveTasks")]
        public IActionResult SaveTasks([FromBody] List<PlanTaskCompJson> planTaskCompJsonList)
        {
            var planTaskCompCreateList = planTaskCompJsonList.Where(x=>x.idDb==0).Select(x => PlanTaskComp.Create(x, context));
            context.PlanTaskComp.AddRange(planTaskCompCreateList);
            planTaskCompJsonList.Where(x => x.idDb != 0).ToList().ForEach(x => PlanTaskComp.Update(x, context));
            context.SaveChanges();            
            return View("Index");
        }

        //// Удаление задачи
        //[HttpDelete("api/gantt/tasks/{id}")]
        //public IActionResult DeleteTask(int id)
        //{
        //    TaskRepository.Delete(id);
        //    return Ok();
        //}
        // 
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
            var kindOfActList = kindOfActs.Select(ka => new { key = ka.Name, label = ka.Name }).ToList();
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