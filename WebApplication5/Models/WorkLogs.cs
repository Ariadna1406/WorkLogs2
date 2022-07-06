using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{
    public class WorkLogs
    {
        [Key]
        public int Id { get; set; }
        public User User { get; set; }

        public string Proj_id { get; set; }
        [NotMapped]
        public string ProjName { get; set; }       

        public KindOfAct KindOfAct { get; set; }

        public DateTime DateOfReport { get; set; }

        public DateTime DateOfSendingReport { get; set; }
        public string Comment { get; set; }

        public string Task_id { get; set; }
        [NotMapped]
        public string TaskName { get; set; }

        [Required(ErrorMessage = "Требуется заполнить кол-во отработанного времени")]
        public TimeSpan WorkTime { get; set; }

        public WorkLogs(User userId, string proj_id, KindOfAct kindOfAct, DateTime dateOfReport, string task_id, TimeSpan workTime, string comment, DateTime dateOfSendingReport)
        {
            User = userId;
            Proj_id = proj_id;
            KindOfAct = kindOfAct;
            DateOfReport = dateOfReport;
            Task_id = task_id;
            WorkTime = workTime;
            Comment = comment;
            DateOfSendingReport = dateOfSendingReport;
        }
        public WorkLogs()
        {            
        }
    }
}
            
    

