﻿using Aspose.Cells;
using DHX.Gantt.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{

    public class ApprovePlanTaskComp
    {
        public int Id { get; set; }
        public User UserCreatedRequest { get; set; }
        public int PlanMonth { get; set; }
        public int PlanYear { get; set; }
        public string RejectComment { get; set; }
        public string RejectedUser { get; set; }
        //public string SEDLink { get; set; }


        public Status PlanTaskCompStatus { get; private set; }
        public enum Status { New, SentToApprove, Read, Confirmed, Declined, NotFound }

        public string GetStatusRus()
        {
            switch (PlanTaskCompStatus)
            {
                case Status.New:
                    return "Неутверждённый";
                case Status.SentToApprove:
                    return "Отправлен на утверждение";
                case Status.Confirmed:
                    return "Утверждён";
                case Status.Declined:
                    return "Отклонен";
            }
            return string.Empty;
        }

        public static string GetStatusRus(Status status)
        {
            switch (status)
            {
                case Status.New:
                    return "Неутверждённый";
                case Status.SentToApprove:
                    return "Отправлен на утверждение";
                case Status.Confirmed:
                    return "Утверждён";
                case Status.Declined:
                    return "Отклонен";
                default:
                    return string.Empty;
            }
        }

        public ApprovePlanTaskComp() { }

        public ApprovePlanTaskComp(User userCreatedRequest, int planMonth, int planYear)
        {
            UserCreatedRequest = userCreatedRequest;
            PlanMonth = planMonth;
            PlanYear = planYear;
            PlanTaskCompStatus = Status.New;
        }

        public static Models.ApprovePlanTaskComp.Status GetStatusFromStr(string status)
        {
            switch (status)
            {
                case "Неутверждённый":
                    return Status.New;
                case "Отправлен на утверждение":
                    return Status.SentToApprove;
                case "Утверждён" :
                    return Status.Confirmed;
                case "Отклонен":
                    return Status.Declined;
                default:
                    return Status.NotFound;
            }
           
        }

        public static ApprovePlanTaskComp FindById(int id, AppDbContext context, out string errors)
        {
            errors = string.Empty;
            var aptc = context.ApprovePlanTaskComp.First(x => x.Id == id);
            if (aptc == null) errors += $"Не найден ApprovePlanTaskComp с id {id.ToString()}";
            return aptc;
        }

        public static ApprovePlanTaskComp ConvertFrom(ApprovePlanTaskCompJson approvePlanTaskCompJson, AppDbContext appDbContext)
        {
            return new ApprovePlanTaskComp
            {
                UserCreatedRequest = User.GetUserById(appDbContext, approvePlanTaskCompJson.authorId),
                PlanMonth = approvePlanTaskCompJson.planMonth,
                PlanYear = approvePlanTaskCompJson.planYear
            };
        }

        public static List<ApprovePlanTaskComp> GetAllWithStatusSendToApprove(AppDbContext context)
        {
            var validStatuses = new[]
            {
                ApprovePlanTaskComp.Status.SentToApprove,
                ApprovePlanTaskComp.Status.Confirmed,
                ApprovePlanTaskComp.Status.Declined
            };

            var aptcForApprove= context.ApprovePlanTaskComp.Include(x=>x.UserCreatedRequest).Where(x => validStatuses.Contains(x.PlanTaskCompStatus)).ToList();
            return aptcForApprove;
        }

        static void FindTaskComp(int taskCompId, AppDbContext context, out TaskComp task)
        {
            var taskCompSet = context.TaskComps.Where(x => x.Id == taskCompId);
            if (taskCompSet.Count() > 0)
            {
                task = taskCompSet.First();
                return;
            }
            throw new Exception($"Задача с {taskCompId} не найдена");

        }

        static void FindKindOfAct(int kindOfActId, AppDbContext context, out KindOfAct kindOfAct)
        {
            var kindOfActSet = context.KindOfAct.Where(x => x.Id == kindOfActId);
            if (kindOfActSet.Count() > 0)
            {
                kindOfAct = kindOfActSet.First();
                return;
            }
            throw new Exception($"KindOfAct с {kindOfActId} не найден");

        }

        static void FindPlanTaskComp(int planTaskCompId, AppDbContext context, out PlanTaskComp planTaskComp)
        {
            var planTaskCompSet = context.PlanTaskComp.Where(x => x.Id == planTaskCompId);
            if (planTaskCompSet.Count() > 0)
            {
                planTaskComp = planTaskCompSet.First();
                return;
            }
            throw new Exception($"Задача с {planTaskCompId} не найдена");

        }

        public static bool SendToApprove(List<PlanTaskCompJson> planTaskCompJsonList, AppDbContext context, out string errors)
        {
            errors = string.Empty;
            if (planTaskCompJsonList.Count == 0) { 
                errors += "Список задач для сохранения пуст";
                return false;
            }
            var planTaskCompJsonFirst = planTaskCompJsonList.First();
            DateTime.TryParse(planTaskCompJsonFirst.startPlanDate, out DateTime startPlanDateParsed);            
            var author = User.GetUserById(context, planTaskCompJsonFirst.authorId);
            //Ищем объект ApprovePlanTaskComp за необходимый месяц, если его нет, то создаём новую 
            var approvePlanTaskCompSet = context.ApprovePlanTaskComp.Where(x => x.UserCreatedRequest == author && x.PlanMonth == startPlanDateParsed.Month && x.PlanYear == startPlanDateParsed.Year);
            if (approvePlanTaskCompSet.Count() > 0)
            {
                var aptc = approvePlanTaskCompSet.First();
                aptc.UserCreatedRequest = author;
                
                aptc.ChangeStatus(author, ApprovePlanTaskComp.Status.SentToApprove, context, out string errorChangeStatus);
            }
            else
            {
                var newApprovePlanTaskComp = new ApprovePlanTaskComp()
                {
                    UserCreatedRequest = author,
                    PlanMonth = startPlanDateParsed.Month,
                    PlanYear = startPlanDateParsed.Year                    
                };
                newApprovePlanTaskComp.ChangeStatus(author, ApprovePlanTaskComp.Status.SentToApprove, context, out string errorChangeStatus);
                context.ApprovePlanTaskComp.Add(newApprovePlanTaskComp);
            }
            context.SaveChanges();
            return true;
        }

        public bool ChangeStatus(User user, ApprovePlanTaskComp.Status status, AppDbContext context, out string errors)
        {
            errors = string.Empty;            
            PlanTaskCompStatus = status;
            var newHistRecord = new ApprovePlanTaskCompStatusHistory(this, status, user);
            context.ApprovePlanTaskCompStatusHistories.Add(newHistRecord);
            return true;
        }

        public static Status GetApprovePlanTaskCompStatus(int planMonth, int planYear, AppDbContext context)
        {
            var approvePlanTaskCompSet= context.ApprovePlanTaskComp.FirstOrDefault(x => x.PlanMonth == planMonth && x.PlanYear == planYear);
            if (approvePlanTaskCompSet == null)
            {
                return Status.New;
            }
            else
            {
                return approvePlanTaskCompSet.PlanTaskCompStatus;
            }
        }
        //public static List<PlanTaskCompJson> GetApprovePlanTaskCompCurUser(User curUser, int month, AppDbContext context)
        //{
        //    var planTaskCompSet = context.ApprovePlanTaskComp.Include(x => x.Author).Include(x => x.TaskComp).Include(x => x.Executer).Include(x=>x.KindOfAct);
        //    var planTaskCompSetFiltered = planTaskCompSet.Where(x => x.Author == curUser).Where(x=>x.StartPlanDate.Month==month);
        //    var planTaskCompJsonSet = planTaskCompSetFiltered.ToTaskCompJsonList();           
        //    return planTaskCompJsonSet;

        //}
    }
}
            
    

