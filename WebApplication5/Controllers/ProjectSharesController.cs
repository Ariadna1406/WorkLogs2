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
using WebApplication5.Models.CheckData;

namespace WebApplication5.Controllers
{
    public class ProjectSharesController : Controller
    {   
        AppDbContext context;
        public ProjectSharesController(AppDbContext appDbContext, IHostingEnvironment appEnv)
        {
            context = appDbContext;
        }

        [HttpGet]
        public ViewResult Index(string startDate, string finishDate, string reasonOfAbsence, string comment)
        {
            var curUser = WebApplication5.Models.User.GetUser(context, HttpContext);
            var successMes = TempData["SuccessMes"] as string[];
            var failMes = TempData["FailMes"] as string[];
            if (successMes != null) ViewData["SuccessMes"] = successMes.ToList() ;
            if (failMes != null) ViewData["FailMes"] = failMes.ToList();
            ViewBag.StartDate = startDate; ViewBag.FinishDate = finishDate; ViewBag.reasonOfAbsence = reasonOfAbsence; ViewBag.Comment = comment;
            ViewBag.AbsenseThisMonth = Absence.GetAbsenceArrCurMonth(context, curUser);
            ViewBag.TotalHoursAbsence = Absence.GetTotalHoursOfAbsenceCurMonth(context, curUser);
            ViewBag.AbsenceReason = AbsenceReason.GetAllAbsenceReason(context);
            ViewData["isAdmin"] = curUser.IsAdmin(context);
            ViewData["isKSP"] = curUser.IsKSP(context);
            ViewData["IsHOD"] = curUser.isHeadOfDepartment(context);
            ViewData["IsBuh"] = curUser.IsBuh(context);
            return View(new Absence());
        }

        public IActionResult Export(string date)
        {
            List<string> errorMes = new List<string>();
            List<string> successMes = new List<string>();
            var curUser = Models.User.GetUser(context, HttpContext);
            var sdRes = DateTime.TryParse(date, out DateTime dDate);
            if (sdRes)
            {
                var filePath = FilePath.GetPathForBuhMain(dDate, "Доли_проектов");
                ExcelFile exFile = new ExcelFile(filePath);
                Workbook wb = new Workbook(Models.ExcelFiles.FilePath.workLogsTemplatePath);
                var psList = ProjectShares.Calculate(context, out List<ProjectSharesTot> projectSharesTotList, dDate.Month, dDate.Year);
                var resExFileCreation = exFile.UploadProjectShares(psList, projectSharesTotList, Models.User.GetAllActiveUsersForReport(context), errorMes, wb);
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
            }
            else
            {
                errorMes.Add("Дата указана неверно.");
                TempData["FailMes"] = errorMes;
            }
            return RedirectToAction("Index");
        }

        public IActionResult ExportReport(string date1, string date2)
        {
            List<string> errorMes = new List<string>();
            List<string> successMes = new List<string>();
            var curUser = Models.User.GetUser(context, HttpContext);
            var sdRes1 = DateTime.TryParse(date1, out DateTime dDate1);
            var sdRes2 = DateTime.TryParse(date2, out DateTime dDate2);
            if (sdRes1 && sdRes2)
            {
                if (dDate1.Year == dDate2.Year && dDate2.Month > dDate1.Month) {
                    var filePath = FilePath.GetPathForBuhMain(dDate2, $"Свод_трудозатрат_{FilePath.GetMonth(dDate1.Month)}-{dDate1.Year}_");
                    ExcelFile exFile = new ExcelFile(filePath);
                    List<WorkLogs> wlList = new List<WorkLogs>();

                    //Последовательно добавляем все трудозатраты по месяцам.
                    for (int i = dDate1.Month; i <= dDate2.Month; i++)
                    {
                       wlList.AddRange(WorkLogs.GetWorkLogsByMonth(context, i, dDate1.Year));
                    }

                    var resExFileCreation = exFile.UploadWorkLogs(wlList, context);
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
                }
                else
                {
                    errorMes.Add("Дата ОТ должна быть меньше даты ДО и Дата ОТ и ДО должны иметь одинаковый год");
                    TempData["FailMes"] = errorMes;
                }
            }
            else
            {
                errorMes.Add("Дата указана неверно.");
                TempData["FailMes"] = errorMes;
            }
            return RedirectToAction("Index");
        }

        public IActionResult ExportExcludeByUsers(string date)
        {
            List<string> errorMes = new List<string>();
            List<string> successMes = new List<string>();
            var curUser = Models.User.GetUser(context, HttpContext);
            var sdRes = DateTime.TryParse(date, out DateTime dDate);
            if (sdRes)
            {
                var filePath = FilePath.GetPathForBuhMain(dDate, "Доли_проектов");
                ExcelFile exFile = new ExcelFile(filePath);
                Workbook wb = new Workbook(Models.ExcelFiles.FilePath.workLogsTemplatePath);
                var psList = ProjectShares.CalculateExcludeByUser(context, out List<ProjectSharesTot> projectSharesTotList, out User[] userAr, dDate.Month, dDate.Year);
                var resExFileCreation = exFile.UploadProjectShares(psList, projectSharesTotList, userAr, errorMes, wb);
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
            }
            else
            {
                errorMes.Add("Дата указана неверно.");
                TempData["FailMes"] = errorMes;
            }
            return RedirectToAction("Index");
        }

        public IActionResult ExportTaskCompShares(string date)
        {
            List<string> errorMes = new List<string>();
            List<string> successMes = new List<string>();
            var taskCompSet = context.TaskComps.ToList();
            var curUser = Models.User.GetUser(context, HttpContext);
            var sdRes = DateTime.TryParse(date, out DateTime dDate);
            if (sdRes)
            {
                var filePath = FilePath.GetPathForBuhMain(dDate, "Доли_проектов");
                ExcelFile exFile = new ExcelFile(filePath);
                Workbook wb = new Workbook(Models.ExcelFiles.FilePath.workLogsTaskCompTemplatePath);
                var psList = ProjectShares.CalculateExcludeByUserTaskComp(context, out List<ProjectSharesTaskCompTot> projectSharesTaskCompTotList, out User[] userAr, dDate.Month, dDate.Year);
                var resExFileCreation = exFile.UploadProjectSharesWithTaskName(psList, projectSharesTaskCompTotList, taskCompSet, userAr, errorMes, wb);
                var psList2 = ProjectShares.Calculate(context, out List<ProjectSharesTot> projectSharesTotList, dDate.Month, dDate.Year);
                var resExFileCreation2 = exFile.UploadProjectShares(psList2, projectSharesTotList, Models.User.GetAllActiveUsersForReport(context), errorMes, wb);
                if (resExFileCreation && resExFileCreation2)
                {
                    successMes.Add($"Файл успешно создан по след. пути: {filePath}");
                    TempData["SuccessMes"] = successMes;
                }
                else
                {
                    errorMes.Add($"Ошибка при записи в файл {filePath}. Возможно файл занят другим приложением.");
                    TempData["FailMes"] = errorMes;
                }
            }
            else
            {
                errorMes.Add("Дата указана неверно.");
                TempData["FailMes"] = errorMes;
            }
            return RedirectToAction("Index");
        }
    }
}