using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication5.Models.CheckData;

namespace WebApplication5.Models
{
    public class Absence
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public Double? HourAmount { get; set; }
        public User User { get; set; }
        public AbsenceReason Reason { get; set; }

        public Absence()
        {

        }
        public Absence(DateTime startDate, DateTime finishDate, AbsenceReason reason, Double? hourAmount, User curUser)
        {
            if (hourAmount != 0)
            {
                HourAmount = hourAmount;
            }
            StartDate = startDate;
            if (finishDate.Year != 1)
            {
                FinishDate = finishDate;
            }
            Reason = reason;
            
            User = curUser;

        }

     
        public static double GetTotalHoursOfAbsenceCurMonth(AppDbContext context, User curUser)
        {            
            var date = DateTime.Now;
            var prevMonth = date.Month - 1;
            var curMonth = date.Month;
            List<Absence> absences = new List<Absence>();
            var absenceSet = context.Absences.Where(x => x.User == curUser).Where(x=>x.StartDate.Month==curMonth);
            var absenceSetAddition = context.Absences.Where(x => x.User == curUser).Where(x => x.StartDate.Month == prevMonth && x.FinishDate.HasValue).Where(x=>x.FinishDate.Value.Month==curMonth);
            absences.AddRange(absenceSet); absences.AddRange(absenceSetAddition);
            var totalHours = Absence.GetTotalHours(absences, context, date.Month);            
            return totalHours;  
        }

        public static double GetTotalHoursOfAbsenceExactMonth(AppDbContext context, User curUser, int month, int year)
        {
            var date = new DateTime(year, month, 1);
            var prevMonth = date.Month - 1;
            var curMonth = date.Month;
            List<Absence> absences = new List<Absence>();
            var absenceSet = context.Absences.Where(x => x.User == curUser).Where(x => x.StartDate.Month == curMonth && x.StartDate.Year == year);
            var absenceSetAddition = context.Absences.Where(x => x.User == curUser).Where(x => x.StartDate.Month == prevMonth && x.FinishDate.HasValue).Where(x => x.FinishDate.Value.Month == curMonth);
            absences.AddRange(absenceSet); absences.AddRange(absenceSetAddition);
            //SetStartAndFinishMonth(absences, month);
            //DeleteDublicates(absences);
            var totalHours = Absence.GetTotalHours(absences, context, month);
            return totalHours;
        }

        public static double GetStartEndDateAbsence(AppDbContext context, User curUser, int month, int year)
        {
            double totalWorkHours = 0;
            List<int> workHourList = new List<int>();
            var daysOffList = context.DayOffs.Where(x => x.Date.Month == month).Select(x => x.Date);
            if (curUser.ImportDate.HasValue)
            {
                var impDate = curUser.ImportDate.Value;
                if (impDate.Month==month && impDate.Year == year)
                {
                    var dayAmount = impDate.AddDays(-1).Day; //День импорта считается как рабочий
                    var startDateForLoop = new DateTime(year, month, 1);
                   
                    for (int i = 1; i < dayAmount; i++)
                    {
                        var dateInLoop = startDateForLoop.AddDays(i);
                        DateConverter.SumWorkDay(workHourList, dateInLoop.Year, month, dateInLoop.Day, daysOffList);
                    }
                    totalWorkHours = workHourList.Sum(x => x);
                }
            }
            if (curUser.BlockDate.HasValue)
            {
                var blockDate = curUser.BlockDate.Value;
                if (blockDate.Month == month && blockDate.Year == year)
                {
                    var blockDayNum = blockDate.Day; //День блокировки считается как нерабочий
                    var lastDateInMonth = DateTime.DaysInMonth(year, month);
                    var endDateForLoop = new DateTime(year, month, blockDayNum);
                    var dayAmount = lastDateInMonth - blockDayNum;
                    for (int i =1 ; i <= dayAmount; i++)
                    {
                        var dateInLoop = endDateForLoop.AddDays(i);
                        DateConverter.SumWorkDay(workHourList, dateInLoop.Year, month, dateInLoop.Day, daysOffList);
                    }
                    totalWorkHours += workHourList.Sum(x => x);
                }
            }
            return totalWorkHours;
        }
        

        public static double GetTotalHours(List<Absence> absenceSet, AppDbContext context, int month)
        {
            double totalHours = 0;
            if (absenceSet != null)
            {
                foreach (var absence in absenceSet)
                {
                    absence.HourAmount = GetWorkHoursFromDates(absence, context, month);
                    if (absence.HourAmount.HasValue)
                    totalHours += absence.HourAmount.Value;
                }
            }
            return totalHours;
        }
      
        public static double GetWorkHoursFromDates(DateTime startDate, DateTime finishDate)
        {
            var totalWorkHours = ((finishDate - startDate).TotalDays + 1) *8;
            return totalWorkHours;
        }
        public static double GetWorkHoursFromDates(Absence absence, AppDbContext context, int month)
        {
            double totalWorkHours = 0;
            if (absence.FinishDate == null || absence.FinishDate == absence.StartDate)
            {
                if (absence.HourAmount.HasValue)
                {
                    totalWorkHours += absence.HourAmount.Value;
                }
                else
                {
                    totalWorkHours += 8; //Если значение HourAmount is null, то прибавляем 8 часов
                }
            }
            else
            {
                List<int> workHourList = new List<int>();
                var monthStart = absence.StartDate.Month;
                var monthFinish = absence.FinishDate?.Month;
                DateTime startDateForLoop = absence.StartDate;
                var daysOffList = context.DayOffs.Where(x => x.Date.Month == monthStart).Select(x => x.Date);
                int dayAmount = 0;
                if (monthStart == month && monthStart<monthFinish)
                {               
                    dayAmount = DateConverter.GetNumberOfDaysInMonth(monthStart) - absence.StartDate.Day + 1;                    
                }
                else if (monthStart == monthFinish)
                {
                    dayAmount = (int)(absence.FinishDate.Value - absence.StartDate).TotalDays + 1;                    
                }
                else if (monthStart!=month && month == monthFinish)
                {
                    dayAmount = absence.FinishDate.Value.Day;
                    startDateForLoop = new DateTime(startDateForLoop.Year, month, 1);
                }              
                
                for (int i = 0; i < dayAmount; i++)
                {
                    var dateInLoop = startDateForLoop.AddDays(i);
                    DateConverter.SumWorkDay(workHourList, dateInLoop.Year, month, dateInLoop.Day, daysOffList);
                }
                totalWorkHours = workHourList.Sum(x => x);
            }

            return totalWorkHours;
        }

        public static Absence[] GetAbsenceArrCurMonth(AppDbContext context, User curUser)
        {

            var curMonth = DateTime.Now.Month;
            var prevMonth = DateTime.Now.Month - 1;
            List<Absence> absences = new List<Absence>();
            var absenseSet = context.Absences.Where(x => x.User == curUser).Where(x => x.StartDate.Month == curMonth);
            var absenseSetAddition = context.Absences.Where(x => x.User == curUser).Where(x => x.FinishDate.HasValue && x.StartDate.Month == prevMonth).Where(x => x.FinishDate.Value.Month == curMonth).Where(x=>x.User==curUser);
            absences.AddRange(absenseSet); absences.AddRange(absenseSetAddition);
            if (absenseSet.Count() > 0) return absences.ToArray();
            else return null;
        }

        public static List<Absence> GetAbsenceSelMonth(int selMonth, int selYear, AppDbContext context, User curUser)
        {
            var curMonth = selMonth;
            var prevMonth = curMonth - 1;           
            List<Absence> absences = new List<Absence>();
            var absenseSet = context.Absences.Where(x => x.User == curUser).Where(x => x.StartDate.Month == selMonth && x.StartDate.Year ==selYear);
            var absenseSetAddition = context.Absences.Where(x => x.User == curUser).Where(x => x.FinishDate.HasValue && x.StartDate.Month == prevMonth && x.StartDate.Year == selYear).Where(x => x.FinishDate.Value.Month == curMonth);            
            absences.AddRange(absenseSet); absences.AddRange(absenseSetAddition);
            if (absenseSet.Count() > 0) return absences;
            else return null;
        }

        public static List<Absence> GetAbsenceSelMonth(int month, int year, AppDbContext context)
        {
            var absenseSet = context.Absences.Include(x => x.User).Include(x => x.User.Department).Include(x => x.Reason).Where(x => (x.StartDate.Month == month && x.StartDate.Year == year) ||
                     (x.FinishDate.HasValue && (x.FinishDate.Value.Month == month && x.FinishDate.Value.Year == year))).ToList();
            return absenseSet;
        }
        public double GetTotalHours()
        {
            var now = DateTime.Now;
            if (FinishDate.HasValue)
            {
                var finishDate = FinishDate.Value;
                var startDate = StartDate;
                if (startDate.Month == now.Month && finishDate.Month == startDate.Month)
                {
                    var absenceHours = GetWorkHoursFromDates(startDate, finishDate);
                    return absenceHours;
                }
                else if (startDate.Month == now.Month && startDate.Month > finishDate.Month)
                {
                    var lastDayOfMonth = DateConverter.GetNumberOfDaysInMonth(startDate.Month);
                    var lastDateOfMonth = new DateTime(startDate.Year, startDate.Month, lastDayOfMonth);
                    var absenceHours = GetWorkHoursFromDates(startDate, lastDateOfMonth);
                    return absenceHours;
                }
                else if (startDate.Month < now.Month && finishDate.Month == now.Month)
                {
                    var firstDateOfMonth = new DateTime(finishDate.Year, finishDate.Month, 1);
                    var absenceHours = GetWorkHoursFromDates(firstDateOfMonth, finishDate);
                    return absenceHours;
                }

            }
            else
            {
                if (HourAmount.HasValue)
                {
                    return HourAmount.Value;
                }
                else
                {
                    return 8;
                }
            }
            return 0;
        }

        public static bool Delete(AppDbContext context, string idStr, List<string> errorMes)
        {
            var res = Int32.TryParse(idStr, out int idInt);
            if (res)
            {
                var abSet = context.Absences.Where(x => x.Id == idInt);
                if (abSet.Count() > 0)
                {
                    var abForRemove = abSet.First();
                    context.Absences.Remove(abForRemove);
                    context.SaveChanges();
                    return true;
                }
                else
                {
                    errorMes.Add($"Absence с ID - {idStr} отсутствует в БД. Обратитесь к администратору. (Нестеров И.Г.)");
                    return false;
                }
            }
            else
            {
                errorMes.Add("Невозможно преобразовать в число ID. Обратитесь к администратору. (Нестеров И.Г.)");
                return false;
            }
        }
    }
}
