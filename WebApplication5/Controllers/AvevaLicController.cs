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
using Microsoft.AspNetCore.Identity;
using System.Net.NetworkInformation;

namespace WebApplication5.Controllers
{
    public class AvevaLicController : Controller
    {
        AppDbContext context;
        public AvevaLicController(AppDbContext appDbContext, IHostingEnvironment appEnv)
        {
            context = appDbContext;
        }

        public IActionResult Index()
        {
            if (TempData["SuccessMes"] != null)
            {                
                var successAr = TempData["SuccessMes"] as string[];
                ViewData["SuccessMes"] = successAr.ToList();               
            }
            if (TempData["FailMes"] != null)
            {               
                var failAr = TempData["FailMes"] as string[];
                ViewData["FailMes"] = failAr.ToList(); ;
            }
            List<Licence> licenceList = new List<Licence>();
            var filterRows = TempData["FilterRows"] as bool?;
            if (filterRows.HasValue && filterRows.Value)
            {
                ViewData["FilterRows"] = filterRows;
                licenceList = context.AvevaLicences.Where(x => x.Status == true).ToList();
            }
            else
            {
                licenceList = context.AvevaLicences.ToList();
            }
            return View(licenceList);
            
        }
        [HttpGet]
        public IActionResult CreateNewHost(string HostName)
        {
            if (PingHost(HostName))
            {
                if (context.AvevaLicences.Where(x => x.HostName == HostName).Count() == 0)
                {
                    context.AvevaLicences.Add(new Licence(HostName));
                    context.SaveChanges();
                    TempData["SuccessMes"] = new List<string> { "Хост успешно добавлен" }; 
                }
                else
                {
                    TempData["FailMes"] = new List<string> { "Хост уже существует" };
                }
            }
            else
            {
                TempData["FailMes"] = new List<string> { "Данный хост не отвечает" };
            }
            return RedirectToAction("Index");

        }

        [HttpGet]
        public IActionResult FilterRows(string filterRows)
        {
            var res = bool.TryParse(filterRows, out bool filter);
            filter = !filter;
                if (res)
                {
                    TempData["FilterRows"] = filter;
                }
                else
                {
                    TempData["FilterRows"] = false;
                }                    
            
            return RedirectToAction("Index");

        }

        public bool PingHost(string nameOrAddress)
        {
            bool pingable = false;
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(nameOrAddress);
                pingable = reply.Status == IPStatus.Success;
                if (pingable == false) Console.WriteLine(nameOrAddress + " ping failed!");
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            return pingable;
        }


    }

 
}