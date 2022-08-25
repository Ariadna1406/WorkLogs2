using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{
    public class TeklaElemAmount
    {
        public int Id { get; set; }
        
        public Project Project { get; set; }

        public DateTime Date { get; set; }

        public int ElemAmount { get; set; }

        public string ProjectAcr { get; set; }


    }
}
