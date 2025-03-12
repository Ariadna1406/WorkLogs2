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
using WebApplication5.Models.ExcelFiles;

namespace WebApplication5.Controllers
{
    public class MyDepartmentController : Controller
    {
        AppDbContext context;
        public MyDepartmentController(AppDbContext appDbContext, IHostingEnvironment appEnv)
        {
            context = appDbContext;
        }

        public ViewResult Index(string showTaskCompCond)
        {
            if (TempData["SuccessMes"] != null)
            {
                //List<string> successMes = new List<string>();
                var successAr = TempData["SuccessMes"] as string[];
                ViewData["SuccessMes"] = successAr.ToList();
            }
            var curUser = WebApplication5.Models.User.GetUser(context, HttpContext);
            ViewData["UsersOfMyDepart"] = curUser.GetAllUsersFromMyDepart(context);
            ViewData["curUser"] = curUser;
            if (curUser != null)
            {
                ViewData["IsHOD"] = curUser.isHeadOfDepartment(context);
                ViewData["IsAdmin"] = curUser.IsAdmin(context);
            }
            if (TempData["TaskCompId"]!=null)
            {
                ViewData["TaskCompId"] = TempData["TaskCompId"];
            }
            if (curUser.isHeadOfDepartment(context))
            {
                List<TaskComp> taskSet;
                var departAcronym = curUser.GetDepartmentAcronym(context);
                var additDepart = curUser.AdditionalDepartmentAcronym;
                if (string.IsNullOrEmpty(showTaskCompCond))
                {
                    showTaskCompCond = TempData["showTaskCompCond"] as string;
                }
                if (string.IsNullOrEmpty(showTaskCompCond) || showTaskCompCond == "3")
                {
                    //Все комплекты моего отдела за тек. месяц
                    taskSet = TaskComp.GetAllTasksForMyDepartmentCurMonthWithoutExecutor(departAcronym, context, additDepart);
                    ViewData["ShowTaskCompCond"] = "3";
                }
                else if (showTaskCompCond == "2")
                {
                    //Все комплекты моего отдела
                    taskSet = TaskComp.GetAllTasksForMyDepartmentWithoutExecutor(departAcronym, context, additDepart);
                    ViewData["ShowTaskCompCond"] = "2";
                }
                else if (showTaskCompCond == "1")
                {
                    //Все комплекты моего отдела
                    taskSet = TaskComp.GetAllTasksForMyDepartmentCurMonth(curUser, context);
                    ViewData["ShowTaskCompCond"] = "1";
                }
                else
                {
                    //Все комплекты
                    taskSet = TaskComp.GetAllTasksForMyDepartment(curUser, context).ToList();
                    ViewData["ShowTaskCompCond"] = "0";

                }
                TaskComp.RemoveStandartTasks(taskSet);
                ViewData["curPage"] = 0;
                return View("SetExecuter", taskSet);
            }
            else
            {

                return View(@"/Views/Shared/AccessDenied.cshtml");
            }

        }

        public IActionResult ApplyExecuter(string executorName, string taskCompId, string showTaskCompCond)
        {
            int taskId = Convert.ToInt32(taskCompId);
            var taskCompSet = context.TaskComps.Where(x => x.Id == taskId);
            if (taskCompSet.Count() > 0)
            {
                var taskComp = taskCompSet.First();
                var executorSet = context.Users.Where(x => x.FullName == executorName);
                if (executorSet.Count() > 0)
                {
                    var executor = executorSet.First();
                    executor.AddUserToTask(taskComp);
                    context.SaveChanges();
                    TempData["SuccessMes"] = new List<string> { $"Исполнитель {executor.FullName} успешно добавлен к задаче" };
                }
            }
            TempData["showTaskCompCond"] = showTaskCompCond;
            TempData["TaskCompId"] = taskCompId;

            return RedirectToAction("Index");
        }

        public IActionResult DeleteExecuter(string executorName, string taskCompId, string showTaskCompCond)
        {
            int taskId = Convert.ToInt32(taskCompId);
            var taskCompSet = context.TaskComps.Where(x => x.Id == taskId);
            if (taskCompSet.Count() > 0)
            {
                var taskComp = taskCompSet.First();
                var executorSet = context.Users.Where(x => x.FullName == executorName);
                if (executorSet.Count() > 0)
                {
                    var executor = executorSet.First();
                    executor.DeleteUserFromTask(taskComp);
                    context.SaveChanges();
                    TempData["SuccessMes"] = new List<string> { $"Исполнитель {executor.FullName} успешно удалён с задачи" };
                }
            }
            TempData["showTaskCompCond"] = showTaskCompCond;
            TempData["TaskCompId"] = taskCompId;
            return RedirectToAction("Index");
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
                var line = sr.ReadLine();
                sr.Close();
                return line;
            }
            return string.Empty;
        }

        public IActionResult ShowMyDepartWorkLogs()
        {
            if (TempData["SuccessMes"] != null)
            {
                //List<string> successMes = new List<string>();
                var successAr = TempData["SuccessMes"] as string[];
                ViewData["SuccessMes"] = successAr.ToList();
            }
            var curUser = WebApplication5.Models.User.GetUser(context, HttpContext);
            if (curUser != null)
            {
                ViewData["IsHOD"] = curUser.isHeadOfDepartment(context);
                ViewData["IsAdmin"] = curUser.IsAdmin(context);
            }
            var userMyDepartList = context.Users.Include(x => x.Department).Where(x => x.Department.Id == curUser.Department.Id).Where(x=>x.IsActive==true).ToList();
            ViewData["UsersOfMyDepart"] = userMyDepartList;            
            ViewData["curUser"] = curUser;
            ViewData["curMonth"] = $"c {DateTime.Now.Date.AddDays(-30).ToShortDateString()} по {DateTime.Now.Date.ToShortDateString()}";
            if (curUser.isHeadOfDepartment(context))
            {
                var taskCompList = WorkLogs.GetAllWorkLogsMyDepartCurMonth(userMyDepartList, context);
                //IEnumerable <IGrouping<string, PrimaTask>> 
                ViewData["curPage"] = 0;
                return View("MyDepartWorkLogs", taskCompList);
            }
            else
            {
                return View(@"/Views/Shared/AccessDenied.cshtml");
            }
        }

        public IActionResult ShowMyDepartTotalResult(int month, int year, bool? uploadToExcel, bool? sendNotifications)
        {            
            List<TotalWorkLog> totalWL = new List<TotalWorkLog>();
           
            var curUser = WebApplication5.Models.User.GetUser(context, HttpContext);
            if (curUser.Id == 263)
            {
                curUser = context.Users.Where(x => x.Id == 101).First();
            }
                if (curUser != null)
            {
                ViewData["IsHOD"] = curUser.isHeadOfDepartment(context);
                ViewData["IsAdmin"] = curUser.IsAdmin(context);
            }
            var userMyDepartList = curUser.GetAllUsersFromMyDepart(context); 
            ViewData["UsersOfMyDepart"] = userMyDepartList;
            ViewData["curUser"] = curUser;
            if (month == 0)
            {
                month = DateTime.Now.Month;
            }
            if (year == 0)
            {
                year = DateTime.Now.Year;
            }
            var monthStr = GetCurMonth(month);
            ViewBag.CurMonth = monthStr;
            ViewBag.CurMonthInt = month;
            ViewBag.CurYear = year.ToString();
            ViewBag.CurYearInt = year;
            if (curUser.isHeadOfDepartment(context))
            {
                var workLogList = WorkLogs.GetAllWorkLogsMyDepartByMonthAndYear(userMyDepartList, context, month, year, out List<User> abuserList);               
                var wllThisMonthGroupedByUser = workLogList.GroupBy(x => x.User);
                var totalWTShouldBe = WorkLogs.GetTotalWTSHouldBe(context, month, DateTime.Now.Year);
                foreach (var wlGrByUser in wllThisMonthGroupedByUser)
                {
                    var totalHours = wlGrByUser.Sum(x => x.WorkTime.TotalHours);
                    totalWL.Add(new TotalWorkLog(wlGrByUser.Key, totalHours, totalWTShouldBe));
                }
                // Добавляем пользователей без трудозатрат
                foreach (var abuser in abuserList)
                {
                    totalWL.Add(new TotalWorkLog(abuser, 0 , totalWTShouldBe));
                }
                var totalWlOrdered = totalWL.OrderBy(x => x.User.LastName);
                List<string> successMesList = new List<string>();
                if (uploadToExcel == true)
                {
                    var dDate = new DateTime(year, month, 1);
                    var filePath = FilePath.GetPathForHOD(dDate, curUser.GetDepartmentAcronym(context));
                    ExcelFile exFile = new ExcelFile(filePath);                    
                    exFile.UploadWorkLogsTotalReport(totalWlOrdered, context);                    
                    successMesList.Add($"Файл успешно выгружен по след. пути: {filePath}");
                }
                if (sendNotifications == true)
                {
                    MailService.SendMessagesToAbusers(totalWlOrdered, curUser, monthStr);
                    successMesList.Add($"Уведомления пользователям успешно отправлены.");
                }
                if (TempData["SuccessMes"] != null)
                {
                    //List<string> successMes = new List<string>();
                    var successAr = TempData["SuccessMes"] as string[];
                    successMesList.AddRange(successAr);
                }
                ViewData["SuccessMes"] = successMesList;
                //IEnumerable <IGrouping<string, PrimaTask>> 
                ViewData["curPage"] = 0;
                
                return View("MyDepartTotalResult", totalWlOrdered);
            }
            else
            {
                return View(@"/Views/Shared/AccessDenied.cshtml");
            }
        }

        public string GetCurMonth(int month)
        {
            switch (month)
            {
                case 1:
                    return "Январь";
                case 2:
                    return "Февраль";
                case 3:
                    return "Март";
                case 4:
                    return "Апрель";
                case 5:
                    return "Май";
                case 6:                
                    return "Июнь";
                case 7:
                    return "Июль";
                case 8:
                    return "Август";
                case 9:
                    return "Сентябрь";
                case 10:
                    return "Октябрь";
                case 11:
                    return "Ноябрь";
                case 12:
                    return "Декабрь";
                default:
                    return string.Empty;

            }
        }
          

        public IActionResult ShowMyDepartSetPercent(string showTaskCompCond)
        {

            List<TaskComp> totalWL = new List<TaskComp>();
            if (TempData["SuccessMes"] != null)
            {
                //List<string> successMes = new List<string>();
                var successAr = TempData["SuccessMes"] as string[];
                if (successAr!=null) ViewData["SuccessMes"] = successAr.ToList();
            }
            if (TempData["FailMes"] != null)
            {
                //List<string> successMes = new List<string>();
                var failAr = TempData["SuccessMes"] as string[];
                if (failAr!=null) ViewData["SuccessMes"] = failAr.ToList();
            }

            var curUser = WebApplication5.Models.User.GetUser(context, HttpContext);
            if (curUser != null)
            {
                ViewData["IsHOD"] = curUser.isHeadOfDepartment(context);
                ViewData["IsAdmin"] = curUser.IsAdmin(context);
            }
            var userMyDepartList = context.Users.Include(x => x.Department).Where(x => x.Department.Id == curUser.Department.Id).ToList();
            ViewData["UsersOfMyDepart"] = userMyDepartList;
            ViewData["curUser"] = curUser;

            if (curUser.isHeadOfDepartment(context))
            {
                var departAcronym = curUser.GetDepartmentAcronym(context);
                //IEnumerable <IGrouping<string, PrimaTask>>
                ViewData["curPage"] = 1;
                if (string.IsNullOrEmpty(showTaskCompCond))
                {
                    showTaskCompCond = TempData["showTaskCompCond"] as string;
                }
                List<TaskComp> taskSet = new List<TaskComp>();
                if (string.IsNullOrEmpty(showTaskCompCond) || showTaskCompCond == "0")
                {
                    //Все незавершённые комплекты моего отдела
                    taskSet = TaskComp.GetAllTasksForMyDepartmentNotCompleted(curUser, context);
                    TaskComp.RemoveStandartTasks(taskSet);
                    ViewData["ShowTaskCompCond"] = "0";
                }
                else if (showTaskCompCond == "1")
                {
                    //Все комплекты моего отдела
                    taskSet = TaskComp.GetAllTasksForMyDepartment(curUser, context).ToList();
                    ViewData["ShowTaskCompCond"] = "1";
                }                
                return View("MyDepartSetPercent", taskSet);
            }
            else
            {
                return View(@"/Views/Shared/AccessDenied.cshtml");
            }
        }

        public IActionResult SetPercent(string curPercent, string taskCompId)
        {
            int taskId = Convert.ToInt32(taskCompId);
            var taskCompSet = context.TaskComps.Where(x => x.Id == taskId);
            if (taskCompSet.Count() > 0)
            {
                var taskComp = taskCompSet.First();
                var res = Double.TryParse(curPercent, out double percent);
                if (res)
                {
                    taskComp.CompletePercent = percent;
                    context.SaveChanges();
                    TempData["SuccessMes"] = new List<string> { $"Процент по комплекту {taskComp.TaskCompName} успешно установлен. " };
                }
                else
                {
                    TempData["FailMes"] = new List<string> { $"Невозможно преобразовать в число \" {curPercent} \" " };
                }
            }
            return RedirectToAction("ShowMyDepartSetPercent");
        }

        public IActionResult SetMultiplePercent(Dictionary<string, string> taskCompDict)
        {
            List<string> successMes = new List<string>();
            List<string> errorMes = new List<string>();
            foreach (var taskCompIdPercent in taskCompDict)
            {
                int taskId = Convert.ToInt32(taskCompIdPercent.Key);
                var taskCompSet = context.TaskComps.Where(x => x.Id == taskId);
                if (taskCompSet.Count() > 0)
                {
                    var taskComp = taskCompSet.First();
                    var res = Double.TryParse(taskCompIdPercent.Value, out double percent);
                    if (res)
                    {
                        taskComp.CompletePercent = percent;
                        context.SaveChanges();
                        successMes.Add($"Процент по комплекту {taskComp.TaskCompName} успешно установлен. ");
                    }
                    else
                    {
                        errorMes.Add($"Невозможно преобразовать в число \" {taskCompIdPercent.Value} \" ");
                        
                    }
                }
            }
            TempData["SuccessMes"] = successMes;
            TempData["FailMes"] = errorMes;
            return RedirectToAction("ShowMyDepartSetPercent");
        }
    }
}