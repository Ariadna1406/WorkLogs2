using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebApplication5.Models.CheckData;

namespace WebApplication5.Models
{
    public class WorkLogs
    {
        [Key]
        public int Id { get; set; }
        public User User { get; set; }

        public string Proj_id { get; set; }
        [NotMapped]
        public string ProjName { get; set; }

        public KindOfAct KindOfAct { get; set; }

        public string KindOfActStr { get; set; }

        public DateTime DateOfReport { get; set; }

        public DateTime DateOfSendingReport { get; set; }
        public string Comment { get; set; }
        public string TaskComp_id { get; set; }
        [NotMapped]
        public string TaskName { get; set; }

        [Required(ErrorMessage = "Требуется заполнить кол-во отработанного времени")]
        public TimeSpan WorkTime { get; set; }

        public WorkLogs(User userId, string proj_id, KindOfAct kindOfAct, DateTime dateOfReport, string taskComp_id, TimeSpan workTime, string comment, DateTime dateOfSendingReport, string kindOfActStr)
        {
            User = userId;
            Proj_id = proj_id;
            KindOfAct = kindOfAct;
            DateOfReport = dateOfReport;
            TaskComp_id = taskComp_id;
            WorkTime = workTime;
            Comment = comment;
            DateOfSendingReport = dateOfSendingReport;
            KindOfActStr = kindOfActStr;
        }
        public WorkLogs()
        {
        }

        public static double? GetTotalHoursOnTask(TaskComp taskComp, AppDbContext context, out DateTime? startDate)
        {
            var workLogSet = context.WorkLogs.Where(x => x.TaskComp_id == taskComp.Id.ToString());
            if (workLogSet.Count() > 0)
            {
                TimeSpan totalWorkTime = new TimeSpan();
                var workLogStartDate = workLogSet.OrderBy(x => x.DateOfSendingReport).First();
                startDate = workLogStartDate.DateOfSendingReport;
                foreach (var workLog in workLogSet)
                {
                    totalWorkTime += workLog.WorkTime;
                }
                return totalWorkTime.TotalHours;
            }
            startDate = null;
            return null;
        }

        public static string GetExecuters(TaskComp taskComp, AppDbContext context)
        {
            var workLogSet = context.WorkLogs.Include(x => x.User).Where(x => x.TaskComp_id == taskComp.Id.ToString());
            if (workLogSet.Count() > 0)
            {
                string executers = string.Empty;
                var worLogsGrouped = workLogSet.GroupBy(x => x.User.FullName);
                foreach (var workLogGr in worLogsGrouped)
                {
                    if (workLogGr == worLogsGrouped.Last())
                    {
                        executers += $"{workLogGr.Key}";
                    }
                    else
                    {
                        executers += $"{workLogGr.Key}; ";
                    }
                }
                return executers;
            }
            return string.Empty;
        }
        public static List<WorkLogs> GetAllWorkLogsMyDepartByMonthAndYear(List<User> usersInMyDepart, AppDbContext context, int month, int year, out List<User> abuserList)
        {
            abuserList = new List<User>();
            List<WorkLogs> workLogs = new List<WorkLogs>();
            foreach (var curUser in usersInMyDepart) {
                var workLogSet = context.WorkLogs.Where(x=>x.DateOfReport.Year==year).Where(x=>x.DateOfReport.Month==month).Where(x => x.User == curUser).Include(y => y.KindOfAct).Include(z => z.User);
                if (workLogSet.Count()>0) {
                    workLogs.AddRange(workLogSet);
                    }
                else
                {
                    abuserList.Add(curUser);
                }
            }
            //Находим имена комплектов
            foreach (var workLog in workLogs)
            {
                //workLog.ProjName = FindProjById(workLog.Proj_id);
                workLog.TaskName = TaskComp.FindTaskNameById(workLog.TaskComp_id, context);
            }
            return workLogs;
        }

        public static List<WorkLogs> GetAllWorkLogsMyDepartCurMonth(List<User> usersInMyDepart, AppDbContext context)
        {
            List<WorkLogs> workLogsFiltered = new List<WorkLogs>();
            List<WorkLogs> workLogs = new List<WorkLogs>();
            foreach (var curUser in usersInMyDepart)
            {
                workLogs.AddRange(context.WorkLogs.Where(x => x.User == curUser).Include(y => y.KindOfAct).Include(z => z.User));
            }
            //Фильтруем по датам и находим имена комплектов
            var nowStart = DateTime.Now.Date.AddDays(-30);
            var nowFinish = DateTime.Now.Date;
            foreach (var workLog in workLogs)
            {
                if (nowStart < workLog.DateOfReport && workLog.DateOfReport <= nowFinish)
                {
                    //workLog.ProjName = FindProjById(workLog.Proj_id);
                    workLog.TaskName = TaskComp.FindTaskNameById(workLog.TaskComp_id, context);
                    workLogsFiltered.Add(workLog);
                }
            }
            return workLogsFiltered;
        }

        public TaskComp GetTaskCompById(AppDbContext context)
        {
            var res = Int32.TryParse(TaskComp_id, out int idInt);
            if (res)
            {
                var taskCompSet = context.TaskComps.Where(x => x.Id == idInt);
                if (taskCompSet.Count() > 0) return taskCompSet.First();
            }
            return null;
        }

        public string GetTaskCompName(AppDbContext context)
        {
            var res = Int32.TryParse(TaskComp_id, out int idInt);
            if (res)
            {
                var taskCompSet = context.TaskComps.Where(x => x.Id == idInt);
                if (taskCompSet.Count() > 0) return taskCompSet.First().TaskCompName;
            }
            return string.Empty;
        }

        public static List<WorkLogs> GetWorkLogsLast30(AppDbContext context, User curUser, out TimeSpan wlCurDay, out Dictionary<DateTime, TimeSpan> wlCurMonth, out Dictionary<DateTime, TimeSpan> wlPrevMonth)
        {
            var startDate = DateTime.Now.AddDays(-30);
            var workLogsSet = context.WorkLogs.Where(x => x.User == curUser).Where(x => x.DateOfReport > startDate && x.DateOfReport <= DateTime.Now.Date).Include(x => x.KindOfAct);

            foreach (var workLog in workLogsSet)
            {
                var taskComp = TaskComp.FindTaskCompById(workLog.TaskComp_id, context);
                if (taskComp != null)
                {
                    workLog.ProjName = taskComp.ProjectNumber;
                    workLog.TaskName = taskComp.TaskCompName;
                }
            }
            var workLogsList = workLogsSet.ToList();
            workLogsList.Reverse();
            wlCurDay = SumWorkLogs(workLogsList.Where(x => x.DateOfReport.Date == DateTime.Now.Date));
            wlCurMonth = GetWlMonth(workLogsList, DateTime.Now.Month);
            wlPrevMonth = new Dictionary<DateTime, TimeSpan>();
            if (DateTime.Now.Month != startDate.Month)
            {
                wlPrevMonth = GetWlMonth(workLogsList, startDate.Month);
            }
            return workLogsList;

        }

        public static List<WorkLogs> GetWorkLogsByMonth(AppDbContext context, User curUser, int month, int year ,out TimeSpan wlCurDay, out Dictionary<DateTime, TimeSpan> wlCurMonth, out Dictionary<DateTime, TimeSpan> wlPrevMonth)
        {
            var startDate = new DateTime(DateTime.Now.Year, month, 1);
            var finishDate = new DateTime(DateTime.Now.Year, month, GetNumOfDayInMonth(month));
            var workLogsSet = context.WorkLogs.Where(x => x.User == curUser).Where(x => x.DateOfReport.Month == month && x.DateOfReport.Year==year).Include(x => x.KindOfAct);
            //var workLogsSet = context.WorkLogs.Where(x => x.User == curUser).Where(x => x.DateOfReport >= startDate && x.DateOfReport <= finishDate).Include(x => x.KindOfAct);
            foreach (var workLog in workLogsSet)
            {
                var taskComp = TaskComp.FindTaskCompById(workLog.TaskComp_id, context);
                if (taskComp != null)
                {
                    workLog.ProjName = taskComp.ProjectNumber;
                    workLog.TaskName = taskComp.TaskCompName;
                }
            }
            var workLogsList = workLogsSet.ToList();
            workLogsList.Reverse();
            wlCurDay = SumWorkLogs(workLogsList.Where(x => x.DateOfReport.Date == DateTime.Now.Date));
            wlCurMonth = GetWlMonth(workLogsList, month);
            wlPrevMonth = new Dictionary<DateTime, TimeSpan>();
            if (DateTime.Now.Month != startDate.Month)
            {
                wlPrevMonth = GetWlMonth(workLogsList, startDate.Month);
            }
            return workLogsList;

        }

        static Dictionary<DateTime, TimeSpan> GetWlMonth(List<WorkLogs> workLogs, int month)
        {
            Dictionary<DateTime, TimeSpan> curMonthWorkLogs = new Dictionary<DateTime, TimeSpan>();
            var workLogsSet = workLogs.Where(x => x.DateOfReport.Month == month);
            var workLogsGroupedByDate = workLogsSet.GroupBy(x => x.DateOfReport).OrderBy(x => x.Key);
            foreach (var workLogsGr in workLogsGroupedByDate)
            {
                var totalCurDay = SumWorkLogs(workLogsGr);
                curMonthWorkLogs.Add(workLogsGr.Key, totalCurDay);
            }
            return curMonthWorkLogs;
        }

        static TimeSpan SumWorkLogs(IEnumerable<WorkLogs> workLogs)
        {
            TimeSpan totalWorkTime = new TimeSpan();
            foreach (var workLog in workLogs)
            {
                totalWorkTime += workLog.WorkTime;
            }
            return totalWorkTime;
        }
        public static TimeSpan GetTotalHoursWorkLogSet(IGrouping<string, WorkLogs> workLogs)
        {

            TimeSpan totalWT = new TimeSpan();
            foreach (var wl in workLogs)
            {
                totalWT += wl.WorkTime;
            }
            return totalWT;


        }

        public static double GetTotalWTSHouldBe(AppDbContext context)
        {
            List<int> workHourList = new List<int>();
            int workHours = 0;
            for (int i = 1; i <= DateTime.Now.Day; i++)
            {
                var daysOffList = context.DayOffs.ToList().Select(x => x.Date);
                var checkDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, i);
                if (checkDate.DayOfWeek != DayOfWeek.Saturday && checkDate.DayOfWeek != DayOfWeek.Sunday && !daysOffList.Contains(checkDate))
                {
                    if (CheckData.DateConverter.CheckIfBeforeDayOff(checkDate, daysOffList))
                    {
                        workHourList.Add(7);
                    }
                    else
                    {
                        workHourList.Add(8);
                    }
                }
            }
            if (DateTime.Now.Hour < 17)
            {
                workHourList.RemoveAt(workHourList.Count - 1);
            }

            return workHourList.Sum(x => x);
        }
        public static double GetTotalWTSHouldBe(AppDbContext context, int month, int year)
        {
            List<int> workHourList = new List<int>();            
            var curMonth = DateTime.Now.Month;
            int dayAmount = 0;
            if (month == curMonth)
            {
                dayAmount = DateTime.Now.Day;
            }
            else
            {
                dayAmount = GetNumOfDayInMonth(month);
            }
            var daysOffList = context.DayOffs.ToList().Select(x => x.Date);
            for (int i = 1; i <= dayAmount; i++)
            {
                DateConverter.SumWorkDay(workHourList,year , month, i, daysOffList);               
            }
            if (DateTime.Now.Hour < 17 && month == curMonth)
            {
                workHourList.RemoveAt(workHourList.Count - 1);
            }

            return workHourList.Sum(x => x);
        }

        static int GetNumOfDayInMonth (int month){
            switch (month)
            {
                case 1:
                    return 31;

                case 2:
                    return 28;

                case 3:
                    return 31;

                case 4:
                    return 30;

                case 5:
                    return 31;

                case 6:
                    return 30;

                case 7:
                    return 31;

                case 8:
                    return 31;

                case 9:
                    return 30;

                case 10:
                    return 31;

                case 11:
                    return 30;

                case 12:
                    return 31;
            }
            return 0;
        }

        public static double GetTotalWTSHouldBeExactMonth(AppDbContext context, int month )
        {
            List<int> workHourList = new List<int>();
            int workHours = 0;
            var numOfDays = CheckData.DateConverter.GetNumberOfDaysInMonth(month);
            for (int i = 1; i <= numOfDays; i++)
            {
                var daysOffList = context.DayOffs.ToList().Select(x => x.Date);
                var checkDate = new DateTime(DateTime.Now.Year, month, i);
                if (checkDate.DayOfWeek != DayOfWeek.Saturday && checkDate.DayOfWeek != DayOfWeek.Sunday && !daysOffList.Contains(checkDate))
                {
                    if (CheckData.DateConverter.CheckIfBeforeDayOff(checkDate, daysOffList))
                    {
                        workHourList.Add(7);
                    }
                    else
                    {
                        workHourList.Add(8);
                    }
                }
            }
            return workHourList.Sum(x => x);
        }

        public static List<WorkLogs> GetWorkLogsByMonth(AppDbContext context, int month, int year)
        {
            return context.WorkLogs.Where(x => x.DateOfReport.Month == month && x.DateOfReport.Year==year).Where(x => x.Proj_id.Trim() != "0" || x.Proj_id!="0000" || x.Proj_id!="Обучение").Where(x => x.TaskComp_id != "undefined").Include(x => x.User).ThenInclude(y=>y.Department).Include(x => x.KindOfAct).ToList();
        }

        public static bool SetExecuterByWL(AppDbContext context, List<string> errorMes)
        {
            foreach (var wlGrByTaskComp in context.WorkLogs.Include(x=>x.User).GroupBy(x=>x.TaskComp_id))
            {
                var tc = TaskComp.FindTaskCompById(wlGrByTaskComp.Key, context);
                if (tc != null)
                {
                    var userList = FindExecuters(wlGrByTaskComp);
                    foreach (var user in userList)
                    {
                        user.AddUserToTask(tc);
                    }
                }               
            }
            context.SaveChanges();
            return true;
        }

        static List<User> FindExecuters(IGrouping<string,WorkLogs> wlGrByTaskComp)
        {
            List<User> userList = new List<User>();
            foreach (var wlGr in wlGrByTaskComp)
            {
                var user = wlGr.User;

                if (user != null && !userList.Contains(wlGr.User)){
                    userList.Add(user);
                }
            }
            return userList;
        }

        public static Dictionary<string, List<ProjectShares>> GetWlProjectShareGroupedByDepart(IQueryable<WorkLogs> wlSet, AppDbContext context)
        {
            Dictionary<string, List<ProjectShares>> psGrByDepart= new Dictionary<string, List<ProjectShares>>();

            var WlSetWithDepart = wlSet.Select(x => new { Depart = x.User.PublicDepart, Wl = x }).GroupBy(x=>x.Depart);
            foreach (var wlSetWithDep in WlSetWithDepart)
            {
                if (wlSetWithDep.Key!=null) {
                    List<ProjectShares> ps = new List<ProjectShares>();
                    var totWt = wlSetWithDep.Select(x => x.Wl).Sum(x => x.WorkTime.TotalHours);
                    var wlGrByProj = wlSetWithDep.Select(x => x.Wl).GroupBy(x => x.Proj_id);
                    ps.AddRange(wlGrByProj.Select(x => new ProjectShares
                    {
                        ProjectShare = x.Sum(y => y.WorkTime.TotalHours) / totWt,
                        ProjectNumber = x.Key
                    }));
                    psGrByDepart.Add(wlSetWithDep.Key, ps);
                }
                else
                {

                }
            }
            return psGrByDepart;
        }

        public static Dictionary<string, List<ProjectShares>> GetWlProjectShareGroupedByDepartTaskComp(IQueryable<WorkLogs> wlSet, AppDbContext context, List<TaskComp> taskCompSet)
        {
            Dictionary<string, List<ProjectShares>> psGrByDepart = new Dictionary<string, List<ProjectShares>>();

            var WlSetWithDepart = wlSet.Select(x => new { Depart = x.User.PublicDepart, Wl = x }).GroupBy(x => x.Depart);
            foreach (var wlSetWithDep in WlSetWithDepart)
            {
                if (wlSetWithDep.Key != null)
                {
                    List<ProjectShares> ps = new List<ProjectShares>();
                    var totWt = wlSetWithDep.Select(x => x.Wl).Sum(x => x.WorkTime.TotalHours);
                    var wlGrByProj = wlSetWithDep.Select(x => x.Wl).GroupBy(x => x.TaskComp_id);
                    ps.AddRange(wlGrByProj.Select(x => new ProjectShares
                    {
                        ProjectShare = x.Sum(y => y.WorkTime.TotalHours) / totWt,
                        TaskCompName = taskCompSet.Single(y=>y.Id.ToString()==x.Key).TaskCompName,
                        ProjectNumber = x.First().Proj_id
                    }));
                    psGrByDepart.Add(wlSetWithDep.Key, ps);
                }
                else
                {

                }
            }
            return psGrByDepart;
        }
    }
}
            
    

