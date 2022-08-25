using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{
    public class ProjectWithElemAmount
    {
        public Project Prj { get; set; }
        public int AvevaElemAmount { get; set; }
        public int TeklaElemAmount { get; set; }

        public ProjectWithElemAmount(Project prj, int avevaElemAmount, int teklaElemAmount)
        {
            Prj = prj;
            AvevaElemAmount = avevaElemAmount;
            TeklaElemAmount = teklaElemAmount;
        }
    }
}
