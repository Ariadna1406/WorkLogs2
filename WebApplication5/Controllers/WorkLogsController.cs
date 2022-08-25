using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication5.Models;
using System.Web;
using System.DirectoryServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;

namespace WebApplication5.Controllers
{
    public class WorkLogsController : Controller
    {   
        AppDbContext context;
        public WorkLogsController(AppDbContext appDbContext, IHostingEnvironment appEnv)
        {
            context = appDbContext;
        }
   
        [HttpGet]
        public ViewResult Index()
        {
            var curUser = WebApplication5.Models.User.GetUser(context, HttpContext);
            ViewData["curUser"] = curUser;
            ViewData["projectName"] = TempData["projectName"]; ViewData["selectedTaskList"] = TempData["selectedTaskList"]; ViewData["selectedKindOfAct"] = TempData["selectedKindOfAct"]; ViewData["workTimeHours"] = TempData["workTimeHours"]; ViewData["workTimeMinutes"] = TempData["workTimeMinutes"]; ViewData["comment"] = TempData["comment"];
            if (TempData["SuccessMes"] != null)
            {
                //List<string> successMes = new List<string>();
                var successAr = TempData["SuccessMes"] as string[];                
                ViewData["SuccessMes"] = successAr.ToList();
                var winUs = GetLogin(HttpContext.User.Identity.Name);
                ViewData["IsShowingBalance"] = true;              
                
                ViewData["WorkLogsLast30"] = GetWorkLogsLast30(curUser);
            }
            if (TempData["FailMes"] != null)
            {
                //List<string> successMes = new List<string>();
                var failAr = TempData["FailMes"] as string[];                 
                ViewData["FailMes"] = failAr.ToList(); ;
            }
            if (ViewData["projectName"] != null)
            {
                var selectedProject = ViewData["projectName"] as string;
                ViewData["PrimaTask"] = PrimaTask.GetTasksFromPrima(selectedProject, curUser);//GetTasksFromPrima(selectedProject, curUser);
            }
            else
            {
                ViewData["PrimaTask"] = new List<PrimaTask>();
            }
            if (TempData["IsShowingBalance"] != null)
            {
                ViewData["IsShowingBalance"] = TempData["IsShowingBalance"];
                ViewData["WorkLogsLast30"] = GetWorkLogsLast30(curUser);
            }            
            ViewData["CurProjects"] = Project.GetProjectsFromPrima(curUser);
            ViewData["KindOfAct"] = GetKindOfAct();
            ViewData["CurUser"] = curUser;
            return View(new WorkLogs() {DateOfReport=DateTime.Now.Date});
        }

        public string GetLogin(string login)
        {
            if (login.Contains("\\"))
            {
                return login.Split("\\").Last();
            }
            return login;
        }

        public ViewResult Index111()
        {
            return View();
        }

            public IActionResult SelectProject(string selectedProject, string DateOfReport, string dateOfReportVal, string selectedTaskList, string selectedKindOfAct, string workTimeHours, string workTimeMinutes, string comment)
        {
            var curUser = GetUser(context);
            DateTime.TryParse(DateOfReport, out DateTime date);
            ViewData["CurProjects"] = Project.GetProjectsFromPrima(curUser);
            ViewData["KindOfAct"] = GetKindOfAct();
            ViewData["CurUser"] = GetUser(context);
            ViewData["PrimaTask"] = PrimaTask.GetTasksFromPrima(selectedProject, curUser); //GetTasksFromPrima(selectedProject, curUser); //context.ProjectSet.ToList();            
            ViewData["projectName"] = selectedProject; ViewData["selectedTaskList"] = selectedTaskList; ViewData["selectedKindOfAct"] = selectedKindOfAct; ViewData["workTimeHours"] = workTimeHours; ViewData["workTimeMinutes"] = workTimeMinutes; ; ViewData["comment"] = comment;
            return View("Index", new WorkLogs() { DateOfReport = GetDate(DateOfReport) });//, successMessage, failMessage);
        }     


        [HttpPost]
        public IActionResult SendWorkLogs(string projectName, string dateOfReportVal, string selectedTaskrsrcId , string selectedTaskList, string selectedKindOfAct, string workTimeHours, string workTimeMinutes, string comment)
        {
            try
            {
                //string selectedTaskrsrcId = selectedTaskList;
                if (ModelState.IsValid)
                {
                    var curUser = GetUser(context);
                    var proj_Id = FindProjIdByName(projectName);
                    var task_Id = FindTaskIdByTaskrsrcId(selectedTaskrsrcId);
                    //var task_rsc_Id = FindRscByTaskAndResName(task_Id, curUser.FullName);
                    var kindOfAct = GetKindOfAct(context, selectedKindOfAct);
                    var workTime = GetTimeSpan(workTimeHours, workTimeMinutes);
                    var dateOfReport = GetDate(dateOfReportVal);
                    var user = GetUser(context);
                    var res = CheckData(user, proj_Id, kindOfAct, dateOfReport, task_Id, workTime, comment, out List<string> error);
                    if (res)
                    {
                        WorkLogs workLog = new WorkLogs(user, proj_Id, kindOfAct, dateOfReport, selectedTaskrsrcId, workTime, comment, DateTime.Now);
                        context.WorkLogs.Add(workLog);
                        context.SaveChanges();
                        TempData["SuccessMes"] = new List<string> { "Трудозатраты успешно добавлены" };
                        var primaTaskrsrcExecute = new PrimaTaskrsrcExecute(Convert.ToInt32(selectedTaskrsrcId));
                        primaTaskrsrcExecute.AddFact(workTime.TotalHours);
                    }
                    else
                    {
                        TempData["FailMes"] = error;
                    }
                }
                //var winUs = GetLogin(HttpContext.User.Identity.Name);
                //ViewData["IsShowingBalance"] = true;
                //var curUser = GetUser(context);
                //ViewData["CurUser"] = curUser;
                //ViewData["WorkLogsLast30"] = GetWorkLogsLast30(curUser);
                //ViewData["CurProjects"] = GetProjectsFromPrima();
                //ViewData["KindOfAct"] = GetKindOfAct();
                //ViewData["PrimaTask"] = GetTasksFromPrima(projectName);
                TempData["projectName"] = projectName; TempData["selectedTaskList"] = selectedTaskList; TempData["selectedKindOfAct"] = selectedKindOfAct;  TempData["workTimeHours"] = workTimeHours; TempData["workTimeMinutes"] = workTimeMinutes; TempData["comment"] = comment;
                return RedirectToAction("Index");//, successMessage, failMessage);
            }
            catch (Exception ex)
            {
                var mesList = new List<string>();
                mesList.Add(ex.ToString());
                TempData["FailMes"] = mesList;
                return View("Index");
            }
        }      


        ViewDataDictionary dataDictionary;
        string projectNameTr;

        public bool CheckData(User user, string proj_Id, KindOfAct kindOfAct, DateTime dateOfReport, string task_Id, TimeSpan workTime, string comment , out List<string> error)
        {
            error = new List<string>();
            //if (user == null)
            //{
            //    error = "Вы не были импортированы в систему обратитесь, пож-та, в отдел ОСПР";
            //    return false;
            //}
            if (new DateTime(2022,01,01)>dateOfReport.Date || dateOfReport> new DateTime(2023, 01, 01))
            {
                error.Add(" Дата отчёта должна находится в пределах текущего года.");
                
            }
            if (string.IsNullOrEmpty(proj_Id))
            {
                error.Add(" Введенного проекта не существует. " + Environment.NewLine);

            }
            if (string.IsNullOrEmpty(task_Id))
            {
                error.Add(" Данная задача не относится к выбранному проекту или не существует " + Environment.NewLine);
                
            }
            if (kindOfAct==null)
            {
                error.Add("Введённый вид деятельности не существует." + Environment.NewLine);

            }
            if (workTime==new TimeSpan() || workTime>new TimeSpan(12,0,0))
            {
                error.Add(" Указанный формат рабочего времени не соответствует шаблону" + Environment.NewLine);
                
            }
            if (comment!=null && comment.Length>200)
            {
                error.Add("Комментарий не должен превышать 200 символов " + Environment.NewLine);

            }
            if (error.Count>0) return false; 
            return true;
        }

        public string FindProjIdByName(string projectName)
        {
            string proj_id = string.Empty;
            string connectionString = @"Data Source=primadb;Initial Catalog=primavera;User ID=privuser;Password=P@ssw0rd";
            string sqlExpression = String.Format("SELECT [proj_id], [proj_short_name] FROM[primavera].[dbo].[PROJECT] Where [proj_short_name]='{0}'", projectName);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    reader.Read();
                    proj_id= reader.GetValue(0).ToString();
                }

                reader.Close();
            }

            return proj_id;
        }     

        public TimeSpan GetTimeSpan(string workTimeHours, string workTimeMinutes)
        {
            var res =Int32.TryParse(workTimeHours, out int workTimeHoursInt);
            res = Int32.TryParse(workTimeMinutes, out int workTimeMinutesInt);
            return new TimeSpan(workTimeHoursInt, workTimeMinutesInt, 0);
        }

        public DateTime GetDate(string date)
        {
            var res = DateTime.TryParse(date, out DateTime dateTime);
            return dateTime;
        }

        public User GetUser(AppDbContext context)
        {
            var winUs = GetLogin(HttpContext.User.Identity.Name);
            var userSet = context.Users.Where(x => x.Login == winUs);
           return userSet.Count() > 0 ? userSet.First() : new User() { FullName = winUs };
        }
        public KindOfAct GetKindOfAct(AppDbContext context, string selectedKindOfAct)
        {
            var KindOfActSet = context.KindOfAct.Where(x => x.Name == selectedKindOfAct);
            if (KindOfActSet.Count() > 0) return KindOfActSet.First(); else return null;
        }      
              
        public List<KindOfAct> GetKindOfAct()
        {
            if (context.KindOfAct.Count() == 0)
            {
                context.KindOfAct.AddRange(
                    new List<KindOfAct>()
                    {
                        new KindOfAct("Разработка проектной документации"),
                new KindOfAct("Разработка и выдача заданий")
                }
                 );
                context.SaveChanges();
            }
            return context.KindOfAct.ToList();
        }
                
        public IActionResult ShowWorkLogsBalance(string projectName, string DateOfReport, string selectedTaskList, string selectedKindOfAct, string workTimeHours, string workTimeMinutes, string isShowingBalance, string comment)
        {
            bool.TryParse(isShowingBalance, out bool isShowingBalanceBool);
            TempData["IsShowingBalance"] = !isShowingBalanceBool;       
            TempData["projectName"] = projectName; TempData["selectedTaskList"] = selectedTaskList; TempData["selectedKindOfAct"] = selectedKindOfAct; TempData["workTimeHours"] = workTimeHours; TempData["workTimeMinutes"] = workTimeMinutes; TempData["comment"] = comment;            
            return RedirectToAction("Index");
        }

        public IActionResult DeleteWorkLog(string IdStr, string projectName, string DateOfReport, string selectedTaskList, string selectedKindOfAct, string workTimeSpan, string isShowingBalance, string comment)
        {
            Int32.TryParse(IdStr, out int IdInt);           
            var workLogsSet = context.WorkLogs.Where(x => x.Id == IdInt);
            if (workLogsSet.Count() > 0)
            {
                var workLog = workLogsSet.First();                
                context.WorkLogs.Remove(workLog);
                context.SaveChanges();
                TempData["IsShowingBalance"] = true;
                TempData["SuccessMes"] = new List<string> { "Трудозатраты успешно удалены" };
                var primaTaskrsrcExecute = new PrimaTaskrsrcExecute(Convert.ToInt32(workLog.Taskrsrc_id));
                primaTaskrsrcExecute.AddFact((-1)*workLog.WorkTime.TotalHours);
                //ViewData["WorkLogsLast30"] = GetWorkLogsLast30(curUser);
                TempData["projectName"] = projectName; TempData["selectedTaskList"] = selectedTaskList; TempData["selectedKindOfAct"] = selectedKindOfAct; TempData["workTimeSpan"] = workTimeSpan; TempData["comment"] = comment;
            }            
            return RedirectToAction("Index");

        }

        public List<WorkLogs> GetWorkLogsLast30(User curUser)
        {
            var startDate = DateTime.Now.AddDays(-30);
            var workLogsSet= context.WorkLogs.Where(x => x.User == curUser).Where(x=>x.DateOfReport>startDate && x.DateOfReport<=DateTime.Now.Date);

            foreach(var workLog in workLogsSet)
            {
                workLog.ProjName = FindProjById(workLog.Proj_id);
                workLog.TaskName = FindTaskNameByTasktsrc_Id(workLog.Taskrsrc_id);
            }
            var workLogsList = workLogsSet.ToList();
            workLogsList.Reverse();
            ViewData["TodayWorkTimeAmount"] = SumWorkLogs(workLogsList.Where(x => x.DateOfReport.Date == DateTime.Now.Date));
            return workLogsList;

        }

        public TimeSpan SumWorkLogs(IEnumerable<WorkLogs> workLogs)
        {            
            TimeSpan totalWorkTime = new TimeSpan();
            foreach (var workLog in workLogs)
            {
                totalWorkTime += workLog.WorkTime;
            }
            return totalWorkTime;
        }

        public string FindProjById(string projId)
        {
            string projName = string.Empty;
            string connectionString = @"Data Source=primadb;Initial Catalog=primavera;User ID=privuser;Password=P@ssw0rd";
            string sqlExpression = String.Format("SELECT proj_short_name FROM[primavera].[dbo].[PROJECT] Where proj_id='{0}'", projId);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    reader.Read();
                    projName = reader.GetValue(0).ToString();               
                }
                reader.Close();
            }
            return projName;
        }
        public string FindTaskIdByTaskrsrcId(string taskrsrcId)
        {
            string taskName = string.Empty;
            string connectionString = @"Data Source=primadb;Initial Catalog=primavera;User ID=privuser;Password=P@ssw0rd";
            string sqlExpression = String.Format("  SELECT task_id FROM[primavera].[dbo].[TASKRSRC] Where taskrsrc_id='{0}'", taskrsrcId);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    reader.Read();
                    taskName = reader.GetValue(0).ToString();
                }
                reader.Close();
            }
            return taskName;
        }
        public string FindTaskNameByTasktsrc_Id(string taskrsrcId)
        {
            string taskName = string.Empty;
            string connectionString = @"Data Source=primadb;Initial Catalog=primavera;User ID=privuser;Password=P@ssw0rd";
            string sqlExpression = String.Format(@"SELECT t.task_name
  FROM[primavera].[dbo].[TASKRSRC] tRs
  JOIN[primavera].[dbo].[TASK] t ON tRs.task_id = t.task_id  Where taskrsrc_id = '{0}'", taskrsrcId);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    reader.Read();
                    taskName = reader.GetValue(0).ToString();
                }
                reader.Close();
            }
            return taskName;
        }
    }
}