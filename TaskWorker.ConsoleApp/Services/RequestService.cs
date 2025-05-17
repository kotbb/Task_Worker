using System;
using System.Data.SqlClient;
using TaskWorker.Models;
using System.Data;

namespace TaskWorker.Services
{
    internal class RequestService
    {
        private string _connectionString;

        // ----------------------  Methods ------------------------------
        public RequestService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void InsertRequest(Request req)
        {
            
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql =
                    "INSERT INTO Request_ (RequestTime, PreferredTimeSlot, Client_ID, Task_ID) VALUES (@RequestTime, @TimeSlot, @ClientID, @TaskID)"+
                    "SELECT CAST(SCOPE_IDENTITY() AS INT);";
                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@RequestTime", req.RequestTime);
                cmd.Parameters.AddWithValue("@TimeSlot", req.PreferredTimeSlot);
                cmd.Parameters.AddWithValue("@ClientID", req.ClientId);
                cmd.Parameters.AddWithValue("@TaskID", req.TaskId);

                conn.Open();
                var newId = (int)cmd.ExecuteScalar();  
                req.Id = newId;
                Console.WriteLine($"Request inserted with ID: {newId}.");
            }
        }

        public void DeleteRequestById(int requestId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "DELETE FROM Request_ WHERE ID = @ID";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ID", requestId);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                Console.WriteLine($"{rows} row(s) deleted.");
            }
        }

        public void UpdateRequest(int requestId, string preferredTimeSlot)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "UPDATE Request_ SET PreferredTimeSlot = @TimeSlot WHERE ID = @ID";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@TimeSlot", preferredTimeSlot);
                cmd.Parameters.AddWithValue("@ID", requestId);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                Console.WriteLine($"{rows} row(s) updated.");
            }
        }
        
        public List<Request> GetAllRequests()
        {
            var requests = new List<Request>();
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT * FROM Request_;";
                var command = new SqlCommand(query, connection);

                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    requests.Add(new Request()
                    {
                        Id = reader.GetInt32(0),
                        RequestTime = reader.GetDateTime(2),
                        PreferredTimeSlot = reader.GetDateTime(3),
                        ClientId = reader.GetInt32(4),
                        TaskId = reader.GetInt32(5),
                    }
                    );
                }
                return requests;
            }
        }
    }
}

