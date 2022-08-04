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
    public class InstructController : Controller
    {
        AppDbContext context;
        public InstructController(AppDbContext appDbContext, IHostingEnvironment appEnv)
        {
            context = appDbContext;
        }
        public ViewResult Index()
        {
            if (IsAdminUser())
            {
                return View();
            }
            return View("/Views/Shared/PageIsInDev.cshtml");
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

    }
}