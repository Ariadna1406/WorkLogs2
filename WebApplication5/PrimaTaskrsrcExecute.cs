using System;
using System.Data.SqlClient;

public class PrimaTaskrsrcExecute
{
    public int Taskrsrc_id { get; private set; }
    public string Task_name { get; private set; }
    public double Act_reg_qty { get; private set; }       //FACT
    public double Act_this_per_qty { get; private set;}
    //public double remain_work_qty { get; private set; }    //REMAIN
    //public double target_work_qty { get; private set; }    //PLAN

    public PrimaTaskrsrcExecute(int taskrsrc_id)
	{
        Taskrsrc_id = taskrsrc_id;
        FillEmptyFields(taskrsrc_id);
    }

    //Добавляет фактические трудозатраты и делает перерасчет остатка
    public void AddFact(double ValueToAdd)
    {
        Act_this_per_qty += ValueToAdd;

        if (Act_this_per_qty < 0)
        {
            Act_this_per_qty = 0;
        }
        Update();              
    }

    private SqlConnection ConnectToDB()
    {
        string connectionString = "Data Source=primadb;Initial Catalog=primavera;User ID=privuser;Password=P@ssw0rd";
        SqlConnection connection = new SqlConnection(connectionString);
        connection.Open();
        return connection;
    }

    private void Update()
    {
        using (SqlConnection _connection = ConnectToDB())
        {
            var query = String.Format(@"UPDATE [primavera].[dbo].[TASKRSRC] Set act_this_per_qty = {1} Where taskrsrc_id = {0}", Taskrsrc_id, Act_this_per_qty.ToString().Replace(',','.'));
             var command = new SqlCommand(query);
            command.Connection = _connection;
            command.ExecuteNonQuery();
        }              
    }

    private void FillEmptyFields(int taskrsrc_id)
    {
        string query = String.Format(@"Select [act_this_per_qty]
    FROM[primavera].[dbo].[TASKRSRC]
    Where taskrsrc_id = {0}", taskrsrc_id);

        using (SqlConnection connection = ConnectToDB())
        {
            using (SqlDataReader reader = GetReader(connection, query))
            {
                if (reader.HasRows == false)
                {
                    new NullReferenceException();
                }

                while (reader.Read()) // построчно считываем данные
                {
                    var val = reader.GetValue(0);
                    if (val != null)
                    {
                        Act_this_per_qty = Convert.ToDouble(val);//FACT
                    }
                    else
                    {
                        Act_this_per_qty = 0;
                    }
                                                                           
                }                
            }
        }
    }

    private SqlDataReader GetReader(SqlConnection connection, string query)
    {
        SqlCommand command = new SqlCommand(query, connection);
        SqlDataReader reader = command.ExecuteReader();
        return reader;
    }
}

//public static class WorklogChanger
//{
//    public static void ChangeFactWorklogs(int TaskId, double value)
//    {
//       // var task = new PrimaTaskExecute(TaskId);
//       // task.AddFact(value);        
//    }

//    public static void RewriteFactWorklogs(int TaskId, double value)
//    {
//       // var task = new PrimaTaskExecute(TaskId);
//       // task.RewriteFact(value);
//    }
//}
