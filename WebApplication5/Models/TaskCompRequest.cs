using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{
    public class TaskCompRequest
    {
        [Key]
        public int Id { get; set; }

        public string ProjectNumber { get; set; }

        public string TaskCompName { get; set; }
        public string Comment { get; set; }

        public User User { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? FinishDate { get; set; }

        public double? PlanWorkLog { get; set; }

        public DateTime? DateOfRequest { get; set; }

        public Status TaskCompStatus { get; set; }

        public string DenyComment { get; set; }

        [NotMapped]
        public bool NeedToImport { get; set; }
        

        public enum Status { New, Read, Confirmed, Declined }

        public TaskCompRequest(string projectNumber, string taskCompName, string comment, User user)
        {
            ProjectNumber = projectNumber;
            TaskCompName = taskCompName;
            Comment = comment;
            this.User = user;
            DateOfRequest = DateTime.Now;
            TaskCompStatus = Status.New;
        }

        public TaskCompRequest(string projectNumber, string taskCompName, DateTime? startDate, DateTime? finishDate, double? workLogPlan, string comment, User user)
        {
            ProjectNumber = projectNumber;
            TaskCompName = taskCompName;
            Comment = comment;
            StartDate = startDate;
            FinishDate = finishDate;
            PlanWorkLog = workLogPlan;
            this.User = user;
            DateOfRequest = DateTime.Now;
            TaskCompStatus = Status.New;
            
        }

        public TaskCompRequest() { }

        public static TaskCompRequest FindTaskCompRequestByName(AppDbContext context, string taskCompRequestName)
        {
            taskCompRequestName = taskCompRequestName.Trim();
            var tcrSet = context.TaskCompRequests.Where(x => x.TaskCompName == taskCompRequestName);
            if (tcrSet.Count() > 0)
            {
                return tcrSet.First();
            }
            return null;
        }

        public static List<TaskCompRequest> GetElemsByIds(AppDbContext context, string[] tcrAr)
        {
            List<TaskCompRequest> tcrList = new List<TaskCompRequest>();
            foreach (var tcrIdStr in tcrAr)
            {
                var res = Int32.TryParse(tcrIdStr, out int tcrId);
                if (res)
                {
                    var tcrSet = context.TaskCompRequests.Where(x => x.Id == tcrId).Include(x=>x.User);
                    if (tcrSet.Count() > 0)
                    {
                        var tcr = tcrSet.First();
                        tcrList.Add(tcr);
                    }
                }
            }
            return tcrList;
        }

        public static List<TaskCompRequest> GetElemsByIds(AppDbContext context, TaskCompRequest[] tcrAr)
        {
            List<TaskCompRequest> tcrList = new List<TaskCompRequest>();
            foreach (var tcrFormWeb in tcrAr)
            {                
                    var tcrSet = context.TaskCompRequests.Where(x => x.Id == tcrFormWeb.Id).Include(x => x.User);
                    if (tcrSet.Count() > 0)
                    {
                        var tcr = tcrSet.First();
                    if (!string.IsNullOrEmpty(tcrFormWeb.DenyComment))
                    {
                        tcr.DenyComment = tcrFormWeb.DenyComment;
                    }
                        tcrList.Add(tcr);
                    }                
            }
            return tcrList;
        }

        public static List<TaskCompRequest> GetElemsByIds(AppDbContext context, IEnumerable<int> tcrIdSet)
        {
            List<TaskCompRequest> tcrList = new List<TaskCompRequest>();
            foreach (var tcrId in tcrIdSet)
            {
                var tcrSet = context.TaskCompRequests.Where(x => x.Id == tcrId).Include(x => x.User).ThenInclude(x=>x.Department);
                if (tcrSet.Count() > 0)
                {
                    var tcr = tcrSet.First();
                    tcrList.Add(tcr);
                }
            }
            return tcrList;
        }

        public static TaskCompRequest[] GetAllTaskCompRequest(AppDbContext context)
        {
            var tcrAr = context.TaskCompRequests.Include(x=>x.User).ToArray();
            return tcrAr;
        }

        public static string GetAmountOfTaskCompRequestStr(AppDbContext context)
        {
            var tcrAr = context.TaskCompRequests.ToArray().Length.ToString();
            return tcrAr;
        }

        public static string GetAmountOfNewTaskCompRequestStr(AppDbContext context)
        {
            var tcrAr = context.TaskCompRequests.Where(x=>x.TaskCompStatus==Status.New).Count().ToString();
            return tcrAr;
        }

        public static TaskCompRequest[] GetTaskCompRequestByStatus(AppDbContext context, Status status)
        {
            var tcrAr = context.TaskCompRequests.Include(x=>x.User).Where(x=>x.TaskCompStatus==status).ToArray();
            return tcrAr;
        }

        public string GetStatusRus()
        {
            switch (TaskCompStatus)
            {
                case Status.New:
                    return "Новая";
                case Status.Read:
                    return "Прочитано (решение не принято)";               
                case Status.Confirmed:
                    return "Подтверждено (комплект создан)";
                case Status.Declined:
                    return "Отклонено";
            }
            return string.Empty;
        }

        public bool ChangeStatus(AppDbContext context, Status status)
        {
            var tcrSet = context.TaskCompRequests.Where(x => x.Id == Id);
            if (tcrSet.Count() > 0)
            {
                tcrSet.First().TaskCompStatus = status;
                return true;
            }
            return false;
        }

        public bool ChangeStatus(Status status)
        {
            TaskCompStatus = status;
            return true;
        }
    }
}
