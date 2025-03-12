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
    public class AbsenceController : Controller
    {   
        AppDbContext context;
        public AbsenceController(AppDbContext appDbContext, IHostingEnvironment appEnv)
        {
            context = appDbContext;
        }

        [HttpGet]
        public ViewResult AbsenceRequest(string startDate, string finishDate, string reasonOfAbsence, string comment, string month, string year, string inputAnotherUserAb, string userSubsSelected)
        {
            int monthInt = DateTime.Now.Month;
            int yearInt = DateTime.Now.Year;
            var curUser = WebApplication5.Models.User.GetUser(context, HttpContext);
            Models.User userSubs;
            if (TempData["userSubsHidden"] != null)
            {
                userSubsSelected = TempData["userSubsHidden"] as string;
            }

            if (!string.IsNullOrEmpty(userSubsSelected))
            {
                var userFound = Models.User.GetUserByFullName(context, userSubsSelected);
                if (userFound != null)
                {
                    userSubs = userFound;
                    ViewData["UserSubsSelected"] = userSubs.FullName;
                    curUser = userFound;
                }
                else
                {
                    TempData["FailMes"] = new string[] { $"Пользователя \"{userSubsSelected}\" не существует." };
                    userSubsSelected = string.Empty;
                }
            }
            
            ViewBag.InputAnotherUserWL = inputAnotherUserAb;            
            ViewBag.UserSubs = UserSubs.GetUserSubs(context, curUser);
            ViewData["UserSubsSelected"] = userSubsSelected;
            var successMes = TempData["SuccessMes"] as string[];
            var failMes = TempData["FailMes"] as string[];
            if (successMes != null) ViewData["SuccessMes"] = successMes.ToList() ;
            if (failMes != null) ViewData["FailMes"] = failMes.ToList();
            if (!string.IsNullOrEmpty(month))
            {
                var res = Int32.TryParse(month, out int monthInte);
                if (res) monthInt = monthInte;
            }
            if (!string.IsNullOrEmpty(year))
            {
                var res = Int32.TryParse(year, out int yearInte);
                if (res) yearInt = yearInte;
            }
            ViewBag.StartDate = startDate; ViewBag.FinishDate = finishDate; ViewBag.reasonOfAbsence = reasonOfAbsence; ViewBag.Comment = comment;
            var absCol = Absence.GetAbsenceSelMonth(monthInt, yearInt, context, curUser);
            ViewBag.AbsenseCol = absCol;
            ViewBag.TotalHoursAbsence = Absence.GetTotalHours(absCol, context, monthInt);
            ViewBag.AbsenceReason = AbsenceReason.GetAllAbsenceReason(context);
            ViewData["isAdmin"] = curUser.IsAdmin(context);
            ViewData["isKSP"] = curUser.IsKSP(context);
            ViewData["IsHOD"] = curUser.isHeadOfDepartment(context);
            
            ViewBag.MonitorMonth = monthInt;            
            var monthStr = monthInt >= 10 ? monthInt.ToString() : $"0{monthInt.ToString()}";
            ViewBag.MonitorMonthStr = $"{DateTime.Now.Year}-{monthStr}";
            return View(new Absence());
        }

        public IActionResult CreateAbsence(string startDateAbsence, string finishDateAbsence, string reasonOfAbsence, string hourAmount ,string comment, string userSubsHidden)
        {
            List<string> errorMes = new List<string>();
            List<string> successMes = new List<string>();
            var curUser = Models.User.GetUser(context, HttpContext);
            TempData["userSubsHidden"] = userSubsHidden;
            if (!string.IsNullOrEmpty(userSubsHidden))
            {
                var userFound = Models.User.GetUserByFullName(context, userSubsHidden);
                if (userFound != null)
                {
                    curUser = userFound;
                }
                else
                {
                    errorMes.Add($"Пользователя \"{userSubsHidden}\" не существует.");
                    userSubsHidden = string.Empty;
                }
            }
            var sdRes = DateTime.TryParse(startDateAbsence, out DateTime sdDate);
            var fdRes = DateTime.TryParse(finishDateAbsence, out DateTime fdDate);
            var haRes = Double.TryParse(hourAmount, out double haDouble);
            var roaRes = AbsenceReason.GetInstance(context, reasonOfAbsence, out AbsenceReason roaInst, errorMes);
            if (sdRes && roaRes)
            {
                context.Absences.Add(new Absence(sdDate, fdDate, roaInst, haDouble, curUser));
                context.SaveChanges();
                successMes.Add("Отсутствие успешно добавлено.");
                TempData["SuccessMes"] = successMes;
            }
            else
            {
                TempData["FailMes"] = errorMes;
            }
            return RedirectToAction("AbsenceRequest");
        }

        public IActionResult DeleteAbsence(string idStr, string startDateAbsence, string finishDateAbsence, string reasonOfAbsence, string hourAmount, string comment, string userSubsHidden)
        {
            List<string> errorMes = new List<string>();
            List<string> successMes = new List<string>();            
            var delRes = Absence.Delete(context, idStr, errorMes);
            TempData["userSubsHidden"] = userSubsHidden;       

            if (delRes)
            {
                successMes.Add("Отсутсвие успешно удалено.");
                TempData["SuccessMes"] = successMes;
            }
            else
            {
                TempData["FailMes"] = errorMes;
            }            
            return RedirectToAction("AbsenceRequest");
        }

     
    }
}