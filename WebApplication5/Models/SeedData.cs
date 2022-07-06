using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{
    public class SeedData
    {
        public static void EnsurePopulated(IApplicationBuilder app)
        {
            AppDbContext context = app.ApplicationServices.GetRequiredService<AppDbContext>();      
            if (!context.Cors.Any())
            {
                context.Cors.AddRange(
                    new Corrections(1, new DateTime(2021, 5, 6), "Всё не правильно"),
                     new Corrections(2, new DateTime(2021, 6, 6), "Всё не правильно 1"),
                      new Corrections(3, new DateTime(2021, 7, 7), "Всё не правильно 2")
                );
                context.SaveChanges();
            }
        }
    }
}
