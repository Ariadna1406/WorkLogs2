using System;
using System.Data.SqlClient;

public class PrimaTaskExecute
{
    public int task_id { get; private set; }
    public string task_name { get; private set; }
    public double act_work_qty { get; private set; }       //FACT
    public double remain_work_qty { get; private set; }    //REMAIN
    public double target_work_qty { get; private set; }    //PLAN

    public PrimaTaskExecute(int Task_id)
	{
        task_id = Task_id;
        FillEmptyFields();
    }

    //Добавляет фактические трудозатраты и делает перерасчет остатка
    public void AddFact(double ValueToAdd)
    {
        act_work_qty += ValueToAdd;

        if (act_work_qty < 0)
        {
            act_work_qty = 0;
        }

        remain_work_qty = target_work_qty - act_work_qty;

        if (remain_work_qty < 0)
        {
            remain_work_qty = 0;
        }

        Update();              
    }

    public void SubFact(double ValueToSub)
    {
        act_work_qty -= ValueToSub;

        if (act_work_qty < 0)
        {
            act_work_qty = 0;
            return;
        }

        remain_work_qty = target_work_qty - act_work_qty;

        if (remain_work_qty < 0)
        {
            remain_work_qty = 0;
        }

        Update();
    }

    //Изменяет фактические трудозатраты и делает перерасчет остатка
    public void RewriteFact(double NewValue)
    {
        if (NewValue >= 0)
        {
            act_work_qty = NewValue;

            remain_work_qty = target_work_qty - remain_work_qty;

            if (remain_work_qty < 0)
            {
                remain_work_qty = 0;
            }

            Update();
        }
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
            var command = new SqlCommand("UPDATE Task SET act_work_qty=@newActValue, remain_work_qty=@newRemainValue WHERE task_id =@id");
            command.Parameters.AddWithValue("@newActValue", act_work_qty);
            command.Parameters.AddWithValue("@newRemainValue", remain_work_qty);
            command.Parameters.AddWithValue("@id", task_id);
            command.Connection = _connection;
            command.ExecuteNonQuery();
        }              
    }

    private void FillEmptyFields()
    {
        string query = String.Format("SELECT TOP (1000) [task_id],[proj_id],[task_name],[act_work_qty],[remain_work_qty],[target_work_qty],[update_date],[update_user]FROM[primavera].[dbo].[TASK]WHERE task_id = {0}", task_id);

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
                    task_name = Convert.ToString(reader.GetValue(2));
                    act_work_qty = Convert.ToDouble(reader.GetValue(3));         //FACT
                    remain_work_qty = Convert.ToDouble(reader.GetValue(4));      //REAMIN
                    target_work_qty = Convert.ToDouble(reader.GetValue(5));      //PLAN
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

public static class WorklogChanger
{
    public static void ChangeFactWorklogs(int TaskId, double value)
    {
        var task = new PrimaTaskExecute(TaskId);
        task.AddFact(value);        
    }

    public static void RewriteFactWorklogs(int TaskId, double value)
    {
        var task = new PrimaTaskExecute(TaskId);
        task.RewriteFact(value);
    }
}
