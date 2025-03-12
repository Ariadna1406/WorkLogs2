using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{
    public class ProjectSharesTot
    {       
        public string ProjectNumber { get; set; }
        public double ProjectShare { get; set; }
        public double TotalWLVal { get; set; }
      

        public ProjectSharesTot(string projectNum, double projectShare, double totalWlVal)
        {
            ProjectNumber = projectNum;
            ProjectShare = projectShare;
            TotalWLVal = totalWlVal;           
        }

       

      
    }
}
