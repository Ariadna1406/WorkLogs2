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
    public class AdminController : Controller
    {
        //public ImportUsersFromAdExecute()
        [HttpPost]
        public IActionResult ClickButton()
        {
            //ImportUsersFromAdExecute();
            ViewData["Logs"] = "След. пользователи были импортированы:";
            return View("AdminMain12");
        }

        public IActionResult UploadToExcel()
        {
            try
            {
                var usersFromAD = GetUsersFromAD();
                UploadToExcelFile(usersFromAD);
                ViewData["Information"] = new List<string>() { "Успешно выгружено по след. пути: ", @"\\test3elma\upload$\ExcelUpload" };
                return View("AdminMain");
            }
            catch (Exception ex)
            {
                var mesList = new List<string>();
                mesList.Add(ex.ToString());
                ViewData["FailMes"] = mesList;
                    return View("AdminMain");
            }
        }

        public void UploadToExcelFile(List<User> users)
        {
            Workbook wb = new Workbook(@"C:\Elma3test\test\upload\Templates\ADTemplate.xlsx");
            Worksheet ws = wb.Worksheets[0];
            int row = 2;
            SetStyle(users.Count, ref ws);
            foreach (var user in users)
            {
                ws.Cells[row, 0].Value = user.Department.Name;
                ws.Cells[row, 1].Value = user.FullName;
                ws.Cells[row, 2].Value = user.Email;
                ws.Cells[row, 3].Value = user.PhoneNumber;
                row++;
            }

            var now = DateTime.Now;
            string dir = @"C:\Elma3test\test\upload\ExcelUpload" + "\\";
            string fileName = String.Format("UploadAD_{0}-{1}-{2}_{3}-{4}-{5}.xlsx", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
            wb.Save(dir + fileName);
        }

        public IActionResult UploadWorkLogs()
        {
            try
            {
                UploadWorkLogsToExcelFile();
                ViewData["Information"] = new List<string>() { "Успешно выгружено по след. пути: ", @"\\test3elma\upload$\ExcelUpload" };
                return View("AdminMain");
            }
            catch (Exception ex)
            {
                var mesList = new List<string>();
                mesList.Add(ex.ToString());
                ViewData["FailMes"] = mesList;
                return View("AdminMain");
            }
        }

        public void UploadWorkLogsToExcelFile()
        {
            Workbook wb = new Workbook(@"C:\Elma3test\test\upload\Templates\WorkLogTemp.xlsx"); //(@"\\test3elma\upload$\Templates\WorkLogTemp.xlsx");
            Worksheet ws = wb.Worksheets[0];
            int row = 2;
            var workLogSet= context.WorkLogs.Include(x => x.User).Include(x => x.User.Department).Include(x=>x.KindOfAct);            
            SetStyle(workLogSet.Count(), ref ws);
            
            foreach (var workLog in workLogSet)
            {
                if (workLog.User != null && workLog.User.Department != null)
                {
                    ws.Cells[row, 0].Value = workLog.User.Department.Name;
                }
                ws.Cells[row, 1].Value = workLog.User.FullName;
                ws.Cells[row, 2].Value = FindProjById(workLog.Proj_id);
                ws.Cells[row, 3].Value = FindTaskById(workLog.Taskrsrc_id);
                if (workLog.KindOfAct != null)
                {
                    ws.Cells[row, 4].Value = workLog.KindOfAct.Name;
                }
                ws.Cells[row, 5].Value = workLog.WorkTime;
                ws.Cells[row, 6].Value = workLog.DateOfReport;
                ws.Cells[row, 7].Value = workLog.DateOfSendingReport;
                ws.Cells[row, 8].Value = workLog.Comment;
                if (workLog.WorkTime != null)
                {
                    double min= workLog.WorkTime.Minutes / 60.0;
                    double hour = workLog.WorkTime.Hours;
                    var workLogsDec = min + hour;
                    ws.Cells[row, 9].Value = workLogsDec;
                }
                row++;
            }

            var now = DateTime.Now;
            string dir = @"C:\Elma3test\test\upload\ExcelUpload" + "\\";
            string fileName = String.Format("UploadWorkLogs_{0}-{1}-{2}_{3}-{4}-{5}.xlsx", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
            wb.Save(dir + fileName);
        }

        public string WriteWorkTime(TimeSpan workTime)
        {
            var workTimeStr = workTime.ToString();
            if (workTimeStr.First() == '0')
            {
               workTimeStr= workTimeStr.TrimStart('0');
            }
            return workTimeStr;
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

        public void SetStyle(int count, ref Worksheet ws)
        {
            var style11 = ws.Cells[2, 0].GetStyle();
            var style12 = ws.Cells[2, 1].GetStyle();
            style12.Number = 49;
            var style13 = ws.Cells[2, 2].GetStyle();
            var style14 = ws.Cells[2, 3].GetStyle();
            var style15 = ws.Cells[2, 4].GetStyle();
            var style16 = ws.Cells[2, 5].GetStyle();
            //style6.Number = 46;
            var style17 = ws.Cells[2, 6].GetStyle();
            style17.Number = 14;
            var style18 = ws.Cells[2, 7].GetStyle();
            style18.Number = 14;
            var style19 = ws.Cells[2, 8].GetStyle();
            var style10 = ws.Cells[2, 9].GetStyle();

            var style21 = ws.Cells[3, 0].GetStyle();
            var style22 = ws.Cells[3, 1].GetStyle();
            style22.Number = 49;
            var style23 = ws.Cells[3, 2].GetStyle();
            var style24 = ws.Cells[3, 3].GetStyle();
            var style25 = ws.Cells[3, 4].GetStyle();
            var style26 = ws.Cells[3, 5].GetStyle();
            //style6.Number = 46;
            var style27 = ws.Cells[3, 6].GetStyle();
            style27.Number = 14;
            var style28 = ws.Cells[3, 7].GetStyle();
            style28.Number = 14;
            var style29 = ws.Cells[3, 8].GetStyle();
            var style20 = ws.Cells[3, 9].GetStyle();
            for (int i=2;i<=count+1; i=i+2)
            {
                ws.Cells[i, 0].SetStyle(style11);
                ws.Cells[i, 1].SetStyle(style12);
                ws.Cells[i, 2].SetStyle(style13);
                ws.Cells[i, 3].SetStyle(style14);
                ws.Cells[i, 4].SetStyle(style15);
                ws.Cells[i, 5].SetStyle(style16);
                ws.Cells[i, 6].SetStyle(style17);
                ws.Cells[i, 7].SetStyle(style18);
                ws.Cells[i, 8].SetStyle(style19);
                ws.Cells[i,9].SetStyle(style10);

            }
            for (int i = 3; i <= count + 1; i=i + 2)
            {
                ws.Cells[i, 0].SetStyle(style21);
                ws.Cells[i, 1].SetStyle(style22);
                ws.Cells[i, 2].SetStyle(style23);
                ws.Cells[i, 3].SetStyle(style24);
                ws.Cells[i, 4].SetStyle(style25);
                ws.Cells[i, 5].SetStyle(style26);
                ws.Cells[i, 6].SetStyle(style27);
                ws.Cells[i, 7].SetStyle(style28);
                ws.Cells[i, 8].SetStyle(style29);
                ws.Cells[i, 9].SetStyle(style20);

            }
        }

        List<User> usersNewList;
        public IActionResult ImportUsersFromAD()
        {
            usersNewList = new List<User>();
            List<User> usersUpdateList = new List<User>();
            var usersFromAD = GetUsersFromAD();
            CompareUsers(usersFromAD, ref usersNewList, ref usersUpdateList); // Можно обработать обновление пользователей, также вывести пользователю для подтверждения
            //context.SaveChanges();
            //ImportUsersFromAdExecute();      

            return View(usersNewList);
        }

        

        public IActionResult ImportUsersPost(List<User> users)
        {
            var userListRet= users.Where(x => x.NeedToImport == false).ToList();
            var userList = users.Where(x => x.NeedToImport == true);
            foreach (var user in userList)
            {
                if (user.Department.Id != 0)
                {
                    var departId = user.Department.Id;
                    user.Department = context.Departments.Where(x => x.Id == user.Department.Id).First();
                }
                else
                {
                    user.Department = null;
                }
                string[] fullNameSplited= user.FullName.Split(' ');
                user.LastName = fullNameSplited[0];
                user.FirstName = fullNameSplited[1];
                user.MiddleName = fullNameSplited[2];
                context.Users.Add(user); //Add department
            }
            context.SaveChanges();
            ViewData["Information"] = new List<string>() { String.Format("Импортировано пользователей - {0}", userList.Count()) };
            return View("ImportUsersFromAD", userListRet);
        }

        [HttpGet]
        public IActionResult AdminMain()
        {            
            if (IsAdminUser())
            {                
                return View("AdminMain");
            }
            return View("/Views/Shared/AccessDenied.cshtml");
        }

        public bool IsAdminUser()
        {
            var winUs = GetLogin(HttpContext.User.Identity.Name);
            var userSet = context.Users.Where(x => x.Login == winUs).Include(x => x.Role);
            if (userSet.Count() > 0 && userSet.First().Role != null && userSet.First().Role.Name == "Admin")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public string GetLogin(string login)
        {
            if (login.Contains("\\"))
            {
                return login.Split("\\").Last();
            }
            return login;
        }

        AppDbContext context;
        public AdminController(AppDbContext appDbContext, IHostingEnvironment appEnv)
        {
            context = appDbContext;
        }

        public IActionResult DropDownList()
        {
            return PartialView(context.Users);
        }

        public IActionResult CreateNewProject()
        {
            ViewData["ActiveProjects"] = context.ProjectSet.Where(x => x.ShowInMenuBar == true); //new Project() { InternalNum = "2603" }; 
                                                                                                 //var user = new User() { LastName = "Ведерникова", FirstName = "Альбина", FullName = "Ведерникова Альбина" };
                                                                                                 // var userList = new List<User>();
                                                                                               //userList.Add(user);
            if (context.Users.Count() == 0) AddDefaultUsers(context);
            ViewData["Users"] = context.Users.ToList();//userList;
            
            return View();
        }

        public List<Statuses> GetStatuses()
        {
            var statuses = new List<Statuses>();
            statuses.Add(new Statuses() { Id = 0, StatusName = "Новое" });
            statuses.Add(new Statuses() { Id = 1, StatusName = "Исправлено исполнителем" });
            statuses.Add(new Statuses() { Id = 2, StatusName = "Проверено BIM-координатором" });
            statuses.Add(new Statuses() { Id = 3, StatusName = "Снято" });
            statuses.Add(new Statuses() { Id = 4, StatusName = "Повторное" });
            return statuses;
        }

        public IActionResult DeleteProjects()
        {
            var listStat = GetStatuses();
            SelectList selectListItems = new SelectList(listStat, "Id", "StatusName", listStat[1]);
            ViewBag.Statuses = selectListItems;
            ViewData["ActiveProjects"] = context.ProjectSet.Where(x => x.ShowInMenuBar == true);
            ViewData["Context"] = context;
            var projectSet = context.ProjectSet.Where(x => x.IsDeleted == false).Include(x => x.Manager).ToList();
            return View(projectSet);
        }

       
       public IActionResult DeleteSelectedProject(int? Id)
        {
           context.ProjectSet.First(x => x.Id == Id).IsDeleted=true;
            context.SaveChanges();
            var projectSet= context.ProjectSet.Where(x => x.IsDeleted == false).ToList();
            return View("DeleteProjects", projectSet);
        }

        public void AddDefaultUsers(AppDbContext appDbContext)
        {
            List<User> userList = new List<User>()
            {
                new User(){FirstName ="Альбина", LastName="Ведерникова", FullName="Альбина Ведерникова" },
                 new User(){FirstName ="Тимофей", LastName="Беликов", FullName="Тимофей Беликов" }
            };
            appDbContext.Users.AddRange(userList);
            appDbContext.SaveChanges();
        }

        [HttpPost]
        public IActionResult CreateNewProject(Project project)
        {
            // var selUserStr = (string)selectedUser;
            string selectedUser = Request.Form["selectedUser"].ToString();
            ViewData["Users"] = context.Users.ToList();
            if (ModelState.IsValid)
            {
                var user= context.Users.Where(x => x.FullName == selectedUser).First();
                project.Manager = user;
                context.ProjectSet.Add(project);
                context.SaveChanges();
                return RedirectToAction("Index", "Home");//(@"~/Views/Home/Index.cshtml", context);
            }
            else
            {
                return View();
            }
        }

        private List<User> GetUsersFromAD()
        {
            string userGrName = "OU=Users";
            var root = new DirectoryEntry("LDAP://" + "oilpro");
            var department = root.Children.Find("OU=Departments");
            var divisionSet = root.Children.Find("OU=Divisions");
            List<DirectoryEntry> listGrUsers = new List<DirectoryEntry>();
            // ОТДЕЛЫ
            foreach (var depart in department.Children)
            {
                var dep = depart as DirectoryEntry;
                GetGroupFromDepart(dep, userGrName, ref listGrUsers);
               
            }
            // ПОДРАЗДЕЛЕНИЯ (АЛЬМЕТЬЕСК)
            foreach (var depart in divisionSet.Children)
            {
                var dep = depart as DirectoryEntry;
                GetGroupFromDepart(dep, userGrName, ref listGrUsers);
               
            }

            return GetUserNames(listGrUsers);
        }     


        public void GetGroupFromDepart(DirectoryEntry depart, string groupName, ref List<DirectoryEntry> listGrUsers)
        {
            var depChildList = depart.ChildrenToList();
            if (depChildList.Count > 0)
            {
                var listFound = depChildList.Where(x => x.Name == groupName);
                if (listFound.Count() == 0)
                {
                    foreach (var dep in depChildList)
                    {
                        var d = dep as DirectoryEntry;
                        GetGroupFromDepart(d, groupName, ref listGrUsers);
                    }
                }
                else
                {
                    listGrUsers.Add(listFound.First());
                }
            }
           
        }

        public List<User> GetUserNames(List<DirectoryEntry> listGrUsers)
        {
            List<User> userList = new List<User>();
            foreach (var userGr in listGrUsers)
            {
                foreach (var user in userGr.Children)
                {                   
                    var us = user as DirectoryEntry;
                    if (us.Name.Contains("Альберт"))
                    {
                        StreamWriter sw = new StreamWriter(@"D:\propSet.txt");
                        foreach (PropertyValueCollection prop in us.Properties)
                        {
                           
                                sw.WriteLine(String.Format("{0} - {1}", prop.PropertyName, prop.Value));
                            
                        }
                        sw.Close();
                    }
                    var fullName = GetProperty("displayName", us);
                    var fullNameSplited = fullName.Split(' ').ToList();
                    if (fullNameSplited.Count == 3)
                    {
                        userList.Add(new User()
                        {
                            LastName = fullNameSplited[0],
                            FirstName = fullNameSplited[1],
                            MiddleName = fullNameSplited[2],
                            AD_GUID = us.NativeGuid,
                            Email = GetProperty("mail", us),
                            Login = GetProperty("mailNickname", us),
                            Department = FindOrCreateDepartment(GetProperty("department", us)),                            
                            FullName = String.Format("{0} {1} {2}", fullNameSplited[0], fullNameSplited[1], fullNameSplited[2]),
                            PhoneNumber = GetProperty("telephoneNumber", us)

                    }) ;
                    }
                    
                }
            }            
            return userList;
        }

        public Department FindOrCreateDepartment(string departmentName)
        {
           var departSet= context.Departments.Where(x => x.Name == departmentName);
            if (departSet.Count() > 0)
            {
                return departSet.First();
            }
            else
            {
                Department newDepart = new Department() { Name = departmentName };
                context.Departments.Add(newDepart);
                context.SaveChanges();
                return newDepart;
            }
        }

        public string GetProperty(string propName, DirectoryEntry directoryEntry)
        {
            try
            {
                var propert = directoryEntry.Properties[propName];
                return propert.Value as string;
            }
            catch
            {
                return string.Empty;
            }
        }

        public void CompareUsers(List<User> usersFromAD, ref List<User> usersNewList, ref List<User> usersUpdateList)
        {
            foreach (var userFromAD in usersFromAD)
            {
                var userSet = context.Users.Where(x =>x.AD_GUID == userFromAD.AD_GUID);
                if (userSet.Count() > 0)
                {
                    var userFromDB = userSet.First();
                    CompareUpdateAllProps(ref userFromDB, userFromAD);
                }
                else
                {
                    usersNewList.Add(userFromAD);
                }
            }
        }

        public void CompareUpdateAllProps(ref User userFromDB, User userFromAD)
        {
            if (userFromDB.FirstName != userFromAD.FirstName)
            {
                userFromDB.FirstName = userFromAD.LastName;
            }
                 // Остальные свойства      

        }



    }

    public static class ExtensionClass
    {    public static List<string> AddToList(this List<string> strList, DirectoryEntry depart)
        {
            Regex regex = new Regex(@"CN=\w{2,4}-\w{4,15}");
            var match = regex.Match(depart.Name);
            if (!string.IsNullOrEmpty(match.Groups[0].Value))
            {
                var cn = depart.Name.Replace("CN=", "");
                strList.Add(cn);
            }
            return strList;
        }
    }
}