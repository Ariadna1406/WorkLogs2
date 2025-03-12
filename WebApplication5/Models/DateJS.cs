using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{
    public class StringConverter
    {              

        public static DateTime? GetDate(string date)
        {
            if (date != null)
            {
                var dateFormParse = date.Split('T').First();
                var res = DateTime.TryParse(dateFormParse, out DateTime dateRes);
                if (res) return dateRes;
                
            }
            return null;
        }

        public static Double? GetDouble(string val)
        {            
            var res = Double.TryParse(val, out double valDouble);
            if (res) return valDouble;
            else return null;
        }
    }
}
