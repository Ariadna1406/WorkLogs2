using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{
    public class PrimaTask
    {
        [Key]
        public int Id { get; set; }
        public string ProjName { get; set; }
        public int ProjId { get; set; }
        public string Task_Id { get; set; }       
        public string TaskName { get; set; }
        public string Taskrsrc_Id { get; set; }
        public string StartDate { get; set; }
        public string FinishDate { get; set; }
        public string Rsrc_id { get; set; }
        public string Rsrc_name { get; set; }
        public string Role_id { get; set; }
        public string Role_name { get; set; }

        private static readonly string connectionString = @"Data Source=primadb;Initial Catalog=primavera;User ID=privuser;Password=P@ssw0rd";
        public static List<PrimaTask> GetTasksFromPrima(string selectedProject, User curUser)
        {            
            var query = SetSqlExpForTask(selectedProject, curUser);
            List<PrimaTask> primaTaskList = new List<PrimaTask>();
            //string connectionString = @"Data Source=primadb;Initial Catalog=primavera;User ID=privuser;Password=P@ssw0rd";
            string sqlExpression = query;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read()) // построчно считываем данные
                    {
                        int projId = -1;
                        Int32.TryParse(reader.GetValue(1).ToString(), out projId);
                        if (projId > 0)
                        {
                            primaTaskList.Add(new PrimaTask() { ProjName = reader.GetValue(0).ToString(), ProjId = projId, Task_Id = reader.GetValue(2).ToString(), TaskName = reader.GetValue(3).ToString(), Taskrsrc_Id = reader.GetValue(4).ToString() });
                        }

                    }
                }

                reader.Close();
            }

            return primaTaskList;
        }

        public static List<TaskComp> GetTasksFromDb (string selectedProject, User curUser, AppDbContext context)
        {
           var taskSet = context.TaskComps.Where(x => x.ProjectNumber == selectedProject && x.Executers.Contains(curUser.FullName)).ToList();
            return taskSet;
        }

        static string SetSqlExpForTask(string selectedProject, User curUser)
        {
            var exp = String.Format(@"DECLARE @projectTable TABLE (prjName NVARCHAR(255), proj_id INT, task_id INT, task_name NVARCHAR(255), taskrsc_id INT);
                                      Declare @FullName NVARCHAR(255)
	                                  Declare @ProjName NVARCHAR(255)
                                      SET @FullName = '{0}'
                                      SET @ProjName = '{1}'

                                      Insert Into @projectTable (prjName, proj_id, task_id, task_name, taskrsc_id)
                                      Select Distinct p.proj_short_name, p.proj_id, t.taskrsrc_id, ta.task_name, t.taskrsrc_id
                                      FROM [primavera].[dbo].[RSRC] rs
                                      JOIN [primavera].[dbo].[TASKRSRC] t ON t.rsrc_id = rs.rsrc_id 
                                      JOIN [primavera].[dbo].[PROJECT] p ON p.proj_id = t.proj_id 	
                                      JOIN [primavera].[dbo].[TASK] ta ON ta.task_id = t.task_id
	                                  Where rsrc_name = @FullName and p.proj_short_name=@ProjName and t.delete_session_id is NULL
                                      Select Distinct *from @projectTable", curUser.FullName, selectedProject);
                                        

            //Запрос для роли
                             /*  Insert Into @projectTable (prjName, proj_id, task_id, task_name, taskrsc_id)
                                                         Select Distinct pr.proj_short_name, pr.proj_id, t.taskrsrc_id, ta.task_name, t.taskrsrc_id
                                                         FROM [primavera].[dbo].[RSRC] rs
                                                         JOIN [primavera].[dbo].[TASKRSRC] t ON t.role_id=rs.role_id
                                                         JOIN [primavera].[dbo].[TASK] ta ON ta.task_id=t.task_id  
                                                         JOIN [primavera].[dbo].[PROJECT] pr ON pr.proj_id=ta.proj_id  
                                                         Where rs.rsrc_name=@FullName and pr.proj_short_name=@ProjName and t.delete_session_id is NULL
                                                         Select Distinct * from @projectTable  */
            return exp;
        }

        public static List<PrimaTask> GetAllTasksFromPrima(User curUser)
        {
            var query = SetSqlExpForAllTask(curUser);
            List<PrimaTask> primaTaskList = new List<PrimaTask>();            
            string sqlExpression = query;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read()) // построчно считываем данные
                    {
                        int projId = -1;
                        Int32.TryParse(reader.GetValue(1).ToString(), out projId);
                        if (projId > 0)
                        {
                            primaTaskList.Add(new PrimaTask() { ProjName = reader.GetValue(0).ToString(), ProjId = projId, Task_Id = reader.GetValue(2).ToString(), TaskName = reader.GetValue(3).ToString(), 
                            Taskrsrc_Id = reader.GetValue(4).ToString(), StartDate=reader.GetValue(5).ToString(), FinishDate= reader.GetValue(6).ToString()});
                        }

                    }
                }

                reader.Close();
            }

            return primaTaskList;
        }

        public static List<PrimaTask> GetAllTasksForMyDepartment(User curUser)
        {
            var query = SetSqlExpForMyDepart(curUser);
            List<PrimaTask> primaTaskList = new List<PrimaTask>();
            string sqlExpression = query;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read()) // построчно считываем данные
                    {
                        int projId = -1;
                        Int32.TryParse(reader.GetValue(1).ToString(), out projId);
                        if (projId > 0)
                        {
                            primaTaskList.Add(new PrimaTask()
                            {
                                ProjName = reader.GetValue(0).ToString(),
                                ProjId = projId,
                                Task_Id = reader.GetValue(2).ToString(),
                                TaskName = reader.GetValue(3).ToString(),
                                Taskrsrc_Id = reader.GetValue(4).ToString(),
                                StartDate = reader.GetValue(5).ToString(),
                                FinishDate = reader.GetValue(6).ToString(),
                                Role_id = reader.GetValue(7).ToString(),
                                Role_name= reader.GetValue(8).ToString(),
                                Rsrc_id = reader.GetValue(9).ToString(),
                                Rsrc_name = reader.GetValue(10).ToString()

                            });
                        }

                    }
                }

                reader.Close();
            }

            return primaTaskList;
        }

        static string SetSqlExpForAllTask(User curUser)
        {
            var exp = String.Format(@"DECLARE @projectTable TABLE (prjName NVARCHAR(255), proj_id INT, task_id INT, task_name NVARCHAR(255), taskrsc_id INT, startDate DateTime, finishDate DateTime);
                                      Declare @FullName NVARCHAR(255)
	                                  Declare @ProjName NVARCHAR(255)
                                      SET @FullName = '{0}'                                      

                                      Insert Into @projectTable (prjName, proj_id, task_id, task_name, taskrsc_id, startDate, finishDate)
                                      Select Distinct p.proj_short_name, p.proj_id, t.taskrsrc_id, ta.task_name, t.taskrsrc_id, t.target_start_date, t.target_end_date
                                      FROM [primavera].[dbo].[RSRC] rs
                                      JOIN [primavera].[dbo].[TASKRSRC] t ON t.rsrc_id = rs.rsrc_id 
                                      JOIN [primavera].[dbo].[PROJECT] p ON p.proj_id = t.proj_id 	
                                      JOIN [primavera].[dbo].[TASK] ta ON ta.task_id = t.task_id
	                                  Where rsrc_name = @FullName and t.delete_session_id is NULL
                                      Select Distinct * from @projectTable", curUser.FullName);

            // Запрос для роли
            /*        Insert Into @projectTable (prjName, proj_id, task_id, task_name, taskrsc_id, startDate, finishDate)
                                      Select Distinct pr.proj_short_name, pr.proj_id, t.taskrsrc_id, ta.task_name, t.taskrsrc_id,  t.target_start_date, t.target_end_date
                                      FROM [primavera].[dbo].[RSRC] rs
                                      JOIN [primavera].[dbo].[TASKRSRC] t ON t.role_id=rs.role_id
                                      JOIN [primavera].[dbo].[TASK] ta ON ta.task_id=t.task_id  
                                      JOIN [primavera].[dbo].[PROJECT] pr ON pr.proj_id=ta.proj_id  
                                      Where rs.rsrc_name=@FullName and t.delete_session_id is NULL
                                      Select Distinct * from @projectTable*/
            return exp;
        }

        static string SetSqlExpForMyDepart(User curUser)
        {
            var exp = String.Format(@"
    Declare @FullName NVARCHAR(255)
  SET @FullName = '{0}'
  DECLARE @HODRole TABLE (role_id NVARCHAR(255), role_name NVARCHAR(255))
  Insert Into @HODRole (role_id, role_name)
  Select rsRole.role_id, r.role_name
  FROM [primavera].[dbo].[RSRC] rs
  JOIN [primavera].[dbo].[RSRCROLE] rsRole on rsRole.rsrc_id=rs.rsrc_id
  JOIN [primavera].[dbo].[ROLES] r on r.role_id=rsRole.role_id
  Where rs.rsrc_name=@FullName

  DECLARE @SubRole TABLE (role_id NVARCHAR(255), role_name NVARCHAR(255))
  Insert Into @SubRole (role_id, role_name)
  Select r.role_id, r.role_name 
  from @HODRole h
  JOIN [primavera].[dbo].[ROLES] r on r.parent_role_id=h.role_id
  
 DECLARE @projectTable TABLE (prjName NVARCHAR(255), proj_id INT, task_id INT, task_name NVARCHAR(255), taskrsc_id INT, startDate DateTime, finishDate DateTime, 
	role_id NVARCHAR(255), role_name NVARCHAR(255), rsrc_id NVARCHAR(255), rsrc_name NVARCHAR(255));
 Insert Into @projectTable (prjName, proj_id, task_id, task_name, taskrsc_id, startDate, finishDate, role_id ,role_name)
                                      Select Distinct pr.proj_short_name, pr.proj_id, t.task_id , t.task_name, tRSRC.taskrsrc_id, t.target_start_date, t.target_end_date, s.role_id ,s.role_name									  
                                      FROM [primavera].[dbo].[TASKRSRC] tRSRC									  
                                      JOIN [primavera].[dbo].[PROJECT] pr ON pr.proj_id=tRSRC.proj_id
									  JOIN [primavera].[dbo].[TASK] t ON t.task_id=tRSRC.task_id
									  JOIN @SubRole s ON s.role_id=tRSRC.role_id
									  Where tRSRC.rsrc_id is NULL
                                      
                                      
  --Find resourse by role 
 Insert Into @projectTable (prjName, proj_id, task_id, task_name, taskrsc_id, startDate, finishDate, rsrc_id, rsrc_name)
  Select pr.proj_short_name, pr.proj_id, t.task_id , t.task_name, tRSRC.taskrsrc_id, t.target_start_date, t.target_end_date, rsrc.rsrc_id ,rsrc.rsrc_name
  from [primavera].[dbo].[RSRCROLE] rRole
  JOIN @SubRole s ON s.role_id=rRole.role_id
  JOIN [primavera].[dbo].[TASKRSRC] tRSRC ON tRSRC.rsrc_id=rRole.rsrc_id 
  JOIN [primavera].[dbo].[TASK] t ON t.task_id=tRSRC.task_id
  JOIN [primavera].[dbo].[PROJECT] pr ON pr.proj_id=tRSRC.proj_id
  JOIN [primavera].[dbo].[RSRC] rsrc ON rsrc.rsrc_id=rRole.rsrc_id

  Select Distinct * from @projectTable
", curUser.FullName);
            
            return exp;
        }
    }
}
            
    

