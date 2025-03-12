using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication5.Models;
using WebApplication5.Models.ExcelFiles;
using System.Web;
using System.DirectoryServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;
using Aspose.Cells;
using System.Threading;

namespace WebApplication5.Controllers
{
    public class ImportCompController : Controller
    {
        AppDbContext context;
        public ImportCompController(AppDbContext appDbContext, IHostingEnvironment appEnv)
        {
            context = appDbContext;
        }

        public enum ShowTaskCompCond
        {
            All,
            MyDepartCurMonth,
            MyDepartAllTasks
        }

        [HttpGet]
        public ViewResult Index(string showTaskCompCond)
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
                //List<string> successMes = new List<string>();
                var failAr = TempData["FailMes"] as string[];
                if (failAr != null)
                {
                    ViewData["FailMes"] = failAr.ToList();
                }
            }

            List<TaskComp> taskSet = new List<TaskComp>();
            var curUser = WebApplication5.Models.User.GetUser(context, HttpContext);

            ////Замена пользователей
            //if (curUser.Id==263)
            //{
            //    curUser = context.Users.First(x => x.Id == 355); // На вкладке importComplect пользователь Беликов Т.П. вместо меня (Нестеров И.Г.)
            //}
            ////Заменяем Халитова на Гильмутдинова
            //if (curUser.Id == 366)
            //{
            //    curUser = context.Users.First(x => x.Id == 360);
            //}

            if (curUser != null)
            {
                ViewData["IsHOD"] = curUser.isHeadOfDepartment(context);
                ViewData["IsAdmin"] = curUser.IsAdmin(context);
                ViewData["IsGIP"] = curUser.IsGIP;
            }
            ViewData["curUser"] = curUser;
            var departAcronym = curUser.GetDepartmentAcronym(context);
            if (curUser.IsGIP == true)
            {
                //if (string.IsNullOrEmpty(showTaskCompCond) || showTaskCompCond == "2")
                //{
                    //Все комплекты моего отдела за тек. месяц
                    //taskSet = TaskComp.GetAllTasksForGIPCurMonth(curUser, context);
                    //ViewData["ShowTaskCompCond"] = "2";
                    //ViewData["CurProjects"] = Project.GetProjectsFromTaskSet(taskSet); //фильтрация проектов, у кот. задач больше двух (Командировка и Работа Нач. Отдела)
                //}
                //else
                //{
                    taskSet = TaskComp.GetAllTasksForGIP(curUser, context);
                    TaskComp.RemoveStandartTasks(taskSet);
                    ViewData["ShowTaskCompCond"] = "0";
                    ViewData["CurProjects"] = Project.GetProjectsFromTaskSet(taskSet);
               // }

            }
            else
            {
                if (string.IsNullOrEmpty(showTaskCompCond) || showTaskCompCond == "2")
                {
                    //Все комплекты моего отдела за тек. месяц
                    taskSet = TaskComp.GetAllTasksForMyDepartmentCurMonth(curUser, context);
                    ViewData["ShowTaskCompCond"] = "2";
                }
                else if (showTaskCompCond == "1")
                {
                    //Все комплекты моего отдела
                    taskSet = TaskComp.GetAllTasksForMyDepartment(curUser, context).ToList();
                    TaskComp.RemoveStandartTasks(taskSet);
                    ViewData["ShowTaskCompCond"] = "1";
                }
                else
                {
                    //Все комплекты
                    taskSet = TaskComp.GetAllTasks(context);
                    TaskComp.RemoveStandartTasks(taskSet);
                    ViewData["ShowTaskCompCond"] = "0";
                }
            }

            List<TaskComp> taskComps = new List<TaskComp>();
            foreach (var taskComp in taskSet)
            {
                taskComp.FactWorkLog = WorkLogs.GetTotalHoursOnTask(taskComp, context, out DateTime? startFactDate);
                taskComp.StartFactDate = startFactDate;                
                taskComps.Add(taskComp);
            }
            
            ViewData["curPage"] = 4;
            ViewData["tcReqCount"] = Models.TaskCompRequest.GetAmountOfNewTaskCompRequestStr(context);
            ViewData["isKSP"] = curUser.IsKSP(context);
            ViewData["curUserId"] = curUser.Id.ToString();
            return View(taskComps);
        }

        //Вызвать метод без кнопки
        //http://localhost:89/ImportComp/ImportCompFromExcelFULL//GetData?excelFileRef=\\srv-ws\%D0%97%D0%B0%D0%B4%D0%B0%D1%87%D0%B8\Backups\7.xlsx
        public IActionResult ImportCompFromExcelFULL(string excelFileRef)
        {
            var errorMessageList = new List<string>();
            var messageList = new List<string>();
            if (System.IO.File.Exists(excelFileRef))
            {
                string errorMessage = string.Empty;
                ExcelFile excelFile = new ExcelFile(excelFileRef);
                // Создание новых комплектов
                excelFile.GetComplects(ref errorMessage, context); //Commit migration               
                if (string.IsNullOrEmpty(errorMessage))
                {
                    if (excelFile.TaskComps != null && excelFile.TaskComps.Count > 0)
                    {
                        context.TaskComps.AddRange(excelFile.TaskComps);
                        context.SaveChanges();
                        excelFile.WriteUidBackToExcelFile();
                        messageList.Add($"Успешно импортировано {excelFile.TaskComps.Count} Работ/Комплектов");
                    }
                    // Обновление
                    excelFile. UpdateComplects(context, out int counterChanges, out int rowChangeCounter);
                    if (counterChanges > 0)
                    {
                        messageList.Add($"Успешно обновлено. Кол-во строк с изменениями : {rowChangeCounter}");
                    }
                    //Удаление строк помеченных красным
                    int counterDeleted = 0;
                    excelFile.DeleteComplects(context, ref counterDeleted);
                    if (counterDeleted > 0)
                    {
                        messageList.Add($"Успешно удалено {counterDeleted} строк.");
                    }

                    //Запись собранных данных 
                    excelFile.WriteDataBackToExcelFile(context);
                    excelFile.DeleteEvaluationWarnings();

                    //Проверка и создание работ по каждому проекту "работа начальника отдела", "командировка"
                    Project.CheckAllProjectsForInternalTasks(context);

                    //Проверка наличия TaskCompEdu
                    TaskComp.CheckTaskCompEdu(context);

                    messageList.Add($"Файл {excelFileRef} успешно обновлен");
                }
                else
                {
                    errorMessageList.Add(errorMessage);
                    TempData["FailMes"] = errorMessageList;
                }
            }
            else
            {
               ExcelFile excelFile = new ExcelFile(excelFileRef);
                excelFile.UploadAllTaskComp(context);
                messageList.Add($"Файл {excelFileRef} успешно создан");
            }
            TempData["SuccessMes"] = messageList;
            return RedirectToAction("Index");
        }

        //МЕТОД ИМПОРТА
        [HttpPost]
        public IActionResult ImportCompFromExcel([FromBody] string[] dataArray)
        {
            var excelFileRef = dataArray[0];
            var errorMessageList = new List<string>();
            var messageList = new List<string>();
            if (System.IO.File.Exists(excelFileRef))
            {
                string errorMessage = string.Empty;
                ExcelFile excelFile = new ExcelFile(excelFileRef);
                // Создание новых комплектов
                excelFile.GetComplects(ref errorMessage, context); //Commit migration               
                if (string.IsNullOrEmpty(errorMessage))
                {
                    if (excelFile.TaskComps != null && excelFile.TaskComps.Count > 0)
                    {
                        context.TaskComps.AddRange(excelFile.TaskComps);
                        context.SaveChanges();
                        excelFile.WriteUidBackToExcelFile();
                        messageList.Add($"Успешно импортировано {excelFile.TaskComps.Count} Работ/Комплектов");
                    }
                    // Обновление
                    excelFile.UpdateComplects(context, out int counterChanges, out int rowChangeCounter);
                    if (counterChanges > 0)
                    {
                        messageList.Add($"Успешно обновлено. Кол-во строк с изменениями : {rowChangeCounter}");
                    }
                    //Удаление строк помеченных красным
                    int counterDeleted = 0;
                    excelFile.DeleteComplects(context, ref counterDeleted);
                    excelFile.DeleteEvaluationWarnings();
                    if (counterDeleted > 0)
                    {
                        messageList.Add($"Успешно удалено {counterDeleted} строк.");
                    }
                    if (messageList.Count == 0)
                    {
                        messageList.Add($"Успешно. Данных для создания, обновления и удаления не обнаружено.");
                    }

                    //Проверка и создание работ по каждому проекту "работа начальника отдела", "командировка"
                    Project.CheckAllProjectsForInternalTasks(context);

                    //Проверка наличия TaskCompEdu
                    TaskComp.CheckTaskCompEdu(context);
                }
                else
                {
                    errorMessageList.Add(errorMessage);
                    TempData["FailMes"] = errorMessageList;
                    var errMesBool = new { mes = errorMessage, isError = true };
                    return new JsonResult(errMesBool);
                }
            }
            else
            {
                ExcelFile excelFile = new ExcelFile(excelFileRef);
                excelFile.UploadAllTaskComp(context);
                messageList.Add($"Файл {excelFileRef} успешно создан");
            }
            //Thread.Sleep(5000);
            TempData["SuccessMes"] = messageList;
            // return new JsonResult($"Задачи успешно импортированы! Время завершения:{DateTime.Now.ToShortTimeString()}.");
            var successMes = string.Join('\n', messageList.ToArray());
           var successMesBool = new { mes = successMes, isError = false };
            return new JsonResult(successMesBool);
        }

        [HttpPost]
        public IActionResult RefreshDataInExcelFile([FromBody] string[] dataArray)
        {
            var excelFileRef = dataArray[0];
            var errorMessageList = new List<string>();
            var messageList = new List<string>();
            if (System.IO.File.Exists(excelFileRef))
            {
                string errorMessage = string.Empty;
                ExcelFile excelFile = new ExcelFile(excelFileRef, true, ref errorMessage);
                // Создание новых комплектов
                //excelFile.GetComplects(ref errorMessage, context); //Commit migration

                if (string.IsNullOrEmpty(errorMessage))
                {

                    //Запись собранных данных 
                    excelFile.WriteDataBackToExcelFile(context, ref errorMessage);
                    //Thread.Sleep(5000);

                    excelFile.DeleteEvaluationWarnings();
                                      

                    messageList.Add($"Файл {excelFileRef} успешно обновлен");
                }
                else
                {
                    errorMessageList.Add(errorMessage);
                    TempData["FailMes"] = errorMessageList;
                }
            }
            else
            {
                ExcelFile excelFile = new ExcelFile(excelFileRef);
                excelFile.UploadAllTaskComp(context);
                messageList.Add($"Файл {excelFileRef} успешно создан");
            }
            TempData["SuccessMes"] = messageList;
            return new JsonResult($"Файл успешно обновлен! Время завершения:{DateTime.Now.ToShortTimeString()}."); //PartialView("PartialInformation", ViewData["SuccessMes"]);
        }

        [HttpPost]
        public IActionResult RefreshHODAndGIP([FromBody] string[] dataArray)
        {
            var excelFileRef = dataArray[0];
            var errorMessageList = new List<string>();
            var messageList = new List<string>();
            if (System.IO.File.Exists(excelFileRef))
            {
                string errorMessage = string.Empty;
                ExcelFile excelFile = new ExcelFile(excelFileRef, true, ref errorMessage);
                // Создание новых комплектов
                //excelFile.GetComplects(ref errorMessage, context); //Commit migration

                if (string.IsNullOrEmpty(errorMessage))
                {                                                            
                    excelFile.UpdateHODAndGIP(context, out int counterChanges, out int rowChangeCounter);
                    if (counterChanges > 0)
                    {
                        messageList.Add($"Успешно обновлено. Кол-во строк с изменениями : {rowChangeCounter}");
                    }
                    if (messageList.Count == 0)
                    {
                        messageList.Add($"Успешно. Данных для создания, обновления и удаления не обнаружено.");
                    }
                    //Thread.Sleep(5000);

                    excelFile.DeleteEvaluationWarnings();


                   
                }
                else
                {
                    errorMessageList.Add(errorMessage);
                    TempData["FailMes"] = errorMessageList;
                }
            }
            else
            {
                ExcelFile excelFile = new ExcelFile(excelFileRef);
                excelFile.UploadAllTaskComp(context);
                messageList.Add($"Файл {excelFileRef} успешно создан");
            }
            TempData["SuccessMes"] = messageList;
            return new JsonResult($"Файл успешно обновлен! Время завершения:{DateTime.Now.ToShortTimeString()}."); //PartialView("PartialInformation", ViewData["SuccessMes"]);
        }

        //public IActionResult RefreshDataInExcelFile(string excelFileRef)
        //{
        //    var errorMessageList = new List<string>();
        //    var messageList = new List<string>();
        //    if (System.IO.File.Exists(excelFileRef))
        //    {
        //        string errorMessage = string.Empty;
        //        ExcelFile excelFile = new ExcelFile(excelFileRef, true, ref errorMessage);
        //        // Создание новых комплектов
        //        //excelFile.GetComplects(ref errorMessage, context); //Commit migration

        //        if (string.IsNullOrEmpty(errorMessage))
        //        {

        //            //Запись собранных данных 
        //            excelFile.WriteDataBackToExcelFile(context, ref errorMessage);

        //            excelFile.DeleteEvaluationWarnings();

        //            //Проверка и создание работ по каждому проекту "работа начальника отдела", "командировка"
        //            Project.CheckAllProjectsForInternalTasks(context);

        //            //Проверка наличия TaskCompEdu
        //            TaskComp.CheckTaskCompEdu(context);

        //            messageList.Add($"Файл {excelFileRef} успешно обновлен");
        //        }
        //        else
        //        {
        //            errorMessageList.Add(errorMessage);
        //            TempData["FailMes"] = errorMessageList;
        //        }
        //    }
        //    else
        //    {
        //        ExcelFile excelFile = new ExcelFile(excelFileRef);
        //        excelFile.UploadAllTaskComp(context);
        //        messageList.Add($"Файл {excelFileRef} успешно создан");
        //    }
        //    TempData["SuccessMes"] = messageList;
        //    return RedirectToAction("Index");
        //}

        [HttpGet]
        public ViewResult TaskCompRequest()
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
                //List<string> successMes = new List<string>();
                var failAr = TempData["FailMes"] as string[];
                if (failAr != null)
                {
                    ViewData["FailMes"] = failAr.ToList();
                }
            }
                        
            var curUser = WebApplication5.Models.User.GetUser(context, HttpContext);
            if (curUser != null)
            {
                ViewData["IsHOD"] = curUser.isHeadOfDepartment(context);
                ViewData["IsAdmin"] = curUser.IsAdmin(context);
            }
            ViewData["curUser"] = curUser;
            ViewData["CurProjects"] = Project.GetAllProjects(context);
            //ViewData["curPage"] = 4;
            ViewData["tcReqCount"] = Models.TaskCompRequest.GetAmountOfNewTaskCompRequestStr(context);
            ViewData["isKSP"] = curUser.IsKSP(context);
            return View();
        }

        public IActionResult SendTaskCompRequest(string project, string taskCompName, string startDate, string finishDate, string workLogPlan, string comment)
        {
            User curUser = WebApplication5.Models.User.GetUser(context, HttpContext);
            var errorMessageList = new List<string>();
            var messageList = new List<string>();
            if (string.IsNullOrEmpty(taskCompName))
            {
                string errorMes = $"Необходимо задать имя комплекта/работы";
                errorMessageList.Add(errorMes);
                TempData["FailMes"] = errorMessageList;
                return RedirectToAction("TaskCompRequest");
            }
            TaskComp taskComp = TaskComp.GetTaskCompByName(taskCompName, context);
            var curProjects = Project.GetAllProjects(context);
            TaskCompRequest tcr = Models.TaskCompRequest.FindTaskCompRequestByName(context, taskCompName);
            if (curProjects.Contains(project))
            {
                if (taskComp != null)
                {
                    string errorMes = $"Данный комплект {taskComp.TaskCompName} по проекту {taskComp.ProjectNumber} уже существует в системе. Попробуйте " +
                        $"найти его во вкладке \"Мои задачи\", либо обратитесь к администратору  системы.";
                    errorMessageList.Add(errorMes);
                    TempData["FailMes"] = errorMessageList;
                }
                else if (tcr != null)
                {
                    string errorMes = $"По комплекту {taskCompName} (проект {project}) уже отправлен запрос на создание.";
                    errorMessageList.Add(errorMes);
                    TempData["FailMes"] = errorMessageList;
                }
                else
                {
                    
                    var startDateDate = StringConverter.GetDate(startDate);
                    var finishDateDate = StringConverter.GetDate(finishDate);
                    var workLogPlanDouble = StringConverter.GetDouble(workLogPlan);
                    var newTCR = new TaskCompRequest(project, taskCompName, startDateDate, finishDateDate, workLogPlanDouble, comment, curUser);
                    context.TaskCompRequests.Add(newTCR);
                    var userAr = Models.User.GetUserKsp(context);
                    MailService.NotifyKspUsers(newTCR, curUser, userAr, errorMessageList);
                    context.SaveChanges();
                    TempData["SuccessMes"] = new List<string>() { $"Запрос на комплект/работу {taskCompName} по проекту {project} успешно создан." };
                }
            }
            else
            {
                string errorMes = $"Данного проекта {project} не существует.";
                errorMessageList.Add(errorMes);
                TempData["FailMes"] = errorMessageList;
            }
            return RedirectToAction("TaskCompRequest");
        }

        public IActionResult TaskCompReqApply(string tcrCond)
        {
            if(TempData["SuccessMes"] != null)
            {
                //List<string> successMes = new List<string>();
                var successAr = TempData["SuccessMes"] as string[];
                if (successAr != null)
                {
                    if (successAr != null)
                    {
                        ViewData["SuccessMes"] = successAr.ToList();
                    }
                }
            }
            if (TempData["FailMes"] != null)
            {
                //List<string> successMes = new List<string>();
                var failAr = TempData["FailMes"] as string[];
                if (failAr != null)
                {
                    ViewData["FailMes"] = failAr.ToList();
                }
            }
            if (TempData["filePath"] != null)
            {
                //List<string> successMes = new List<string>();
                var filePath = TempData["filePath"] as string;
                if (!string.IsNullOrEmpty(filePath))
                {
                    ViewData["filePath"] = filePath;
                }
            }
            User curUser = WebApplication5.Models.User.GetUser(context, HttpContext);
            var errorMessageList = new List<string>();
            TaskCompRequest[] tcrAr;         
            if (curUser != null)
            {
                ViewData["IsHOD"] = curUser.isHeadOfDepartment(context);
                ViewData["IsAdmin"] = curUser.IsAdmin(context);
            }
            ViewData["curUser"] = curUser;

            if (string.IsNullOrEmpty(tcrCond) || tcrCond == "0")
            {
                //Запросы на работы "Новые"
                tcrAr = WebApplication5.Models.TaskCompRequest.GetTaskCompRequestByStatus(context, Models.TaskCompRequest.Status.New);
                ViewData["tcrCond"] = "0";
            }
            else if (tcrCond == "1")
            {
                //Все на работы "Прочитанные"
                tcrAr = WebApplication5.Models.TaskCompRequest.GetTaskCompRequestByStatus(context, Models.TaskCompRequest.Status.Read);
                ViewData["tcrCond"] = "1";
            }
            else if (tcrCond == "2")
            {
                //Все на работы "Прочитанные"
                tcrAr = WebApplication5.Models.TaskCompRequest.GetTaskCompRequestByStatus(context, Models.TaskCompRequest.Status.Confirmed);
                ViewData["tcrCond"] = "2";
            }
            else if (tcrCond == "3")
            {
                //Запросы на работы "Отклоненные"
                tcrAr = WebApplication5.Models.TaskCompRequest.GetTaskCompRequestByStatus(context, Models.TaskCompRequest.Status.Declined);
                ViewData["tcrCond"] = "3";
            }            
            else
            {
                //Запросы на работы "Все"
                tcrAr = WebApplication5.Models.TaskCompRequest.GetAllTaskCompRequest(context);
                ViewData["tcrCond"] = "4";
            }
            ViewData["isKSP"] = curUser.IsKSP(context);
            ViewData["tcReqCount"] = Models.TaskCompRequest.GetAmountOfNewTaskCompRequestStr(context);
            return View(tcrAr);
        }

        public IActionResult CreateTaskCompFromRequest(TaskCompRequest[] tcrAr, string filePath)
        {
            var userForAction = WebApplication5.Models.User.GetUser(context, HttpContext);
            var errorList = new List<string>();
            var tcrArImp= tcrAr.Where(x => x.NeedToImport == true);
            ExcelFile excelFile = new ExcelFile(filePath);
            var tcrIdSet = tcrArImp.Select(x => x.Id);
            var tcrList = WebApplication5.Models.TaskCompRequest.GetElemsByIds(context, tcrIdSet);            
            excelFile.CreateTCR(context, tcrList, errorList);
            TempData["filePath"] = filePath;
            MailService.Notification(tcrList, userForAction, MailService.Status.Confirmed, errorList, tcrAr);
            if (errorList.Count == 0)
            {                
                TempData["SuccessMes"] = new List<string>() { $"Успешно импортировано {tcrArImp.Count()} работ." };
            }
            else
            {
                TempData["FailMes"] = errorList;
            }
            return RedirectToAction("TaskCompReqApply");
        }

        //public IActionResult DeclineTaskCompRequest(string[] tcrAr)
        //{
        //    var userForAction = WebApplication5.Models.User.GetUser(context, HttpContext);
        //    List<string> errorMes = new List<string>();
        //    var tcrList = WebApplication5.Models.TaskCompRequest.GetElemsByIds(context, tcrAr);
        //    foreach (var tcr in tcrList)
        //    {
        //        tcr.ChangeStatus(WebApplication5.Models.TaskCompRequest.Status.Declined);
        //        context.SaveChanges();
        //    }
        //    MailService.Notification(tcrList, userForAction, MailService.Status.Declined, errorMes);
        //    TempData["SuccessMes"] = new List<string>() { $"Статус {tcrList.Count()} комплектов/работ успешно изменён. Пользователь, подавший заявку, уведомлён." };            
        //    return RedirectToAction("TaskCompReqApply");
        //}

        public IActionResult DeclineTaskCompRequest(TaskCompRequest[] tcrAr)
        {
            var userForAction = WebApplication5.Models.User.GetUser(context, HttpContext);
            List<string> errorMes = new List<string>();
            var tcrList = WebApplication5.Models.TaskCompRequest.GetElemsByIds(context, tcrAr);
            foreach (var tcr in tcrList)
            {
                tcr.ChangeStatus(WebApplication5.Models.TaskCompRequest.Status.Declined);
                context.SaveChanges();
            }
            MailService.Notification(tcrList, userForAction, MailService.Status.Declined, errorMes, tcrAr);
            TempData["SuccessMes"] = new List<string>() { $"Статус {tcrList.Count()} комплектов/работ успешно изменён. Пользователь, подавший заявку, уведомлён." };
            return RedirectToAction("TaskCompReqApply");
            
        }

        public IActionResult ExportProjectReport(string ProjectName, string curUserId)
        {
           
            List<string> errorMes = new List<string>();
            List<string> successMes = new List<string>();
            List<TaskComp> taskCompList = new List<TaskComp>();
            List<WorkLogs> wlList = new List<WorkLogs>();
            string userName = string.Empty;

            if (string.IsNullOrEmpty(ProjectName))
            {
                errorMes.Add($"Выберите проект для выгрузки.");
                TempData["FailMes"] = errorMes;
                return RedirectToAction("Index");
            }

            if (ProjectName == "Все проекты")
            {
                var res = Int32.TryParse(curUserId, out int userId);
                if (res)
                {
                    var curUser = context.Users.First(x => x.Id == userId);
                    userName = $"{curUser.LastName}{curUser.FirstName.First()}{curUser.MiddleName.First()}";
                    taskCompList = TaskComp.GetAllTasksForGIP(curUser, context);
                    TaskComp.RemoveStandartTasks(taskCompList);
                    var projectList = Project.GetProjectsFromTaskSet(taskCompList);
                    foreach (var project in projectList)
                    {
                        taskCompList.AddRange(context.TaskComps.Where(x => x.ProjectNumber == ProjectName).ToList());
                        wlList.AddRange(context.WorkLogs.Where(x => x.Proj_id == project).Include(x => x.User).ToList());
                    }
                }
            }
            else
            {
                taskCompList = context.TaskComps.Where(x => x.ProjectNumber == ProjectName).ToList();
                wlList = context.WorkLogs.Where(x => x.Proj_id == ProjectName).Include(x => x.User).ToList();
            }
            foreach (var taskComp in taskCompList)
            {
                taskComp.FactWorkLog = WorkLogs.GetTotalHoursOnTask(taskComp, context, out DateTime? startFactDate);
                taskComp.StartFactDate = startFactDate;
            }
            
            var filePath = FilePath.GetPathForGIP($"Свод_трудозатрат_{ProjectName}");
            if (ProjectName == "Все проекты")
            {
                filePath = FilePath.GetPathForGIP($"Свод_трудозатрат_{userName}_{ProjectName}");
            }
                ExcelFile exFile = new ExcelFile(filePath);
            var resExFileCreation=exFile.UploadTaskCompsAndWorkLogs(wlList, taskCompList, context);

            if (resExFileCreation)
            {
                successMes.Add($"Файл успешно создан по след. пути: {filePath}");
                TempData["SuccessMes"] = successMes;
            }
            else
            {
                errorMes.Add($"Ошибка при записи в файл {filePath}. Возможно файл занят другим приложением.");
                TempData["FailMes"] = errorMes;
            }

            return RedirectToAction("Index");
        }
    }
}