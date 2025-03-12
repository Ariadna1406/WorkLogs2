using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models.ExcelFiles
{
    public class FilePath
    {
        public static readonly string path = @"\\srv-ws\Задачи\Детальный план_факт НХП.xlsx";
        static readonly string pathForBuhFolder = @"\\srv-ws\buh\";
        static readonly string pathForGeneralFolder = @"\\srv-ws\Общая\";
        static readonly string pathForGIPFolder = @"\\srv-ws\ГИПы\";
        public static readonly string workLogsTemplatePath = @"\\srv-ws\WsSettings\WorkLogTemplate.xlsm";
        public static readonly string workLogsTaskCompTemplatePath = @"\\srv-ws\WsSettings\WorkLogTaskCompTemplate.xlsm";

        public static string GetPathForBuhMain(DateTime dateOfReport, string frontFileName)
        {
            string path = string.Empty;
            var fileRes = false;
            int count = 0;
            while (!fileRes) {                
                fileRes = GetPathForBuhSub(dateOfReport, count, out path, frontFileName);
                    count++;
                    }
            return path;
        }

        public static string GetPathForGIP(string frontFileName)
        {
            string path = string.Empty;
            var fileRes = false;
            int count = 0;
            while (!fileRes)
            {
                fileRes = GetPathForBuhGIPSub(count, out path, frontFileName);
                count++;
            }
            return path;
        }

        public static string GetPathForHOD(DateTime dateOfReport, string departAcr)
        {
            string path = string.Empty;
            var fileRes = false;
            int count = 0;
            while (!fileRes)
            {
                fileRes = GetPathForHODSub(dateOfReport, count, out path, departAcr);
                count++;
            }
            return path;
        }

        public static string GetPathForReport(DateTime dateOfReport)
        {
            string path = string.Empty;
            var fileRes = false;
            int count = 0;
            while (!fileRes)
            {
                fileRes = GetPathForWL(dateOfReport, count, out path);
                count++;
            }
            return path;
        }

        static bool GetPathForBuhSub (DateTime dateOfReport, int count, out string path, string frontFileName)
        {
            string fileName = string.Empty;
            if (count == 0)
            {
                fileName = $"{frontFileName}_{GetMonth(dateOfReport.Month)}-{dateOfReport.Year}.xlsx";
            }
            else
            {
                fileName = $"{frontFileName}_{GetMonth(dateOfReport.Month)}-{dateOfReport.Year}_{count}.xlsx";
            }
            path = pathForBuhFolder + fileName;
            var IsExist = File.Exists(path);
            if (!IsExist)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static bool GetPathForBuhGIPSub(int count, out string path, string frontFileName)
        {
            string fileName = string.Empty;
            if (count == 0)
            {
                fileName = $"{frontFileName}.xlsx";
            }
            else
            {
                fileName = $"{frontFileName}_{count}.xlsx";
            }
            path = pathForGIPFolder + fileName;
            var IsExist = File.Exists(path);
            if (!IsExist)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static bool GetPathForWL(DateTime dateOfReport, int count, out string path)
        {
            string fileName = string.Empty;
            if (count == 0)
            {
                fileName = $"Сводный_отчёт_за месяц_{GetMonth(dateOfReport.Month)}-{dateOfReport.Year}.xlsx";
            }
            else
            {
                fileName = $"Сводный_отчёт_за месяц__{GetMonth(dateOfReport.Month)}-{dateOfReport.Year}_{count}.xlsx";
            }
            path = pathForBuhFolder + fileName;
            var IsExist = File.Exists(path);
            if (!IsExist)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static bool GetPathForHODSub(DateTime dateOfReport, int count, out string path, string departAcr)
        {
            string fileName = string.Empty;
            if (count == 0)
            {
                fileName = $"Сводный_отчёт_за месяц_{departAcr}_{GetMonth(dateOfReport.Month)}-{dateOfReport.Year}.xlsx";
            }
            else
            {
                fileName = $"Сводный_отчёт_за месяц__{departAcr}_{GetMonth(dateOfReport.Month)}-{dateOfReport.Year}_{count}.xlsx";
            }
            path = pathForGeneralFolder + fileName;
            var IsExist = File.Exists(path);
            if (!IsExist)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string GetMonth(int month)
        {
            switch (month)
            {
                case 1:
                    return "Январь";

                case 2:
                    return "Февраль";

                case 3:
                    return "Март";

                case 4:
                    return "Апрель";

                case 5:
                    return "Май";

                case 6:
                    return "Июнь";

                case 7:
                    return "Июль";

                case 8:
                    return "Август";

                case 9:
                    return "Сентябрь";

                case 10:
                    return "Октябрь";

                case 11:
                    return "Ноябрь";

                case 12:
                    return "Декабрь";
            }
            return string.Empty;
        }
    }
}
