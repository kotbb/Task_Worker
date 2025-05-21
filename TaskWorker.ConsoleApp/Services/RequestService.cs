using System;
using System.Data.SqlClient;
using TaskWorker.Models;
using System.Data;
using System.Collections.Generic;

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
            try
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
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting request: {ex.Message}");
                throw;
            }
        }

        public void DeleteRequestById(int requestId)
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting request: {ex.Message}");
                throw;
            }
        }

        public void UpdateRequest(int requestId, string preferredTimeSlot)
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating request: {ex.Message}");
                throw;
            }
        }
        
        public List<Request> GetAllRequests()
        {
            try
            {
                var requests = new List<Request>();
                using (var connection = new SqlConnection(_connectionString))
                {
                    string query = "SELECT * FROM Request_;";
                    var command = new SqlCommand(query, connection);

                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        requests.Add(new Request()
                        {
                            Id = reader.GetInt32(0),
                            RequestTime = reader.GetDateTime(1),
                            PreferredTimeSlot = reader.GetDateTime(2),
                            ClientId = reader.GetInt32(3),
                            TaskId = reader.GetInt32(4),
                        }
                        );
                    }
                    return requests;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting all requests: {ex.Message}");
                throw;
            }
        }

        public Request GetRequestById(int requestId)
        {
            try
            {
                const string query = @"
                    SELECT ID, RequestTime, PreferredTimeSlot, Client_ID, Task_ID
                    FROM Request_
                    WHERE ID = @RequestId";

                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RequestId", requestId);
                    connection.Open();
                    using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (reader.Read())
                        {
                            return new Request
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ID")),
                                RequestTime = reader.GetDateTime(reader.GetOrdinal("RequestTime")),
                                PreferredTimeSlot = reader.GetDateTime(reader.GetOrdinal("PreferredTimeSlot")),
                                ClientId = reader.GetInt32(reader.GetOrdinal("Client_ID")),
                                TaskId = reader.GetInt32(reader.GetOrdinal("Task_ID"))
                            };
                        }
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting request by ID: {ex.Message}");
                throw;
            }
        }
    }
}

