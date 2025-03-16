using Aspose.Cells;
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

    public class PlanTaskComp
    {
        public int Id { get; set; }
        public TaskComp TaskComp { get; set; }
        public KindOfAct KindOfAct { get; set; }
        public DateTime StartPlanDate { get; set; }
        public DateTime FinishPlanDate { get; set; }
        public double Intencity { get; set; }
        public int Percent { get; set; }
        public User Executer { get; set; }
        public User Author { get; set; }


        // public PlanTaskComp() { }

        public static PlanTaskComp Create(PlanTaskCompJson taskCompJson, AppDbContext context)
        {
            FindTaskComp(taskCompJson.taskCompId, context, out TaskComp taskComp);
            FindKindOfAct(taskCompJson.kindofactId, context, out KindOfAct kindOfAct);
            DateTime.TryParse(taskCompJson.startPlanDate, out DateTime startPlanDateParsed);
            DateTime.TryParse(taskCompJson.finishPlanDate, out DateTime finishPlanDateParsed);
            var author = User.GetUserById(context, taskCompJson.authorId);
            var executer = User.GetUserById(context, taskCompJson.executerId);

            return new PlanTaskComp()
            {
                TaskComp = taskComp,
                StartPlanDate = startPlanDateParsed,
                FinishPlanDate = finishPlanDateParsed,
                Author = author,
                Executer = executer,
                Intencity = taskCompJson.intensity,
                Percent= taskCompJson.percent,
                KindOfAct=kindOfAct
            };
        }

        public static void Update(PlanTaskCompJson taskCompJson, AppDbContext context)
        {
            FindPlanTaskComp(taskCompJson.idDb, context, out PlanTaskComp planTaskComp);
            FindTaskComp(taskCompJson.taskCompId, context, out TaskComp taskComp);
            FindKindOfAct(taskCompJson.kindofactId, context, out KindOfAct kindOfAct);
            DateTime.TryParse(taskCompJson.startPlanDate, out DateTime startPlanDateParsed);
            DateTime.TryParse(taskCompJson.finishPlanDate, out DateTime finishPlanDateParsed);
            var author = User.GetUserById(context, taskCompJson.authorId);
            var executer = User.GetUserById(context, taskCompJson.executerId);

            //Подменяем на новые свойства
            planTaskComp.TaskComp = taskComp;
            planTaskComp.StartPlanDate = startPlanDateParsed;
            planTaskComp.FinishPlanDate = finishPlanDateParsed;
            planTaskComp.Executer = executer;
            planTaskComp.Author = author;
            planTaskComp.Intencity = taskCompJson.intensity;
            planTaskComp.Percent = taskCompJson.percent;
            planTaskComp.KindOfAct = kindOfAct;
            context.SaveChanges();

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

        public static List<PlanTaskCompJson> GetPlanTaskCompCurUser(User curUser, int month, AppDbContext context)
        {
            var planTaskCompSet = context.PlanTaskComp.Include(x => x.Author).Include(x => x.TaskComp).Include(x => x.Executer).Include(x=>x.KindOfAct);
            var planTaskCompSetFiltered = planTaskCompSet.Where(x => x.Author == curUser).Where(x=>x.StartPlanDate.Month==month);
            var planTaskCompJsonSet = planTaskCompSetFiltered.ToTaskCompJsonList();           
            return planTaskCompJsonSet;

        }

       
    }

    public static class ExtensionPlanTaskComp {
        public static List<PlanTaskCompJson> ToTaskCompJsonList(this IQueryable<PlanTaskComp> planTaskCompList)
        {
            List<PlanTaskCompJson> planTaskCompJsonList = new List<PlanTaskCompJson>();
            foreach (var planTaskComp in planTaskCompList)
            {
                planTaskCompJsonList.Add(new PlanTaskCompJson()
                {
                    idDb = planTaskComp.Id,
                    authorId = planTaskComp.Author.Id,
                    executerId = planTaskComp.Executer.Id,
                    taskCompId = planTaskComp.TaskComp.Id,
                    startPlanDate = planTaskComp.StartPlanDate.ToString("dd-MM-yyyy"),
                    finishPlanDate = planTaskComp.FinishPlanDate.ToString("dd-MM-yyyy"),
                    kindofactId = planTaskComp.KindOfAct.Id,
                    intensity = planTaskComp.Intencity,
                    percent = planTaskComp.Percent
                });
            }
            return planTaskCompJsonList;
        }
    }
}
            
    

