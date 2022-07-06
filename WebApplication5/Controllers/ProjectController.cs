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
    public class ProjectController : Controller
    {
        AppDbContext context;
        public ProjectController(AppDbContext appDbContext, IHostingEnvironment appEnv)
        {
            context = appDbContext;
        }
        public ViewResult Index()
        {
            IOrderedQueryable<Project> projectSet = context.ProjectSet.OrderBy(x=>x.AvevaAcronym);
            foreach (var project in projectSet)
            {
                project.AvevaElemAmount = GetAvevaElemAmount(project.AvevaAcronym);
            }

            return View(projectSet);
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