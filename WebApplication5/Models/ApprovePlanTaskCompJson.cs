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

    public class ApprovePlanTaskCompJson
    {
        public int id { get; set; }
        public int authorId { get; set; }
        public int planMonth { get; set; }
        public int planYear { get; set; }


        public ApprovePlanTaskCompJson() { }

        
    }       
}
            
    

