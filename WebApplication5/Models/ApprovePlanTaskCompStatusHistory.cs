using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{
    public class ApprovePlanTaskCompStatusHistory
    {
        public int Id { get; set; }
        public ApprovePlanTaskComp ApprovePlanTaskComp { get; set; }
        public DateTime ChangedStatusDate { get; set; }
        public ApprovePlanTaskComp.Status Status { get; set; }
        public User UserChangedStatus { get; set; }
        
        public ApprovePlanTaskCompStatusHistory() { }

        public ApprovePlanTaskCompStatusHistory(ApprovePlanTaskComp approvePlanTaskComp, ApprovePlanTaskComp.Status status, User user) 
        {
            ApprovePlanTaskComp = approvePlanTaskComp;
            ChangedStatusDate = DateTime.Now;
            UserChangedStatus = user;
            Status = status;

        }


    }
}
