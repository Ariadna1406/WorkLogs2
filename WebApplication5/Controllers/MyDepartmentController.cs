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
    public class MyDepartmentController : Controller
    {
        AppDbContext context;
        public MyDepartmentController(AppDbContext appDbContext, IHostingEnvironment appEnv)
        {
            context = appDbContext;
        }
     
        public ViewResult Index()
        {
           
            var curUser = WebApplication5.Models.User.GetUser(context, HttpContext);            
            ViewData["curUser"] = curUser;
            if (curUser.isHeadOfDepartment())
            {
                IEnumerable<IGrouping<string, PrimaTask>> taskSet = PrimaTask.GetAllTasksForMyDepartment(curUser).GroupBy(x => x.Task_Id);
                              
                //IEnumerable <IGrouping<string, PrimaTask>> 
                return View(taskSet);
            }
            else
            {

                return View(@"/Views/Shared/AccessDenied.cshtml");
            }
            
        }

     

        public ViewResult Graphs(string prjAcr)
        {
            var elemSet = context.AvevaElemAmounts.Where(x => x.ProjectAcr == prjAcr).ToList();
            //if (elemSet.Count > 0)
            // {
            ViewData["prjAcr"] = prjAcr;
                return View(elemSet);
            //}
            //return View("/Views/Project/")
        }

        public string GetAvevaElemAmount(string avevaAcr)
        {
            string filePath = String.Format(@"\\it-andrey\ModelElemAmount\{0}\ElemAmount.txt", avevaAcr);
            if (System.IO.File.Exists(filePath))
            {
                StreamReader sr = new StreamReader(filePath);
                var line= sr.ReadLine();
                sr.Close();
                return line;
            }
            return string.Empty;
        }
    }
}