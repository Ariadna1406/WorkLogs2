using Aspose.Cells;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebApplication5.Models;
using WebApplication5.Models.ExcelFiles;

namespace WebApplication5.Models.ExcelFiles
{
    public class ExcelFile
    {
        public List<TaskComp> TaskComps { get; private set; }
        public string FilePath { get; set; }
        public Workbook Wb { get; private set; }
        public Worksheet Ws { get; private set; }

        public ExcelFile(string filePath)
        {
            FilePath = filePath;
        }

        public ExcelFile()
        {

        }

        public ExcelFile(string filePath, bool needToCreateWb, ref string errorMes)
        {            
            FilePath = filePath;
            if (needToCreateWb)
            {
                try
                {
                    Wb = new Workbook(FilePath);
                    Ws = Wb.Worksheets["Справочник работ-комплектов"];
                }
                catch(Exception ex)
                {
                    errorMes = $"Файл {this.FilePath} недоступен, возможно он занят другим приложением";
                }
            }
            
        }

        public void GetComplects(ref string errorMes, AppDbContext context)
        {
            List<TaskComp> taskCompList = new List<TaskComp>();
            try
            {
                this.Wb = new Workbook(this.FilePath);
            }
            catch
            {
                errorMes = $"Файл {this.FilePath} недоступен, возможно он занят другим приложением";
                return;
            }
            this.Ws = this.Wb.Worksheets["Справочник работ-комплектов"];
            var ws = this.Ws;
            if (ws != null)
            {
                //var st = ws.Cells[7, 20].GetStyle();
                bool proceedReading = true;
                int count = 1;
                while (proceedReading)
                {                   
                    if (string.IsNullOrEmpty(ws.Cells[count, ExcelDataColomns.ProjectNumber].StringValue))
                    {
                        if (string.IsNullOrEmpty(ws.Cells[count + 1, ExcelDataColomns.ProjectNumber].StringValue))
                        {
                            proceedReading = false;
                        }
                    }
                    else
                    {                       
                       if (CheckIfNeedImport(ws, count,context))
                        {

                            taskCompList.Add(
                                new TaskComp(Guid.NewGuid(), count)
                                {
                                    //Фактическое начало, фактические трудозатраты и исполнители не импортируются, а заполняются в системе.
                                    GIPAcronym = ws.Cells[count, ExcelDataColomns.GIPAcronym].StringValue,
                                    ProjectShortName = ws.Cells[count, ExcelDataColomns.ProjectShortName].StringValue,
                                    ProjectNumber = ws.Cells[count, ExcelDataColomns.ProjectNumber].StringValue,
                                    ProjectStage = ws.Cells[count, ExcelDataColomns.ProjectStage].StringValue,
                                    TaskCompName = ws.Cells[count, ExcelDataColomns.TaskCompName].StringValue,
                                    OperationName = ws.Cells[count, ExcelDataColomns.OperationName].StringValue,
                                    Department = ws.Cells[count, ExcelDataColomns.Department].StringValue,
                                    //Executers = ws.Cells[count, ExcelDataColomns.Executers].StringValue,
                                    DepartmentSendTask = ws.Cells[count, ExcelDataColomns.DepartmentSendTask].StringValue,
                                    PlanDateSendTask = GetDateOrNull(ws.Cells[count, ExcelDataColomns.PlanDateSendTask].StringValue),
                                    FactDateSendTask = GetDateOrNull(ws.Cells[count, ExcelDataColomns.FactDateSendTask].StringValue),
                                    FisnishContractDate = GetDateOrNull(ws.Cells[count, ExcelDataColomns.FinishContractDate].StringValue),
                                    ContractStatus = ws.Cells[count, ExcelDataColomns.ContactStatus].StringValue,
                                    StartPlanDate = GetDateOrNull(ws.Cells[count, ExcelDataColomns.StartPlanDate].StringValue),
                                    PlanWorkLog = GetDoubleOrNull(ws.Cells[count, ExcelDataColomns.PlanWorkLog].StringValue),
                                    FinishPlanDate = GetDateOrNull(ws.Cells[count, ExcelDataColomns.FinishPlanDate].StringValue),
                                    Price = GetDoubleOrNull(ws.Cells[count, ExcelDataColomns.Price].StringValue),
                                    Prepayment = GetDoubleOrNull(ws.Cells[count, ExcelDataColomns.Prepayment].StringValue),
                                    Moneyleft = GetDoubleOrNull(ws.Cells[count, ExcelDataColomns.Moneyleft].StringValue),
                                    SubContractorPart = GetDoubleOrNull(ws.Cells[count, ExcelDataColomns.SubContractorPart].StringValue),
                                    MoneyleftNHP = GetDoubleOrNull(ws.Cells[count, ExcelDataColomns.MoneyleftNHP].StringValue),
                                    DateMoneyGet = GetDateOrNull(ws.Cells[count, ExcelDataColomns.Moneyleft].StringValue),
                                    NumberOfUPD = ws.Cells[count, ExcelDataColomns.NumberOfUPD].StringValue,
                                    TaskCompStatus = ws.Cells[count, ExcelDataColomns.CommentToPrice].StringValue,
                                    TaskCompCreationDate = DateTime.Now

                                }
                            ) ;
                        }

                    }
                    count++;
                }

            }
            TaskComps = taskCompList;
        }
       

        public bool CheckIfNeedImport(Worksheet ws, int count, AppDbContext context)
        {
            var firstCond = string.IsNullOrEmpty(ws.Cells[count, ExcelDataColomns.UID].StringValue);
            //var needToImportStr = ws.Cells[count, ExcelDataColomns.NeedToImport].StringValue;
            //var secondCond = needToImportStr.ToUpper().Equals("ДА");
            var taskCompSet = context.TaskComps.Where(x => x.TaskCompName == ws.Cells[count, ExcelDataColomns.TaskCompName].StringValue);
            var secCond = taskCompSet.Count()==0;
            if (firstCond && secCond) //Если поле UID пустное и комплект с данным именем не найден
            {
                return true;
            }
            else if (firstCond && secCond==false) //Если поле UID пустное, но комплект с данным именем найден
            {               
                ws.Cells[count, ExcelDataColomns.UID].Value = taskCompSet.First().UID;
                //Wb.Save(this.FilePath);

            }
            return false;
        }

        DateTime? GetDateOrNull(string date)
        {
            var res = DateTime.TryParse(date, out DateTime dateTime);
            if (res)
            {
                return dateTime;
            }
            return null;
        }

        Double? GetDoubleOrNull(string doubleVal)
        {
            doubleVal = doubleVal.Replace(" ", "");
            var regex = new Regex("\\d*\\s*\\d+[,.]*\\d*");
            var match = regex.Match(doubleVal);
            var res = Double.TryParse(match.Value, out Double val);
            if (res)
            {
                return val;
            }
            return null;
        }


        public void WriteUidBackToExcelFile()
        {
            if (this.TaskComps != null && this.Ws != null)
            {
                foreach (var taskComp in TaskComps)
                {
                    var commonStyle = GetCommonStyle(Ws);
                    Ws.Cells[taskComp.ExcelLineNumber, ExcelDataColomns.UID].Value = taskComp.UID;
                    Ws.Cells[taskComp.ExcelLineNumber, ExcelDataColomns.UID].SetStyle(commonStyle);
                }

            }
            Wb.Save(this.FilePath);
        }

        public bool ExportHODToExcel(AppDbContext context, List<string> errorList)
        {
            Workbook wb = new Workbook();
            string sheetName = "Отделы";
            var ws = wb.Worksheets.Add(sheetName);            
            int count = 1;

            //Заголовки            
            ws.Cells[0, 0].Value = "Id";
            ws.Cells[0, 1].Value = "Наименование отдела";
            ws.Cells[0, 2].Value = "Начальник отдела";
            ws.Cells[0, 3].Value = "Аббревиатура отдела";
            ws.Cells[0, 4].Value = "Производственники да-нет";
            ws.Cells[0, 5].Value = "Игнорировать в отчёте";
            ws.Cells[0, 6].Value = "Город";

            var styleCommon = ws.Cells[0, 0].GetStyle();
            System.Drawing.Color color = System.Drawing.ColorTranslator.FromHtml("#B4C6E7");
            styleCommon.ForegroundColor = color;
            styleCommon.Pattern = BackgroundType.Solid;
            styleCommon.Font.Size = 12;
            styleCommon.Font.IsBold = true;
            styleCommon.HorizontalAlignment = TextAlignmentType.Center;

            var styleUpdate = ws.Cells[0, 0].GetStyle();
            styleUpdate.Copy(styleCommon);
            System.Drawing.Color color1 = System.Drawing.ColorTranslator.FromHtml("#F4A460");
            styleUpdate.ForegroundColor = color1;

            ws.Cells[0, 0].SetStyle(styleCommon);
            ws.Cells[0, 1].SetStyle(styleCommon);
            ws.Cells[0, 2].SetStyle(styleUpdate);
            ws.Cells[0, 3].SetStyle(styleUpdate);
            ws.Cells[0, 4].SetStyle(styleUpdate);
            ws.Cells[0, 5].SetStyle(styleUpdate);
            ws.Cells[0, 6].SetStyle(styleCommon);                                          

            foreach (var department in context.Departments.Where(x=>x.Name!=null).Include(x=>x.HeadOfDepartment).OrderBy(x=>x.Id))
            {

                ws.Cells[count, 0].Value = department.Id.ToString();
                ws.Cells[count, 1].Value = department.Name;
                ws.Cells[count, 2].Value = department.HeadOfDepartment?.FullName;
                ws.Cells[count, 3].Value = department.Acronym!=null?department.Acronym:string.Empty;
                ws.Cells[count, 4].Value = GetIntVal(department.Production);
                ws.Cells[count, 5].Value = GetIntVal(department.IgnoreInReport);
                ws.Cells[count, 6].Value = department.City != null ? department.City.ToString() : string.Empty;

                count++;
            }          
            ws.AutoFitColumns(0,6);

            //Все сотрудники
            var wsUsers = wb.Worksheets.Add("Сотрудники");
            var activeUsers = Models.User.GetAllActiveUsers(context);

            //Заголовки            
            wsUsers.Cells[0, 0].Value = "ФИО";
            wsUsers.Cells[0, 1].Value = "Наименование отдела";
            wsUsers.Cells[0, 2].Value = "Должность";
            wsUsers.Cells[0, 2].Value = "Город";

            var styleCommon1 = wsUsers.Cells[0, 0].GetStyle();
            System.Drawing.Color color2 = System.Drawing.ColorTranslator.FromHtml("#B4C6E7");
            styleCommon1.ForegroundColor = color2;
            styleCommon1.Pattern = BackgroundType.Solid;
            styleCommon1.Font.Size = 12;
            styleCommon1.Font.IsBold = true;
            styleCommon1.HorizontalAlignment = TextAlignmentType.Center;

            wsUsers.Cells[0, 0].SetStyle(styleCommon1);
            wsUsers.Cells[0, 1].SetStyle(styleCommon1);
            wsUsers.Cells[0, 2].SetStyle(styleCommon1);
            wsUsers.Cells[0, 3].SetStyle(styleCommon1);

            wsUsers.AutoFitColumns(0, 4);

            count = 1;
            foreach (var user in activeUsers)
            {
                wsUsers.Cells[count, 0].Value = user.FullName;
                wsUsers.Cells[count, 1].Value = user.Department.Name;
                wsUsers.Cells[count, 2].Value = user.Position;
                wsUsers.Cells[count, 3].Value = user.Department.City;
                count++;
            }

            try
            {
                wb.Save(this.FilePath);
                return true;
            }
            catch (Exception exception)
            {
                errorList.Add(exception.ToString());
                return false;
            }
        }

    int? GetIntVal(bool? boolVal)
        {
            int? intVal=null;
            if (boolVal.HasValue)
            {
                if (boolVal.Value) intVal = 1;
                else intVal = 0;
            }
            return intVal;
        }

        public int UpdateHeadOfDepartmentFromExcel(AppDbContext context)
        {
            List<Department> departList = new List<Department>();
            this.Wb = new Workbook(this.FilePath);
            this.Ws = this.Wb.Worksheets["Отделы"];
            var ws = this.Ws;
            int countChangedRows = 0;
            if (ws != null)
            {
                bool proceedReading = true;
                int count = 1;
                while (proceedReading)
                {
                    if (string.IsNullOrEmpty(ws.Cells[count, 0].StringValue))
                    {
                        if (string.IsNullOrEmpty(ws.Cells[count + 1, 0].StringValue))
                        {
                            proceedReading = false;
                        }
                    }
                    else
                    {
                        var id = Convert.ToInt32(ws.Cells[count, 0].StringValue);
                        var departSetFound = context.Departments.Where(x => x.Id == id);
                        if (departSetFound.Count() > 0)
                        {
                            var depart = departSetFound.First();
                            var hod = User.GetUserByFullNameFromDb(context, ws.Cells[count, 2].StringValue);
                            var acronym = ws.Cells[count, 3].StringValue;

                            if (hod != null) depart.HeadOfDepartment = hod;
                            depart.Acronym = acronym;
                            depart.Production = GetBoolValFromString(ws.Cells[count, 4].StringValue);
                            depart.IgnoreInReport = GetBoolValFromString(ws.Cells[count, 5].StringValue);
                            countChangedRows++;
                        }

                    }
                    count++;
                }

                context.SaveChanges();
            }
            return countChangedRows;

        }

        bool? GetBoolValFromString(string val)
        {           
          if (!string.IsNullOrEmpty(val))
            {
                var res = Int32.TryParse(val, out int valInt);
                if (res)
                {
                    if (valInt == 1)
                    {
                        return true;
                    }
                }
            }
            return null;
        }

        public void UpdateUIDInExcelFileFromPrevCreatedTask(AppDbContext context)
        {
            List<TaskComp> taskCompList = new List<TaskComp>();
            try
            {
                this.Wb = new Workbook(this.FilePath);
            }
            catch
            {
               // errorMes = $"Файл {this.FilePath} недоступен, возможно он занят другим приложением";
                return;
            }
            this.Ws = this.Wb.Worksheets["Справочник работ-комплектов"];
            
            int counterChanges = 0;
            int rowChangeCounter = 0;
            var ws = this.Ws;
            if (ws != null)
            {
                bool proceedReading = true;
                int count = 1;

                while (proceedReading)
                {
                    if (string.IsNullOrEmpty(ws.Cells[count, ExcelDataColomns.TaskCompName].StringValue))
                    {
                        if (string.IsNullOrEmpty(ws.Cells[count + 1, ExcelDataColomns.TaskCompName].StringValue))
                        {
                            proceedReading = false;
                        }
                    }
                    else
                    {
                        var prevCounterChanges = counterChanges;
                        var taskCompNameFromExcel = ws.Cells[count, ExcelDataColomns.TaskCompName].StringValue;
                        if (!string.IsNullOrEmpty(taskCompNameFromExcel))
                        {
                            var taskCompSet = context.TaskComps.Where(x => x.TaskCompName == taskCompNameFromExcel);
                            if (taskCompSet.Count() == 2)
                            {
                                var taskComp1 = taskCompSet.First();
                                var taskComp2 = taskCompSet.Last();
                                if (taskComp1.TaskCompName== "3081-МТС")
                                {
                                    
                                }
                                if (taskComp1.Id < taskComp2.Id)
                                {
                                    ResetWorkLogs(taskComp2, taskComp1, context);
                                    context.TaskComps.Remove(taskComp1);
                                    ws.Cells[count, ExcelDataColomns.UID].Value = taskComp2.UID;
                                    Wb.Save(FilePath);
                                    context.SaveChanges();
                                    //Обновление данных
                                    taskComp2.FactWorkLog = WorkLogs.GetTotalHoursOnTask(taskComp2, context, out DateTime? startDateOut);
                                    taskComp2.Executers = WorkLogs.GetExecuters(taskComp2, context);
                                    
                                }
                                else
                                {
                                    ResetWorkLogs(taskComp1, taskComp2, context);
                                    context.TaskComps.Remove(taskComp2);    
                                    ws.Cells[count, ExcelDataColomns.UID].Value = taskComp1.UID;
                                    Wb.Save(FilePath);
                                    context.SaveChanges();
                                    //Обновление данных
                                    taskComp2.FactWorkLog = WorkLogs.GetTotalHoursOnTask(taskComp1, context, out DateTime? startDateOut);
                                    taskComp2.Executers = WorkLogs.GetExecuters(taskComp1, context);
                                    
                                }

                            }
                            else if (taskCompSet.Count() == 1)
                            {
                                var taskComp1 = taskCompSet.First();
                                ws.Cells[count, ExcelDataColomns.UID].Value = taskComp1.UID;
                                //Обновление данных
                                taskComp1.FactWorkLog = WorkLogs.GetTotalHoursOnTask(taskComp1, context, out DateTime? startDateOut);
                                taskComp1.Executers = WorkLogs.GetExecuters(taskComp1, context);
                            }
                        }
                        context.SaveChanges();

                    }
                    count++;

                }
                //Wb.Save(FilePath);
                //context.SaveChanges();
            }
        }

        public void UpdateFactWorkLogsAndExecutersInDatabase(AppDbContext context)
        {
            List<TaskComp> taskCompList = new List<TaskComp>();
            try
            {
                this.Wb = new Workbook(this.FilePath);
            }
            catch
            {
                // errorMes = $"Файл {this.FilePath} недоступен, возможно он занят другим приложением";
                return;
            }
            this.Ws = this.Wb.Worksheets["Справочник работ-комплектов"];

            int counterChanges = 0;
            int rowChangeCounter = 0;
            var ws = this.Ws;
            if (ws != null)
            {
                bool proceedReading = true;
                int count = 1;

                while (proceedReading)
                {
                    if (string.IsNullOrEmpty(ws.Cells[count, ExcelDataColomns.TaskCompName].StringValue))
                    {
                        if (string.IsNullOrEmpty(ws.Cells[count + 1, ExcelDataColomns.TaskCompName].StringValue))
                        {
                            proceedReading = false;
                        }
                    }
                    else
                    {
                        var uidFromExcel = ws.Cells[count, ExcelDataColomns.UID].StringValue;
                        if (!string.IsNullOrEmpty(uidFromExcel))
                        {
                            if (uidFromExcel== "c2377d00-d323-4595-9b15-de3dfd63c32a")
                            {

                            }
                            var taskCompSet = context.TaskComps.Where(x => x.UID == uidFromExcel);
                            if (taskCompSet.Count() == 1)
                            {
                                var taskComp1 = taskCompSet.First();
                                taskComp1.FactWorkLog = WorkLogs.GetTotalHoursOnTask(taskComp1, context, out DateTime? startDateOut);
                                taskComp1.Executers = WorkLogs.GetExecuters(taskComp1, context);
                            }
                        }                        
                    }
                    count++;
                }               
                context.SaveChanges();
            }
        }

        public void ResetWorkLogs(TaskComp taskCompReceive, TaskComp taskCompGive, AppDbContext context)
        {
            var workLogSet = context.WorkLogs.Where(x => x.TaskComp_id == taskCompGive.Id.ToString());
            foreach (var wl in workLogSet)
            {
                wl.TaskComp_id = taskCompReceive.Id.ToString();
            }

            var percentHistorySet = context.TaskCompPercentHistories.Where(x => x.TaskComp.Id == taskCompGive.Id);
            foreach (var ph in percentHistorySet)
            {
                ph.TaskComp = taskCompReceive;
            }
        }

        public void UpdateComplects(AppDbContext context, out int counterChanges, out int rowChangeCounter)
        {
            counterChanges = 0;
            rowChangeCounter = 0;
            var ws = this.Ws;
            if (ws != null)
            {
                bool proceedReading = true;
                int count = 1;

                while (proceedReading)
                {
                    if (string.IsNullOrEmpty(ws.Cells[count, ExcelDataColomns.ProjectNumber].StringValue))
                    {
                        if (string.IsNullOrEmpty(ws.Cells[count + 1, ExcelDataColomns.ProjectNumber].StringValue))
                        {
                            proceedReading = false;
                        }
                    }
                    else
                    {
                        var prevCounterChanges = counterChanges;
                        var uidFromExcel = ws.Cells[count, ExcelDataColomns.UID].StringValue;
                        if (!string.IsNullOrEmpty(uidFromExcel))
                        {
                            var taskCompSet = context.TaskComps.Where(x => x.UID == uidFromExcel);
                            if (taskCompSet.Count() > 0)
                            {
                                var taskComp = taskCompSet.First();
                                //var prjNumExcel = ws.Cells[count, ExcelDataColomns.ProjectNumber].StringValue;
                                //var taskCompNameExcel = ws.Cells[count, ExcelDataColomns.TaskCompName].StringValue;
                                //var departmentExcel = ws.Cells[count, ExcelDataColomns.Department].StringValue;
                                //var startPlanDateExcel = GetDateOrNull(ws.Cells[count, ExcelDataColomns.StartPlanDate].StringValue);
                                //var finishPlanDateExcel = GetDateOrNull(ws.Cells[count, ExcelDataColomns.FinishPlanDate].StringValue);
                                //var planWorkLog = GetDoubleOrNull(ws.Cells[count, ExcelDataColomns.PlanWorkLog].StringValue);

                                var gIPAcronym = ws.Cells[count, ExcelDataColomns.GIPAcronym].StringValue;
                                var projectShortName = ws.Cells[count, ExcelDataColomns.ProjectShortName].StringValue;
                                var projectNumber = ws.Cells[count, ExcelDataColomns.ProjectNumber].StringValue;
                                var projectStage = ws.Cells[count, ExcelDataColomns.ProjectStage].StringValue;
                                var taskCompName = ws.Cells[count, ExcelDataColomns.TaskCompName].StringValue;
                                var operationName = ws.Cells[count, ExcelDataColomns.OperationName].StringValue;
                                var department = ws.Cells[count, ExcelDataColomns.Department].StringValue;
                                var departmentSendTask = ws.Cells[count, ExcelDataColomns.DepartmentSendTask].StringValue;
                                var planDateSendTask = GetDateOrNull(ws.Cells[count, ExcelDataColomns.PlanDateSendTask].StringValue);
                                var factDateSendTask = GetDateOrNull(ws.Cells[count, ExcelDataColomns.FactDateSendTask].StringValue);
                                var fisnishContractDate = GetDateOrNull(ws.Cells[count, ExcelDataColomns.FinishContractDate].StringValue);
                                var contractStatus = ws.Cells[count, ExcelDataColomns.ContactStatus].StringValue;
                                var startPlanDate = GetDateOrNull(ws.Cells[count, ExcelDataColomns.StartPlanDate].StringValue);
                                var planWorkLog = GetDoubleOrNull(ws.Cells[count, ExcelDataColomns.PlanWorkLog].StringValue);
                                var finishPlanDate = GetDateOrNull(ws.Cells[count, ExcelDataColomns.FinishPlanDate].StringValue);
                                var price = GetDoubleOrNull(ws.Cells[count, ExcelDataColomns.Price].StringValue);
                                var prepayment = GetDoubleOrNull(ws.Cells[count, ExcelDataColomns.Prepayment].StringValue);
                                var moneyleft = GetDoubleOrNull(ws.Cells[count, ExcelDataColomns.Moneyleft].StringValue);
                                var subContractorPart = GetDoubleOrNull(ws.Cells[count, ExcelDataColomns.SubContractorPart].StringValue);
                                var moneyleftNHP = GetDoubleOrNull(ws.Cells[count, ExcelDataColomns.MoneyleftNHP].StringValue);
                                var dateMoneyGet = GetDateOrNull(ws.Cells[count, ExcelDataColomns.Moneyleft].StringValue);
                                var numberOfUPD = ws.Cells[count, ExcelDataColomns.NumberOfUPD].StringValue;
                                var commentToPrice = ws.Cells[count, ExcelDataColomns.CommentToPrice].StringValue;


                                CheckStrProp(taskComp, "ProjectNumber", projectNumber, ref counterChanges);
                                CheckStrProp(taskComp, "GIPAcronym", gIPAcronym, ref counterChanges);
                                CheckStrProp(taskComp, "ProjectShortName", projectShortName, ref counterChanges);
                                CheckStrProp(taskComp, "ProjectStage", projectStage, ref counterChanges);
                                CheckStrProp(taskComp, "TaskCompName", taskCompName, ref counterChanges);
                                CheckStrProp(taskComp, "OperationName", operationName, ref counterChanges);
                                CheckStrProp(taskComp, "Department", department, ref counterChanges);
                                CheckStrProp(taskComp, "DepartmentSendTask", departmentSendTask, ref counterChanges);
                                CheckStrProp(taskComp, "ContractStatus", contractStatus, ref counterChanges);                          
                                CheckDateProp(taskComp, "PlanDateSendTask", planDateSendTask, ref counterChanges);
                                CheckDateProp(taskComp, "FactDateSendTask", factDateSendTask, ref counterChanges);
                                CheckDateProp(taskComp, "FisnishContractDate", fisnishContractDate, ref counterChanges);
                                CheckDateProp(taskComp, "StartPlanDate", startPlanDate, ref counterChanges);

                                CheckDoubleProp(taskComp, "Price", price, ref counterChanges);
                                CheckDoubleProp(taskComp, "Prepayment", prepayment, ref counterChanges);
                                CheckDoubleProp(taskComp, "Moneyleft", moneyleft, ref counterChanges);
                                CheckDoubleProp(taskComp, "SubContractorPart", subContractorPart, ref counterChanges);
                                CheckDoubleProp(taskComp, "MoneyleftNHP", moneyleftNHP, ref counterChanges);
                                CheckDateProp(taskComp, "DateMoneyGet", dateMoneyGet, ref counterChanges);
                                CheckStrProp(taskComp, "NumberOfUPD", numberOfUPD, ref counterChanges);
                                CheckStrProp(taskComp, "CommentToPrice", commentToPrice, ref counterChanges);

                                CheckDoubleProp(taskComp, "PlanWorkLog", planWorkLog, ref counterChanges);

                                if (counterChanges > prevCounterChanges) { rowChangeCounter++; }
                                //if (taskComp.ProjectNumber != projectNumber)
                                //{
                                //    taskComp.ProjectNumber = projectNumber; //CheckValues(taskComp.ProjectNumber,ref prjNumExcel,ref counterChanges); 
                                //    counterChanges++;
                                //}
                                //if (taskComp.TaskCompName != taskCompNameExcel)
                                //{
                                //    taskComp.TaskCompName = taskCompNameExcel;
                                //}
                                //if (taskComp.Department != departmentExcel)
                                //{
                                //    taskComp.Department = departmentExcel;
                                //}
                                //if (taskComp.StartPlanDate != startPlanDateExcel) {taskComp.StartPlanDate = startPlanDateExcel;}
                                //if (taskComp.FinishFactDate != finishPlanDateExcel)
                                //{ taskComp.FinishPlanDate = finishPlanDateExcel; }
                                //if (taskComp.PlanWorkLog != planWorkLog) { taskComp.PlanWorkLog = planWorkLog; counterChanges++; }

                            }
                        }
                    }

                    count++;
                }
                context.SaveChanges();

            }

        }

        public void UpdateHODAndGIP(AppDbContext context, out int counterChanges, out int rowChangeCounter)
        {
            counterChanges = 0;
            rowChangeCounter = 0;
            var ws = this.Ws;
            if (ws != null)
            {
                bool proceedReading = true;
                int count = 1;

                while (proceedReading)
                {
                    if (string.IsNullOrEmpty(ws.Cells[count, ExcelDataColomns.ProjectNumber].StringValue))
                    {
                        if (string.IsNullOrEmpty(ws.Cells[count + 1, ExcelDataColomns.ProjectNumber].StringValue))
                        {
                            proceedReading = false;
                        }
                    }
                    else
                    {
                        var prevCounterChanges = counterChanges;
                        var uidFromExcel = ws.Cells[count, ExcelDataColomns.UID].StringValue;
                        if (!string.IsNullOrEmpty(uidFromExcel))
                        {
                            var taskCompSet = context.TaskComps.Where(x => x.UID == uidFromExcel);
                            if (taskCompSet.Count() > 0)
                            {
                                var taskComp = taskCompSet.First();
                                //var prjNumExcel = ws.Cells[count, ExcelDataColomns.ProjectNumber].StringValue;
                                //var taskCompNameExcel = ws.Cells[count, ExcelDataColomns.TaskCompName].StringValue;
                                //var departmentExcel = ws.Cells[count, ExcelDataColomns.Department].StringValue;
                                //var startPlanDateExcel = GetDateOrNull(ws.Cells[count, ExcelDataColomns.StartPlanDate].StringValue);
                                //var finishPlanDateExcel = GetDateOrNull(ws.Cells[count, ExcelDataColomns.FinishPlanDate].StringValue);
                                //var planWorkLog = GetDoubleOrNull(ws.Cells[count, ExcelDataColomns.PlanWorkLog].StringValue);

                                var gIPAcronym = ws.Cells[count, ExcelDataColomns.GIPAcronym].StringValue;
                                var projectShortName = ws.Cells[count, ExcelDataColomns.ProjectShortName].StringValue;
                                var projectNumber = ws.Cells[count, ExcelDataColomns.ProjectNumber].StringValue;
                                
                                var taskCompName = ws.Cells[count, ExcelDataColomns.TaskCompName].StringValue;
                                var operationName = ws.Cells[count, ExcelDataColomns.OperationName].StringValue;
                                var department = ws.Cells[count, ExcelDataColomns.Department].StringValue;
                               


                                CheckStrProp(taskComp, "ProjectNumber", projectNumber, ref counterChanges);
                                CheckStrProp(taskComp, "GIPAcronym", gIPAcronym, ref counterChanges);
                                CheckStrProp(taskComp, "ProjectShortName", projectShortName, ref counterChanges);                              
                                CheckStrProp(taskComp, "TaskCompName", taskCompName, ref counterChanges);
                                CheckStrProp(taskComp, "OperationName", operationName, ref counterChanges);
                                CheckStrProp(taskComp, "Department", department, ref counterChanges);
                               

                                if (counterChanges > prevCounterChanges) { rowChangeCounter++; }
                                //if (taskComp.ProjectNumber != projectNumber)
                                //{
                                //    taskComp.ProjectNumber = projectNumber; //CheckValues(taskComp.ProjectNumber,ref prjNumExcel,ref counterChanges); 
                                //    counterChanges++;
                                //}
                                //if (taskComp.TaskCompName != taskCompNameExcel)
                                //{
                                //    taskComp.TaskCompName = taskCompNameExcel;
                                //}
                                //if (taskComp.Department != departmentExcel)
                                //{
                                //    taskComp.Department = departmentExcel;
                                //}
                                //if (taskComp.StartPlanDate != startPlanDateExcel) {taskComp.StartPlanDate = startPlanDateExcel;}
                                //if (taskComp.FinishFactDate != finishPlanDateExcel)
                                //{ taskComp.FinishPlanDate = finishPlanDateExcel; }
                                //if (taskComp.PlanWorkLog != planWorkLog) { taskComp.PlanWorkLog = planWorkLog; counterChanges++; }

                            }
                        }
                    }

                    count++;
                }
                context.SaveChanges();

            }

        }

        public void DeleteComplects(AppDbContext context, ref int counterDeleted)
        {
            var ws = this.Ws;
            if (ws != null)
            {
                bool proceedReading = true;
                int count = 1;

                while (proceedReading)
                {
                    if (string.IsNullOrEmpty(ws.Cells[count, ExcelDataColomns.ProjectNumber].StringValue))
                    {
                        if (string.IsNullOrEmpty(ws.Cells[count + 1, ExcelDataColomns.ProjectNumber].StringValue))
                        {
                            proceedReading = false;
                        }
                    }
                    else
                    {
                        int counterRedCells = 0;
                        var uidFromExcel = ws.Cells[count, ExcelDataColomns.UID].StringValue;
                        if (!string.IsNullOrEmpty(uidFromExcel))
                        {
                            var taskCompSet = context.TaskComps.Where(x => x.UID == uidFromExcel);
                            if (taskCompSet.Count() > 0)
                            {
                                var taskComp = taskCompSet.First();
                                //var prjNumExcel = ws.Cells[count, ExcelDataColomns.ProjectNumber].StringValue;
                                //var taskCompNameExcel = ws.Cells[count, ExcelDataColomns.TaskCompName].StringValue;
                                //var departmentExcel = ws.Cells[count, ExcelDataColomns.Department].StringValue;
                                //var startPlanDateExcel = GetDateOrNull(ws.Cells[count, ExcelDataColomns.StartPlanDate].StringValue);
                                //var finishPlanDateExcel = GetDateOrNull(ws.Cells[count, ExcelDataColomns.FinishPlanDate].StringValue);
                                //var planWorkLog = GetDoubleOrNull(ws.Cells[count, ExcelDataColomns.PlanWorkLog].StringValue);
                                List<Style> styleList = new List<Style>() {
                                ws.Cells[count, ExcelDataColomns.GIPAcronym].GetStyle(),
                                ws.Cells[count, ExcelDataColomns.ProjectShortName].GetStyle(),
                                ws.Cells[count, ExcelDataColomns.ProjectNumber].GetStyle(),
                                ws.Cells[count, ExcelDataColomns.ProjectStage].GetStyle(),
                                ws.Cells[count, ExcelDataColomns.TaskCompName].GetStyle(),
                                ws.Cells[count, ExcelDataColomns.OperationName].GetStyle(),
                                ws.Cells[count, ExcelDataColomns.Department].GetStyle(),
                                ws.Cells[count, ExcelDataColomns.DepartmentSendTask].GetStyle(),
                                ws.Cells[count, ExcelDataColomns.PlanDateSendTask].GetStyle(),
                                ws.Cells[count, ExcelDataColomns.FactDateSendTask].GetStyle(),
                                ws.Cells[count, ExcelDataColomns.FinishContractDate].GetStyle(),
                                ws.Cells[count, ExcelDataColomns.ContactStatus].GetStyle(),
                                ws.Cells[count, ExcelDataColomns.StartPlanDate].GetStyle(),
                                ws.Cells[count, ExcelDataColomns.PlanWorkLog].GetStyle(),
                                ws.Cells[count, ExcelDataColomns.FinishPlanDate].GetStyle()
                            };
                                foreach (var st in styleList)
                                {
                                    CheckForeColorIsRed(st, ref counterRedCells);
                                }
                                if (styleList.Count == counterRedCells)
                                {
                                    context.TaskComps.Remove(taskComp);
                                    ws.Cells.DeleteRow(count);
                                    counterDeleted++;
                                }

                            }
                        }
                    }

                    count++;
                }
                context.SaveChanges();
                this.Wb.Save(FilePath);
            }

        }
        void CheckForeColorIsRed(Style style, ref int counterRedCells)
        {
            var valStr = (Color)style.GetType().GetProperty("ForegroundColor").GetValue(style);
            if (valStr == Color.FromArgb(255, 255, 0, 0))
            {
                counterRedCells++;
            }
        }

        void CheckStrProp(TaskComp taskComp, string propName, string newVal, ref int counterChanges)
        {
            var val = taskComp.GetType().GetProperty(propName);
                if (val != null) {
                var valStr = val.GetValue(taskComp);
                if (valStr != null && valStr.ToString() != newVal)
                {
                    taskComp.GetType().GetProperty(propName).SetValue(taskComp, newVal);
                    counterChanges++;
                }
            }
        }

        void CheckDateProp(TaskComp taskComp, string propName, DateTime? newVal, ref int counterChanges)
        {
            var valDate = (DateTime?)taskComp.GetType().GetProperty(propName).GetValue(taskComp, null);
            if (valDate != newVal)
            {
                taskComp.GetType().GetProperty(propName).SetValue(taskComp, newVal);
                counterChanges++;
            }
        }

        void CheckDoubleProp(TaskComp taskComp, string propName, Double? newVal, ref int counterChanges)
        {
            var valDate = (Double?)taskComp.GetType().GetProperty(propName).GetValue(taskComp);
            if (valDate != newVal)
            {
                taskComp.GetType().GetProperty(propName).SetValue(taskComp, newVal);
                counterChanges++;
            }
        }

        public void WriteDataBackToExcelFile(AppDbContext context)
        {
            var ws = this.Ws;
            if (ws != null)
            {
                bool proceedReading = true;
                int count = 1;
                AddHeaderForUpload(ws);
                while (proceedReading)
                {
                    if (string.IsNullOrEmpty(ws.Cells[count, ExcelDataColomns.ProjectNumber].StringValue))
                    {
                        if (string.IsNullOrEmpty(ws.Cells[count + 1, ExcelDataColomns.ProjectNumber].StringValue))
                        {
                            proceedReading = false;
                        }
                    }
                    else
                    {

                        var uidFromExcel = ws.Cells[count, ExcelDataColomns.UID].StringValue;
                        if (!string.IsNullOrEmpty(uidFromExcel))
                        {
                            var taskCompSet = context.TaskComps.Where(x => x.UID == uidFromExcel);
                            if (taskCompSet.Count() > 0)
                            {
                                var taskComp = taskCompSet.First();
                                WriteDataBackToExcelFileSingleTaskComp(context, ws, taskComp, count);

                            }
                        }
                    }

                    count++;
                }
                this.Wb.Save(FilePath);

            }
        }

        public void WriteDataBackToExcelFile(AppDbContext context,ref string errorMessage)
        {
            var ws = this.Ws;
            if (ws != null)
            {
                bool proceedReading = true;
                int count = 1;
                AddHeaderForUpload(ws);
                while (proceedReading)
                {
                    if (string.IsNullOrEmpty(ws.Cells[count, ExcelDataColomns.ProjectNumber].StringValue))
                    {
                        if (string.IsNullOrEmpty(ws.Cells[count + 1, ExcelDataColomns.ProjectNumber].StringValue))
                        {
                            proceedReading = false;
                        }
                    }
                    else
                    {

                        var uidFromExcel = ws.Cells[count, ExcelDataColomns.UID].StringValue;
                        if (!string.IsNullOrEmpty(uidFromExcel))
                        {
                            var taskCompSet = context.TaskComps.Where(x => x.UID == uidFromExcel);
                            if (taskCompSet.Count() > 0)
                            {
                                var taskComp = taskCompSet.First();
                                WriteDataBackToExcelFileSingleTaskComp(context, ws, taskComp, count);
                               
                            }
                        }
                    }

                    count++;
                }
                this.Wb.Save(FilePath);

            }
        }

        void WriteDataBackToExcelFileSingleTaskComp(WebApplication5.Models.AppDbContext context, Worksheet ws ,TaskComp taskComp, int count)
        {
            taskComp.FactWorkLog = WorkLogs.GetTotalHoursOnTask(taskComp, context, out DateTime? startDate);
            //STYLE
            Style commonStyle = GetCommonStyle(ws);
            Style dateStyle = GetDateStyle(ws);

            ws.Cells[count, ExcelDataColomns.Executers].Value = taskComp.Executers;
            ws.Cells[count, ExcelDataColomns.Executers].SetStyle(commonStyle);
            ws.Cells[count, ExcelDataColomns.FactWorkLogs].Value = taskComp.FactWorkLog;
            ws.Cells[count, ExcelDataColomns.FactWorkLogs].SetStyle(commonStyle);
            ws.Cells[count, ExcelDataColomns.StartFactDate].Value = startDate;
            ws.Cells[count, ExcelDataColomns.StartFactDate].SetStyle(dateStyle);
            ws.Cells[count, ExcelDataColomns.PlanPercentByDate].Value = GetPlanPercent(taskComp);
            ws.Cells[count, ExcelDataColomns.PlanPercentByDate].SetStyle(commonStyle);
            ws.Cells[count, ExcelDataColomns.FactPercentByWorkLogs].Value = GetFactPercentByWorkLogs(taskComp);
            ws.Cells[count, ExcelDataColomns.FactPercentByWorkLogs].SetStyle(commonStyle);
            ws.Cells[count, ExcelDataColomns.FactPercentByHOD].Value = taskComp.CompletePercent;
            ws.Cells[count, ExcelDataColomns.FactPercentByHOD].SetStyle(commonStyle);

        }

        Style GetCommonStyle(Worksheet ws)
        {
            Style commonStyle = ws.Cells[1, 1].GetStyle();
            commonStyle.HorizontalAlignment = TextAlignmentType.Center;
            commonStyle.VerticalAlignment = TextAlignmentType.Center;
            commonStyle.ForegroundColor = Color.LightGreen;
            commonStyle.Pattern = BackgroundType.Solid;
            return commonStyle;
        }

        Style GetDateStyle(Worksheet ws)
        {
            var dateStyle = ws.Cells[1, 1].GetStyle();
            dateStyle.HorizontalAlignment = TextAlignmentType.Center;
            dateStyle.VerticalAlignment = TextAlignmentType.Center;
            dateStyle.ForegroundColor = Color.LightGreen;
            dateStyle.Pattern = BackgroundType.Solid;
            dateStyle.Number = 14;
            return dateStyle;
        }


        double? GetPlanPercent(TaskComp taskComp)
        {
            var finishDate = taskComp.FinishPlanDate;
            var startDate = taskComp.StartPlanDate;
            var now = DateTime.Now;
            if (startDate != null && finishDate != null && finishDate > startDate && startDate < now)
            {
                if (DateTime.Now < finishDate)
                {
                    var f1 = DateTime.Now.Date - startDate;
                    var f2 = finishDate - startDate;
                    var ratio = (f1 / f2) * 100;
                    if (ratio.HasValue)
                    {
                        var planPercent = Math.Round(ratio.Value, 2);
                        return planPercent;
                    }
                }
                else
                {
                    return 100;
                }

            }
            return null;
        }



        double? GetFactPercentByWorkLogs(TaskComp taskComp)
        {
            var factWorkLogs = taskComp.FactWorkLog;
            var planWorkLogs = taskComp.PlanWorkLog;
            if (factWorkLogs != null && planWorkLogs != null)
            {
                var ratio = (factWorkLogs / planWorkLogs) * 100;
                if (ratio.HasValue)
                {
                    var percentByWL = Math.Round(ratio.Value, 2);
                    return percentByWL;
                }
            }
            return null;
        }

        public void UploadAllTaskComp(AppDbContext context)
        {
            Workbook wb = new Workbook();
            wb.Worksheets.Add("Справочник работ-комплектов");
            var ws = wb.Worksheets["Справочник работ-комплектов"];
            AddHeader(ws);
            int count = 1;
            foreach (var taskComp in context.TaskComps)
            {
                var dateStyle = ws.Cells[count, ExcelDataColomns.StartPlanDate].GetStyle();
                dateStyle.Number = 14; // for date

                var finStyle = ws.Cells[count, ExcelDataColomns.Price].GetStyle();
                finStyle.Number = 44;

                ws.Cells[count, ExcelDataColomns.GIPAcronym].Value = taskComp.GIPAcronym;
                ws.Cells[count, ExcelDataColomns.ProjectShortName].Value = taskComp.ProjectShortName;
                ws.Cells[count, ExcelDataColomns.ProjectNumber].Value = taskComp.ProjectNumber;
                ws.Cells[count, ExcelDataColomns.ProjectStage].Value = taskComp.ProjectStage;
                ws.Cells[count, ExcelDataColomns.TaskCompName].Value = taskComp.TaskCompName;
                ws.Cells[count, ExcelDataColomns.OperationName].Value = taskComp.OperationName;
                ws.Cells[count, ExcelDataColomns.Department].Value = taskComp.Department;
                ws.Cells[count, ExcelDataColomns.DepartmentSendTask].Value = taskComp.DepartmentSendTask;
                ws.Cells[count, ExcelDataColomns.PlanDateSendTask].Value = taskComp.PlanDateSendTask;
                ws.Cells[count, ExcelDataColomns.PlanDateSendTask].SetStyle(dateStyle);
                ws.Cells[count, ExcelDataColomns.FactDateSendTask].Value = taskComp.FactDateSendTask;
                ws.Cells[count, ExcelDataColomns.FactDateSendTask].SetStyle(dateStyle);
                ws.Cells[count, ExcelDataColomns.FinishContractDate].Value = taskComp.FisnishContractDate;
                ws.Cells[count, ExcelDataColomns.FinishContractDate].SetStyle(dateStyle);
                ws.Cells[count, ExcelDataColomns.ContactStatus].Value = taskComp.ContractStatus;
                ws.Cells[count, ExcelDataColomns.StartPlanDate].Value = taskComp.StartPlanDate;
                ws.Cells[count, ExcelDataColomns.StartPlanDate].SetStyle(dateStyle);
                ws.Cells[count, ExcelDataColomns.FinishPlanDate].Value = taskComp.FinishPlanDate;
                ws.Cells[count, ExcelDataColomns.FinishPlanDate].SetStyle(dateStyle);
                ws.Cells[count, ExcelDataColomns.FinishFactDate].Value = taskComp.FinishFactDate;
                ws.Cells[count, ExcelDataColomns.PortalApproveFactDate].Value = taskComp.ApproveFactDate;

                ws.Cells[count, ExcelDataColomns.Price].Value = taskComp.Price;
                ws.Cells[count, ExcelDataColomns.Price].SetStyle(finStyle);
                ws.Cells[count, ExcelDataColomns.Prepayment].Value = taskComp.Prepayment;
                ws.Cells[count, ExcelDataColomns.Prepayment].SetStyle(finStyle);
                ws.Cells[count, ExcelDataColomns.Moneyleft].Value = taskComp.Moneyleft;
                ws.Cells[count, ExcelDataColomns.Moneyleft].SetStyle(finStyle);
                ws.Cells[count, ExcelDataColomns.SubContractorPart].Value = taskComp.SubContractorPart;
                ws.Cells[count, ExcelDataColomns.SubContractorPart].SetStyle(finStyle);
                ws.Cells[count, ExcelDataColomns.MoneyleftNHP].Value = taskComp.MoneyleftNHP;
                ws.Cells[count, ExcelDataColomns.MoneyleftNHP].SetStyle(finStyle);
                ws.Cells[count, ExcelDataColomns.DateMoneyGet].Value = taskComp.DateMoneyGet;
                ws.Cells[count, ExcelDataColomns.DateMoneyGet].SetStyle(dateStyle);
                ws.Cells[count, ExcelDataColomns.NumberOfUPD].Value = taskComp.NumberOfUPD;
                ws.Cells[count, ExcelDataColomns.CommentToPrice].Value = taskComp.TaskCompStatus;


                WriteDataBackToExcelFileSingleTaskComp(context, ws, taskComp, count);
                ws.Cells[count, ExcelDataColomns.NeedToImport].Value = "Да";
                ws.Cells[count, ExcelDataColomns.UID].Value = taskComp.UID;

                count++;
            }
            ws.AutoFitColumns(0, 50);
            wb.Save(this.FilePath);
        }

        void AddHeader(Worksheet ws)
        {
            ws.Cells[0, ExcelDataColomns.GIPAcronym].Value = "ГИП";
            ws.Cells[0, ExcelDataColomns.ProjectShortName].Value = "Имя объекта";
            ws.Cells[0, ExcelDataColomns.ProjectNumber].Value = "Проект";
            ws.Cells[0, ExcelDataColomns.ProjectStage].Value = "Стадия";
            ws.Cells[0, ExcelDataColomns.TaskCompName].Value = "Работа/Комплект";
            ws.Cells[0, ExcelDataColomns.OperationName].Value = "Комментарий";
            ws.Cells[0, ExcelDataColomns.Department].Value = "Отдел";
            ws.Cells[0, ExcelDataColomns.DepartmentSendTask].Value = "Отдел выдающий задание";
            ws.Cells[0, ExcelDataColomns.PlanDateSendTask].Value = "Плановая дата выдачи задания";
            ws.Cells[0, ExcelDataColomns.FactDateSendTask].Value = "Фактическая дата выдачи задания";
            ws.Cells[0, ExcelDataColomns.FinishContractDate].Value = "Окончание по договору";
            ws.Cells[0, ExcelDataColomns.ContactStatus].Value = "Статус заключения договора (ДС)";
            
            ws.Cells[0, ExcelDataColomns.StartPlanDate].Value = "Начало";
            ws.Cells[0, ExcelDataColomns.FinishPlanDate].Value = "План окончание, срок выдачи на портал";            
            ws.Cells[0, ExcelDataColomns.FinishFactDate].Value = "Факт выкладки на портал";
            ws.Cells[0, ExcelDataColomns.PortalApproveFactDate].Value = "Факт утверждения";

            ws.Cells[0, ExcelDataColomns.Price].Value = "Стоимость";
            ws.Cells[0, ExcelDataColomns.Prepayment].Value = "Аванс";
            ws.Cells[0, ExcelDataColomns.Moneyleft].Value = "Остаток";
            ws.Cells[0, ExcelDataColomns.SubContractorPart].Value = "Субподрядчик";
            ws.Cells[0, ExcelDataColomns.MoneyleftNHP].Value = "Остаток в НХП";
            ws.Cells[0, ExcelDataColomns.DateMoneyGet].Value = "Месяц закрытия акта";
            ws.Cells[0, ExcelDataColomns.NumberOfUPD].Value = "УПД";
            ws.Cells[0, ExcelDataColomns.CommentToPrice].Value = "Доп.комментарий";

            var styleCommon= SetCommonStyle(ws);            

            ws.Cells[0, ExcelDataColomns.GIPAcronym].SetStyle(styleCommon);
            ws.Cells[0, ExcelDataColomns.ProjectShortName].SetStyle(styleCommon);
            ws.Cells[0, ExcelDataColomns.ProjectNumber].SetStyle(styleCommon);
            ws.Cells[0, ExcelDataColomns.ProjectStage].SetStyle(styleCommon);
            ws.Cells[0, ExcelDataColomns.TaskCompName].SetStyle(styleCommon);
            ws.Cells[0, ExcelDataColomns.OperationName].SetStyle(styleCommon);
            ws.Cells[0, ExcelDataColomns.Department].SetStyle(styleCommon);
            ws.Cells[0, ExcelDataColomns.DepartmentSendTask].SetStyle(styleCommon);
            ws.Cells[0, ExcelDataColomns.PlanDateSendTask].SetStyle(styleCommon);
            ws.Cells[0, ExcelDataColomns.FactDateSendTask].SetStyle(styleCommon);
            ws.Cells[0, ExcelDataColomns.FinishContractDate].SetStyle(styleCommon);
            ws.Cells[0, ExcelDataColomns.ContactStatus].SetStyle(styleCommon);

            ws.Cells[0, ExcelDataColomns.StartPlanDate].SetStyle(styleCommon);
            ws.Cells[0, ExcelDataColomns.FinishPlanDate].SetStyle(styleCommon);
            ws.Cells[0, ExcelDataColomns.FinishFactDate].SetStyle(styleCommon);
            ws.Cells[0, ExcelDataColomns.PortalApproveFactDate].SetStyle(styleCommon);

            ws.Cells[0, ExcelDataColomns.Price].SetStyle(styleCommon);
            ws.Cells[0, ExcelDataColomns.Prepayment].SetStyle(styleCommon);
            ws.Cells[0, ExcelDataColomns.Moneyleft].SetStyle(styleCommon);
            ws.Cells[0, ExcelDataColomns.SubContractorPart].SetStyle(styleCommon);
            ws.Cells[0, ExcelDataColomns.MoneyleftNHP].SetStyle(styleCommon);
            ws.Cells[0, ExcelDataColomns.DateMoneyGet].SetStyle(styleCommon);
            ws.Cells[0, ExcelDataColomns.NumberOfUPD].SetStyle(styleCommon);
            ws.Cells[0, ExcelDataColomns.CommentToPrice].SetStyle(styleCommon);

            AddHeaderForUpload(ws);

            //ws.Cells[0, 10].SetStyle(styleCommon);


        }

        Style SetCommonStyle(Worksheet ws)
        {
            var styleCommon = ws.Cells[0, 0].GetStyle();
            styleCommon.ForegroundColor = Color.LightGray;
            styleCommon.Pattern = BackgroundType.Solid;
            styleCommon.Font.Size = 12;
            styleCommon.Font.IsBold = true;
            return styleCommon;
        }

        void AddHeaderForUpload(Worksheet ws)
        {
            var styleCommon = SetCommonStyle(ws);
            ws.Cells[0, ExcelDataColomns.Executers].Value = "Исполнители";
            ws.Cells[0, ExcelDataColomns.PlanWorkLog].Value = "Плановые трудозатраты";
            ws.Cells[0, ExcelDataColomns.FactWorkLogs].Value = "Фактические трудозатраты";
            ws.Cells[0, ExcelDataColomns.StartFactDate].Value = "Фактическое начало";
            ws.Cells[0, ExcelDataColomns.PlanPercentByDate].Value = "Плановый процент";
            ws.Cells[0, ExcelDataColomns.FactPercentByWorkLogs].Value = "Процент использования плановых трудозатрат";
            ws.Cells[0, ExcelDataColomns.FactPercentByHOD].Value = "Фактический процент (экспертная оценка)";

            ws.Cells[0, ExcelDataColomns.NeedToImport].Value = "Импортировать?";
            ws.Cells[0, ExcelDataColomns.UID].Value = "UID";

            ws.Cells[0, ExcelDataColomns.Executers].SetStyle(styleCommon);
            ws.Cells[0, ExcelDataColomns.PlanWorkLog].SetStyle(styleCommon);
            ws.Cells[0, ExcelDataColomns.FactWorkLogs].SetStyle(styleCommon);
            ws.Cells[0, ExcelDataColomns.StartFactDate].SetStyle(styleCommon);
            ws.Cells[0, ExcelDataColomns.FinishFactDate].SetStyle(styleCommon);
            ws.Cells[0, ExcelDataColomns.PlanPercentByDate].SetStyle(styleCommon);
            ws.Cells[0, ExcelDataColomns.FactPercentByWorkLogs].SetStyle(styleCommon);
            ws.Cells[0, ExcelDataColomns.FactPercentByHOD].SetStyle(styleCommon);

            ws.Cells[0, ExcelDataColomns.NeedToImport].SetStyle(styleCommon);
            ws.Cells[0, ExcelDataColomns.UID].SetStyle(styleCommon);
        }

        void CheckValues(string str1, ref string str2, ref int counter)
        {
            if (str1 != str2)
            {
                str1 = str2;
                counter++;
            }
        }

        public void UploadAllDepartWithHOD(AppDbContext context, List<string> errorList)
        {
            Workbook wb = new Workbook();
            string sheetName = "Справочник отделов";
            wb.Worksheets.Add(sheetName);
            var ws = wb.Worksheets[sheetName];
            AddHeaderDepartWithHOD(ws);
            int count = 1;
            foreach (var depart in context.Departments.Include(x => x.HeadOfDepartment))
            {
                ws.Cells[count, ExcelDataDepart.Id].Value = depart.Id;
                ws.Cells[count, ExcelDataDepart.Name].Value = depart.Name;

                ws.Cells[count, ExcelDataDepart.HeadOfDepartment].Value = depart.HeadOfDepartment?.FullName;
                ws.Cells[count, ExcelDataDepart.Acronym].Value = depart.Acronym;
                ws.Cells[count, ExcelDataDepart.IsProduction].Value = depart.Production;
                count++;
            }
            ws.AutoFitColumns(0, 10);
            // Справочник:
            wb.Worksheets.Add("Сотрудники");
            var wsUsers = wb.Worksheets["Сотрудники"];
            //Выгружаем всех юзеров
            var users = Models.User.GetAllActiveUsersForReport(context);
            count = 1;
            foreach (var user in users)
            {
                var depart = user.Department;
                var type = depart.Production.HasValue && depart.Production.Value ? "пр" : "нп";
                wsUsers.Cells[count, 0].Value = depart.Name;
                wsUsers.Cells[count, 1].Value = user.FullName;

                wsUsers.Cells[count, 3].Value = type;
                count++;
            }

            try
            {
                wb.Save(this.FilePath);
            }
            catch (Exception exception)
            {
                errorList.Add(exception.ToString());
            }
        }

        void AddHeaderDepartWithHOD(Worksheet ws)
        {
            ws.Cells[0, ExcelDataDepart.Id].Value = "Id";
            ws.Cells[0, ExcelDataDepart.Name].Value = "Name";
            ws.Cells[0, ExcelDataDepart.HeadOfDepartment].Value = "HeadOfDepartment";
            ws.Cells[0, ExcelDataDepart.Acronym].Value = "Acronym";

            var styleCommon = ws.Cells[0, 0].GetStyle();
            styleCommon.ForegroundColor = Color.DarkCyan;
            styleCommon.Pattern = BackgroundType.Solid;
            styleCommon.Font.Size = 12;
            styleCommon.Font.IsBold = true;

            ws.Cells[0, ExcelDataDepart.Id].SetStyle(styleCommon);
            ws.Cells[0, ExcelDataDepart.Name].SetStyle(styleCommon);
            ws.Cells[0, ExcelDataDepart.HeadOfDepartment].SetStyle(styleCommon);
            ws.Cells[0, ExcelDataDepart.Acronym].SetStyle(styleCommon);

        }

        public int UploadDaysOffFromExcel(AppDbContext context)
        {
            List<Department> departList = new List<Department>();
            this.Wb = new Workbook(this.FilePath);
            this.Ws = this.Wb.Worksheets["Праздничные дни"];
            var ws = this.Ws;
            int countChangedRows = 0;
            if (ws != null)
            {
                //Удаляем все элементы 
                //List<DayOff> doList = new List<DayOff>();
                //var dayOffList = context.DayOffs.ToList();
                //foreach (var dayOff in dayOffList)
                //{
                //    context.DayOffs.Remove(dayOff);
                //}

                bool proceedReading = true;
                int count = 1;
                while (proceedReading)
                {
                    if (string.IsNullOrEmpty(ws.Cells[count, 0].StringValue))
                    {
                        if (string.IsNullOrEmpty(ws.Cells[count + 1, 0].StringValue))
                        {
                            proceedReading = false;
                        }
                    }
                    else
                    {                        
                        var res = DateTime.TryParse(ws.Cells[count, 0].StringValue, out DateTime dateDayOff);
                        if (res)
                        {
                            var newDayOff = new DayOff(dateDayOff);
                            if (context.DayOffs.Where(x => x.Date == dateDayOff).Count() == 0)
                            {
                                context.DayOffs.Add(newDayOff);
                                countChangedRows++;
                            }
                        }

                    }
                    count++;
                }                
                context.SaveChanges();
            }
            return countChangedRows;

        }
        public void UploadAllDaysOffToExcel(AppDbContext context, List<string> errorList)
        {
            Workbook wb = new Workbook();
            string sheetName = "Праздничные дни";
            wb.Worksheets.Add(sheetName);
            var ws = wb.Worksheets[sheetName];
            AddHeaderDaysOff(ws);
            int count = 1;
            var styleCommon = ws.Cells[0, 1].GetStyle();
            styleCommon.Number = 14;
            foreach (var dayOff in context.DayOffs)
            {
                ws.Cells[count, 0].Value = dayOff.Date;
                ws.Cells[count, 0].SetStyle(styleCommon);
                count++;
            }
            ws.AutoFitColumns(0, 10);
            try
            {
                wb.Save(this.FilePath);
            }
            catch (Exception exception)
            {
                errorList.Add(exception.ToString());
            }
        }

        void AddHeaderDaysOff(Worksheet ws)
        {
            ws.Cells[0, 0].Value = "Даты праздничных дней";

            var styleCommon = ws.Cells[0, 0].GetStyle();
            styleCommon.ForegroundColor = Color.DarkCyan;
            styleCommon.Pattern = BackgroundType.Solid;
            styleCommon.Font.Size = 12;
            styleCommon.Font.IsBold = true;

            ws.Cells[0, 0].SetStyle(styleCommon);

        }

        public int UploadAdmins(AppDbContext context)
        {
            List<User> userList = new List<User>();
            this.Wb = new Workbook(this.FilePath);
            this.Ws = this.Wb.Worksheets["Администраторы"];
            var ws = this.Ws;
            int countChangedRows = 0;
            if (ws != null)
            {              

                bool proceedReading = true;
                int count = 1;
                while (proceedReading)
                {
                    if (string.IsNullOrEmpty(ws.Cells[count, 0].StringValue))
                    {
                        if (string.IsNullOrEmpty(ws.Cells[count + 1, 0].StringValue))
                        {
                            proceedReading = false;
                        }
                    }
                    else
                    {
                        var resId = Int32.TryParse(ws.Cells[count, 0].StringValue, out int userId);
                        var resIsAdmin = Int32.TryParse(ws.Cells[count, 2].StringValue, out int val);
                        if (resId && resIsAdmin)
                        {
                            var roleSet = context.Roles.Where(x => x.Id == val);
                            if (roleSet.Count() > 0) {
                                userList.Add(new User() { Id = userId, Role = roleSet.First()});
                                countChangedRows++;
                            }
                        }

                    }
                    count++;
                }
                foreach (var user in userList)
                {
                    var userSetFound = context.Users.Where(x => x.Id == user.Id);
                    if (userSetFound.Count() > 0)
                    {
                        var userFromDB = userSetFound.First();
                        userFromDB.Role = user.Role;
                    }
                }
                context.SaveChanges();
            }
            return countChangedRows;

        }

        public void UploadAllAdmins(AppDbContext context, List<string> errorList)
        {
            Workbook wb = new Workbook();
            string sheetName = "Администраторы";
            wb.Worksheets.Add(sheetName);
            var ws = wb.Worksheets[sheetName];
            AddHeaderAdmins(ws);
            int count = 1;
            foreach (var user in context.Users.Include(x=>x.Role))
            {
                ws.Cells[count, 0].Value = user.Id;
                ws.Cells[count, 1].Value = user.FullName;
                ws.Cells[count, 2].Value = user.Role?.Id;
                count++;
            }
            ws.AutoFitColumns(0, 10);
            try
            {
                wb.Save(this.FilePath);
            }
            catch (Exception exception)
            {
                errorList.Add(exception.ToString());
            }
        }
        void AddHeaderAdmins(Worksheet ws)
        {
            ws.Cells[0, 0].Value = "Id";
            ws.Cells[0, 1].Value = "FullName";
            ws.Cells[0, 2].Value = "Role";

            var styleCommon = ws.Cells[0, 0].GetStyle();
            styleCommon.ForegroundColor = Color.DarkCyan;
            styleCommon.Pattern = BackgroundType.Solid;
            styleCommon.Font.Size = 12;
            styleCommon.Font.IsBold = true;

            ws.Cells[0, 0].SetStyle(styleCommon);

        }

        public void DeleteEvaluationWarnings()
        {
            var warningList = Wb.Worksheets.Where(x => x.Name.Contains("Evaluation Warning")).ToList();
            foreach (var excelList in warningList)
            {
                Wb.Worksheets.RemoveAt(excelList.Name);

            }
            Wb.Save(FilePath);
            //Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            //Microsoft.Office.Interop.Excel.Workbook wb = app.Workbooks.Open(FilePath);
            //Microsoft.Office.Interop.Excel.Sheets wsSet = wb.Worksheets;            
            //foreach (var ws in wsSet)
            //{
            //    Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)ws;
            //    if (worksheet.Name.Contains("Evaluation"))
            //    {
            //        worksheet.Delete();
            //    }

            //}
            //wb.Save();

        }

        public void CreateTCR(AppDbContext context, List<TaskCompRequest> tcrList, List<string> errorMes)
        {
            try
            {
                Wb = new Workbook(this.FilePath);
            }
            catch(Exception ex)
            {
                errorMes.Add($"Не удалось обраться к файлу {FilePath}. Возможно файл занят другим приложением или не существует.");
                return;
            }
            Ws = this.Wb.Worksheets["Справочник работ-комплектов"];
            var lastRow = GetLastFilledRow();
            var row = lastRow;
            Style commonStyle = GetCommonStyle(Ws);
            Style dateStyle = GetDateStyle(Ws);
            List<TaskComp> tcList = new List<TaskComp>();
            foreach (var tcr in tcrList)
            {
                #region Create TaskComp
                var newTaskComp = new TaskComp() //UID заполняется в конструкторе из-за недоступности
                {                  
                ProjectNumber = tcr.ProjectNumber,
                    TaskCompName = tcr.TaskCompName,
                    PlanWorkLog = tcr.PlanWorkLog,
                    StartPlanDate = tcr.StartDate,
                    FinishPlanDate = tcr.FinishDate,
                    Department = tcr.User.GetDepartmentAcronym(context)
                };
                TaskComp.GetSingleTaskCompForProject(context, newTaskComp); // newTaskComp передаётся для получения стандартных свойств проекта
                tcList.Add(newTaskComp);
                #endregion

                Ws.Cells[row, ExcelDataColomns.ProjectNumber].Value = tcr.ProjectNumber;
                Ws.Cells[row, ExcelDataColomns.ProjectNumber].SetStyle(commonStyle);
                Ws.Cells[row, ExcelDataColomns.TaskCompName].Value = tcr.TaskCompName;
                Ws.Cells[row, ExcelDataColomns.TaskCompName].SetStyle(commonStyle);
                Ws.Cells[row, ExcelDataColomns.ProjectShortName].Value = newTaskComp.ProjectShortName;
                Ws.Cells[row, ExcelDataColomns.ProjectShortName].SetStyle(commonStyle);
                Ws.Cells[row, ExcelDataColomns.GIPAcronym].Value = newTaskComp.GIPAcronym;
                Ws.Cells[row, ExcelDataColomns.GIPAcronym].SetStyle(commonStyle);
                Ws.Cells[row, ExcelDataColomns.PlanWorkLog].Value = tcr.PlanWorkLog;
                Ws.Cells[row, ExcelDataColomns.PlanWorkLog].SetStyle(commonStyle);
                Ws.Cells[row, ExcelDataColomns.StartPlanDate].Value = tcr.StartDate;
                Ws.Cells[row, ExcelDataColomns.StartPlanDate].SetStyle(dateStyle);
                Ws.Cells[row, ExcelDataColomns.FinishPlanDate].Value = tcr.FinishDate;
                Ws.Cells[row, ExcelDataColomns.FinishPlanDate].SetStyle(dateStyle);
                Ws.Cells[row, ExcelDataColomns.Department].Value = newTaskComp.Department;
                Ws.Cells[row, ExcelDataColomns.Department].SetStyle(dateStyle);
                Ws.Cells[row, ExcelDataColomns.UID].Value = newTaskComp.UID;
                Ws.Cells[row, ExcelDataColomns.UID].SetStyle(commonStyle);
                var resCS= tcr.ChangeStatus(context, WebApplication5.Models.TaskCompRequest.Status.Confirmed);
                if (!resCS) { errorMes.Add($"Невозможно поменять статус у запроса {tcr.TaskCompName} по проекту {tcr.ProjectNumber}."); }
                row++;
            }
            context.TaskComps.AddRange(tcList);
            context.SaveChanges();
            DeleteEvaluationWarnings();            
        }

        void GetProjectData(AppDbContext context, TaskComp tc)
        {
            TaskComp.GetSingleTaskCompForProject(context, tc);                   
        }

        int GetLastFilledRow()
        {
            bool proceedReading = true;
            int count = 1;
            while (proceedReading)
            {
                if (string.IsNullOrEmpty(Ws.Cells[count, ExcelDataColomns.ProjectNumber].StringValue))
                {
                    if (string.IsNullOrEmpty(Ws.Cells[count + 1, ExcelDataColomns.ProjectNumber].StringValue))
                    {
                        proceedReading = false;
                    }
                }
                count++;
            }
            return count-1;
        }

        public bool UploadProjectShares(List<ProjectShares> projectShares, List<ProjectSharesTot> prjShareTot, User[] users ,List<string> errorList, Workbook wb)
        {
            //Workbook wb = new Workbook(filepath);
            string sheetName = "ResultUser";
            //wb.Worksheets.Add(sheetName);
            var ws = wb.Worksheets[sheetName];
            //AddHeaderProjectShares(ws);
            int count = 3;

            //var styleCommon = ws.Cells[0, 0].GetStyle();          
            //styleCommon.Font.Size = 10;         
            //styleCommon.HorizontalAlignment = TextAlignmentType.Center;
                       

            foreach (var ps in projectShares)
            {
                ws.Cells[count, 0].Value = ps.User.FullName;                
                ws.Cells[count, 1].Value = GetProjectNumber(ps.ProjectNumber);
                ws.Cells[count, 2].Value = ps.TotalProjectWLVal;
                ws.Cells[count, 3].Value = ps.ProjectShare;
                count++;
            }
            count = 3;
            var styleForNonProd = ws.Cells[4, 3].GetStyle();
            styleForNonProd.Font.Color = Color.FromArgb(0, 176, 240);
            var styleForProd = ws.Cells[4, 3].GetStyle();
            styleForProd.Font.Color = Color.Black;
            var styleForAbusers = ws.Cells[4, 3].GetStyle();
            styleForAbusers.Font.Color = Color.Red;
            foreach (var ps in projectShares)
            {
                if (ps.IsProduction == false)
                {
                    ws.Cells[count, 2].SetStyle(styleForNonProd);
                    ws.Cells[count, 3].SetStyle(styleForNonProd);
                }
                else if(ps.IsProduction == true)
                {
                    ws.Cells[count, 2].SetStyle(styleForProd);
                    ws.Cells[count, 3].SetStyle(styleForProd);
                }
                if (ps.HighLight == 1)
                {
                    ws.Cells[count, 2].SetStyle(styleForAbusers);
                }
                count++;
            }

            ws.AutoFitColumn(0);
            //Aspose.Cells.Column = ws.Cells.Columns[0].Width =//GridDesktop.Data.GridColumn column = ws.Columns[0];
            //column.Width = 150;

            count = 3;
            var wsPrRes = wb.Worksheets["ResultProj"];
            foreach (var prjTotRow in prjShareTot.OrderByDescending(x=>x.TotalWLVal))
            {
                wsPrRes.Cells[count, 0].Value = GetProjectNumber(prjTotRow.ProjectNumber);
                wsPrRes.Cells[count, 1].Value = prjTotRow.TotalWLVal;
                count++;
            }

            count = 3;
            var wsUsers = wb.Worksheets["Сотрудники"];
            //Выгружаем всех юзеров
            foreach (var user in users)
            {
                string type = string.Empty;
                if (user.Department != null)
                {
                    var depart = user.Department;
                    type = depart.Production.HasValue && depart.Production.Value ? "пр" : "нп";
                    wsUsers.Cells[count, 0].Value = user.Department.Name;
                }
                else
                {
                    type = "нп";
                }                
                wsUsers.Cells[count, 1].Value = user.FullName;
                wsUsers.Cells[count, 3].Value = type;
                wsUsers.Cells[count, 4].Value = user.City;
                count++;
            }
            
            try
            {
                wb.Save(this.FilePath);
                return true;
            }
            catch (Exception exception)
            {
                errorList.Add(exception.ToString());
                return false;
            }
        }

        
        public string GetProjectNumber(string projectNumber)
        {
            var pattern = ".*\\((.+)\\).*";
            Regex regex = new Regex(pattern);
            var match = regex.Match(projectNumber);
            if (match.Captures.Count > 0)
            {
                return match.Groups[1].Value;
            }
            return projectNumber;
        }
        public bool UploadProjectSharesWithTaskName(List<ProjectShares> projectShares, List<ProjectSharesTaskCompTot> projectSharesTaskCompTotList, List<TaskComp> taskCompSet, User[] users, List<string> errorList, Workbook wb)
        {
           // Workbook wb = new Workbook(ExcelFiles.FilePath.workLogsTaskCompTemplatePath);
            string sheetName = "Доли по сотрудникам(комплекты)";
            //wb.Worksheets.Add(sheetName);
            var ws = wb.Worksheets[sheetName];
            //AddHeaderProjectShares(ws);
            int count = 3;

            //var styleCommon = ws.Cells[0, 0].GetStyle();          
            //styleCommon.Font.Size = 10;         
            //styleCommon.HorizontalAlignment = TextAlignmentType.Center;

            foreach (var ps in projectShares)
            {
                ws.Cells[count, 0].Value = ps.User.FullName;
                ws.Cells[count, 1].Value = ps.TaskCompName;
                ws.Cells[count, 2].Value = ps.ProjectNumber;
                ws.Cells[count, 3].Value = ps.TotalProjectWLVal;
                ws.Cells[count, 4].Value = ps.ProjectShare;
                count++;
            }
            count = 3;
            var styleForNonProd = ws.Cells[4, 3].GetStyle();
            styleForNonProd.Font.Color = Color.FromArgb(0, 176, 240);
            var styleForProd = ws.Cells[4, 3].GetStyle();
            styleForProd.Font.Color = Color.Black;
            var styleForAbusers = ws.Cells[4, 3].GetStyle();
            styleForAbusers.Font.Color = Color.Red;
            foreach (var ps in projectShares)
            {
                //if (ps.IsProduction == false)
                //{
                //    ws.Cells[count, 2].SetStyle(styleForNonProd);
                //    ws.Cells[count, 3].SetStyle(styleForNonProd);
                //}
                //else if (ps.IsProduction == true)
                //{
                    ws.Cells[count, 2].SetStyle(styleForProd);
                    ws.Cells[count, 3].SetStyle(styleForProd);
               // }
                if (ps.HighLight == 1)
                {
                    ws.Cells[count, 2].SetStyle(styleForAbusers);
                }
                count++;
            }

            ws.AutoFitColumn(0);
            //Aspose.Cells.Column = ws.Cells.Columns[0].Width =//GridDesktop.Data.GridColumn column = ws.Columns[0];
            //column.Width = 150;

            count = 3;
            var wsPrRes = wb.Worksheets["Доли по комплектам"];
            foreach (var prjTotRow in projectSharesTaskCompTotList.OrderByDescending(x=>x.TotalWLVal))
            {
                
                wsPrRes.Cells[count, 0].Value = prjTotRow.ProjectNumber;
                wsPrRes.Cells[count, 1].Value = prjTotRow.TaskCompName;
                wsPrRes.Cells[count, 2].Value = prjTotRow.TotalWLVal;
                count++;
            }

            //count = 3;
            //var wsUsers = wb.Worksheets["Сотрудники"];
            ////Выгружаем всех юзеров
            //foreach (var user in users)
            //{
            //    string type = string.Empty;
            //    if (user.Department != null)
            //    {
            //        var depart = user.Department;
            //        type = depart.Production.HasValue && depart.Production.Value ? "пр" : "нп";
            //        wsUsers.Cells[count, 0].Value = user.Department.Name;
            //    }
            //    else
            //    {
            //        type = "нп";
            //    }
            //    wsUsers.Cells[count, 1].Value = user.FullName;
            //    wsUsers.Cells[count, 3].Value = type;
            //    count++;
            //}

            try
            {
                wb.Save(this.FilePath);
                return true;
            }
            catch (Exception exception)
            {
                errorList.Add(exception.ToString());
                return false;
            }
        }

        void AddHeaderProjectShares(Worksheet ws)
        {
            ws.Cells[0, 2].PutValue("=ПРОМЕЖУТОЧНЫЕ.ИТОГИ(9;C4:C5035)");
            ws.Cells[0, 3].Value = "=ПРОМЕЖУТОЧНЫЕ.ИТОГИ(9;D4:D5035)";

            ws.Cells[1, 0].Value = "ФИО";
            ws.Cells[1, 1].Value = "Внутренний номер проекта";
            ws.Cells[1, 2].Value = "Поданные трудозатраты";
            ws.Cells[1, 3].Value = "Доля трудозатрат";

            var styleCommon = ws.Cells[0, 0].GetStyle();
            System.Drawing.Color color = System.Drawing.ColorTranslator.FromHtml("#B4C6E7");
            styleCommon.ForegroundColor = color;
            styleCommon.Pattern = BackgroundType.Solid;
            styleCommon.Font.Size = 12;
            styleCommon.Font.IsBold = true;
            styleCommon.HorizontalAlignment = TextAlignmentType.Center;

            ws.Cells[1, 0].SetStyle(styleCommon);
            ws.Cells[1, 1].SetStyle(styleCommon);
            ws.Cells[1, 2].SetStyle(styleCommon);
            ws.Cells[1, 3].SetStyle(styleCommon);

            ws.Cells[2, 0].Value = "1";
            ws.Cells[2, 1].Value = "2";
            ws.Cells[2, 2].Value = "3";
            ws.Cells[2, 3].Value = "4";

            var styleNum = ws.Cells[0, 0].GetStyle();
            styleNum.Pattern = BackgroundType.Solid;
            styleNum.Font.Size = 12;
            styleNum.Font.IsBold = true;
            styleNum.HorizontalAlignment = TextAlignmentType.Center;

            ws.Cells[2, 0].SetStyle(styleNum);
            ws.Cells[2, 1].SetStyle(styleNum);
            ws.Cells[2, 2].SetStyle(styleNum);
            ws.Cells[2, 3].SetStyle(styleNum);
        }

        public bool UploadTaskCompsAndWorkLogs(List<WorkLogs> wlList, List<TaskComp> taskCompList , AppDbContext context)
        {            
            Workbook wb = new Workbook();
            wb.Worksheets.RemoveAt("Sheet1");

            //Выгружаем taskComp по проекту
            var ws = wb.Worksheets.Add("Отчёт");
            int count = 1;
            ws.Cells[0, 0].Value = "Проект";
            ws.Cells[0, 1].Value = "Наименование работы";
            ws.Cells[0, 2].Value = "Ответственный отдел";
            ws.Cells[0, 3].Value = "Исполнители";
            ws.Cells[0, 4].Value = "Дата начала";
            ws.Cells[0, 5].Value = "Дата окончания";
            ws.Cells[0, 6].Value = "Плановые трудозатраты";
            ws.Cells[0, 7].Value = "Дата начала факт";
            ws.Cells[0, 8].Value = "Процент завершения";
            ws.Cells[0, 9].Value = "Трудозатраты факт";

            var styleCommon = ws.Cells[0, 0].GetStyle();
            System.Drawing.Color color = System.Drawing.ColorTranslator.FromHtml("#B4C6E7");
            styleCommon.ForegroundColor = color;
            styleCommon.Pattern = BackgroundType.Solid;
            styleCommon.Font.Size = 12;
            styleCommon.Font.IsBold = true;
            styleCommon.HorizontalAlignment = TextAlignmentType.Center;
            styleCommon.IsTextWrapped = true;            

            var styleRow = ws.Cells[1, 0].GetStyle();
            styleRow.IsTextWrapped = true;
            styleRow.HorizontalAlignment = TextAlignmentType.Center;
            styleRow.VerticalAlignment = TextAlignmentType.Center;

            ws.Cells[0, 0].SetStyle(styleCommon);
            ws.Cells[0, 1].SetStyle(styleCommon);            
            ws.Cells[0, 2].SetStyle(styleCommon);
            ws.Cells[0, 3].SetStyle(styleCommon);
            ws.Cells[0, 4].SetStyle(styleCommon);
            ws.Cells[0, 5].SetStyle(styleCommon);
            ws.Cells[0, 6].SetStyle(styleCommon);
            ws.Cells[0, 7].SetStyle(styleCommon);
            ws.Cells[0, 8].SetStyle(styleCommon);
            ws.Cells[0, 9].SetStyle(styleCommon);

            ws.Cells.SetColumnWidth(0, 23.0);
            ws.Cells.SetColumnWidth(1, 70.0);
            ws.Cells.SetColumnWidth(2, 20.0);
            ws.Cells.SetColumnWidth(3, 50.0);
            ws.Cells.SetColumnWidth(4, 15.0);
            ws.Cells.SetColumnWidth(5, 15.0);
            ws.Cells.SetColumnWidth(6, 20.0);
            ws.Cells.SetColumnWidth(7, 15.0);
            ws.Cells.SetColumnWidth(8, 15.0);
            ws.Cells.SetColumnWidth(9, 20.0);

            foreach (var taskComp in taskCompList)
            {
                ws.Cells[count, 0].Value = taskComp.ProjectNumber;                
                ws.Cells[count, 1].Value = taskComp.TaskCompName;
                ws.Cells[count, 2].Value = taskComp.Department;
                ws.Cells[count, 3].Value = taskComp.Executers;
                ws.Cells[count, 4].Value = taskComp.StartPlanDate?.ToShortDateString();
                ws.Cells[count, 5].Value = taskComp.FinishPlanDate?.ToShortDateString();
                ws.Cells[count, 6].Value = taskComp.PlanWorkLog?.ToString();
                ws.Cells[count, 7].Value = taskComp.StartFactDate?.ToShortDateString();
                ws.Cells[count, 8].Value = taskComp.CompletePercent?.ToString();
                ws.Cells[count, 9].Value = taskComp.FactWorkLog?.ToString();

                ws.Cells[count, 0].SetStyle(styleRow);
                ws.Cells[count, 1].SetStyle(styleRow);
                ws.Cells[count, 2].SetStyle(styleRow);
                ws.Cells[count, 3].SetStyle(styleRow);
                ws.Cells[count, 4].SetStyle(styleRow);
                ws.Cells[count, 5].SetStyle(styleRow);
                ws.Cells[count, 6].SetStyle(styleRow);
                ws.Cells[count, 7].SetStyle(styleRow);
                ws.Cells[count, 8].SetStyle(styleRow);
                ws.Cells[count, 9].SetStyle(styleRow);

                count++;
            }           

            //Выгрузка детального отчёта
            ws = wb.Worksheets.Add("Детальный отчёт ");

            count = 1;
            ws.Cells[0, 0].Value = "Проект";
            ws.Cells[0, 1].Value = "Работа/Комплект";
            ws.Cells[0, 2].Value = "ФИО";
            ws.Cells[0, 3].Value = "Отдел";
            ws.Cells[0, 4].Value = "Дата трудозатрат";
            ws.Cells[0, 5].Value = "Кол-во, ч";            
                        
            ws.Cells[0, 0].SetStyle(styleCommon);
            ws.Cells[0, 1].SetStyle(styleCommon);
            ws.Cells[0, 2].SetStyle(styleCommon);
            ws.Cells[0, 3].SetStyle(styleCommon);
            ws.Cells[0, 4].SetStyle(styleCommon);
            ws.Cells[0, 5].SetStyle(styleCommon);

            count = 1;
            
            foreach (var wl in wlList.OrderBy(x => x.DateOfReport))
            {
                
                if (wl.User != null && wl.User.FirstName != null)
                {
                    var taskComp = TaskComp.FindTaskCompById(wl.TaskComp_id, context);
                    ws.Cells[count, 0].Value = wl.Proj_id;
                    ws.Cells[count, 1].Value = taskComp != null ? taskComp.TaskCompName : string.Empty;
                    ws.Cells[count, 2].Value = $"{wl.User.LastName} {wl.User.FirstName[0]}.{wl.User.MiddleName[0]}.";
                    ws.Cells[count, 3].Value = wl.User.GetDepartmentAcronym(context);                       
                    ws.Cells[count, 4].Value = wl.DateOfReport.ToShortDateString();
                    ws.Cells[count, 5].Value = wl.WorkTime.TotalHours;
                    count++;
                }
            }
            
            ws.AutoFitColumns(0, 9);            
            wb.Save(FilePath);
            return true;
        }

        public bool UploadWorkLogs(List<WorkLogs> wlList, AppDbContext context)
        {
            Workbook wb = new Workbook();
           // string sheetName = "ResultUser";
            //wb.Worksheets.Add(sheetName);
            var ws = wb.Worksheets.Add("Выгрузка за период");
            //AddHeaderProjectShares(ws);
            int count = 1;

            //var styleCommon = ws.Cells[0, 0].GetStyle();          
            //styleCommon.Font.Size = 10;         
            //styleCommon.HorizontalAlignment = TextAlignmentType.Center;

            ws.Cells[0, 0].Value = "Отдел";
            ws.Cells[0, 1].Value = "ФИО";
            ws.Cells[0, 2].Value = "Проект";
            ws.Cells[0, 3].Value = "Работа/Комплект";
            ws.Cells[0, 4].Value = "Дата трудозатрат";
            ws.Cells[0, 5].Value = "Кол-во, ч";


            var styleCommon = ws.Cells[0, 0].GetStyle();
            System.Drawing.Color color = System.Drawing.ColorTranslator.FromHtml("#B4C6E7");
            styleCommon.ForegroundColor = color;
            styleCommon.Pattern = BackgroundType.Solid;
            styleCommon.Font.Size = 12;
            styleCommon.Font.IsBold = true;
            styleCommon.HorizontalAlignment = TextAlignmentType.Center;

            ws.Cells[0, 0].SetStyle(styleCommon);
            ws.Cells[0, 1].SetStyle(styleCommon);
            ws.Cells[0, 2].SetStyle(styleCommon);
            ws.Cells[0, 3].SetStyle(styleCommon);
            ws.Cells[0, 4].SetStyle(styleCommon);
            ws.Cells[0, 5].SetStyle(styleCommon);

            foreach (var wl in wlList.OrderBy(x => x.DateOfReport))
            {
                if (wl.User!=null && wl.User.FirstName != null)
                {
                    var taskComp = TaskComp.FindTaskCompById(wl.TaskComp_id, context);
                    ws.Cells[count, 0].Value = wl.User.GetDepartmentAcronym(context);
                    ws.Cells[count, 1].Value = $"{wl.User.LastName} {wl.User.FirstName[0]}.{wl.User.MiddleName[0]}.";
                    ws.Cells[count, 2].Value = wl.Proj_id;
                    ws.Cells[count, 3].Value = taskComp != null ? taskComp.TaskCompName : string.Empty;
                    ws.Cells[count, 4].Value = wl.DateOfReport.ToShortDateString();
                    ws.Cells[count, 5].Value = wl.WorkTime.TotalHours;
                    count++;
                }
            }
            ws.AutoFitColumns(0, 9);
            wb.Save(FilePath);
            return true;
        }

        public bool UploadWorkLogsTotalReport(IOrderedEnumerable<TotalWorkLog> wlList, AppDbContext context)
        {
            Workbook wb = new Workbook();
            // string sheetName = "ResultUser";
            //wb.Worksheets.Add(sheetName);
            var ws = wb.Worksheets.Add("Выгрузка за месяц");
            wb.Worksheets.RemoveAt(0);
            //AddHeaderProjectShares(ws);
            int count = 1;

            //var styleCommon = ws.Cells[0, 0].GetStyle();          
            //styleCommon.Font.Size = 10;         
            //styleCommon.HorizontalAlignment = TextAlignmentType.Center;
            Style abuserStyle = GetAbuserStyle(ws);
            Style commonStyle = GetCommonStyleWl(ws);
            AddHeadersWlList(ws);
            foreach (var wl in wlList)
            {
                var shortName = GetShortName(wl.User);
                ws.Cells[count, 0].Value = shortName;
                //ws.Cells[count, 0].Value = wl.DepartAcr;
                ws.Cells[count, 1].Value = wl.TotalSendedWorkLogs;
                ws.Cells[count, 2].Value = wl.TotalWorkLogsShouldBe;
              //  ws.Cells[count, 4].Value = wl.User.Id;

                if (wl.TotalWorkLogsShouldBe > wl.TotalSendedWorkLogs + 16)
                {
                    ws.Cells[count, 2].SetStyle(abuserStyle);
                }
                else
                {
                    ws.Cells[count, 2].SetStyle(commonStyle);
                }
                ws.Cells[count, 3].SetStyle(commonStyle);
                count++;                              
            }
            ws.AutoFitColumns(0, 3);
            wb.Save(FilePath);
            return true;
        }

        public bool UploadAbsenceTotalReport(List<Absence> abList, AppDbContext context)
        {
            Workbook wb = new Workbook();
            // string sheetName = "ResultUser";
            //wb.Worksheets.Add(sheetName);
            var ws = wb.Worksheets.Add("Отсутствия за месяц");
            //AddHeaderProjectShares(ws);
            int count = 1;

            //var styleCommon = ws.Cells[0, 0].GetStyle();          
            //styleCommon.Font.Size = 10;         
            //styleCommon.HorizontalAlignment = TextAlignmentType.Center;            
            Style commonStyle = GetCommonStyleWl(ws);
            AddHeadersAbList(ws);
            foreach (var ab in abList)
            {
                var shortName = GetShortName(ab.User);
                ws.Cells[count, 1].Value = shortName;
                ws.Cells[count, 0].Value = ab.User.GetDepartmentAcronym(context);
                ws.Cells[count, 2].Value = ab.StartDate.ToShortDateString();
                ws.Cells[count, 3].Value = ab.FinishDate?.ToShortDateString();
                ws.Cells[count, 4].Value = ab.HourAmount;
                ws.Cells[count, 5].Value = ab.Reason.ReasonName;

                ws.Cells[count, 0].SetStyle(commonStyle);
                ws.Cells[count, 1].SetStyle(commonStyle);
                ws.Cells[count, 2].SetStyle(commonStyle);                
                ws.Cells[count, 3].SetStyle(commonStyle);
                ws.Cells[count, 4].SetStyle(commonStyle);
                ws.Cells[count, 5].SetStyle(commonStyle);
                count++;
            }
            ws.AutoFitColumns(0, 5);
            wb.Save(FilePath);
            return true;
        }

        Style GetCommonStyleWl(Worksheet ws)
        {
            var style = ws.Cells[0, 0].GetStyle();
            style.Font.Size = 10;
            style.Font.IsBold = true;            
            style.HorizontalAlignment = TextAlignmentType.Center;
            return style;
        }

        Style GetAbuserStyle(Worksheet ws)
            {
                var style = ws.Cells[0, 0].GetStyle();
                style.Font.Size = 12;
                style.Font.IsBold = true;
                style.Font.Color = Color.Red;
                style.HorizontalAlignment = TextAlignmentType.Center;
                return style;
            }

        void AddHeadersWlList(Worksheet ws)
        {
           // ws.Cells[0, 0].Value = "Отдел";
            ws.Cells[0, 0].Value = "ФИО";
            ws.Cells[0, 1].Value = "Кол-во поданных трудозатрат";
            ws.Cells[0, 2].Value = "Нормативное кол-во поданных трудозатрат";

            var styleCommon = ws.Cells[0, 0].GetStyle();
            System.Drawing.Color color = System.Drawing.ColorTranslator.FromHtml("#B4C6E7");
            styleCommon.ForegroundColor = color;
            styleCommon.Pattern = BackgroundType.Solid;
            styleCommon.Font.Size = 12;
            styleCommon.Font.IsBold = true;
            styleCommon.HorizontalAlignment = TextAlignmentType.Center;

            ws.Cells[0, 0].SetStyle(styleCommon);
            ws.Cells[0, 1].SetStyle(styleCommon);
            ws.Cells[0, 2].SetStyle(styleCommon);
            ws.Cells[0, 3].SetStyle(styleCommon);
        }

        void AddHeadersAbList(Worksheet ws)
        {
            ws.Cells[0, 0].Value = "Отдел";
            ws.Cells[0, 1].Value = "ФИО";
            ws.Cells[0, 2].Value = "Дата начала отсутствия";
            ws.Cells[0, 3].Value = "Дата окончания отсутсвия";
            ws.Cells[0, 4].Value = "Кол-во часов";
            ws.Cells[0, 5].Value = "Причина отсутствия";

            var styleCommon = ws.Cells[0, 0].GetStyle();
            System.Drawing.Color color = System.Drawing.ColorTranslator.FromHtml("#B4C6E7");
            styleCommon.ForegroundColor = color;
            styleCommon.Pattern = BackgroundType.Solid;
            styleCommon.Font.Size = 12;
            styleCommon.Font.IsBold = true;
            styleCommon.HorizontalAlignment = TextAlignmentType.Center;

            ws.Cells[0, 0].SetStyle(styleCommon);
            ws.Cells[0, 1].SetStyle(styleCommon);
            ws.Cells[0, 2].SetStyle(styleCommon);
            ws.Cells[0, 3].SetStyle(styleCommon);
            ws.Cells[0, 4].SetStyle(styleCommon);
            ws.Cells[0, 5].SetStyle(styleCommon);
        }



        string GetShortName(User user)
        {
            string lastName = string.Empty;
            string firstName = string.Empty;
            string middleName = string.Empty;
            if (!string.IsNullOrEmpty(user.LastName)){
                lastName = user.LastName;
            }
            if (!string.IsNullOrEmpty(user.FirstName)){
                firstName = user.FirstName[0].ToString();
            }
            if (!string.IsNullOrEmpty(user.MiddleName)){
                middleName = user.MiddleName[0].ToString();
            }
            return $"{lastName} {firstName}.{middleName}.";
        }
    }
}