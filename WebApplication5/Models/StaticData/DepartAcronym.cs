using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models.StaticData
{
    public static class DepartAcronym
    {
        //public static readonly Dictionary<string,string> subsidiary = new Dictionary<string, string>()
        
        public static readonly string MSK = "МскУП";
        public static readonly string SLV = "СлтУП";
        public static readonly string NK = "НкУП";
        public static readonly string ALM = "КОА";
        public static readonly string NNIZ = "ННУП";
        public static readonly string UFA = "УУП";
        public static readonly string ETO = "ЭТО";

        public static string[] GetAllSubsidiaryAcronyms()
        {
            string[] strAr = new string[5] { MSK, NK, ALM, NNIZ, UFA};
            return strAr;
        }
    }
}
