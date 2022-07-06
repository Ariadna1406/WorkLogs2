using System;
using System.Collections.Generic;
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
using WebApplication5.Models;

namespace WebApplication5
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {        
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
            services.AddDbContext<AppDbContext>(x => x.UseSqlServer("Data Source=msdb; Database = WebReport; User Id=WRService; Password=P@ssw0rd; Persist Security Info=false ; MultipleActiveResultSets = True"));
            
                //(x => x.UseSqlServer("Data Source=msdb; Database = Corrections; User Id=AdminRem; Password=123456; Persist Security Info=false ; MultipleActiveResultSets = True"));
            //services.AddSingleton<AppDbContext>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
           // SeedData.EnsurePopulated(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
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
    }
}
