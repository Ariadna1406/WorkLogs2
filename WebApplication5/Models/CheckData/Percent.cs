using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models.CheckData
{
    public class Percent
    {
        public static bool CheckPercent(string percentStr, List<string> errorMes)
        {
            var res = Double.TryParse(percentStr, out double percent);
            if (res || string.IsNullOrEmpty(percentStr))
            {
                if (percent > 100 || percent < 0)
                {
                    errorMes.Add($"Процент должен быть в диапазоне от 0 до 100.");
                }
                else
                {                    
                    return true;
                }
            }
            else
            {
                errorMes.Add($"Невозможно преобразовать в число \" {percentStr}\" ");                

            }
            return false;
        }
    }
}
