using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApplication5.Controllers;
using WebApplication5.Models;

namespace WebApplication5
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            try
            {
                Configuration = configuration;
            }
            catch (Exception ex)
            {
                StreamWriter streamWriter = new StreamWriter(@"C:\inetpub\01.txt");
                streamWriter.WriteLine(ex);
                streamWriter.Close();
            }
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddSingleton<TimerService>(provider =>
            //{
            //    TimeSpan interval = TimeSpan.FromSeconds(10); // Интервал выполнения задачи
            //    Func<Task> callback = async () =>
            //    {
            //        using (var scope = provider.CreateScope())
            //        {
            //            var controller = scope.ServiceProvider.GetService<AdminController>(); // Замените на ваш класс контроллера
            //            await controller.MyMethod(); // Замените на ваш метод контроллера
            //        }
            //    };

            //    var timerService = new TimerService(interval, callback);
            //    timerService.Start();

            //    return timerService;
            //});
            services.AddAuthentication(IISDefaults.AuthenticationScheme);
            services.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit = int.MaxValue;
            });
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            //services.AddTransient<>
            //services.AddDbContext<AppDbContext>(x => x.UseSqlServer("Data Source=msdb; Database = WebReport; User Id=WRService; Password=P@ssw0rd; Persist Security Info=false ; MultipleActiveResultSets = True"));
            services.AddDbContext<AppDbContext>(x => x.UseSqlServer("Data Source=msdb; Database = WebReportTest; User Id=WRService; Password=P@ssw0rd; Persist Security Info=false ; MultipleActiveResultSets = True"));
            //(x => x.UseSqlServer("Data Source=msdb; Database = Corrections; User Id=AdminRem; Password=123456; Persist Security Info=false ; MultipleActiveResultSets = True"));
            //services.AddSingleton<AppDbContext>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!Timer!!!!!!!!!!!!!!!!!!!!!!

           
            // SeedData.EnsurePopulated(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            try
            {
                if (env.IsDevelopment())
                {

                    //app.UseExceptionHandler("/Home/Index");
                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    app.UseDeveloperExceptionPage();
                    //app.UseExceptionHandler("/Home/Error");

                }




                app.UseStaticFiles();
                app.UseCookiePolicy();


                app.UseMvc(routes =>
                {
                    routes.MapRoute(
                        name: "default",
                        template: "{controller=WorkLogs}/{action=Index}/{id?}");

                    routes.MapRoute("indexx1213", "indexx", new { controller = "Home", action = "Indexx" });
                    routes.MapRoute("indexxx1", "indexxx1", new { controller = "Home", action = "Indexx" });
                });

            }
            catch { }
           
        }
    }
}
