using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{

    public class PlanTaskCompJson
    {
        public int id { get; set; }
        public int idDb { get; set; }
        public int taskCompId { get; set; }
        //public string work { get; set; }
        public string startPlanDate { get; set; }
        public string finishPlanDate { get; set; }
        public double intensity { get; set; }
        //public int planned_progress { get; set; }
        public int executerId { get; set; }
        public int authorId { get; set; }


        //public PlanTaskCompJson() { }
    }

}
            
    

