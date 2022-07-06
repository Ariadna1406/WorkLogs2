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
            var curUser = GetUser(context);
            ViewData["projectName"] = TempData["projectName"]; ViewData["selectedTaskList"] = TempData["selectedTaskList"]; ViewData["selectedKindOfAct"] = TempData["selectedKindOfAct"]; ViewData["workTimeSpan"] = TempData["workTimeSpan"]; ViewData["comment"] = TempData["comment"];
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
                ViewData["PrimaTask"] = GetTasksFromPrima(selectedProject);
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
            //var winUser = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
            ViewData["CurProjects"] = GetProjectsFromPrima();
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

            public IActionResult SelectProject(string selectedProject, string DateOfReport, string dateOfReportVal, string selectedTaskList, string selectedKindOfAct, string workTimeSpan, string comment)
        {
            DateTime.TryParse(DateOfReport, out DateTime date);
            ViewData["CurProjects"] = GetProjectsFromPrima();
            ViewData["KindOfAct"] = GetKindOfAct();
            ViewData["CurUser"] = GetUser(context);
            ViewData["PrimaTask"] = GetTasksFromPrima(selectedProject); //context.ProjectSet.ToList();            
            ViewData["projectName"] = selectedProject; ViewData["selectedTaskList"] = selectedTaskList; ViewData["selectedKindOfAct"] = selectedKindOfAct; ViewData["workTimeSpan"] = workTimeSpan; ViewData["comment"] = comment;
            return View("Index", new WorkLogs() { DateOfReport = GetDate(DateOfReport) });//, successMessage, failMessage);
        }


        [HttpPost]
        public IActionResult SendWorkLogs(string projectName, string dateOfReportVal, string selectedTaskList, string selectedKindOfAct, string workTimeSpan, string comment)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var proj_Id = FindProjIdByName(projectName);
                    var task_Id = FindTaskIdByName(proj_Id, selectedTaskList);
                    var kindOfAct = GetKindOfAct(context, selectedKindOfAct);
                    var workTime = GetTimeSpan(workTimeSpan);
                    var dateOfReport = GetDate(dateOfReportVal);
                    var user = GetUser(context);
                    var res = CheckData(user, proj_Id, kindOfAct, dateOfReport, task_Id, workTime, comment, out List<string> error);
                    if (res)
                    {
                        WorkLogs workLog = new WorkLogs(user, proj_Id, kindOfAct, dateOfReport, task_Id, workTime, comment, DateTime.Now);
                        context.WorkLogs.Add(workLog);
                        context.SaveChanges();
                        TempData["SuccessMes"] = new List<string> { "Трудозатраты успешно добавлены" };
                        var primaTaskEx = new PrimaTaskExecute(Convert.ToInt32(task_Id));
                        primaTaskEx.AddFact(workTime.TotalHours);
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
                TempData["projectName"] = projectName; TempData["selectedTaskList"] = selectedTaskList; TempData["selectedKindOfAct"] = selectedKindOfAct; TempData["workTimeSpan"] = workTimeSpan; TempData["comment"] = comment;
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

        public string FindTaskIdByName(string proj_Id,string selectedTaskName)
        {
            string task_id = string.Empty;
            string connectionString = @"Data Source=primadb;Initial Catalog=primavera;User ID=privuser;Password=P@ssw0rd";
            string sqlExpression = String.Format("SELECT [task_id] FROM[primavera].[dbo].[TASK] Where proj_id='{0}' AND task_name='{1}'", proj_Id, selectedTaskName);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    reader.Read();
                    task_id = reader.GetValue(0).ToString();
                }

                reader.Close();
            }

            return task_id;
        }

        public TimeSpan GetTimeSpan(string workTime)
        {
            var res = TimeSpan.TryParse(workTime, out TimeSpan timeSpan);
            return timeSpan;
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
        

        public List<Project> GetProjectsFromPrima()
        {
            List<Project> projectList = new List<Project>();
            string connectionString = @"Data Source=primadb;Initial Catalog=primavera;User ID=privuser;Password=P@ssw0rd";
            string sqlExpression = "SELECT [proj_id], [proj_short_name] FROM[primavera].[dbo].[PROJECT] Where project_flag='Y'";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {                  
                    while (reader.Read()) // построчно считываем данные
                    {
                        projectList.Add(new Project() { Id= (int)reader.GetValue(0), InternalNum = reader.GetValue(1).ToString() });                  

                     
                    }
                }

                reader.Close();
            }

            return projectList;
        }

        public List<PrimaTask> GetTasksFromPrima()
        {
            List<PrimaTask> primaTaskList = new List<PrimaTask>();
            string connectionString = @"Data Source=primadb;Initial Catalog=primavera;User ID=privuser;Password=P@ssw0rd";
            string sqlExpression = "SELECT proj_id, task_name, task_id FROM[primavera].[dbo].[TASK]";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read()) // построчно считываем данные
                    {
                        int projId = -1;
                        Int32.TryParse(reader.GetValue(0).ToString(), out projId);
                        if (projId > 0)
                        {
                            primaTaskList.Add(new PrimaTask() { ProjId = projId, TaskName = reader.GetValue(1).ToString(), Id=(int)reader.GetValue(2) });
                        }

                    }
                }

                reader.Close();
            }

            return primaTaskList;
        }

        public List<PrimaTask> GetTasksFromPrima(string selectedProject)
        {
            //selectedProject = selectedProject.Split('-').First();
            var query= String.Format(@"select p.proj_short_name, p.proj_id,  t.task_id, t.task_name 
from[primavera].[dbo].[PROJECT] p
JOIN[primavera].[dbo].[TASK] t ON p.proj_id = t.proj_id
Where p.proj_short_name = '{0}'", selectedProject);

           // var internalNum = selectedProject.Split('-').First();
            List<PrimaTask> primaTaskList = new List<PrimaTask>();
            string connectionString = @"Data Source=primadb;Initial Catalog=primavera;User ID=privuser;Password=P@ssw0rd";
            string sqlExpression = query;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read()) // построчно считываем данные
                    {
                        int projId = -1;
                        Int32.TryParse(reader.GetValue(1).ToString(), out projId);
                        if (projId > 0)
                        {
                            primaTaskList.Add(new PrimaTask() { ProjId = projId, TaskName = reader.GetValue(3).ToString(), Id = (int)reader.GetValue(2) });
                        }

                    }
                }

                reader.Close();
            }

            return primaTaskList;
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
                
        public IActionResult ShowWorkLogsBalance(string projectName, string DateOfReport, string selectedTaskList, string selectedKindOfAct, string workTimeSpan, string isShowingBalance, string comment)
        {
            bool.TryParse(isShowingBalance, out bool isShowingBalanceBool);
            TempData["IsShowingBalance"] = !isShowingBalanceBool;
            //var winUs = GetLogin(HttpContext.User.Identity.Name);
            //var winUser = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
            //var curUser= GetUser(context);
            //ViewData["CurUser"] = curUser;
            //ViewData["WorkLogsLast30"] = GetWorkLogsLast30(curUser);
            //ViewData["CurProjects"] = GetProjectsFromPrima();
            //ViewData["KindOfAct"] = GetKindOfAct();
            //Dictionary<string, string> prevValues = new Dictionary<string, string>();
            //prevValues.Add("projectName", projectName); prevValues.Add("selectedTaskList", selectedTaskList); prevValues.Add("selectedTaskList", selectedTaskList);
            TempData["projectName"] = projectName; TempData["selectedTaskList"] = selectedTaskList; TempData["selectedKindOfAct"] = selectedKindOfAct; TempData["workTimeSpan"] = workTimeSpan; TempData["comment"] = comment;
            // ViewData["PrimaTask"] = GetTasksFromPrima(projectName);            
            //return  View("Index", new WorkLogs() { DateOfReport = GetDate(DateOfReport) });//, successMessage, failMessage);
            return RedirectToAction("Index");
        }

        public IActionResult DeleteWorkLog(string IdStr, string projectName, string DateOfReport, string selectedTaskList, string selectedKindOfAct, string workTimeSpan, string isShowingBalance, string comment)
        {
            Int32.TryParse(IdStr, out int IdInt);
            
            //ViewData["CurProjects"] = GetProjectsFromPrima();
            //ViewData["KindOfAct"] = GetKindOfAct();
            //var curUser = GetUser(context);
            //ViewData["CurUser"] = curUser;
            //ViewData["PrimaTask"] = GetTasksFromPrima();
            
            var workLogsSet = context.WorkLogs.Where(x => x.Id == IdInt);
            if (workLogsSet.Count() > 0)
            {
                var workLog = workLogsSet.First();
                new PrimaTaskExecute(Convert.ToInt32(workLog.Task_id)).SubFact(workLog.WorkTime.TotalHours);
                context.WorkLogs.Remove(workLog);
                context.SaveChanges();
                TempData["IsShowingBalance"] = true;
                TempData["SuccessMes"] = new List<string> { "Трудозатраты успешно удалены" };
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
                workLog.TaskName = FindTaskById(workLog.Task_id);
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
        public string FindTaskById(string taskId)
        {
            string taskName = string.Empty;
            string connectionString = @"Data Source=primadb;Initial Catalog=primavera;User ID=privuser;Password=P@ssw0rd";
            string sqlExpression = String.Format("SELECT task_name FROM[primavera].[dbo].[TASK] Where task_id='{0}'", taskId);
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