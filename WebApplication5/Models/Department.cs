using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebApplication5.Models.StaticData;

namespace WebApplication5.Models
{
    public class Department
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Acronym { get; set; }

        [ForeignKey("HeadOfDepartmentId")]
        public User HeadOfDepartment { get; set; }

        public bool? Production { get; set; }
        public bool? IgnoreInReport { get; set; }

        public string City { get; set; }

        public static string GetAllDepartAcronyms(AppDbContext context)
        {
            List<string> acronymList = new List<string>();            
            foreach (var depart in context.Departments)
            {
                var acronym = depart.Acronym;
                if (!string.IsNullOrEmpty(acronym))
                {
                    acronymList.Add(depart.Acronym);
                }                
            }
            acronymList.AddRange(DepartAcronym.GetAllSubsidiaryAcronyms()); // Получаем и добавляем все абривиатуры филиалов
            string allDepartAcronymStr = string.Empty;
            foreach (var departAcronym in acronymList)
            {
                if (departAcronym == acronymList.First())
                {
                    allDepartAcronymStr = $"{departAcronym}";
                }
                else
                {
                    allDepartAcronymStr = $"{allDepartAcronymStr}, {departAcronym}";
                }
            }
            return allDepartAcronymStr;
        }

        public static Department GetDepartByName(AppDbContext context, string departFullName)
        {
            var departSet = context.Departments.Where(x => x.Name == departFullName);
            if (departSet.Count() > 0)
            {
                return departSet.First();
            }
            else return null;
        }
    }
}
