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
                ViewData["PrimaTask"] = GetTasksFromPrima(selectedProject, curUser);
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
            ViewData["CurProjects"] = GetProjectsFromPrima(curUser);
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
            var curUser = GetUser(context);
            DateTime.TryParse(DateOfReport, out DateTime date);
            ViewData["CurProjects"] = GetProjectsFromPrima(curUser);
            ViewData["KindOfAct"] = GetKindOfAct();
            ViewData["CurUser"] = GetUser(context);
            ViewData["PrimaTask"] = GetTasksFromPrima(selectedProject, curUser); //context.ProjectSet.ToList();            
            ViewData["projectName"] = selectedProject; ViewData["selectedTaskList"] = selectedTaskList; ViewData["selectedKindOfAct"] = selectedKindOfAct; ViewData["workTimeSpan"] = workTimeSpan; ViewData["comment"] = comment;
            return View("Index", new WorkLogs() { DateOfReport = GetDate(DateOfReport) });//, successMessage, failMessage);
        }


        [HttpPost]
        public IActionResult SendWorkLogs(string projectName, string dateOfReportVal, string selectedTaskrsrcId , string selectedTaskList, string selectedKindOfAct, string workTimeSpan, string comment)
        {
            try
            {
                //string selectedTaskrsrcId = selectedTaskList;
                if (ModelState.IsValid)
                {
                    var curUser = GetUser(context);
                    var proj_Id = FindProjIdByName(projectName);
                    //var taskrsrc_Id = FindTaskrsrcById(selectedTaskrsrcId);
                    //var task_rsc_Id = FindRscByTaskAndResName(task_Id, curUser.FullName);
                    var kindOfAct = GetKindOfAct(context, selectedKindOfAct);
                    var workTime = GetTimeSpan(workTimeSpan);
                    var dateOfReport = GetDate(dateOfReportVal);
                    var user = GetUser(context);
                    var res = CheckData(user, proj_Id, kindOfAct, dateOfReport, selectedTaskrsrcId, workTime, comment, out List<string> error);
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

        //string FindTaskrsrcById(string taskrsrc_Id)
        //{
        //    string taskRsrc = string.Empty;
        //    string sqlExpression = String.Format(@"Select tRs.rsrc_id   
        //FROM[primavera].[dbo].[TASKRSRC] tRs
        // Where taskrsrc_Id = '{0}'", taskrsrc_Id);

        //    string connectionString = @"Data Source=primadb;Initial Catalog=primavera;User ID=privuser;Password=P@ssw0rd";
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        connection.Open();
        //        SqlCommand command = new SqlCommand(sqlExpression, connection);
        //        SqlDataReader reader = command.ExecuteReader();
        //        if (reader.HasRows) // если есть данные
        //        {
        //            reader.Read();
        //            var taskrsrc_id = reader.GetValue(0).ToString();
        //            if (string.IsNullOrEmpty(rsrc_id))
        //            {

        //            }
        //            else
        //            {
        //                FindTaskRsc();

        //            }
        //        }
        //        reader.Close();
        //    }
        //    return;
        //}

        string FindTaskRscByRole(string task_id, string role_id)
        {
            string taskrsrc_id = string.Empty;
            string sqlExpression = String.Format(@"Select tRs.taskrsrc_id     
        FROM [primavera].[dbo].[TASKRSRC] tRs
         Where task_id='{0}' and role_id='{1}'", task_id, role_id);

            string connectionString = @"Data Source=primadb;Initial Catalog=primavera;User ID=privuser;Password=P@ssw0rd";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows) // если есть данные
                {
                    reader.Read();
                    taskrsrc_id = reader.GetValue(0).ToString();
                }
                reader.Close();
            }

            return taskrsrc_id;
        }

        //      string FindTaskRscByRole(string fullName, string roleId)
        //      {
        //          string taskrsrc_id = string.Empty;
        //          string sqlExpression = String.Format(@"Select tRs.taskrsrc_id     
        //FROM [primavera].[dbo].[TASKRSRC] tRs
        // Where task_id='{0}' and role_id='{1}'", fullName, roleId);

        //          string connectionString = @"Data Source=primadb;Initial Catalog=primavera;User ID=privuser;Password=P@ssw0rd";
        //          using (SqlConnection connection = new SqlConnection(connectionString))
        //          {
        //              connection.Open();
        //              SqlCommand command = new SqlCommand(sqlExpression, connection);
        //              SqlDataReader reader = command.ExecuteReader();
        //              if (reader.HasRows) // если есть данные
        //              {
        //                  reader.Read();
        //                  taskrsrc_id = reader.GetValue(0).ToString();
        //              }
        //              reader.Close();
        //          }

        //          return taskrsrc_id;
        //      }

        List<string> GetCurUserRoleId(string fullName)
        {
            string sqlExpression = String.Format(@"Select rsRole.role_id
  FROM[primavera].[dbo].[RSRC] rs
 JOIN[primavera].[dbo].[RSRCROLE] rsRole ON rsRole.rsrc_id = rs.rsrc_id
   Where rsrc_name = '{0}'", fullName);

            List<string> roleList = new List<string>();
            string connectionString = @"Data Source=primadb;Initial Catalog=primavera;User ID=privuser;Password=P@ssw0rd";            
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows) // если есть данные
                {
                    reader.Read();
                    roleList.Add(reader.GetValue(0).ToString());
                }
                reader.Close();
            }

            return roleList;

        }

        List<string> GetCurUserRscId(string fullName)
        {
            string sqlExpression = String.Format(@"Select rsRole.role_id
  FROM[primavera].[dbo].[RSRC] rs
 JOIN[primavera].[dbo].[RSRCROLE] rsRole ON rsRole.rsrc_id = rs.rsrc_id
   Where rsrc_name = '{0}'", fullName);

            List<string> roleList = new List<string>();
            string connectionString = @"Data Source=primadb;Initial Catalog=primavera;User ID=privuser;Password=P@ssw0rd";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows) // если есть данные
                {
                    reader.Read();
                    roleList.Add(reader.GetValue(0).ToString());
                }
                reader.Close();
            }

            return roleList;

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
        

        public List<Project> GetProjectsFromPrima(User curUser)
        {
            List<Project> projectList = new List<Project>();
            string connectionString = @"Data Source=primadb;Initial Catalog=primavera;User ID=privuser;Password=P@ssw0rd";
            string sqlExpression = SetSqlExpForProj(curUser.FullName);
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

        string SetSqlExpForProj (string fullName)
        {
            string exp = String.Format(@" DECLARE @projectTable TABLE (proj_id INT, prjName NVARCHAR(255));
    Declare @FullName NVARCHAR(255)
   SET @FullName = 'Нестеров Игорь Гидалевич'
  
  Insert Into @projectTable (proj_id, prjName)
   Select Distinct p.proj_id, p.proj_short_name
   FROM [primavera].[dbo].[RSRC] rs
    JOIN [primavera].[dbo].[TASKRSRC] t ON t.rsrc_id = rs.rsrc_id 
	JOIN [primavera].[dbo].[PROJECT] p ON p.proj_id = t.proj_id 	
	Where rsrc_name = @FullName
	
	
  Insert Into @projectTable (proj_id, prjName)
  Select Distinct pr.proj_id, pr.proj_short_name
  FROM [primavera].[dbo].[RSRC] rs
  JOIN [primavera].[dbo].[TASKRSRC] t ON t.role_id=rs.role_id
  JOIN [primavera].[dbo].[TASK] ta ON ta.task_id=t.task_id  
  JOIN [primavera].[dbo].[PROJECT] pr ON pr.proj_id=ta.proj_id  
  Where rs.rsrc_name=@FullName
  Select Distinct * from @projectTable ", fullName);
            return exp;
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

        public List<PrimaTask> GetTasksFromPrima(string selectedProject, User curUser)
        {
            //selectedProject = selectedProject.Split('-').First();
            /* var query= String.Format(@"select p.proj_short_name, p.proj_id,  t.task_id, t.task_name 
 from[primavera].[dbo].[PROJECT] p
 JOIN[primavera].[dbo].[TASK] t ON p.proj_id = t.proj_id
 Where p.proj_short_name = '{0}'", selectedProject);*/

            // var internalNum = selectedProject.Split('-').First();
            
            var query = SetSqlExpForTask(selectedProject, curUser);
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
                            primaTaskList.Add(new PrimaTask() { ProjName= reader.GetValue(0).ToString(), ProjId = projId, Task_Id = reader.GetValue(2).ToString(), TaskName = reader.GetValue(3).ToString(), Taskrsrc_Id= reader.GetValue(4).ToString()});
                        }

                    }
                }

                reader.Close();
            }

            return primaTaskList;
        }

        string SetSqlExpForTask(string selectedProject, User curUser)
        {
          var exp=  String.Format(@"DECLARE @projectTable TABLE (prjName NVARCHAR(255), proj_id INT, task_id INT, task_name NVARCHAR(255), taskrsc_id INT);
    Declare @FullName NVARCHAR(255)
	Declare @ProjName NVARCHAR(255)
   SET @FullName = '{0}'
  SET @ProjName = '{1}'

  Insert Into @projectTable (prjName, proj_id, task_id, task_name, taskrsc_id)
  Select Distinct p.proj_short_name, p.proj_id, t.taskrsrc_id, ta.task_name, t.taskrsrc_id
  FROM [primavera].[dbo].[RSRC] rs
  JOIN [primavera].[dbo].[TASKRSRC] t ON t.rsrc_id = rs.rsrc_id 
  JOIN [primavera].[dbo].[PROJECT] p ON p.proj_id = t.proj_id 	
  JOIN [primavera].[dbo].[TASK] ta ON ta.task_id = t.task_id
	Where rsrc_name = @FullName and p.proj_short_name=@ProjName
		
  Insert Into @projectTable (prjName, proj_id, task_id, task_name, taskrsc_id)
  Select Distinct pr.proj_short_name, pr.proj_id, t.taskrsrc_id, ta.task_name, t.taskrsrc_id
  FROM [primavera].[dbo].[RSRC] rs
  JOIN [primavera].[dbo].[TASKRSRC] t ON t.role_id=rs.role_id
  JOIN [primavera].[dbo].[TASK] ta ON ta.task_id=t.task_id  
  JOIN [primavera].[dbo].[PROJECT] pr ON pr.proj_id=ta.proj_id  
  Where rs.rsrc_name=@FullName and pr.proj_short_name=@ProjName
  Select Distinct * from @projectTable  ", curUser.FullName, selectedProject);
            return exp;
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
                //new PrimaTaskrsrcExecute(Convert.ToInt32(workLog.Task_id)).SubFact(workLog.WorkTime.TotalHours);
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