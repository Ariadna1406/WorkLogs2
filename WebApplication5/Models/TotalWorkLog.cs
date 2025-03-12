using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{
    public class TotalWorkLog
    {
        public string DepartAcr { get; set; }
        public User User { get; set; }
     public double TotalSendedWorkLogs { get; set; }
     public double TotalWorkLogsShouldBe { get; set; }

        public TotalWorkLog(string departAcr, User user, double totalSendedWorkLogs, double totalWorkLogsShouldBe)
        {
            DepartAcr = departAcr;
            User = user;
            TotalSendedWorkLogs = totalSendedWorkLogs;
            TotalWorkLogsShouldBe = totalWorkLogsShouldBe;
        }

        public TotalWorkLog(User user, double totalSendedWorkLogs, double totalWorkLogsShouldBe)
        {            
            User = user;
            TotalSendedWorkLogs = totalSendedWorkLogs;
            TotalWorkLogsShouldBe = totalWorkLogsShouldBe;
        }
    }
}
