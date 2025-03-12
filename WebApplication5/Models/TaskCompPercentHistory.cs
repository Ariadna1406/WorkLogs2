using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{
    public class TaskCompPercentHistory
    {
        public int Id { get; set; }
        public TaskComp TaskComp { get; set; }
        public DateTime ChangePercentDate { get; set; }
        public double Percent { get; set; }
        public User User { get; set; }
        
        public TaskCompPercentHistory() { }

        public TaskCompPercentHistory(TaskComp taskComp, double percent, User user) 
        {
            TaskComp = taskComp;
            Percent = percent;
            User = user;
            ChangePercentDate = DateTime.Now;
        }


    }
}
