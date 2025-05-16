using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskWorker.Models;
using System.Data.SqlClient;


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
        public bool addTask(Task_ task)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO TASK_ (NAME,REQUIREDSPECIALTY) VALUES (@NAME , @REQUIREDSPECIALTY);";
                var command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@Name", task.Name);
                command.Parameters.AddWithValue("@REQUIREDSPECIALTY", task.RequiredSpecialty);
                connection.Open();

                return command.ExecuteNonQuery() > 0;
            }
        }
        public List<Task_> getallTasks()
        {
            var tasks = new List<Task_>();
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT * FROM TASK_;";
                var command = new SqlCommand(query, connection);

                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    tasks.Add(new Task_(
                        reader.GetInt32(reader.GetOrdinal("ID")),
                        reader.GetString(reader.GetOrdinal("Name")),
                        reader.GetString(reader.GetOrdinal("RequiredSpecialty"))
                    ));
                }
                return tasks;
            }
        }
        public List<Task_> selectbyName(string name)
        {
            var tasks = new List<Task_>();
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT * FROM TASK_ WHERE Name = @NAME;";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@NAME", name);

                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    tasks.Add(new Task_(
                        reader.GetInt32(reader.GetOrdinal("ID")),
                        reader.GetString(reader.GetOrdinal("Name")),
                        reader.GetString(reader.GetOrdinal("RequiredSpecialty"))
                    ));
                }
                return tasks;
            }
        }
        public List<Task_> selectbyID(int id)
        {
            var tasks = new List<Task_>();
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT * FROM TASK_ WHERE ID = @id;";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    tasks.Add(new Task_(
                        reader.GetInt32(reader.GetOrdinal("ID")),
                        reader.GetString(reader.GetOrdinal("Name")),
                        reader.GetString(reader.GetOrdinal("RequiredSpecialty"))
                    ));
                }
                return tasks;
            }
        }
        public List<Task_> selectbySpecialty(string Req)
        {
            var tasks = new List<Task_>();
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT * FROM TASK_ WHERE RequiredSpecialty = @Req;";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Req", Req);

                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    tasks.Add(new Task_(
                        reader.GetInt32(reader.GetOrdinal("ID")),
                        reader.GetString(reader.GetOrdinal("Name")),
                        reader.GetString(reader.GetOrdinal("RequiredSpecialty"))
                    ));
                }
                return tasks;
            }
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
        
    }
}
