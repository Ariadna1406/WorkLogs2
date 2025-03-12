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
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text;
using System.Xml.Linq;

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
            var curUser = WebApplication5.Models.User.GetUser(context, HttpContext);
            ViewData["curUser"] = curUser;
            List<ProjectWithElemAmount> projectWithElemAmounts = new List<ProjectWithElemAmount>();
            var projectSet = context.ProjectSet.Where(x => x.Status == Status.NotInWork);
            foreach (var prj in projectSet)
            {
                int avevaElemAmount = 0;
                var avevaElemAmountSet = context.AvevaElemAmounts.Where(x => x.ProjectAcr == prj.AvevaAcronym);
                if (avevaElemAmountSet.Count() > 0)
                {
                    avevaElemAmount = avevaElemAmountSet.Last().PipeLineAmount;
                }

                int teklaElemAmount = 0;
                var teklaElemAmountSet = context.TeklaElemAmounts.Where(x => x.ProjectAcr == prj.AvevaAcronym);
                if (teklaElemAmountSet.Count() > 0)
                {
                    teklaElemAmount = teklaElemAmountSet.Last().ElemAmount;
                }
                projectWithElemAmounts.Add(new ProjectWithElemAmount(prj, avevaElemAmount, teklaElemAmount));
            }
                return View(projectWithElemAmounts);
        }

        public ViewResult Graphs(string prjAcr)
        {
            var elemSet = context.AvevaElemAmounts.Where(x => x.ProjectAcr == prjAcr).ToList();
            //if (elemSet.Count > 0)
            // {
            ViewData["prjAcr"] = prjAcr;
            ViewData["TeklaElemList"]=context.TeklaElemAmounts.Where(x => x.ProjectAcr == prjAcr).ToList();
            var avevaPipeLengthSet = context.avevaPipeLengths.Where(x => x.ProjectAcr == prjAcr);
            var pipeLength = avevaPipeLengthSet.GroupBy(x => x.Date).Select(g => new AvevaPipeLengthReport(g.Key, g.Sum(y => y.PipeLineLength))).ToList();
            var lastDate = avevaPipeLengthSet.OrderBy(x => x.Date).Last().Date;
            var avevaPipeLengthSetLastDate = avevaPipeLengthSet.Where(y => y.Date == lastDate).OrderBy(z=>z.PipeLineBore).ToList();
            ViewData["AvevaPipeLengthReport"] = pipeLength;
            ViewData["AvevaPipeLengthLastDate"] = avevaPipeLengthSetLastDate;
            return View(elemSet);
            //}
            //return View("/Views/Project/")
        }

        public ViewResult BoreDetailed(string prjAcr)
        {
            List<AvevaBoreReport> avevaBoreReports = new List<AvevaBoreReport>();
            var boreGrouped = context.avevaPipeLengths.Where(x => x.ProjectAcr == prjAcr).GroupBy(x => x.PipeLineBore);
            foreach (var boreGr in boreGrouped)
            {
                var boreGroupedDate= boreGr.GroupBy(x => x.Date);
                foreach (var boreGrDate in boreGroupedDate)
                {
                    var totLength= boreGrDate.Sum(y => y.PipeLineLength)/1000;
                    avevaBoreReports.Add(new AvevaBoreReport(boreGr.Key, boreGrDate.Key, totLength));
                }
            } 
            return View(avevaBoreReports);
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

        public string GetNewDocsFromPortal (string avevaAcr)
        {
            if (context.ProjectLinks.Count()>0)
            {
                var prjLink = context.ProjectLinks.First();

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(prjLink.RDPortalLink);
                request.Credentials = new NetworkCredential("urpskug\\IlyinAL", "715zxtGJL");                
                request.PreAuthenticate = true;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var encoding = ASCIIEncoding.ASCII;
                using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
                {
                    string responseText = reader.ReadToEnd();
                    var doc = XDocument.Load(reader);
                }
            }
            return string.Empty;
        }
    }
    public class AvevaPipeLengthReport
    {
        public DateTime Date { get; set; }
        public long TotalLength  { get; set; }

        public AvevaPipeLengthReport(DateTime date, long totalLength)
        {
            Date = date;
            TotalLength = totalLength;
        }
    }

    public class AvevaBoreReport
    {
        public int Bore { get; set; }
        public DateTime Date { get; set; }
        public long TotalLength { get; set; }

        public AvevaBoreReport(int bore, DateTime date, long totalLength)
        {
            Bore = bore;
            Date = date;
            TotalLength = totalLength;
        }
    }
}