using System;
using System.Data.SqlClient;
using Task = TaskWorker.Models.Task;
using System.Data;

namespace TaskWorker.Services
{
    internal class TaskService
    {
        private string _connectionString;

        // ----------------------  Methods ------------------------------
        public TaskService(string connectionString)
        {
            _connectionString = connectionString;
        }
        public void addTask(Task task)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO TASK_ (NAME,REQUIREDSPECIALTY,AverageTimeNeeded,AverageTaskFee) 
                                 VALUES (@Name , @RequiredSpecialty, @AverageTimeNeeded,@AverageTaskFee);
                                 SELECT CAST(SCOPE_IDENTITY() AS INT);";
                var command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@Name", task.Name);
                command.Parameters.AddWithValue("@RequiredSpecialty", task.RequiredSpecialty);
                command.Parameters.AddWithValue("@AverageTimeNeeded", task.AverageTimeNeeded);
                command.Parameters.AddWithValue("@AverageTaskFee", task.AverageTaskFee);
                connection.Open();
                var newId = (int)command.ExecuteScalar();
                task.Id = newId;
            }
        }
        public List<Task> getallTasks()
        {
            var tasks = new List<Task>();
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT * FROM TASK_;";
                var command = new SqlCommand(query, connection);

                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    tasks.Add(new Task(
                        reader.GetInt32(reader.GetOrdinal("ID")),
                        reader.GetString(reader.GetOrdinal("Name")),
                        reader.GetString(reader.GetOrdinal("RequiredSpecialty")),
                        reader.GetInt32(reader.GetOrdinal("AverageTimeNeeded")),
                        reader.GetDecimal(reader.GetOrdinal("AverageTaskFee"))
                    ));
                }
                return tasks;
            }
        }
        public List<Task> selectbyName(string name)
        {
            var tasks = new List<Task>();
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT * FROM TASK_ WHERE Name = @NAME;";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@NAME", name);

                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    tasks.Add(new Task(
                        reader.GetInt32(reader.GetOrdinal("ID")),
                        reader.GetString(reader.GetOrdinal("Name")),
                        reader.GetString(reader.GetOrdinal("RequiredSpecialty")),
                        reader.GetInt32(reader.GetOrdinal("AverageTimeNeeded")),
                        reader.GetDecimal(reader.GetOrdinal("AverageTaskFee"))
                    ));
                }
                return tasks;
            }
        }
        public Task getTaskById(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string query = @"SELECT * FROM TASK_ WHERE ID = @id;";
                    var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        return(new Task(
                            reader.GetInt32(reader.GetOrdinal("ID")),
                            reader.GetString(reader.GetOrdinal("Name")),
                            reader.GetString(reader.GetOrdinal("RequiredSpecialty")),
                            reader.GetInt32(reader.GetOrdinal("AverageTimeNeeded")),
                            reader.GetDecimal(reader.GetOrdinal("AverageTaskFee"))
                        ));
                    }
                } 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            return null;
        }
        public Task getTaskBySpecialty(string Req)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string query = @"SELECT * FROM TASK_ WHERE RequiredSpecialty = @Req;";
                    var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Req", Req);

                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        return (new Task(
                            reader.GetInt32(reader.GetOrdinal("ID")),
                            reader.GetString(reader.GetOrdinal("Name")),
                            reader.GetString(reader.GetOrdinal("RequiredSpecialty")),
                            reader.GetInt32(reader.GetOrdinal("AverageTimeNeeded")),
                            reader.GetDecimal(reader.GetOrdinal("AverageTaskFee"))
                        ));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }
        public bool deleteallTasks() {

            using (var connection = new SqlConnection(_connectionString)) {

                string query = "DELETE FROM Task_;" +
                               "DBCC CHECKIDENT ('Task_', RESEED, 0);";

                var command = new SqlCommand(query, connection);
                connection.Open();
                
                return command.ExecuteNonQuery() > 0;
            }
        }
        public bool deleteTaskById(int taskId) {

            using (var connection = new SqlConnection(_connectionString))
            {
                string query = @"DELETE FROM Task_
                                 WHERE ID = @id";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", taskId);
                connection.Open();
                
                return command.ExecuteNonQuery() > 0;
            }
        }
        
    }
}
