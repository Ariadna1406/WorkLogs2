using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{
    public class Project
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Заполните внутренний номер проекта.")]
        public string InternalNum { get; set; }
        public string ContractNumber { get; set; }
        public string ShortName { get; set; }
        public string AvevaAcronym { get; set; }

        [NotMapped]
        public string AvevaElemAmount { get; set; }
        

        [Required(ErrorMessage = "Заполните наименование проекта.")]
        public string FullName { get; set; }
        public User Manager { get; set; }
        public List<Corrections> Corrections { get; set; }
        public Status Status { get; set; }
        public bool ShowInMenuBar { get; set; }
        public bool IsDeleted { get; set; }

        public static List<string> GetProjects(User curUser, AppDbContext context)
        {
            List<string> projectNumList = new List<string>();
            foreach (var taskComp in context.TaskComps.Where(x=>x.Executers.Contains(curUser.FullName)))
            {
                var prjNum = taskComp.ProjectNumber;
                if (!projectNumList.Contains(prjNum))
                {
                    projectNumList.Add(prjNum);
                }
                
            }
            return projectNumList;
        }


        public static List<string> GetAllProjects(AppDbContext context)
        {
            List<string> projectNumList = new List<string>();
            foreach (var taskComp in context.TaskComps)
            {
                var prjNum = taskComp.ProjectNumber;
                if (!projectNumList.Contains(prjNum))
                {
                    projectNumList.Add(prjNum);
                }

            }
            return projectNumList;
        }

        public static void CheckAllProjectsForInternalTasks(AppDbContext context)
        {
            var allPrj = GetAllProjects(context);
            var departAcronymsSet = Department.GetAllDepartAcronyms(context);
            foreach (var projectNum in allPrj)
            {
                var prjTaskSet = TaskComp.GetAllTasksForSelectedProject(context, projectNum);
                foreach (var standartTaskCompName in TaskComp.StandartTaskCompAr) //Стандартные работы в массиве TaskComp.StandartTaskCompAr
                {
                    var stanTaskCompSet = prjTaskSet.Where(x => x.TaskCompName == standartTaskCompName);
                    if (stanTaskCompSet.Count() == 0)
                    {
                        var newTaskComp = new TaskComp()
                        {
                            ProjectNumber = projectNum,
                            TaskCompName = standartTaskCompName,
                            Department = departAcronymsSet
                            
                        };
                        context.TaskComps.Add(newTaskComp);                        
                    }
                    //Обновление вновь добавленных отделов и подразделений
                    else
                    {
                        var existTaskComp = stanTaskCompSet.First();
                        existTaskComp.Department = departAcronymsSet;
                    }
                }
            }
            context.SaveChanges();
        }

     

        public static void AddEducationProjectWithTaskComps(AppDbContext context)
        {
            var allPrj = GetAllProjects(context).Where(x=>x == "Учебный");
            if (allPrj.Count() == 0)
            {

            }

        }

        public static List<string> GetProjectsForMyDepart(User curUser, AppDbContext context)
        {
            var IsGIP = curUser.IsGIPCheck(context);
            List<string> projectNumList = new List<string>();
            var taskCompSet = TaskComp.GetAllTasksForMyDepartment(curUser, context);
            foreach (var taskCompGrouped in taskCompSet.GroupBy(x => x.ProjectNumber))
            {
                var prjNum = taskCompGrouped.Key;
                if (IsGIP) // Для ГИПа добавляются все проекты
                {
                    projectNumList.Add(prjNum);
                }
                else
                {
                    //Проверяем наличие реальных комплектов помимо "Командировка", "Работа начальника отдела", "Работа ГИПа"
                    //if (taskCompGrouped.Count() > TaskComp.StandartTaskCompAr.Count())
                    //{
                        projectNumList.Add(prjNum);
                    //}
                }
            }
            return projectNumList;
        }

        public static List<string> GetProjectsFromTaskSet(List<TaskComp> taskCompSet)
        {
            return taskCompSet.GroupBy(x => x.ProjectNumber).Select(x => x.Key).ToList();                       
        }

        public static List<Project> GetProjectsFromPrima(User curUser)
        {
            List<Project> projectList = new List<Project>();
            string connectionString = @"Data Source=primadb;Initial Catalog=primavera;User ID=privuser;Password=P@ssw0rd";
            string sqlExpression = SetSqlExpForProj(curUser.FullName);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read()) // построчно считываем данные
                    {
                        projectList.Add(new Project() { Id = (int)reader.GetValue(0), InternalNum = reader.GetValue(1).ToString() });


                    }
                }

                reader.Close();
            }

            return projectList;
        }

        static string SetSqlExpForProj(string fullName)
        {
            string exp = String.Format(@" DECLARE @projectTable TABLE (proj_id INT, prjName NVARCHAR(255));
    Declare @FullName NVARCHAR(255)
   SET @FullName = 'Нестеров Игорь Гидалевич'
  
  Insert Into @projectTable (proj_id, prjName)
   Select Distinct p.proj_id, p.proj_short_name
   FROM [primavera].[dbo].[RSRC] rs
    JOIN [primavera].[dbo].[TASKRSRC] t ON t.rsrc_id = rs.rsrc_id 
	JOIN [primavera].[dbo].[PROJECT] p ON p.proj_id = t.proj_id 	
	Where rsrc_name = @FullName
	
	
  Insert Into @projectTable (proj_id, prjName)
  Select Distinct pr.proj_id, pr.proj_short_name
  FROM [primavera].[dbo].[RSRC] rs
  JOIN [primavera].[dbo].[TASKRSRC] t ON t.role_id=rs.role_id
  JOIN [primavera].[dbo].[TASK] ta ON ta.task_id=t.task_id  
  JOIN [primavera].[dbo].[PROJECT] pr ON pr.proj_id=ta.proj_id  
  Where rs.rsrc_name=@FullName
  Select Distinct * from @projectTable ", fullName);
            return exp;
        }

    }

    public enum Status
    { NotInWork, InWork }

   
}
