using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{
    public class DayOff
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }

        public DayOff(DateTime date)
        {
            Date = date;
        }
    }
}
