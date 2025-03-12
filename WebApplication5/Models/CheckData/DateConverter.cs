using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models.CheckData
{
    public class DateConverter
    {
        public static int GetNumberOfDaysInMonth(int month)
        {
            if (month>0 && month <= 12)
            {
                switch (month)
                {
                    case 1:
                        return 31;
                    case 2:
                        return 28;
                    case 3:
                        return 31;
                    case 4:
                        return 30;
                    case 5:
                        return 31;
                    case 6:
                        return 30;
                    case 7:
                        return 31;
                    case 8:
                        return 31;
                    case 9:
                        return 30;
                    case 10:
                        return 31;
                    case 11:
                        return 30;
                    case 12:
                        return 31;
                }
            }
            return 0;
        }

        public static bool CheckIfBeforeDayOff(DateTime date, IEnumerable<DateTime> dayOffList)
        {
            var nextDate = date.AddDays(1);
            if (dayOffList.Contains(nextDate))
            {
                return true;
            }
            return false;
        }

        public static void SumWorkDay(List<int> workHourList, int month, int day, IEnumerable<DateTime> daysOffList)
        {
            var checkDate = new DateTime(DateTime.Now.Year ,month, day);
            if (checkDate.DayOfWeek != DayOfWeek.Saturday && checkDate.DayOfWeek != DayOfWeek.Sunday && !daysOffList.Contains(checkDate))
            {
                if (CheckData.DateConverter.CheckIfBeforeDayOff(checkDate, daysOffList))
                {
                    workHourList.Add(7);
                }
                else
                {
                    workHourList.Add(8);
                }
            }
        }

        public static void SumWorkDay(List<int> workHourList, int year ,int month, int day, IEnumerable<DateTime> daysOffList)
        {            
            var checkDate = new DateTime(year, month, day);
            if (checkDate.DayOfWeek != DayOfWeek.Saturday && checkDate.DayOfWeek != DayOfWeek.Sunday && !daysOffList.Contains(checkDate))
            {
                if (CheckData.DateConverter.CheckIfBeforeDayOff(checkDate, daysOffList))
                {
                    workHourList.Add(7);
                }
                else
                {
                    workHourList.Add(8);
                }
            }
        }

    }
}
