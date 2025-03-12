using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{
    public class ProjectSharesTaskCompTot
    {       
        public string ProjectNumber { get; set; }
        public double ProjectShare { get; set; }
        public double TotalWLVal { get; set; }
        public string TaskCompName{get;set;}

        public ProjectSharesTaskCompTot(string projectNum, double projectShare, double totalWlVal, string taskCompName)
        {
            ProjectNumber = projectNum;
            ProjectShare = projectShare;
            TotalWLVal = totalWlVal;
            TaskCompName = taskCompName;
        }

       

      
    }
}
