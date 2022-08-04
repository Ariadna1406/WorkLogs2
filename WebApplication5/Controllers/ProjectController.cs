﻿using System;
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
        public ViewResult Index()
        {
            var projectSet = context.ProjectSet.Where(x => x.Status == Status.NotInWork);
            return View(projectSet);
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