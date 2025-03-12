using Aspose.Cells;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebApplication5.Models.StaticData;

namespace WebApplication5.Models
{
    public class TaskComp
    {     
        [Key]
        public int Id { get; set; }
        public string UID { get; private set; }
        public string GIPAcronym { get; set; }
        public string Division { get; set; }
        public string ProjectShortName { get; set; }
        public string ProjectNumber { get; set; }
        public string ProjectStage { get; set; }

        public string TaskCompName { get; set; }
        public string OperationName { get; set; }
        public string Department { get; set; }
        public string DivisionForReport { get; set; }

        public string Executers { get; set; }
        public string DepartmentSendTask { get; set; }
        public DateTime? PlanDateSendTask { get; set; }

        public DateTime? FactDateSendTask { get; set; }

        public DateTime? FisnishContractDate { get; set; }

        public string ContractStatus { get; set; }

        public DateTime? StartPlanDate { get; set; }

        public DateTime? FinishPlanDate { get; set; }

        public Double? PlanWorkLog { get; set; }

        public DateTime? StartFactDate { get; set; }

        public DateTime? FinishFactDate { get; set; }

        public DateTime? ApproveFactDate { get; set; }

        public Double? FactWorkLog { get; set; }

        public Double? CompletePercent { get; set; }

        public int ExcelLineNumber { get; }         
        
        //СВОЙСТВА ОТНОСЯЩИЕСЯ К СТОИМОСТИ

        public double? Price { get; set; }
        public double? Prepayment { get; set; }
        public double? Moneyleft { get; set; }

        public double? SubContractorPart { get; set; }

        public double? MoneyleftNHP { get; set; }
        public DateTime? DateMoneyGet { get; set; }
        public string NumberOfUPD { get; set; }
        public string TaskCompStatus { get; set; }

        public DateTime? TaskCompCreationDate { get; set; }

        [NotMapped]
        public static string[] StandartTaskCompAr = new string[] { "Командировка"};

        public static string[] StandartEduTaskCompAr { get; set; }

        public enum TaskCompStatusEnum {DELETED, COMPLETED, ACTIVE }
        static public List<string> TaskCompStatusStr= new List<string> { "Удален", "Завершён" };
        public TaskComp(Guid uid, int excelLineNumber)
        {
            UID = uid.ToString();
            ExcelLineNumber = excelLineNumber;
        }
        public TaskComp(Guid uid)
        {
            UID = uid.ToString();           
        }
        /// <summary>
        /// Создаётся GUID автоматически
        /// </summary>
        public TaskComp()
        {
            UID = Guid.NewGuid().ToString();
        }


        public static void CheckTaskCompEdu(AppDbContext context)
        {
            var taskCompEduSet = GetStandartEduTaskCompName();
            var existTaskCompEduSet = context.TaskComps.Where(x => x.ProjectNumber == ProjectNames.ProjectEdu);
            var departAcron = Models.Department.GetAllDepartAcronyms(context);
            foreach (var taskCompEdu in taskCompEduSet)
            {
               if (existTaskCompEduSet.Where(x => x.TaskCompName == taskCompEdu).Count()==0)
                {
                    var newTaskCompEdu = new TaskComp()
                    {
                        ProjectNumber = ProjectNames.ProjectEdu,
                        TaskCompName = taskCompEdu,
                        Department = departAcron
                    };
                    context.TaskComps.Add(newTaskCompEdu);
                }               
            }
            foreach (var taskCompEdu in existTaskCompEduSet)
            {
                taskCompEdu.Department = departAcron;
            }
            context.SaveChanges();
        }

        static List<string> GetStandartEduTaskCompName()
        {
            List<string> taskCompEdu = new List<string>();
            string errorMes = string.Empty;
            var path = @"\\srv-ws\WsSettings\TaskCompEduPrj.xlsx";
            Workbook wb = null;
            try
            {
                wb = new Workbook(path);
                Worksheet ws = wb.Worksheets["TaskCompEdu"];
                if (ws != null)
                {
                    bool continueToRead = true;
                    int i = 0;
                    while (continueToRead)
                    {
                        var val = ws.Cells[i, 0].Value;
                        if (val != null)
                        {
                            taskCompEdu.Add(val.ToString());                            
                        }
                        else
                        {
                            continueToRead = false;
                        }
                        i++;
                    }
                }

            }
            catch
            {
                errorMes = $"Файл {path} недоступен, возможно он занят другим приложением";                
            }
            return taskCompEdu;        
            
        } 

        static IQueryable<TaskComp> GetAllTaskForDepart(string acronymDepart, AppDbContext context, string additDepart)
        {
            IQueryable<TaskComp> taskCompList;
            if (string.IsNullOrEmpty(additDepart))
            {
                if (acronymDepart == DepartAcronym.ALM)
                {
                    taskCompList = context.TaskComps.Where(x => x.Department.Contains(DepartAcronym.ALM));
                }
                else if (acronymDepart == DepartAcronym.MSK)
                {
                    taskCompList = context.TaskComps.Where(x => x.Department.Contains(DepartAcronym.MSK));
                }
                else if (acronymDepart == DepartAcronym.NK)
                {
                    taskCompList = context.TaskComps.Where(x => x.Department.Contains(DepartAcronym.NK));
                }
                else if (acronymDepart == DepartAcronym.UFA)
                {
                    taskCompList = context.TaskComps.Where(x => x.Department.Contains(DepartAcronym.UFA));
                }
                else if (acronymDepart == DepartAcronym.NNIZ)
                {
                    taskCompList = context.TaskComps.Where(x => x.Department.Contains(DepartAcronym.NNIZ));
                }
                else if (acronymDepart == DepartAcronym.SLV)
                {
                    taskCompList = context.TaskComps.Where(x => x.Department.Contains(DepartAcronym.SLV));
                }
                else
                {
                    ReplaceNum(ref acronymDepart); //Чтобы отображались работы СО для СО2
                    taskCompList = context.TaskComps.Where(x => x.Department.Contains(acronymDepart));
                }
            }
            else
            {
                if (acronymDepart == DepartAcronym.ALM)
                {
                    taskCompList = context.TaskComps.Where(x => x.Department.Contains(DepartAcronym.ALM) || x.Department.Contains(additDepart));
                }
                else if (acronymDepart == DepartAcronym.MSK)
                {
                    taskCompList = context.TaskComps.Where(x => x.Department.Contains(DepartAcronym.MSK) || x.Department.Contains(additDepart));
                }
                else if (acronymDepart == DepartAcronym.NK)
                {
                    taskCompList = context.TaskComps.Where(x => x.Department.Contains(DepartAcronym.NK) || x.Department.Contains(additDepart));
                }
                else if (acronymDepart == DepartAcronym.UFA)
                {
                    taskCompList = context.TaskComps.Where(x => x.Department.Contains(DepartAcronym.UFA) || x.Department.Contains(additDepart));
                }
                else if (acronymDepart == DepartAcronym.NNIZ)
                {
                    taskCompList = context.TaskComps.Where(x => x.Department.Contains(DepartAcronym.NNIZ) || x.Department.Contains(additDepart));
                }

                else
                {
                    ReplaceNum(ref acronymDepart); //Чтобы отображались работы СО для СО2
                    taskCompList = context.TaskComps.Where(x => x.Department.Contains(acronymDepart) || x.Department.Contains(additDepart));
                }
            }          
            

            return taskCompList;
        }

       

        static void ReplaceNum(ref string acronymDepart)
        {
            if (!string.IsNullOrEmpty(acronymDepart))
            acronymDepart = acronymDepart.Replace("1", "").Replace("2", "").Replace("3", "");
        }

        public static IQueryable<TaskComp> GetAllTasksForSelectedProject(AppDbContext context, string projectNum)
        {
            var taskCompSet = context.TaskComps.Where(x => x.ProjectNumber == projectNum);
            var taskCompList = FilterByStatus(taskCompSet);
            return taskCompSet;
        }

        public static List<TaskComp> GetAllTasksForMyDepartmentCurMonth(User curUser, AppDbContext context)
        {
            var taskCompList = GetAllTasksForMyDep(curUser, context);
                var taskCompListFiltered = FilterTaskSetByCurMonth(taskCompList);
            taskCompListFiltered = FilterByStatus(taskCompListFiltered);
                return taskCompListFiltered;      
        }

        public static List<DHX.Gantt.Models.GanttTask> GetAllTasksForMyDepartmentCurMonthJson(User curUser, AppDbContext context)
        {
            var taskCompList = GetAllTasksForMyDep(curUser, context);
            var taskCompListFiltered = FilterTaskSetByCurMonth(taskCompList);
            taskCompListFiltered = FilterByStatus(taskCompListFiltered);
            var jsonTaskCompList = ConvertToTask(taskCompListFiltered);
            return jsonTaskCompList;
        }

        public static List<DHX.Gantt.Models.GanttTask> ConvertToTask(List<TaskComp> taskCompListFiltered)
        {
            //string jsonString = "[";
            List<DHX.Gantt.Models.GanttTask> taskList = new List<DHX.Gantt.Models.GanttTask>(); 
            foreach (var elem in taskCompListFiltered)
            {
                var newTask = new DHX.Gantt.Models.GanttTask()
                {
                    Id = elem.Id,
                    Text = elem.TaskCompName,
                    Duration = 4,
                    Progress = 0.4m,
                    StartDate = elem.StartPlanDate.HasValue ? elem.StartPlanDate.Value : DateTime.Now
                };
                taskList.Add(newTask);
                //string dateForJS = string.Empty;
                //if (elem.StartPlanDate.HasValue) {
                //    var startDate = elem.StartPlanDate.Value;
                //    dateForJS = $"{startDate.Day}-{startDate.Month}-{startDate.Year}";
                //        }
                //string duration = string.Empty;
                //if (elem.StartPlanDate.HasValue && elem.FinishPlanDate.HasValue)
                //{
                //    duration = (elem.FinishPlanDate.Value - elem.StartPlanDate.Value).TotalDays.ToString();
                //}
                //jsonString += $"{{ id: {elem.Id}, project: \"{elem.ProjectNumber}\", text:\"{elem.TaskCompName}\", work:\"\", resources: \"\", start_date: \"{dateForJS}\", duration: \"{duration}\", intensity: 1, planned_progress: {elem.CompletePercent}}}";
            }
            //var dataser = JsonConvert.SerializeObject(taskList);
            //jsonString += "]";
            //string jsonStringRepl = jsonString.Replace(@"\", "");

            return taskList;
        }

        public static List<TaskComp> GetAllTasksForGIPCurMonth(User curUser, AppDbContext context)
        {
            var taskCompList = GetAllTasksForGIP(curUser, context);
            taskCompList = FilterTaskSetByCurMonth(taskCompList);
            return taskCompList;
        }

        public static List<TaskComp> GetAllTasksForGIP(User curUser, AppDbContext context)
        {
            List<string> acronymList = curUser.GetOptionsOfGIPAcromym();
            //Фильтрация TaskComp по ГИПу
            var taskSet = TaskComp.GetAllTasks(context).Where(x => acronymList.Contains(x.GIPAcronym?.Trim().Replace(".", "").Replace(" ", ""))).ToList();
            taskSet = FilterByStatus(taskSet);
            return taskSet;
        }

        public static List<TaskComp> GetAllTasksForMyDepartmentCurMonthWithoutExecutor(string acronymDepart, AppDbContext context, string additDepart)
        {

            var taskCompQua = GetAllTaskForDepart(acronymDepart, context, additDepart);
            taskCompQua = FilterTaskSetByAppreveFactDate(taskCompQua);
            var taskCompList = FilterTaskSetByCurMonth(taskCompQua);
            taskCompList = FilterTaskSetByNoExecuter(taskCompList);
            taskCompList = FilterTaskByYear(taskCompList);
            //taskCompList = FilterByStatus(taskCompList);
            return taskCompList;
        }

        static List<TaskComp> FilterTaskByYear(List<TaskComp> taskSetList)
        {
            return taskSetList.Where(x => x.FinishPlanDate.HasValue && x.FinishPlanDate.Value.Year>=DateTime.Now.Year-1).ToList();
        }

        static IQueryable<TaskComp> FilterTaskSetByAppreveFactDate(IQueryable<TaskComp> taskSet)
        {
            return taskSet.Where(x=>x.ApproveFactDate==null);
        }

        


        public static List<TaskComp> GetAllTasksForMyDepartmentWithoutExecutor(string acronymDepart, AppDbContext context, string additDepart)
        {
            var taskCompList = GetAllTaskForDepart(acronymDepart, context, additDepart);         
            var taskCompListFiltered1 = FilterTaskSetByNoExecuter(taskCompList);
            taskCompListFiltered1 = FilterTaskByYear(taskCompListFiltered1); // 
            return taskCompListFiltered1;
        }

        public static IQueryable<TaskComp> GetAllTasksForMyDepartmentCalcFactWorkLog(User curUser, AppDbContext context)
        {
            var taskCompList = GetAllTasksForMyDep(curUser, context);
            foreach (var taskComp in taskCompList.OrderBy(x=>x.TaskCompName))
            {
                taskComp.FactWorkLog = WorkLogs.GetTotalHoursOnTask(taskComp, context, out DateTime? startDate);
            }
            return taskCompList;
        }

        public static IQueryable<TaskComp> GetAllTasksForMyDepartment(User curUser, AppDbContext context)
        {
            var taskCompList = GetAllTasksForMyDep(curUser, context);           
            return taskCompList;
        }

        public static IQueryable<TaskComp> GetAllTasksForMyDepartmentForProj(User curUser, AppDbContext context, string projectNum)
        {
            var taskCompList = GetAllTasksForMyDep(curUser, context);         
            return taskCompList;
        }

        static IQueryable<TaskComp> GetAllTasksForMyDep(User curUser, AppDbContext context)
        {            
            var acronymDepart = curUser.GetDepartmentAcronym(context);
            var acronymDepartAddit = curUser.AdditionalDepartmentAcronym;
            var taskCompList = GetAllTaskForDepart(acronymDepart, context, acronymDepartAddit);            
            return taskCompList;
        }

        public static List<TaskComp> GetAllTasksForMyDepartSelectedProject(User curUser, string project , AppDbContext context)
        {
            var isHOD = curUser.isHeadOfDepartment(context);
            var IsGIP = curUser.IsGIPCheck(context);
            var taskCompList = GetAllTasksForMyDep(curUser, context).Where(x=>x.ProjectNumber==project).ToList();
            //Исключаем из списка "Работу начальника отдела", если не начальник отдела.
            //if (!isHOD)
            //{
            //    var hodWorkSet = taskCompList.Where(x => x.TaskCompName == TaskComp.StandartTaskCompAr[0]);
            //    if (hodWorkSet.Count() > 0)
            //    {
            //        var hodWork = hodWorkSet.First();
            //        taskCompList.Remove(hodWork);
            //    }
            //}
            //if (!IsGIP)
            //{
            //    var hodWorkSet = taskCompList.Where(x => x.TaskCompName == TaskComp.StandartTaskCompAr[2]);
            //    if (hodWorkSet.Count() > 0)
            //    {
            //        var hodWork = hodWorkSet.First();
            //        taskCompList.Remove(hodWork);
            //    }
            //}

            foreach (var taskComp in taskCompList.OrderBy(x=>x.TaskCompName))
            {
                taskComp.FactWorkLog = WorkLogs.GetTotalHoursOnTask(taskComp, context, out DateTime? startDate);
            }
            //taskCompList = FilterTaskByYear(taskCompList);
            return taskCompList;
        }

       

        public static List<TaskComp> GetAllTasksForMyDepartmentNotCompleted(User curUser, AppDbContext context)
        {
            var taskCompList = GetAllTasksForMyDep(curUser, context).Where(x=>x.CompletePercent==null || x.CompletePercent<100).ToList();
            taskCompList = FilterTaskByYear(taskCompList); // 
            taskCompList = FilterByStatus(taskCompList); // 

            foreach (var taskComp in taskCompList)
            {
                taskComp.FactWorkLog = WorkLogs.GetTotalHoursOnTask(taskComp, context, out DateTime? startDate);
            }
            return taskCompList;
        }

        public static List<TaskComp> GetAllTasks(AppDbContext context)
        {
            var taskCompList = FilterByStatus(context.TaskComps);
            return taskCompList;
        }
      

        //public static List<TaskComp> GetAllMyTasksFromDb(User curUser, AppDbContext context)
        //{
        //    var taskSet = context.TaskComps.Where(x=>x.Executers.Contains(curUser.FullName)).ToList();            
        //    return taskSet;
        //}

        //public static List<TaskComp> GetMyCurMonthTasksFromDb(User curUser, AppDbContext context)
        //{           
        //    var taskSet = context.TaskComps.Where(x => x.Executers.Contains(curUser.FullName)).ToList();
        //    var taskSetFiltered = FilterTaskSetByCurMonth(taskSet);            
        //    return taskSetFiltered;
        //}

        static List<TaskComp> FilterTaskSetByCurMonth(IQueryable<TaskComp> taskSet)
        {
           List<TaskComp> taskCompsFiltered = new List<TaskComp>();
            var nowStart = DateTime.Now.Date.AddDays(-15);
            var nowFinish = DateTime.Now.Date.AddDays(15);
            foreach (var taskComp in taskSet)
            {
                if (taskComp.StartPlanDate < nowFinish && taskComp.CompletePercent != 100)
                {
                    taskCompsFiltered.Add(taskComp);
                }
            }
            return taskCompsFiltered;
        }

        static List<TaskComp> FilterByStatus(IEnumerable<TaskComp> taskComps)
        {
            List<TaskComp> taskCompFiltered = new List<TaskComp>();
            foreach (var taskComp in taskComps)
            {
                if (!string.IsNullOrEmpty(taskComp.TaskCompStatus))
                {
                    var status = taskComp.TaskCompStatus.Trim().Replace("ё", "е").ToUpper();
                    var statForExclude1 = TaskCompStatusStr[0].ToUpper();
                    var statForExclude2 = TaskCompStatusStr[1].ToUpper();
                    if (status != statForExclude1 && status != statForExclude2)
                    {
                        taskCompFiltered.Add(taskComp);
                    }
                }
            }
            return taskCompFiltered;
        }

        static List<TaskComp> FilterTaskSetByCurMonth(List<TaskComp> taskSet)
        {
            List<TaskComp> taskCompsFiltered = new List<TaskComp>();
            var nowStart = DateTime.Now.Date.AddDays(-15);
            var nowFinish = DateTime.Now.Date.AddDays(15);
            foreach (var taskComp in taskSet)
            {
                if (taskComp.StartPlanDate < nowFinish && taskComp.CompletePercent != 100)
                {
                    taskCompsFiltered.Add(taskComp);
                }
            }
            return taskCompsFiltered;
        }

        static List<TaskComp> FilterTaskSetByNoExecuter(List<TaskComp> taskSet)
        {
            return taskSet.Where(x => x.Executers == string.Empty || x.Executers == null).ToList();
        }

        static List<TaskComp> FilterTaskSetByNoExecuter(IQueryable<TaskComp> taskSet)
        {
            return taskSet.Where(x => x.Executers == string.Empty || x.Executers == null).ToList();
        }

        public static string FindTaskNameById(string taskCompId, AppDbContext context)
        {
            var res = Int32.TryParse(taskCompId, out int taskComId_Int);
            if (res)
            {
                var taskCompSet = context.TaskComps.Where(x => x.Id == taskComId_Int);
                if (taskCompSet.Count()>0)
                {
                    return taskCompSet.First().TaskCompName;
                }
            }
            return string.Empty;
        }

        public static TaskComp FindTaskCompById(string taskCompId, AppDbContext context)
        {
            var res = Int32.TryParse(taskCompId, out int taskComId_Int);
            if (res)
            {
                var taskCompSet = context.TaskComps.Where(x => x.Id == taskComId_Int);
                if (taskCompSet.Count() > 0)
                {
                    return taskCompSet.First();
                }
            }
            return null;
        }

        public static TaskComp GetTaskCompByName(string taskCompName, AppDbContext context)
        {
            if (!string.IsNullOrEmpty(taskCompName))
            {
                taskCompName = taskCompName.Trim();
                var taskCompSet = context.TaskComps.Where(x => x.TaskCompName == taskCompName);
                if (taskCompSet.Count() > 0)
                {
                    return taskCompSet.First();
                }
            }
            return null;
        }

        public bool SetPercent (AppDbContext context, string taskCompIdPercent, User curUser, List<string> successMes, List<string> errorMes)
        {
            var res = Double.TryParse(taskCompIdPercent, out double percent);
            var IsExecuterBool = IsExecuter(curUser);
            //if (IsExecuterBool)
            //{
            if (!string.IsNullOrEmpty(taskCompIdPercent))
            {
                if (res)
                {
                    if (percent > 100 || percent < 0)
                    {
                        errorMes.Add($"Процент должен быть в диапазоне от 0 до 100.");
                    }
                    else
                    {
                        CompletePercent = percent;
                        context.TaskCompPercentHistories.Add(new TaskCompPercentHistory(this, percent, curUser));                        
                        successMes.Add($"Процент по комплекту {TaskCompName} успешно установлен. ");
                        return true;
                    }
                }
                else
                {
                    errorMes.Add($"Невозможно преобразовать в число \" {taskCompIdPercent}\" ");

                }
            }
            //}
            //else
            //{
            //    errorMes.Add($"Вы не являетесь исполнителем по данной задаче. Чтобы стать исполнителем требуется отчитаться по задаче (подать трудозатраты)");
            //}
            return false;
        }



        public bool IsExecuter(User curUser)
        {
            if (string.IsNullOrEmpty(Executers))
            {
                return false;
            }
            else
            {
               return Executers.Contains(curUser?.FullName);
            }

            
        }

        public static void GetSingleTaskCompForProject(AppDbContext context, TaskComp tc)
        {
            var taskCompSet = context.TaskComps.Where(x => x.ProjectNumber == tc.ProjectNumber);
            if (taskCompSet.Count() > 0)
            {
                var defTc = taskCompSet.Last();
                tc.ProjectShortName = defTc.ProjectShortName;
                tc.GIPAcronym = defTc.GIPAcronym;
            }
            else
            {
                tc.ProjectShortName = string.Empty;
                tc.GIPAcronym = string.Empty;
            }
        }

        public static void RemoveStandartTasks(List<TaskComp> taskCompList)
        {
            foreach (var standartTaskCompName in TaskComp.StandartTaskCompAr) //Стандартные работы в массиве TaskComp.StandartTaskCompAr
            {
                taskCompList.RemoveAll(x => x.TaskCompName == standartTaskCompName);
            }
        }

     
    }
}
