using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{
    public class AvevaPipeLength
    {
        public int Id { get; set; }
        
        public Project Project { get; set; }

        public DateTime Date { get; set; }

        public int PipeLineLength { get; set; }
        public int PipeLineBore { get; set; }

        public string ProjectAcr { get; set; }


    }
}
