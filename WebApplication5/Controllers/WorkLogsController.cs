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

            if (curUser.Id == 263)
            {
                curUser = context.Users.First(x => x.Id == 1709);
            }

            if (curUser != null)
            {
                ViewData["IsHOD"] = curUser.isHeadOfDepartment(context);
                ViewData["IsAdmin"] = curUser.IsAdmin(context);
            }
            ViewBag.UserSubs = UserSubs.GetUserSubs(context, curUser);
            if (curUser.IsAdmin(context))
            {
                ViewBag.UserSubs = WebApplication5.Models.User.GetAllActiveUsers(context).ToList();
            }
            ViewData["curUser"] = curUser;
            ViewData["projectName"] = TempData["projectName"]; ViewData["selectedTaskList"] = TempData["selectedTaskList"]; ViewData["selectedKindOfAct"] = TempData["selectedKindOfAct"]; ViewData["workTimeHours"] = TempData["workTimeHours"]; ViewData["workTimeMinutes"] = TempData["workTimeMinutes"]; ViewData["comment"] = TempData["comment"]; ViewData["curPercent"] = TempData["curPercent"];
            var dateOfReport =GetDate(TempData["DateOfReport"] as string);
            //if (!string.IsNullOrEmpty(ViewData["selectedTaskList"] as string))
            //{
                
            //}


            ViewData["UserSubsSelected"] = TempData["UserSubsSelected"]; ViewBag.InputAnotherUserWL = TempData["InputAnotherUserWL"];
            int month = 0;
            if (TempData["Month"]!=null) month =(int)TempData["Month"];            
            else month = DateTime.Now.Month;                
            
            var monthStr = GetCurMonth(month);
            ViewBag.CurMonth = monthStr;
            ViewBag.CurMonthInt = month;

            Models.User userSubs = curUser;
            var userSubsSelected = ViewData["UserSubsSelected"] as string;
            if (!string.IsNullOrEmpty(userSubsSelected))
            {
                var userFound = Models.User.GetUserByFullName(context, userSubsSelected);
                if (userFound != null)
                {
                    userSubs = userFound;
                }
            }
            var year = dateOfReport.Year;
            var workLogList = WorkLogs.GetWorkLogsByMonth(context, userSubs, month, year, out TimeSpan wlCurDay, out Dictionary<DateTime, TimeSpan> wlCurMonth, out Dictionary<DateTime, TimeSpan> wlCurPrevMonth);
            if (TempData["SuccessMes"] != null)
            {
                //List<string> successMes = new List<string>();
                var successAr = TempData["SuccessMes"] as string[];           
                if (successAr!=null) ViewData["SuccessMes"] = successAr.ToList();
                var winUs = GetLogin(HttpContext.User.Identity.Name);
                ViewData["IsShowingBalance"] = true;

                ViewData["WorkLogsLast30"] = workLogList;
            }
            if (TempData["FailMes"] != null)
            {
                //List<string> successMes = new List<string>();
                var failAr = TempData["FailMes"] as string[];
                if (failAr != null)
                {
                    ViewData["FailMes"] = failAr.ToList(); ;
                }
            }
            if (ViewData["projectName"] != null)
            {
                var selectedProject = ViewData["projectName"] as string;
                ViewData["TaskCompSet"] = TaskComp.GetAllTasksForMyDepartSelectedProject(curUser, selectedProject, context);

                // ViewData["TaskCompSet"] = TaskComp.GetAllTasksForMyDepartment(curUser, context);  //TaskComp.GetTasksFromDb(selectedProject, curUser, context);//GetTasksFromPrima(selectedProject, curUser);
            }
            else
            {
                ViewData["PrimaTask"] = new List<PrimaTask>();
            }
            
            if (TempData["IsShowingBalance"] != null)
            {
                ViewData["IsShowingBalance"] = TempData["IsShowingBalance"];
                ViewData["WorkLogsLast30"] = workLogList;
            }
            else
            {
                ViewData["IsShowingBalance"] = true;
                ViewData["WorkLogsLast30"] = workLogList;
            }
            ViewData["TodayWorkTimeAmount"] = wlCurDay;
            ViewData["wlCurMonth"] = wlCurMonth;
            ViewData["wlCurPrevMonth"] = wlCurPrevMonth;
            ViewData["CurProjects"] = Project.GetProjectsForMyDepart(curUser, context); //фильтрация проектов, у кот. задач больше двух (Командировка и Работа Нач. Отдела)
            ViewData["KindOfAct"] = GetKindOfAct();
            ViewData["CurUser"] = curUser;
            ViewData["curPage"] = 3;
            ViewData["tcReqCount"] = TaskCompRequest.GetAmountOfNewTaskCompRequestStr(context);
            ViewData["isKSP"] = curUser.IsKSP(context);
            
            return View(new WorkLogs() {DateOfReport = dateOfReport });
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

            public IActionResult SelectProject(string selectedProject, string DateOfReport, string dateOfReportVal, string selectedTaskList, string selectedKindOfAct, string workTimeHours, string workTimeMinutes, string comment, int month, string userSubsSelected, string inputAnotherUserWL)
        {
            var curUser = GetUser(context);

            if (curUser.Id == 263)
            {
                curUser = context.Users.First(x => x.Id == 1709);
            }

            Models.User userSubs = curUser;

            

            //
            //if (!string.IsNullOrEmpty(TempData["projectName"] as string))
            //{
            //    selectedProject = TempData["projectName"] as string;
            //    DateOfReport = TempData["DateOfReport"] as string;
            //    selectedTaskList = TempData["selectedTaskList"] as string;
            //    workTimeHours = TempData["workTimeHours"] as string;
            //    workTimeMinutes = TempData["workTimeMinutes"] as string;
            //    comment = TempData["comment"] as string;


            //}

            if (!string.IsNullOrEmpty(userSubsSelected))
            {
                var userFound = Models.User.GetUserByFullName(context, userSubsSelected);
                if (userFound != null)
                {
                    userSubs = userFound;
                }
                else
                {
                    TempData["FailMes"]=new string[]{ $"Пользователя \"{userSubsSelected}\" не существует." };
                    userSubsSelected = string.Empty;
                }
            }            
            //Месяц
            if (month == 0)
            {
                month = DateTime.Now.Month;
            }
            var monthStr = GetCurMonth(month);
            ViewBag.CurMonth = monthStr;
            ViewBag.CurMonthInt = month;            
            ControllerTemplate.ExecuteCommonFunctions(TempData, ViewData, context, HttpContext);
            var resDateOfRep = DateTime.TryParse(DateOfReport, out DateTime date);
            ViewData["CurProjects"] = Project.GetProjectsForMyDepart(userSubs , context); //Project.GetProjects(curUser, context);
            ViewData["KindOfAct"] = GetKindOfAct();
            ViewData["CurUser"] = curUser;
           ViewData["TaskCompSet"] = TaskComp.GetAllTasksForMyDepartSelectedProject(userSubs, selectedProject, context); //TaskComp.GetTasksFromDb(selectedProject, curUser, context); //GetTasksFromPrima(selectedProject, curUser); //context.ProjectSet.ToList();            
            ViewData["projectName"] = selectedProject; ViewData["selectedTaskList"] = selectedTaskList; ViewData["selectedKindOfAct"] = selectedKindOfAct; ViewData["workTimeHours"] = workTimeHours; ViewData["workTimeMinutes"] = workTimeMinutes; ; ViewData["comment"] = comment;
            ViewData["curPage"] = 3;
            int year;
            if (resDateOfRep) { year = date.Year; } else { year = DateTime.Now.Year; }
            var workLogList = WorkLogs.GetWorkLogsByMonth(context, userSubs, month, year, out TimeSpan wlCurDay, out Dictionary<DateTime, TimeSpan> wlCurMonth, out Dictionary<DateTime, TimeSpan> wlCurPrevMonth);
            ViewData["WorkLogsLast30"] = workLogList;
            ViewData["TodayWorkTimeAmount"] = wlCurDay;
            ViewData["wlCurMonth"] = wlCurMonth;
            ViewData["wlCurPrevMonth"] = wlCurPrevMonth;
            ViewData["IsShowingBalance"] = true;
            ViewData["UserSubsSelected"] = userSubsSelected;//UserSubs.GetInst(context, curUser, userSubsSelected);
            ViewBag.InputAnotherUserWL = inputAnotherUserWL;
            ViewBag.UserSubs = UserSubs.GetUserSubs(context, curUser);
            return View("Index", new WorkLogs() { DateOfReport = GetDate(DateOfReport) });//, successMessage, failMessage);
        }
    

        [HttpPost]
        public IActionResult SendWorkLogs(string projectName, string dateOfReportVal, string selectedTaskrsrcId , string selectedTaskList, string selectedKindOfAct, string workTimeHours, string workTimeMinutes, string comment, string kindOfActStr, string curPercent, string userSubsHidden)
        {
            try
            {
                //string selectedTaskrsrcId = selectedTaskList;
                if (ModelState.IsValid)
                {
                    var error = new List<string>();
                    var successMes = new List<string>();
                    var curUser = GetUser(context);                 
                    var proj_Id = projectName;
                    var task_Id = selectedTaskrsrcId; //TaskCompId
                    //var task_rsc_Id = FindRscByTaskAndResName(task_Id, curUser.FullName);
                    var kindOfAct = GetKindOfAct(context, selectedKindOfAct);
                    var workTime = GetTimeSpan(workTimeHours, workTimeMinutes);
                    var dateOfReport = GetDate(dateOfReportVal);
                    Models.User user = curUser;
                    if (!string.IsNullOrEmpty(userSubsHidden)) { 
                        var userFound = Models.User.GetUserByFullName(context, userSubsHidden); 
                        if (userFound != null)
                        {
                            user = userFound;                            
                        }
                        else
                        {
                            error.Add($"Пользователя \"{userSubsHidden}\" не существует.");
                            userSubsHidden = string.Empty;
                        }
                    }
                    var workTimeAmountSet = context.WorkLogs.Where(x => x.DateOfReport.Date == dateOfReport.Date && x.User == curUser);
                    var workTimeAmount = workTimeAmountSet.Sum(x => x.WorkTime.TotalHours);
                    var res = CheckData(context ,user, proj_Id, kindOfAct, dateOfReport, task_Id, workTime, comment, error, workTimeAmount);
                    var percentRes = Models.CheckData.Percent.CheckPercent(curPercent, error);
                    if (res && percentRes)
                    {
                        var taskCompIdStr = selectedTaskrsrcId; //Кривое название
                        WorkLogs workLog = new WorkLogs(user, proj_Id, kindOfAct, dateOfReport, taskCompIdStr, workTime, comment, DateTime.Now, kindOfActStr);                       
                        context.WorkLogs.Add(workLog);
                        context.SaveChanges();
                        var res_id = Int32.TryParse(taskCompIdStr, out int taskCompIdInt);
                        if (res_id) {
                            var taskCompSet = context.TaskComps.Where(x => x.Id == taskCompIdInt);
                            if (taskCompSet.Count() > 0)
                            {
                                var taskComp = taskCompSet.First();
                                user.AddUserToTask(taskComp);
                                taskComp.SetPercent(context, curPercent, user, successMes, error);
                                context.SaveChanges();                                
                            }
                                }
                        successMes.Add("Трудозатраты успешно добавлены");
                        TempData["SuccessMes"] = successMes;
                       // var primaTaskrsrcExecute = new PrimaTaskrsrcExecute(Convert.ToInt32(selectedTaskrsrcId));
                       // primaTaskrsrcExecute.AddFact(workTime.TotalHours);
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
                if (!string.IsNullOrEmpty(userSubsHidden))
                {
                 TempData["UserSubsSelected"] = userSubsHidden;//UserSubs.GetInst(context, curUser, userSubsSelected);
                 TempData["InputAnotherUserWL"] = "true"; 
                }
                TempData["projectName"] = projectName; TempData["selectedTaskList"] = selectedTaskList; TempData["selectedKindOfAct"] = selectedKindOfAct;  TempData["workTimeHours"] = workTimeHours; TempData["workTimeMinutes"] = workTimeMinutes; TempData["comment"] = comment; TempData["curPercent"] = curPercent;
                TempData["DateOfReport"] = dateOfReportVal;
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

        public bool CheckData(AppDbContext context ,User user, string proj_Id, KindOfAct kindOfAct, DateTime dateOfReport, string task_Id, TimeSpan workTime, string comment , List<string> error, double workTimeAmount)
        {
            //error = new List<string>();
            //if (user == null)
            //{
            //    error = "Вы не были импортированы в систему обратитесь, пож-та, в отдел ОСПР";
            //    return false;
            //}
            if (new DateTime(2022,01,01)>dateOfReport.Date || dateOfReport> new DateTime(2030, 01, 01))
            {
                error.Add(" Дата отчёта должна находится в пределах текущего года.");
                
            }
            if (!string.IsNullOrEmpty(proj_Id))
            {
                var taskCompSet = context.TaskComps.Where(x => x.ProjectNumber == proj_Id);
                if (taskCompSet.Count() == 0)
                {
                    error.Add(" Введенного проекта не существует. " + Environment.NewLine);
                }
            }
            else
            {
                error.Add("Заполните поле проект. " + Environment.NewLine);
            }
                        
            if (string.IsNullOrEmpty(task_Id) || task_Id=="undefined")
            {
                error.Add(" Данная задача не относится к выбранному проекту или не существует " + Environment.NewLine);
                
            }           
            if (kindOfAct==null)
            {
                error.Add("Введённый вид деятельности не существует." + Environment.NewLine);

            }
            if (workTime==new TimeSpan() || workTime>new TimeSpan(12,0,0))
            {
                error.Add("Поле \"Фактические трудозатраты\" заполнено неверно." + Environment.NewLine);
                
            }
            if (comment!=null && comment.Length>200)
            {
                error.Add("Комментарий не должен превышать 200 символов " + Environment.NewLine);

            }
            if (string.IsNullOrEmpty(user.AD_GUID))
            {
                error.Add("Вы не были импортированы в систему. Обратитесь к администратору." + Environment.NewLine);
            }
            if (workTime.TotalHours + workTimeAmount > 12)
            {
                error.Add("Суммарное кол-во трудозатрат за день должно быть не более 12 часов.");
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
            if (res) {
                return dateTime;
            }
            else
            {
                return DateTime.Now.Date;
            }
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

        public IActionResult DeleteWorkLog(string IdStr, string projectName, string DateOfReport, string selectedTaskList, string selectedKindOfAct, string workTimeSpan, string isShowingBalance, string comment, string userSubsHidden, int month)
        {
            TempData["Month"] = month;
            Int32.TryParse(IdStr, out int IdInt);           
            var workLogsSet = context.WorkLogs.Where(x => x.Id == IdInt);
            if (workLogsSet.Count() > 0)
            {
                var workLog = workLogsSet.First();                
                context.WorkLogs.Remove(workLog);
                context.SaveChanges();
                TempData["IsShowingBalance"] = true;
                TempData["SuccessMes"] = new List<string> { "Трудозатраты успешно удалены" };
                //var primaTaskrsrcExecute = new PrimaTaskrsrcExecute(Convert.ToInt32(workLog.TaskComp_id));
                //primaTaskrsrcExecute.AddFact((-1)*workLog.WorkTime.TotalHours);
                //ViewData["WorkLogsLast30"] = GetWorkLogsLast30(curUser);
                TempData["projectName"] = projectName; TempData["selectedTaskList"] = selectedTaskList; TempData["selectedKindOfAct"] = selectedKindOfAct; TempData["workTimeSpan"] = workTimeSpan; TempData["comment"] = comment;
            }
            if (!string.IsNullOrEmpty(userSubsHidden))
            {
                TempData["UserSubsSelected"] = userSubsHidden;//UserSubs.GetInst(context, curUser, userSubsSelected);
                TempData["InputAnotherUserWL"] = "true";
            }
            return RedirectToAction("Index");

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

    }
}