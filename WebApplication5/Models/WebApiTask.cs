using System;
using System.Text.Encodings.Web;
using WebApplication5.Models;

namespace DHX.Gantt.Models
{
    public class WebApiTask
    {
        public int id { get; set; }
        public string project { get; set; }
        public string text { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public double? progress { get; set; }
        public int? parent { get; set; }
        public string type { get; set; }
        public string target { get; set; }
        public bool open
        {
            get { return true; }
            set { }
        }

        public static explicit operator WebApiTask(TaskComp task)
        {
            return new WebApiTask
            {
                id = task.Id,
                project = task.ProjectNumber != null ? task.ProjectNumber : "",
                text = task.TaskCompName != null ? task.TaskCompName : "",
                start_date = task.StartPlanDate!=null ? task.StartPlanDate.Value.ToString("dd-MM-yyyy") : null,
                end_date = task.FinishPlanDate != null ? task.FinishPlanDate.Value.ToString("dd-MM-yyyy") : null,
                parent = null,
                type = string.Empty,
                progress = task.CompletePercent
            };
        }

        //public static explicit operator WebApiTask(TaskComp task)
        //{
        //    return new WebApiTask
        //    {
        //        id = task.Id,
        //        text = task.TaskCompName,
        //        duration = 4,
        //        progress = 0.4m,
        //        start_date = task.StartPlanDate.HasValue ? task.StartPlanDate.Value.ToString("yyyy-MM-dd HH:mm") : DateTime.Now.ToString("yyyy-MM-dd HH:mm")
        //    };
        //}

        //public static explicit operator Task(WebApiTask task)
        //{
        //    return new Task
        //    {
        //        Id = task.id,
        //        Text = task.text,
        //        StartDate = task.start_date != null ? DateTime.Parse(task.start_date,
        //            System.Globalization.CultureInfo.InvariantCulture) : new DateTime(),
        //        Duration = task.duration,
        //        ParentId = task.parent,
        //        Type = task.type,
        //        Progress = task.progress
        //    };
        //}
    }
}