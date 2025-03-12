using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{
    public class PlanDay
    {
        public PlanDay() { }
        public PlanDay(DateTime date)
        {
            Date = date;
        }
        public DateTime Date { get; set; }

        public bool IsDayOff()
        {
            if (Date.DayOfWeek == DayOfWeek.Saturday || Date.DayOfWeek == DayOfWeek.Sunday)
            {
                return true;
            }
            return false;
        }

        public string GetDayStr()
        {
           if (Date.Day < 10)
            {
                return $"0{Date.Day}";
            }
            else
            {
                return Date.Day.ToString();
            }
        }

        public string GetDay()
        {
            return Date.Day.ToString();
        }


    }
}
