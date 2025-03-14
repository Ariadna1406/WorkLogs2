using Aspose.Cells;
using DHX.Gantt.Models;
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
        public DateTime StartPlanDate { get; set; }
        public DateTime FinishPlanDate { get; set; }
        public double Intencity { get; set; }
        public User Executer { get; set; }
        public User Author { get; set; }


        // public PlanTaskComp() { }

        public static PlanTaskComp Create(PlanTaskCompJson taskCompJson, AppDbContext context)
        {
            FindTaskComp(taskCompJson.taskCompId, context, out TaskComp taskComp);
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
                Intencity = taskCompJson.intensity
            };
        }

        public static void Update(PlanTaskCompJson taskCompJson, AppDbContext context)
        {
            FindPlanTaskComp(taskCompJson.idDb, context, out PlanTaskComp planTaskComp);
            FindTaskComp(taskCompJson.taskCompId, context, out TaskComp taskComp);
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

        public static List<PlanTaskComp> GetPlanTaskCompCurUser(User curUser, AppDbContext context)
        {
            var planTaskCompList = context.PlanTaskComp.Where(x => x.Author == curUser).ToList();

            return planTaskCompList;
        
        }
    }
}
            
    

