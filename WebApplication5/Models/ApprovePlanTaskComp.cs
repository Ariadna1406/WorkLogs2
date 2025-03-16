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

        public Status PlanTaskCompStatus { get; set; }
        public enum Status { New, Read, Confirmed, Declined }

        public string GetStatusRus()
        {
            switch (PlanTaskCompStatus)
            {
                case Status.New:
                    return "На согласовании";
                case Status.Read:
                    return "Прочитано (решение не принято)";
                case Status.Confirmed:
                    return "Подтверждено (комплект создан)";
                case Status.Declined:
                    return "Отклонено";
            }
            return string.Empty;
        }


        public ApprovePlanTaskComp() { }

        public ApprovePlanTaskComp(User userCreatedRequest, int planMonth, int planYear)
        {
            UserCreatedRequest = userCreatedRequest;
            PlanMonth = planMonth;
            PlanYear = planYear;
            PlanTaskCompStatus = Status.New;
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

        //public static List<PlanTaskCompJson> GetApprovePlanTaskCompCurUser(User curUser, int month, AppDbContext context)
        //{
        //    var planTaskCompSet = context.ApprovePlanTaskComp.Include(x => x.Author).Include(x => x.TaskComp).Include(x => x.Executer).Include(x=>x.KindOfAct);
        //    var planTaskCompSetFiltered = planTaskCompSet.Where(x => x.Author == curUser).Where(x=>x.StartPlanDate.Month==month);
        //    var planTaskCompJsonSet = planTaskCompSetFiltered.ToTaskCompJsonList();           
        //    return planTaskCompJsonSet;

        //}

       
    }

   
}
            
    

