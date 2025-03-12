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

namespace WebApplication5.Controllers
{
    public class MyTasksController : Controller
    {
        AppDbContext context;
        public MyTasksController(AppDbContext appDbContext, IHostingEnvironment appEnv)
        {
            context = appDbContext;
        }

        public IActionResult Index(string showTaskCompCond)
        {
            if (TempData["SuccessMes"] != null)
            {
                //List<string> successMes = new List<string>();
                var successAr = TempData["SuccessMes"] as string[];
                if (successAr != null)
                {
                    ViewData["SuccessMes"] = successAr.ToList();
                }
            }
            if (TempData["FailMes"] != null)
            {                
                var failAr = TempData["FailMes"] as string[];
                if (failAr != null)
                {
                    ViewData["FailMes"] = failAr.ToList(); ;
                }
            }
            var curUser = WebApplication5.Models.User.GetUser(context, HttpContext);
            ViewData["UsersOfMyDepart"] = curUser.GetAllUsersFromMyDepart(context);
            ViewData["curUser"] = curUser;
            if (curUser != null)
            {
                ViewData["IsHOD"] = curUser.isHeadOfDepartment(context);
                ViewData["IsAdmin"] = curUser.IsAdmin(context);
            }
            //if (TempData["TaskCompId"] != null)
            //{
            //    ViewData["TaskCompId"] = TempData["TaskCompId"];
            //}
           

            List<TaskComp> taskSet = new List<TaskComp>();
            var departAcronym = curUser.GetDepartmentAcronym(context);
            if (string.IsNullOrEmpty(showTaskCompCond))
            {
                if (TempData["showTaskCompCond"] != null)
                {
                    showTaskCompCond = TempData["showTaskCompCond"] as string;
                }
                else
                {
                    showTaskCompCond = "1";
                }
            }
            if (string.IsNullOrEmpty(showTaskCompCond) || showTaskCompCond == "1")
            {
                //Все комплекты моего отдела за тек. месяц
                taskSet = TaskComp.GetAllTasksForMyDepartmentCurMonth(curUser, context);                
                ViewData["ShowTaskCompCond"] = "1";
            }
            else if (showTaskCompCond == "2")
            {
                //Все комплекты моего отдела
                taskSet = TaskComp.GetAllTasksForMyDepartment(curUser, context).ToList();
                TaskComp.RemoveStandartTasks(taskSet);
                ViewData["ShowTaskCompCond"] = "2";
            }
            ViewData["curPage"] = 2;
            ViewData["tcReqCount"] = TaskCompRequest.GetAmountOfNewTaskCompRequestStr(context);
            ViewData["isKSP"] = curUser.IsKSP(context);
            return View(taskSet);


        }

        public ViewResult ShowAllMyTasks()
        {
            var curUser = WebApplication5.Models.User.GetUser(context, HttpContext);
            ViewData["curUser"] = curUser;
            if (curUser != null)
            {
                ViewData["IsHOD"] = curUser.isHeadOfDepartment(context);
                ViewData["IsAdmin"] = curUser.IsAdmin(context);
            }
            ViewData["IsShowAllTask"] = true;
            var taskSet = TaskComp.GetAllTasks(context);
            ViewData["curPage"] = 2;
            return View("Index",taskSet);
        }


        public ViewResult Graphs(string prjAcr)
        {
            var elemSet = context.AvevaElemAmounts.Where(x => x.ProjectAcr == prjAcr).ToList();
            //if (elemSet.Count > 0)
            // {
            ViewData["prjAcr"] = prjAcr;
                return View(elemSet);
            //}
            //return View("/Views/Project/")
        }

        public string GetAvevaElemAmount(string avevaAcr)
        {
            string filePath = String.Format(@"\\it-andrey\ModelElemAmount\{0}\ElemAmount.txt", avevaAcr);
            if (System.IO.File.Exists(filePath))
            {
                StreamReader sr = new StreamReader(filePath);
                var line= sr.ReadLine();
                sr.Close();
                return line;
            }
            return string.Empty;
        }
        public IActionResult SetMultiplePercent(Dictionary<string, string> taskCompDict)
        {
            List<string> successMes = new List<string>();
            List<string> errorMes = new List<string>();
            var curUser = WebApplication5.Models.User.GetUser(context, HttpContext);
            foreach (var taskCompIdPercent in taskCompDict)
            {
                int taskId = Convert.ToInt32(taskCompIdPercent.Key);
                var taskCompSet = context.TaskComps.Where(x => x.Id == taskId);
                if (taskCompSet.Count() > 0)
                {
                    var taskComp = taskCompSet.First();
                    taskComp.SetPercent(context, taskCompIdPercent.Value, curUser ,successMes, errorMes);
                    context.SaveChanges();
                }
            }
            TempData["SuccessMes"] = successMes;
            TempData["FailMes"] = errorMes;
            return RedirectToAction("Index");
        }
    }
}