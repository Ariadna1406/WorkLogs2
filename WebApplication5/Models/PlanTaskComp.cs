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

        public static explicit operator PlanTaskComp(PlanTaskCompJson taskCompJson)
        {
            return new PlanTaskComp()
            {
                //id = task.Id,
                //project = task.ProjectNumber != null ? task.ProjectNumber : "",
                //text = task.TaskCompName != null ? task.TaskCompName : "",
                //start_date = task.StartPlanDate != null ? task.StartPlanDate.Value.ToString("dd-MM-yyyy") : null,
                //end_date = task.FinishPlanDate != null ? task.FinishPlanDate.Value.ToString("dd-MM-yyyy") : null,
                //parent = null,
                //type = string.Empty,
                //progress = task.CompletePercent
            };
        }

    }

}
            
    

