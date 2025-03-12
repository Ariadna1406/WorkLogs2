using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{
    public class ControllerTemplate
    {
        public static void ExecuteCommonFunctions(ITempDataDictionary TempData, ViewDataDictionary ViewData, AppDbContext context, HttpContext HttpContext)
        {
            var successAr = TempData["SuccessMes"] as string[];
            var failAr = TempData["FailMes"] as string[];
            if (successAr != null)
            {
                ViewData["SuccessMes"] = successAr.ToList();
            }
            if (failAr != null)
            {
                ViewData["FailMes"] = failAr.ToList();
            }
            var curUser = WebApplication5.Models.User.GetUser(context, HttpContext);
            if (curUser != null)
            {
                ViewData["curUser"] = curUser;
                ViewData["IsAdmin"] = curUser.IsAdmin(context);
                ViewData["IsHOD"] = curUser.isHeadOfDepartment(context);
            }
        }
    }
}
