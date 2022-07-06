using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication5.Models;
using WebApplication5.Util;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

using System.IO;
using Microsoft.AspNetCore.Hosting;
using static WebApplication5.Models.Corrections;
using Microsoft.EntityFrameworkCore;

namespace WebApplication5.Controllers
{
    
    public class HomeController : Controller
    {
        Microsoft.AspNetCore.Hosting.IHostingEnvironment _appEnvironment;

        [HttpPost]
        public ViewResult CreateRemark(Corrections newCor)
        {
            if (ModelState.IsValid)
            {
                // "~/images" + fileName;
                //SaveFile
                var fileName = Path.GetFileNameWithoutExtension(newCor.ImageFile.FileName);
                var extension = Path.GetExtension(newCor.ImageFile.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                var path = "//images//" + fileName;
                newCor.ImageLink = fileName;
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    newCor.ImageFile.CopyTo(fileStream);
                }
                int idnum;
                var res = Int32.TryParse(Request.Form["UserName"], out idnum);
                if (res)
                {
                    var userFound = newCor.Executor = context.Users.Single(x => x.Id == idnum);
                    newCor.Executor = userFound;
                }


                context.Cors.Add(newCor); //new Corrections(3, new DateTime(2021,6,7), "Everything checked"));
                context.SaveChanges();
                return View("Index", context);
            }
            else
            {
                return CreateCor();
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> AddFile(IFormFile uploadedFile)
        //{
        //    if (uploadedFile != null)
        //    {
        //        // путь к папке Files
        //        string path = "/Files/" + uploadedFile.FileName;
        //        // сохраняем файл в папку Files в каталоге wwwroot
        //        using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
        //        {
        //            await uploadedFile.CopyToAsync(fileStream);
        //        }
        //        FileModel file = new FileModel { Name = uploadedFile.FileName, Path = path };
        //        _context.Files.Add(file);
        //        _context.SaveChanges();
        //    }

        //    return RedirectToAction("Index");
        //}
        //[HttpPost]
        //public ViewResult ImportImage(HttpPostedFileBase image )
        //{

        //    return View(context);
        //}

        public ViewResult Index()
        {
            var listStat = GetStatuses();
            SelectList selectListItems = new SelectList(listStat, "Id", "StatusName", listStat[1]);
            ViewBag.Statuses = selectListItems;
            ViewData["ActiveProjects"] = context.ProjectSet.Where(x=>x.IsDeleted==false).Where(x => x.ShowInMenuBar == true);
            ViewData["Context"] = context;
            var projectSet= context.ProjectSet.Where(x=>x.IsDeleted==false).Include(x=>x.Manager).ToList();
            return View(projectSet);
        }

        [Route("Admin/AdminMain.cshtml")]
        public ViewResult AdminMain()
        {                       
            return View();
        }

        public ViewResult EditStatus(int corId)
        {
            var listStat = GetStatuses();
            SelectList selectListItems = new SelectList(listStat, "Id", "StatusName", listStat[1]);
            ViewBag.Statuses = selectListItems;
            var model= context.Cors.Single(x => x.Id == corId);
            return View(model);
            
        }

       

        public List<Statuses> GetStatuses()
        {
            var statuses = new List<Statuses>();
            statuses.Add(new Statuses() { Id = 0, StatusName = "Новое" });
            statuses.Add(new Statuses() { Id = 1, StatusName = "Исправлено исполнителем" });
            statuses.Add(new Statuses() { Id = 2, StatusName = "Проверено BIM-координатором" });
            statuses.Add(new Statuses() { Id = 3, StatusName = "Снято" });
            statuses.Add(new Statuses() { Id = 4, StatusName = "Повторное" });
            return statuses;
        }

        [HttpPost]
        public ViewResult CreateCor()
        {
            ViewData["ActiveProjects"] = context.ProjectSet.Where(x => x.ShowInMenuBar == true);
            ViewBag.NewId = context.Cors.Count()+1;
            var usersForFillName= context.Users.Where(x => x.FullName == null || x.FullName == string.Empty);
            FillFullName(usersForFillName);
            ViewData["Users"] = new SelectList(context.Users,"Id", "FullName");
            return View();
        }

        public void FillFullName(IQueryable<User> users)
        {
            foreach (var user in users)
            {
                user.FullName = "1"; // нужно вызвать функцию set для автоматического заполнения
                
            }
            context.SaveChanges();
        }

        public ViewResult CreateCor1()
        {
            ViewBag.NewId = context.Cors.Count() + 1;
            var usersForFillName = context.Users.Where(x => x.FullName == null || x.FullName == string.Empty);
            FillFullName(usersForFillName);
            ViewData["Users"] = new SelectList(context.Users, "Id", "FullName");
            ViewBag.NewId = context.Cors.Count() + 1;
            return View("CreateCor");
        }

        AppDbContext context;
        //List<Phone> phones;
        public HomeController(AppDbContext appDbContext, IHostingEnvironment appEnv)
        {
            context = appDbContext;
            _appEnvironment = appEnv;
            //if (context.Cors.Count() == 0)
            //{
            //    context.Cors.AddRange(new Corrections(1, new DateTime(2021, 5, 5), "Everything just wrong"));
            //    context.SaveChanges();
            //}
        }

        //    Company apple = new Company { Id = 1, Name = "Apple", Country = "США" };
        //    Company microsoft = new Company { Id = 2, Name = "Samsung", Country = "Республика Корея" };
        //    Company google = new Company { Id = 3, Name = "Google", Country = "США" };


        //phones= new List<Phone>
        //    {
        //        new Phone { Id=1, Manufacturer= apple, Name="iPhone X", Price=56000 },
        //        new Phone { Id=2, Manufacturer= apple, Name="iPhone XZ", Price=41000 },
        //        new Phone { Id=3, Manufacturer= microsoft, Name="Galaxy 9", Price=9000 },
        //        new Phone { Id=4, Manufacturer= microsoft, Name="Galaxy 10", Price=40000 },
        //        new Phone { Id=5, Manufacturer= google, Name="Pixel 2", Price=30000 },
        //        new Phone { Id=6, Manufacturer= google, Name="Pixel XL", Price=50000 }
        //    };
        //    string Name = System.Environment.UserName;
        //}
        //[HttpPost]
        //public string MyAction(string product, string action)
        //{
        //    //if (action == "add")
        //    //{

        //    //}
        //    //else if (action == "delete")
        //    //{

        //    //}
        //    //// остальной код метода
        //    //

        //}
        //public IActionResult GetHtml()
        //{
        //    //return View(phones);
        //    //var filepath = Path.Combine("Files", "ShortName.pdf");
        //    //    return File(filepath, "application/pdf");
        //    //return Ok("Успешно");

        //    //return Redirect(@"~/Home/About");
        //   // return Content("12234"); //HtmlResult("<h2>HI! ASP.NET 5</h2>");
        //}
        [HttpPost]
        public CorStatus GetStat(string Statuses)
        {
            switch (Statuses)
            {
                case "0":
                    return CorStatus.New;
                case "1":
                    return CorStatus.CorrectedByExecutor;
                case "2":
                    return CorStatus.CheckedByBim;
                case "3":
                    return CorStatus.Done;
                case "4":
                    return CorStatus.NewAgain;

            }
            return CorStatus.New;
        }

        public ActionResult GetSt()
        {
            var listStat = GetStatuses();
            SelectList selectListItems = new SelectList(listStat, "Id", "StatusName", listStat[1]);
            ViewBag.Statuses = selectListItems;
            return View(selectListItems);
        }

        [HttpPost]
        public ViewResult ChangeStatus(int corId, string Status)
        {
            var cor1= context.Cors.Single(x => x.Id == corId);
            cor1.Status= GetStat(Status);
            context.SaveChanges();
            return View("Index", context);
        }

        [HttpPost]
        public String GetSelectedPhone(string phone)
        {
            string phonesStr = string.Empty;
                          phonesStr += phone + ", ";
            
            return phonesStr; //"Площадь треугольника с основанием {altitude} и высотой {height} равна {square}";
        }

        [HttpPost]
        public HtmlResult Calc(int a, int h, string aaa)
        {
            string altitudeString = Request.Form.FirstOrDefault(p => p.Key == "a").Value;
            int altitude = Int32.Parse(altitudeString);

            string heightString = Request.Form.FirstOrDefault(p => p.Key == "h").Value;
            int height = Int32.Parse(heightString);

            aaa = Request.Form.FirstOrDefault(p => p.Key == "aaa").Value;
            int aa = Int32.Parse(aaa);

            double square = altitude * height / 2;
            return CreateTable(altitude, height, square); //"Площадь треугольника с основанием {altitude} и высотой {height} равна {square}";
        }

        public HtmlResult CreateTable(int alt, int h, double square)
        {
            return new HtmlResult( String.Format("<table>" +
                "<tr><td>Ширина</td><td>Высота</td><td>Площадь</td></tr> " +
                "<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr> " +
                "</table>", alt, h, square));
        }

        [ActionName("hello123")]
        public string SayHello(string Name)
        {
            return "Hello," + Name +"!" ;
        }

        [Route("Home/Index1/{id?}")]
        public IActionResult Indexx(int? id)
        {
            if (id == null)
            {
                return Content("Indexx");
            }
            else
            {
                return Content("Indexx" + id.ToString());
            }
        }


        //public IActionResult Index()
        //{
        //    return View(phones);
        //}

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult NewPage()
        {
            ViewData["Message"] = "Your application description page.";

            return View("NewPage1");
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
