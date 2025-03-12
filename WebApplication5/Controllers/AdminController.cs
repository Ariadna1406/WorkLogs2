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
using System.Net.Mail;
using System.Net;

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

        public async Task<IActionResult> MyMethod()
        {
            // Логика вашего метода
            StreamWriter sw = new StreamWriter(@"\\srv-ws\inetpub\test.txt");
            sw.WriteLine("2312312");
            sw.Close();

            await Task.Delay(1000); // Пример асинхронной операции (ожидание 1 секунды)
            return Ok();
        }



        public IActionResult UploadToExcel()
        {
            try
            {
                var usersFromAD = GetUsersFromAD();
                UploadToExcelFile(usersFromAD);
                ViewData["Information"] = new List<string>() { "Успешно выгружено по след. пути: \\\\srv-ws\\inetpub" };
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
            Workbook wb = new Workbook();
            Worksheet ws = wb.Worksheets[0];
            int row = 2;
            SetStyle(users.Count, ref ws);
            foreach (var user in users)
            {
                if (user.IsActive==null || user.IsActive==true)
                {
                    ws.Cells[row, 0].Value = user.Department.Name;
                    ws.Cells[row, 1].Value = user.Position;
                    ws.Cells[row, 2].Value = user.FullName;
                    ws.Cells[row, 3].Value = user.Email;
                    ws.Cells[row, 4].Value = user.PhoneNumber;
                    ws.Cells[row, 5].Value = user.City;

                    row++;
                }
            }
            ws.AutoFitColumns();
            var now = DateTime.Now;
            string dir = @"\\srv-ws\inetpub" + "\\";
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
            var workLogSet = context.WorkLogs.Include(x => x.User).Include(x => x.User.Department).Include(x => x.KindOfAct);
            SetStyle(workLogSet.Count(), ref ws);

            foreach (var workLog in workLogSet)
            {
                if (workLog.User != null && workLog.User.Department != null)
                {
                    ws.Cells[row, 0].Value = workLog.User.Department.Name;
                }
                ws.Cells[row, 1].Value = workLog.User.FullName;
                ws.Cells[row, 2].Value = FindProjById(workLog.Proj_id);
                ws.Cells[row, 3].Value = FindTaskById(workLog.TaskComp_id);
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
                    double min = workLog.WorkTime.Minutes / 60.0;
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
                workTimeStr = workTimeStr.TrimStart('0');
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
            for (int i = 2; i <= count + 1; i = i + 2)
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
                ws.Cells[i, 9].SetStyle(style10);

            }
            for (int i = 3; i <= count + 1; i = i + 2)
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
            ControllerTemplate.ExecuteCommonFunctions(TempData, ViewData, context, HttpContext);
            usersNewList = new List<User>();
            List<User> usersUpdateList = new List<User>();
            var usersFromAD = GetUsersFromAD();
            CompareUsers(usersFromAD, ref usersNewList, ref usersUpdateList); // Можно обработать обновление пользователей, также вывести пользователю для подтверждения
            Models.User.RefreshPublicDepart(context);
            context.SaveChanges();
            //ImportUsersFromAdExecute();      

            return View(usersNewList);
        }


        public IActionResult ImportUsersPost(List<User> users)
        {
            var userListRet = users.Where(x => x.NeedToImport == false).ToList();
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
                string[] fullNameSplited = user.FullName.Split(' ');
                user.LastName = fullNameSplited[0];
                user.FirstName = fullNameSplited[1];
                user.MiddleName = fullNameSplited[2];
                user.ImportDate = DateTime.Now;
                context.Users.Add(user); //Add department
            }
            Models.User.RefreshPublicDepart(context);
            context.SaveChanges();
            ViewData["Information"] = new List<string>() { String.Format("Импортировано пользователей - {0}", userList.Count()) };
            return View("ImportUsersFromAD", userListRet);
        }

        public string ImportUsersAuto()
        {
            //ImportUsersFromAD
            ControllerTemplate.ExecuteCommonFunctions(TempData, ViewData, context, HttpContext);
            usersNewList = new List<User>();
            List<User> usersUpdateList = new List<User>();
            var usersFromAD = GetUsersFromAD();
            CompareUsers(usersFromAD, ref usersNewList, ref usersUpdateList); // Можно обработать обновление пользователей, также вывести пользователю для подтверждения
            Models.User.RefreshPublicDepart(context);
            context.SaveChanges();

            //ImportUsersPost                        
            foreach (var user in usersNewList)
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
                //string[] fullNameSplited = user.FullName.Split(' ');
                //user.LastName = fullNameSplited[0];
                //user.FirstName = fullNameSplited[1];
                //user.MiddleName = fullNameSplited[2];
                user.ImportDate = DateTime.Now;
                context.Users.Add(user); //Add department
            }
            var usersImp = string.Join(',', usersNewList.Select(x=>x.FullName));
            Models.User.RefreshPublicDepart(context);
            context.SaveChanges();

            return $"AUTOMATIC IMPORT DONE!!! Кол-во импортированных польз. - {usersNewList.Count}. Импортированы след. пользователи: {usersImp} ............. {DateTime.Now}\n";
        }

        public string ReturnText()
        {
            return $"AUTOMATIC IMPORT DONE!!!................{DateTime.Now}\n";
        }


        [HttpGet]
        public IActionResult AdminMain()
        {
            bool isAdmin = false;
            var curUser = WebApplication5.Models.User.GetUser(context, HttpContext);
            ViewBag.UserDepartAcronym = TempData["UserDepartAcronym"];
            ViewBag.UserFullName=TempData["UserFullName"];
            ViewBag.TaskCompInfo =TempData["taskCompInfo"];
            if (curUser != null)
            {
                isAdmin = curUser.IsAdmin(context);
                ViewData["curUser"] = curUser;
                ViewData["IsAdmin"] = isAdmin;

                ViewData["Information"] = TempData["SuccessMes"];
                ViewData["FailMes"]= TempData["FailMes"];
                ViewData["IsHOD"] = curUser.isHeadOfDepartment(context);
            }
            if (isAdmin)
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
            context.ProjectSet.First(x => x.Id == Id).IsDeleted = true;
            context.SaveChanges();
            var projectSet = context.ProjectSet.Where(x => x.IsDeleted == false).ToList();
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
                var user = context.Users.Where(x => x.FullName == selectedUser).First();
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
            var fired1 = root.Children.Find("OU=_Trash");
            var fired2 = root.Children.Find("OU=Уволенные");
            List<DirectoryEntry> listGrUsers = new List<DirectoryEntry>();
            // ОТДЕЛЫ
            foreach (var depart in department.Children)
            {
                var dep = depart as DirectoryEntry;
                int depth = 0;
                GetGroupFromDepart(dep, userGrName, ref listGrUsers, ref depth);

            }
            // ПОДРАЗДЕЛЕНИЯ (АЛЬМЕТЬЕСК)
            foreach (var depart in divisionSet.Children)
            {
                int depth = 0;
                var dep = depart as DirectoryEntry;
                if (dep.Name== "OU=Управление проектирования г. Уфа")
                {
                    int b = 1;
                }
                GetGroupFromDepart(dep, userGrName, ref listGrUsers, ref depth);
            }
            var userList = GetUserNames(listGrUsers);
            // Получаем уволенных пользователей
            GetUsersFromGroup(fired1, userList, false);
            GetUsersFromGroup(fired2, userList, false);

            return userList;
        }


        public void GetGroupFromDepart(DirectoryEntry depart, string groupName, ref List<DirectoryEntry> listGrUsers, ref int depth)
        {        
            depth++;
            var depChildList = depart.ChildrenToList();
            if (depChildList.Count > 0)
            {
                var listFound = depChildList.Where(x => x.Name == groupName);
                if (listFound.Count() != 0)
                {
                    listGrUsers.Add(listFound.First());
                }
                if (depart.Name.Contains("промышленной"))
                {
                    var listGrSet = depChildList.Where(x => x.Name == "OU=Группа охраны окружающей среды");
                //    if (listGrSet.Count() > 0)
                //    {
                //        listGrUsers.Add(listGrSet.First());
                //    }
                }

                //if (depth < 100) //Глубина считается неадекватно
                //{
                    int count = 0; 
                    foreach (var dep in depChildList)
                    {                        
                        var d = dep as DirectoryEntry;
                        if (d.Name== "OU=Отдел отопления\\, вентиляции\\, водоснабжения и канализации")
                        {
                            int c = 3;
                        }
                        GetGroupFromDepart(d, groupName, ref listGrUsers, ref depth);
                        count++;
                    }
                //}
            }

        }

        public List<User> GetUserNames(List<DirectoryEntry> listGrUsers)
        {
            List<User> userList = new List<User>();
            List<string> fnList = new List<string>();
            int count = 0;
            foreach (var userGr in listGrUsers)
            {
                if (count == 20)
                {
                    int bb = 1;
                }
                GetUsersFromGroup(userGr, userList, true);
                count++;
            }
            var listOr = fnList.OrderBy(x => x);
            return userList;
        }

        void GetUsersFromGroup(DirectoryEntry userGr, List<User> userList, bool IsActive)
        {
            var firedDepart = context.Departments.First(x => x.Id == 52);
            foreach (var user in userGr.Children)
            {
                var us = user as DirectoryEntry;
                //if (us.Name.Contains("Альберт"))
                //{
                //    StreamWriter sw = new StreamWriter("");
                //    foreach (PropertyValueCollection prop in us.Properties)
                //    {

                //            sw.WriteLine(String.Format("{0} - {1}", prop.PropertyName, prop.Value));

                //    }
                //    sw.Close();
                //}
                var fullName = GetProperty("displayName", us);
                if (!string.IsNullOrEmpty(fullName))
                {
                    if (fullName.Contains("Аркишиев"))
                    {
                        int a = 1;
                        CheckIfActive(us.Properties, IsActive);
                    }
                    var fullNameSplited = fullName.Split(' ').ToList();
                    if (fullNameSplited.Count == 3)
                    {
                        var departName = GetProperty("department", us);
                        var city = GetProperty("l", us);                        
                        var userAd = new User()
                        {
                            LastName = fullNameSplited[0],
                            FirstName = fullNameSplited[1],
                            MiddleName = fullNameSplited[2],
                            AD_GUID = us.NativeGuid,
                            Email = GetProperty("mail", us),
                            Login = GetProperty("sAMAccountName", us),                           
                            FullName = String.Format("{0} {1} {2}", fullNameSplited[0], fullNameSplited[1], fullNameSplited[2]),
                            PhoneNumber = GetProperty("telephoneNumber", us),
                            IsActive = CheckIfActive(us.Properties, IsActive),
                            Position = GetProperty("title", us),                            
                            City = city                            
                        } ;
                        if (userAd.IsActive == true) 
                        {
                            if (city!=null && !departName.Contains(city))
                            {
                                userAd.Department = FindOrCreateDepartment($"{departName} г.{city}");
                            }
                            else
                            {
                                userAd.Department = FindOrCreateDepartment(departName);
                            }

                        }
                        else userAd.Department = firedDepart;
                        userList.Add(userAd);

                    }
                    else if (fullNameSplited.Count == 4 && fullNameSplited[3] == "оглы")
                    {
                        var departName = GetProperty("department", us);
                        var city = GetProperty("l", us);
                        var userAd = new User()
                        {
                            LastName = fullNameSplited[0],
                            FirstName = fullNameSplited[1],
                            MiddleName = fullNameSplited[2]+" "+ fullNameSplited[3],
                            AD_GUID = us.NativeGuid,
                            Email = GetProperty("mail", us),
                            Login = GetProperty("sAMAccountName", us),                            
                            FullName = String.Format("{0} {1} {2} {3}", fullNameSplited[0], fullNameSplited[1], fullNameSplited[2], fullNameSplited[3]),
                            PhoneNumber = GetProperty("telephoneNumber", us),
                            IsActive = CheckIfActive(us.Properties, IsActive),
                            Position = GetProperty("title", us),
                            City = city
                        };
                        if (userAd.IsActive == true)
                        {
                            if (city!=null && !departName.Contains(city))
                            {
                                userAd.Department = FindOrCreateDepartment($"{departName} г.{city}");
                            }
                            else
                            {
                                userAd.Department = FindOrCreateDepartment(departName);
                            }

                        }
                        else userAd.Department = firedDepart;
                        userList.Add(userAd);
                    }                   

                }
            }
        }

        bool CheckIfActive(PropertyCollection propCol, bool IsActive)
        {
            if (IsActive)
            {
                foreach (PropertyValueCollection prop in propCol)
                {
                    if (prop.PropertyName == "userAccountControl")
                    {
                        var IsActiveInt = (int)prop.Value;
                        if (IsActiveInt == 512 || IsActiveInt==66048)
                        {
                            return true;
                        }                       
                    }
                    //if (prop.PropertyName == "msRTCSIP-UserEnabled")
                    //{
                    //    var IsActive = (bool)prop.Value;
                    //    if (IsActive)
                    //    {
                    //        return true;
                    //    }
                    //}
                }
            }
            return false;
        }

        public Department FindOrCreateDepartment(string departmentName)
        {
            var departSet = context.Departments.Where(x => x.Name == departmentName);
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
            int count = 0;
           
            foreach (var userFromAD in usersFromAD)
            {
                if (count == 113)
                {

                }
                if (userFromAD.LastName == "Арсланов")
                {
                    int a = 1;
                }
                var userSet = context.Users.Where(x => x.AD_GUID == userFromAD.AD_GUID);
                if (userSet.Count() > 0)
                {
                    var userFromDB = userSet.First();
                    CompareUpdateAllProps(ref userFromDB, userFromAD);
                }
                else
                {
                    if (userFromAD.IsActive == true)
                    {
                        usersNewList.Add(userFromAD);
                    }
                }
                count++;
            }
        }

        public void CompareUpdateAllProps(ref User userFromDB, User userFromAD)
        {
            if (userFromDB.FirstName != userFromAD.FirstName)
            {
                userFromDB.FirstName = userFromAD.FirstName;
            }
            if (userFromDB.LastName != userFromAD.LastName)
            {
                userFromDB.LastName = userFromAD.LastName;
            }
            if (userFromDB.MiddleName != userFromAD.MiddleName)
            {
                if (userFromDB.LastName!= "Галиуллин")
                userFromDB.MiddleName = userFromAD.MiddleName;
            }

            if (userFromDB.FullName != userFromAD.FullName)
            {
                userFromDB.FullName = userFromAD.FullName;
            }
            if (userFromDB.Department != userFromAD.Department)
            {
                userFromDB.Department = userFromAD.Department;
            }
            if (userFromDB.IsActive != userFromAD.IsActive)
            {
                if (userFromAD.IsActive == false)
                {
                    userFromDB.SetIsNotActive(); //Для 
                }
                else
                {
                    userFromDB.IsActive = userFromAD.IsActive; 
                }
            }
            if (userFromDB.Login != userFromAD.Login)
            {
                userFromDB.Login = userFromAD.Login;
            }
            if (userFromDB.City != userFromAD.City)
            {
                userFromDB.City = userFromAD.City;
            }
            if (userFromDB.Email != userFromAD.Email)
            {
                userFromDB.Email = userFromAD.Email;
            }
            if (userFromDB.Position != userFromAD.Position)
            {
                userFromDB.Position = userFromAD.Position;
            }
            // Остальные свойства      

        }       

        public IActionResult SetHeadOfDepartment()
        {            
            ControllerTemplate.ExecuteCommonFunctions(TempData, ViewData, context, HttpContext);
            var model = Models.User.GetAllActiveUsers(context).GroupBy(x => x.Department);
            ////List<string> successMes = new List<string>();
            //var successAr = TempData["SuccessMes"] as string[];
            //var failAr = TempData["FailMes"] as string[];
            //if (successAr != null)
            //{
            //    ViewData["SuccessMes"] = successAr.ToList();
            //}
            //if (failAr != null)
            //{
            //    ViewData["FailMes"] = failAr.ToList();
            //}

            return View("SetHeadOfDepartment", model);
        }

        public IActionResult UpdateHODPost(List<string> hod, List<string> depart)
        {
            for (int i = 0; i < hod.Count; i++)
            {
                var newHod = Models.User.GetUserByFullNameFromDb(context, hod[i]);
                var department = Department.GetDepartByName(context, depart[i]);
                if (department != null)
                {
                    if (newHod == null)
                    {
                        department.HeadOfDepartment = null;
                    }
                    else department.HeadOfDepartment = newHod;
                }
            }
            TempData["SuccessMes"] = new List<string>() { "Начальники отделов успешно обновлены" };
            context.SaveChanges();
            return RedirectToAction("SetHeadOfDepartment");
        }
            

        public IActionResult UpdateHeadOfDepartmentFromExcel(string excelFileRef)
        {
            var errorMessageList = new List<string>();
            var messageList = new List<string>();
            if (System.IO.File.Exists(excelFileRef))
            {
                string errorMessage = string.Empty;
                ExcelFile excelFile = new ExcelFile(excelFileRef);
                int countChangedRows = excelFile.UpdateHeadOfDepartmentFromExcel(context);
                if (countChangedRows > 0)
                {
                    TempData["SuccessMes"] = new List<string> { $"Успешно обновлено {countChangedRows.ToString()} записей" };
                }

            }
            else
            {
                ExcelFile excelFile = new ExcelFile(excelFileRef);
                excelFile.UploadAllDepartWithHOD(context, errorMessageList);
                if (errorMessageList.Count == 0)
                {
                    messageList.Add($"Файл {excelFileRef} успешно создан");
                }
            }
            TempData["SuccessMes"] = messageList;
            TempData["FailMes"] = errorMessageList;
            /////////////////////////////


            return RedirectToAction("SetHeadOfDepartment");
        }

        public IActionResult SetDaysOff()
        {
            // ControllerTemplate.ExecuteCommonFunctions(TempData, ViewData, context, HttpContext);
            return View("SetDaysOff");
        }

        public IActionResult UpdateDaysOffFromExcel(string excelFileRef)
        {
            var errorMessageList = new List<string>();
            var messageList = new List<string>();
            if (System.IO.File.Exists(excelFileRef))
            {
                string errorMessage = string.Empty;
                ExcelFile excelFile = new ExcelFile(excelFileRef);
                int countChangedRows = excelFile.UploadDaysOffFromExcel(context);
                if (countChangedRows > 0)
                {
                    messageList.Add($"Записано праздничных дней - {countChangedRows.ToString()} ");
                }

            }
            else
            {
                ExcelFile excelFile = new ExcelFile(excelFileRef);
                excelFile.UploadAllDaysOffToExcel(context, errorMessageList);
                if (errorMessageList.Count == 0)
                {
                    messageList.Add($"Файл {excelFileRef} успешно создан");
                }
            }
            TempData["SuccessMes"] = messageList;
            TempData["FailMes"] = errorMessageList;
            /////////////////////////////


            return RedirectToAction("SetDaysOff");
        }

        public IActionResult RefreshUIDInExcelFile()
        {
            var errorMessageList = new List<string>();
            var messageList = new List<string>();
            ExcelFile excelFile = new ExcelFile(FilePath.path);
            excelFile.UpdateUIDInExcelFileFromPrevCreatedTask(context);
            TempData["SuccessMes"] = messageList;
            TempData["FailMes"] = errorMessageList;
            return View("AdminMain");
        }

        public IActionResult UpdateFactWorkLogsAndExecutersInDatabase()
        {
            var errorMessageList = new List<string>();
            var messageList = new List<string>();
            ExcelFile excelFile = new ExcelFile(FilePath.path);
            excelFile.UpdateFactWorkLogsAndExecutersInDatabase(context);
            TempData["SuccessMes"] = messageList;
            TempData["FailMes"] = errorMessageList;
            return View("AdminMain");
        }

        public IActionResult MergeTaskCompSameName()
        {
            var errorMessageList = new List<string>();
            var messageList = new List<string>();
            var taskCompList = context.TaskComps;
            var taskCompDict = new Dictionary<int, string>();
            foreach (var taskComp in taskCompList)
            {
                taskCompDict.Add(taskComp.Id, $"{taskComp.TaskCompName}#{taskComp.ProjectNumber}");
            }
            var taskCompDictGroupByNameSet = taskCompDict.GroupBy(x => x.Value);
            int count = 0;
            foreach (var taskCompDictGr in taskCompDictGroupByNameSet)
            {
                if (taskCompDictGr.Count() > 1)
                {
                    count++;
                    var taskCompDictGrSplited = taskCompDictGr.Key.Split('#');
                    if (taskCompDictGrSplited.Length > 1)
                    {
                        var taskCompName = taskCompDictGrSplited[0];
                        var projectNum = taskCompDictGrSplited[1];
                        var taskCompSet = context.TaskComps.Where(x => x.TaskCompName == taskCompName && x.ProjectNumber==projectNum);
                        ResetWorkLogsAndDelete(taskCompSet);
                    }
                }
            }
            if (count > 0)
            {
                messageList.Add($"Удалено дубликатов - {count.ToString()} ");
            }
            TempData["SuccessMes"] = messageList;
            TempData["FailMes"] = errorMessageList;
            return View("AdminMain");
        }

        public void ResetWorkLogsAndDelete(IQueryable<TaskComp> taskCompSet)
        {
            if (taskCompSet.Count() > 1)
            {
                var taskCompArr = taskCompSet.ToArray();
                if (taskCompArr[0].Id > taskCompArr[1].Id)
                {
                    ResetWorkLogs(taskCompArr[0], taskCompArr[1]);
                    context.TaskComps.Remove(taskCompArr[1]);
                }
                else
                {
                    ResetWorkLogs(taskCompArr[1], taskCompArr[0]);
                    context.TaskComps.Remove(taskCompArr[0]);
                }
            }
            context.SaveChanges();
        }

        public void ResetWorkLogs(TaskComp taskCompReceive, TaskComp taskCompGive)
        {
            var workLogSet = context.WorkLogs.Where(x => x.TaskComp_id == taskCompGive.Id.ToString());
            foreach (var wl in workLogSet)
            {
                wl.TaskComp_id = taskCompReceive.Id.ToString();
            }

            var percentHistorySet = context.TaskCompPercentHistories.Where(x => x.TaskComp.Id == taskCompGive.Id);
            foreach (var ph in percentHistorySet)
            {
                ph.TaskComp = taskCompReceive;
            }
        }

        public IActionResult SetAdminList()
        {
            ControllerTemplate.ExecuteCommonFunctions(TempData, ViewData, context, HttpContext);
            return View("SetAdmins");
        }

        public IActionResult UpdateAdminList(string excelFileRef)
        {
            var errorMessageList = new List<string>();
            var messageList = new List<string>();
            if (System.IO.File.Exists(excelFileRef))
            {
                string errorMessage = string.Empty;
                ExcelFile excelFile = new ExcelFile(excelFileRef);
                int countChangedRows = excelFile.UploadAdmins(context);
                if (countChangedRows > 0)
                {
                    messageList.Add($"Обновлено ролей пользователей - {countChangedRows.ToString()} ");
                }

            }
            else
            {
                ExcelFile excelFile = new ExcelFile(excelFileRef);
                excelFile.UploadAllAdmins(context, errorMessageList);
                if (errorMessageList.Count == 0)
                {
                    messageList.Add($"Файл {excelFileRef} успешно создан");
                }
            }
            TempData["SuccessMes"] = messageList;
            TempData["FailMes"] = errorMessageList;
            /////////////////////////////


            return RedirectToAction("SetAdminList");
        }

        public IActionResult SendMessage()
        {
            var errorMes = new List<string>();
            var res = MailService.SendMessage("nesterovig@oilpro.ru", "Тестовое письмо", "Отказ в создании комплекта, прошу связаться с _____", errorMes);
            if (res)
            {
                TempData["SuccessMes"] = "Письмо успешно отправлено";
            }
            else
            {
                TempData["FailMes"] = errorMes;
            }
            return RedirectToAction("AdminMain");
        }

        public IActionResult SetExecuterByWL()
        {
            var errorMes = new List<string>();
            var res = WorkLogs.SetExecuterByWL(context, errorMes);
            if (res)
            {
                TempData["SuccessMes"] = "Исполнители успешно обновлены";
            }
            else
            {
                TempData["FailMes"] = errorMes;
            }
            return RedirectToAction("AdminMain");
        }

        public IActionResult TotalWLResult(int month, int year)
        {
            if (month == 0)
            {
                month = DateTime.Now.Month;
            }
            if (year == 0)
            {
                year = DateTime.Now.Year;
            }
            List<TotalWorkLog> totalWL = new List<TotalWorkLog>();
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
            var userMyDepartList = context.Users.Include(x => x.Department).Where(x => x.Department.Id == curUser.Department.Id).ToList();
            ViewData["UsersOfMyDepart"] = userMyDepartList;
            ViewData["curUser"] = curUser;

            var userList = Models.User.GetProductionUsers(context).ToList();

            var workLogList = WorkLogs.GetWorkLogsByMonth(context, month, year);
            var wllThisMonthGroupedByUser = workLogList.GroupBy(x => x.User);
            var totalWTShouldBe = WorkLogs.GetTotalWTSHouldBe(context, month, DateTime.Now.Year);
            foreach (var wlGrByUser in wllThisMonthGroupedByUser)
            {
                userList.Remove(wlGrByUser.Key);
                var absenseHours = Absence.GetTotalHoursOfAbsenceCurMonth(context, wlGrByUser.Key);
                var startEndDateAbsence = Absence.GetStartEndDateAbsence(context, wlGrByUser.Key, month, year);
                var tWTSBWOAH = totalWTShouldBe - absenseHours - startEndDateAbsence;
                var totalHours = wlGrByUser.Sum(x => x.WorkTime.TotalHours);
                var user = wlGrByUser.Key;
                var departAcr = user.GetDepartmentAcronym(context);
                totalWL.Add(new TotalWorkLog(departAcr, user, totalHours, tWTSBWOAH));
            }
            //Добавляем пользователей, которые не отчитывались вообще
            foreach (var usAb in userList)
            {
                var departAcr = usAb.GetDepartmentAcronym(context);
                var absenseHours = Absence.GetTotalHoursOfAbsenceCurMonth(context, usAb);
                var startEndDateAbsence = Absence.GetStartEndDateAbsence(context, usAb, month, year);
                var tWTSBWOAH = totalWTShouldBe - absenseHours - startEndDateAbsence;
                totalWL.Add(new TotalWorkLog(departAcr, usAb, 0, tWTSBWOAH));
            }

            var totalWlOrdered = totalWL.OrderBy(x => x.DepartAcr);
            ViewBag.UserListWLAbusers = userList;
            ViewBag.CurMonth = month;
            ViewBag.CurYear = year;
            ViewBag.CurMonthStr = FilePath.GetMonth(month);
            //IEnumerable <IGrouping<string, PrimaTask>> 
            ViewData["curPage"] = 0;
            return View("TotalWorkLogResult", totalWlOrdered);
        }

        public IActionResult ExportReportToExcel(int month, int year)
        {
            if (month == 0)
            {
                month = DateTime.Now.Month;
            }
            if (year == 0)
            {
                year = DateTime.Now.Year;
            }

            List<TotalWorkLog> totalWL = new List<TotalWorkLog>();
            var curUser = WebApplication5.Models.User.GetUser(context, HttpContext);
            if (curUser != null)
            {
                ViewData["IsHOD"] = curUser.isHeadOfDepartment(context);
                ViewData["IsAdmin"] = curUser.IsAdmin(context);
            }
            var userMyDepartList = context.Users.Include(x => x.Department).Where(x => x.Department.Id == curUser.Department.Id).ToList();
            ViewData["UsersOfMyDepart"] = userMyDepartList;
            ViewData["curUser"] = curUser;
            var userList = Models.User.GetProductionUsersWOHOD(context).ToList();
            var workLogList = WorkLogs.GetWorkLogsByMonth(context, month, year);
            var wllThisMonthGroupedByUser = workLogList.GroupBy(x => x.User);
            var totalWTShouldBe = WorkLogs.GetTotalWTSHouldBe(context, month, year);           
            foreach (var wlGrByUser in wllThisMonthGroupedByUser)
            {
                if (wlGrByUser.Key.Id == 32)
                {

                }
                var user = wlGrByUser.Key;
                userList.Remove(user);                
                var absenseHours = Absence.GetTotalHoursOfAbsenceExactMonth(context, wlGrByUser.Key, month, year);
                var startDateAbsence = Absence.GetStartEndDateAbsence(context, wlGrByUser.Key, month, year);
                var tWTSBWOAH = totalWTShouldBe - absenseHours-startDateAbsence;
                if (user.Rate != null)
                    tWTSBWOAH *= user.Rate.Value; // Ставка сотрудника
                var totalHours = wlGrByUser.Sum(x => x.WorkTime.TotalHours);
                
                var departAcr = user.GetDepartmentAcronym(context);
                totalWL.Add(new TotalWorkLog(departAcr, user, totalHours, tWTSBWOAH));
            }
            //Добавляем пользователей, которые не отчитывались вообще
            foreach (var usAb in userList)
            {
                var absenseHours = Absence.GetTotalHoursOfAbsenceExactMonth(context, usAb, month, year);
                var startEndDateAbsence = Absence.GetStartEndDateAbsence(context, usAb, month, year);
                var departAcr = usAb.GetDepartmentAcronym(context);
                var tWTSBWOAH = totalWTShouldBe - absenseHours - startEndDateAbsence;
                if (usAb.Rate != null)
                    tWTSBWOAH *= usAb.Rate.Value; // Ставка сотрудника
                totalWL.Add(new TotalWorkLog(departAcr, usAb, 0, tWTSBWOAH));
            }
            var totalWlOrdered = totalWL.OrderBy(x => x.DepartAcr).ThenBy(y => y.User.LastName);
            var dDate = new DateTime(year, month, 1);
            var filePath = FilePath.GetPathForBuhMain(dDate, "Доли_проектов");
            ExcelFile exFile = new ExcelFile(filePath);
            exFile.UploadWorkLogsTotalReport(totalWlOrdered, context);
            ViewBag.UserListWLAbusers = userList;
            ViewBag.CurMonth = month;
            ViewBag.CurYear = year;
            ViewBag.CurMonthStr = FilePath.GetMonth(month);
            //IEnumerable <IGrouping<string, PrimaTask>> 
            ViewData["curPage"] = 0;
            List<string> successMesList = new List<string>();
            if (TempData["SuccessMes"] != null)
            {
                var successAr = TempData["SuccessMes"] as string[];
                successMesList.AddRange(successAr);
            }
            successMesList.Add($"Файл успешно выгружен по след. пути: {filePath}");
            ViewData["SuccessMes"] = successMesList;
            return View("TotalWorkLogResult", totalWlOrdered);
        }

        public IActionResult CheckUserDepartment(string fullName)
        {
            fullName = fullName.Trim();
            var user = WebApplication5.Models.User.GetUserByFullName(context, fullName);
            if (user != null)
            {
                TempData["UserDepartAcronym"] = user.GetDepartmentAcronym(context);
                TempData["UserFullName"] = fullName;
            }
            return RedirectToAction("AdminMain");
        }

        public IActionResult CheckTaskComp(string taskCompName)
        {
            taskCompName = taskCompName.Trim();
            var taskComp = WebApplication5.Models.TaskComp.GetTaskCompByName(taskCompName, context);
            if (taskComp != null)
            {
                TempData["taskCompInfo"] = $"Комплект {taskComp.TaskCompName} существует на портале.\n Проект {taskComp.ProjectNumber} \n Отв. отдел {taskComp.Department}";                 
            }
            else
            {
                TempData["taskCompInfo"] = $"Комплект {taskCompName} НЕ существует на портале";
            }
            return RedirectToAction("AdminMain");
        }

        public IActionResult TotalAbsenceRes(int month, int year)
        {
            if (month == 0)
            {
                month = DateTime.Now.Month;
            }
            if (year == 0)
            {
                year = DateTime.Now.Year;
            }
            var absenseSet = Absence.GetAbsenceSelMonth(month, year, context);
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
            var userMyDepartList = context.Users.Include(x => x.Department).Where(x => x.Department.Id == curUser.Department.Id).ToList();
            ViewData["UsersOfMyDepart"] = userMyDepartList;
            ViewData["curUser"] = curUser;                   
            
            ViewBag.CurMonth = month;
            ViewBag.CurYear = year;
            ViewBag.CurMonthStr = FilePath.GetMonth(month);
            //IEnumerable <IGrouping<string, PrimaTask>> 
            ViewData["curPage"] = 0;
            return View("TotalAbsenceRes", absenseSet);
        }               

               public IActionResult ExportAbsenceReportToExcel(int month, int year)
        {
            if (month == 0)
            {
                month = DateTime.Now.Month;
            }
            if (year == 0)
            {
                year = DateTime.Now.Year;
            }

            var absenseSet = Absence.GetAbsenceSelMonth(month, year, context);
            var curUser = WebApplication5.Models.User.GetUser(context, HttpContext);
            if (curUser != null)
            {
                ViewData["IsHOD"] = curUser.isHeadOfDepartment(context);
                ViewData["IsAdmin"] = curUser.IsAdmin(context);
            }
            var userMyDepartList = context.Users.Include(x => x.Department).Where(x => x.Department.Id == curUser.Department.Id).ToList();
            ViewData["UsersOfMyDepart"] = userMyDepartList;
            ViewData["curUser"] = curUser;         
           
            var dDate = new DateTime(year, month, 1);
            var filePath = FilePath.GetPathForBuhMain(dDate, "Отсутствия");
            ExcelFile exFile = new ExcelFile(filePath);
            exFile.UploadAbsenceTotalReport(absenseSet , context);           
            ViewBag.CurMonth = month;
            ViewBag.CurYear = year;
            ViewBag.CurMonthStr = FilePath.GetMonth(month);
            //IEnumerable <IGrouping<string, PrimaTask>> 
            ViewData["curPage"] = 0;
            List<string> successMesList = new List<string>();
            if (TempData["SuccessMes"] != null)
            {
                var successAr = TempData["SuccessMes"] as string[];
                successMesList.AddRange(successAr);
            }
            successMesList.Add($"Файл успешно выгружен по след. пути: {filePath}");
            ViewData["SuccessMes"] = successMesList;
            return View("TotalAbsenceRes", absenseSet);
        }

        public IActionResult MatchTaskCompNameWithUIDFromDb()
        {
            Workbook wb = new Workbook(@"\\srv-ws\Задачи\Backups\Детальный план_факт НХП_22-11-2024_BACK_UP.xlsx");
            var ws = wb.Worksheets["Справочник работ-комплектов"];
            int i = 1;
            var style = ws.Cells[i, 4].GetStyle();
            style.ForegroundColor = System.Drawing.Color.Azure;
            style.Pattern = BackgroundType.Solid;
            while (true)
            {
                var taskCompName = ws.Cells[i, 4].StringValue;
                var taskCompNameNext = ws.Cells[i+1, 4].StringValue;
                var taskCompNameNextNext = ws.Cells[i + 2, 4].StringValue;
                if (string.IsNullOrEmpty(taskCompName) && string.IsNullOrEmpty(taskCompNameNext) && string.IsNullOrEmpty(taskCompNameNextNext))
                {
                    break;
                }
                var taskCompSet = context.TaskComps.Where(x => x.TaskCompName == taskCompName);
                if (taskCompSet.Count() > 0)
                {
                    ws.Cells[i, 49].Value = taskCompSet.First().UID;
                    ws.Cells[i, 49].SetStyle(style);
                }
                i++;
            }
            ViewData["SuccessMes"] = new List<string>() {  $"UID успешно обновлены в файле excel, кол-во обработанных строк {i-1}" };
            wb.Save(@"\\SRV-ws\Задачи\Backups\Детальный план_факт НХП_22-11-2024_BACKUP_NEW.xlsx");
            return RedirectToAction("AdminMain");
        }

        public IActionResult RefreshWorkLogsProjIdFromTaskComps()
        {
            int count = 0;
            List<WorkLogs> wlList = new List<WorkLogs>();
            List<string> newProjId = new List<string>();
            foreach (var wl in context.WorkLogs)
            {
                var res = Int32.TryParse(wl.TaskComp_id, out int taskCompId);
                if (res)
                {
                    var taskCompSet = context.TaskComps.Where(x => x.Id == taskCompId);
                    if (taskCompSet.Count() > 0)
                    {
                        var projectNum = taskCompSet.First().ProjectNumber;
                        if (wl.Proj_id != projectNum)
                        {                            
                            wlList.Add(wl);
                            newProjId.Add(projectNum);
                            count++;
                        }
                    }
                }

            }
            for (int i = 0; i < wlList.Count; i++)
            {
                wlList[i].Proj_id = newProjId[i];
            }
            context.SaveChanges();
            return RedirectToAction("AdminMain");
        }


        public IActionResult ResetWorkLogsForNullUsers()
        {
            var userSet = context.Users.Where(x => x.AD_GUID == null);
            List<WorkLogs> wlForReset = new List<WorkLogs>();
            List<Absence> absForReset = new List<Absence>();
            List<TaskCompRequest> tcrForReset = new List<TaskCompRequest>();
            List<TaskCompPercentHistory> tcphForReset = new List<TaskCompPercentHistory>();
            foreach (var user in userSet)
            {
                wlForReset.AddRange(context.WorkLogs.Where(x => x.User.Id == user.Id));
                absForReset.AddRange(context.Absences.Where(x => x.User.Id == user.Id));
                tcrForReset.AddRange(context.TaskCompRequests.Where(x => x.User.Id == user.Id));
                tcphForReset.AddRange(context.TaskCompPercentHistories.Where(x => x.User.Id == user.Id));
                var userSetReal = context.Users.Where(x => x.Login == user.FullName && x.FirstName != null);
                User userReal = null;
                if (userSetReal.Count() > 0) 
                {
                    userReal = userSetReal.First();
                    foreach (var wl in wlForReset)
                    {
                        wl.User = userReal;
                    }
                    foreach (var abs in absForReset)
                    {
                        abs.User = userReal;
                    }
                    foreach (var tcr in tcrForReset)
                    {
                        tcr.User = userReal;
                    }
                    foreach (var tcph in tcphForReset)
                    {
                        tcph.User = userReal;
                    }
                }
               
            }    
            

            foreach (var user1 in userSet)
            {
                context.Users.Remove(user1);
            }

            context.SaveChanges();
            TempData["SuccessMes"] = new List<string>() { $"Работы успешно переназначены. Кол-во удалённых пользователей {userSet.Count()}" };
            return RedirectToAction("AdminMain");
        }

        public IActionResult DeleteDepartWithoutUsers() 
        {
            List<Department> departmentsForDel = new List<Department>();
            var departUserList = context.Users.Select(x=>x.Department).Distinct().ToList();
            foreach (var depart in context.Departments)
            {
                if (!departUserList.Contains(depart))
                {
                    departmentsForDel.Add(depart);
                }
            }
            var departForDelStr = string.Join(", \n", departmentsForDel.Select(x => x.Name + " " +x.Id.ToString() ));

            //Удаление отделов
            foreach (var departForDel in departmentsForDel)
            {
                context.Departments.Remove(departForDel);
            }
            context.SaveChanges();

            TempData["SuccessMes"] = new List<string>() { $"Найдены след. отделы для удаления: {departForDelStr}" };
            return RedirectToAction("AdminMain");
        }

        public IActionResult SetFiredDepartToNotActiveUsers() //НЕ ИСПОЛЬЗУЕТСЯ ИЗ ВЕБ
        {
            var firedDepart= context.Departments.First(x => x.Id == 52);
            var firedUsersSet= context.Users.Where(x =>x.IsActive==null || x.IsActive == false);
            foreach(var userFired in firedUsersSet)
            {
                userFired.Department = firedDepart;
            }
            context.SaveChanges();
            return RedirectToAction("AdminMain");
        }

            public IActionResult RenameDepartmnets() //НЕ ИСПОЛЬЗУЕТСЯ ИЗ ВЕБ
        {
            List<string> departWithDiffrentCities = new List<string>();
            var users= Models.User.GetAllActiveUsers(context); 
            foreach (var depart in context.Departments)
            {
                var citySet = users.Where(x => x.Department.Id == depart.Id).Select(x=>x.City);
                List<CityCount> cityCountList = new List<CityCount>();
                foreach (var city in citySet)
                {
                    if (cityCountList.Where(x=>x.CityName==city).Count()>0)
                    {
                        var cityCount = cityCountList.First(x => x.CityName == city);
                        cityCount.Amount++;
                    }
                    else
                    {
                        cityCountList.Add(new CityCount() { CityName = city, Amount = 1 });
                    }
                }

                var citySetF= cityCountList.Where(x => x.Amount == cityCountList.Max(y => y.Amount)).Select(x => x.CityName).Distinct();
                if (citySetF.Count() == 1 && citySetF.First()!=null)
                {                    
                    var city = citySetF.First();
                    if (!depart.Name.Contains(city))
                    {
                        depart.Name = $"{depart.Name} г.{city}";
                        depart.City = city;
                    }
                    else
                    {
                        depart.City = city;
                    }
                }
                else
                {
                    departWithDiffrentCities.Add(depart.Name);
                }
            }
            var departWithDiffrentCitiesStr = string.Join(", ", departWithDiffrentCities);
            TempData["SuccessMes"] = new List<string>() { $"Наименование отделов успешно обновлены. Отделы с различными городами у пользователей\n {departWithDiffrentCitiesStr}" };
            context.SaveChanges();
            return RedirectToAction("SetHeadOfDepartment");
        }

        public IActionResult ExportHODToExcel()
        {
            var errorList = new List<string>();
            var date = DateTime.Now;            
            var filePath =$"\\\\srv-ws\\Общая\\Отделы_НХП{date.Year}-{date.Month}-{date.Day}_{date.Hour}-{date.Minute}-{date.Second}.xlsx";
            ExcelFile excelFile = new ExcelFile(filePath);
            excelFile.ExportHODToExcel(context, errorList);
            TempData["SuccessMes"] = new List<string>() { $"Файл успешно выгружен по след. пути: {filePath}" };
            TempData["FailMes"] = errorList;
            return RedirectToAction("SetHeadOfDepartment");
        }

        public IActionResult FillHODAuto()
        {
            int count = 0;
            var departments = context.Departments.Include(x => x.HeadOfDepartment).Where(x => x.HeadOfDepartment == null);
            foreach (var depart in departments)
            {
                var hodSet = context.Users.Where(x => x.Department.Id == depart.Id);
                var hodSetFiltered = hodSet.Where(x => x.Position.ToLower().Contains("начальник"));
                if (hodSetFiltered.Count() == 0)
                {
                    hodSetFiltered = hodSet.Where(x => x.Position.ToLower().Contains("руководитель"));                    
                }
                if (hodSetFiltered.Count() > 0)
                {
                    depart.HeadOfDepartment = hodSetFiltered.First();
                    count++;
                }
            }
            context.SaveChanges();

            //Обновление свойства User.IsHeadOfDepartment
            var hodForUpdateSet = context.Departments.Include(x => x.HeadOfDepartment).Where(x => x.HeadOfDepartment != null).Select(x => x.HeadOfDepartment).Distinct();
            foreach (var hod in hodForUpdateSet)
            {
                hod.IsHeadOfDepartment = true;
            }
            context.SaveChanges();

            TempData["SuccessMes"] = new List<string>() { $"Отделы с начальниками отделов не переназначаются, для это воспользуйтесь таблицей ниже. " +
                $"Кол-во назначенных начальников отделов: {count}"};
            return RedirectToAction("SetHeadOfDepartment");
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