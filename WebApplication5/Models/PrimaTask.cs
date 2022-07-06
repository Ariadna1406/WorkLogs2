using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{
    public class PrimaTask
    {
        [Key]
        public int Id { get; set; }
        public string TaskName { get; set; }

        public int ProjId { get; set; } 

        
    }
}
            
    

