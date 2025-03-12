using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{
    public class AbsenceReason
    {
        public int Id { get; set; }
        public string ReasonName { get; set; }

        public static IQueryable<AbsenceReason> GetAllAbsenceReason(AppDbContext context)
        {
            return context.AbsenceReasons;
        }

        public static bool GetInstance(AppDbContext context, string nameAbsenceReason, out AbsenceReason arInst, List<string> errorMes)
        {
            var arSet = context.AbsenceReasons.Where(x => x.ReasonName == nameAbsenceReason);
            if (arSet.Count() > 0)
            {
                arInst = arSet.First();
                return true;
            }
            errorMes.Add("Не найдено соответсвущей причины отсутствия. Требуется выбрать причины из списка.");
            arInst = null;
            return false;
        }

    }
}
