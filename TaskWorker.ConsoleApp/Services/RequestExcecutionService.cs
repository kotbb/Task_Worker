using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TaskWorker.Models;

namespace TaskWorker.Services
{
    internal class RequestExecutionService
    {
        private  string _connectionString;

        public RequestExecutionService(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Add a new RequestExecution
        public void AddRequestExecution(RequestExecution exec)
        {
            string commandText = @"
                INSERT INTO RequestExecution 
                (ActualTime, WorkerRating, ClientRating, RequestStatus, ClientFeedback, WorkerFeedback, Worker_ID, Request_ID)
                VALUES
                (@ActualTime, @WorkerRating, @ClientRating, @RequestStatus, @ClientFeedback, @WorkerFeedback, @Worker_ID, @Request_ID)";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@ActualTime", exec.ActualTime);
                command.Parameters.AddWithValue("@WorkerRating", exec.WorkerRating);
                command.Parameters.AddWithValue("@ClientRating", exec.ClientRating);
                command.Parameters.AddWithValue("@RequestStatus", exec.Status);
                command.Parameters.AddWithValue("@ClientFeedback", (object)exec.ClientFeedback ?? DBNull.Value);
                command.Parameters.AddWithValue("@WorkerFeedback", (object)exec.WorkerFeedback ?? DBNull.Value);
                command.Parameters.AddWithValue("@Worker_ID", exec.WorkerId);
                command.Parameters.AddWithValue("@Request_ID", exec.RequestId);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error adding RequestExecution: {ex.Message}");
                    throw;
                }
            }
        }

        // Get a RequestExecution by composite primary key (WorkerId + RequestId)
        public RequestExecution GetRequestExecution(int workerId, int requestId)
        {
            string query = @"
                SELECT ActualTime, WorkerRating, ClientRating, RequestStatus, ClientFeedback, WorkerFeedback, Worker_ID, Request_ID
                FROM RequestExecution
                WHERE Worker_ID = @WorkerId AND Request_ID = @RequestId";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@WorkerId", workerId);
                command.Parameters.AddWithValue("@RequestId", requestId);

                try
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (reader.Read())
                        {
                            return new RequestExecution
                            {
                                ActualTime = reader.GetDateTime(0),
                                WorkerRating = reader.GetDecimal(1),
                                ClientRating = reader.GetDecimal(2),
                                Status = reader.GetString(3),
                                ClientFeedback = reader.IsDBNull(4) ? null : reader.GetString(4),
                                WorkerFeedback = reader.IsDBNull(5) ? null : reader.GetString(5),
                                WorkerId = reader.GetInt32(6),
                                RequestId = reader.GetInt32(7)
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving RequestExecution: {ex.Message}");
                    throw;
                }
            }
            return null; // Not found
        }

        // Update existing RequestExecution
        public bool UpdateRequestExecution(RequestExecution exec)
        {
            string commandText = @"
                UPDATE RequestExecution SET
                    ActualTime = @ActualTime,
                    WorkerRating = @WorkerRating,
                    ClientRating = @ClientRating,
                    RequestStatus = @RequestStatus,
                    ClientFeedback = @ClientFeedback,
                    WorkerFeedback = @WorkerFeedback
                WHERE Worker_ID = @Worker_ID AND Request_ID = @Request_ID";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@ActualTime", exec.ActualTime);
                command.Parameters.AddWithValue("@WorkerRating", exec.WorkerRating);
                command.Parameters.AddWithValue("@ClientRating", exec.ClientRating);
                command.Parameters.AddWithValue("@RequestStatus", exec.Status);
                command.Parameters.AddWithValue("@ClientFeedback", (object)exec.ClientFeedback ?? DBNull.Value);
                command.Parameters.AddWithValue("@WorkerFeedback", (object)exec.WorkerFeedback ?? DBNull.Value);
                command.Parameters.AddWithValue("@Worker_ID", exec.WorkerId);
                command.Parameters.AddWithValue("@Request_ID", exec.RequestId);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating RequestExecution: {ex.Message}");
                    throw;
                }
            }
        }

        // Delete a RequestExecution by composite key
        public bool DeleteRequestExecution(int workerId, int requestId)
        {
            string commandText = @"
                DELETE FROM RequestExecution
                WHERE Worker_ID = @WorkerId AND Request_ID = @RequestId";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@WorkerId", workerId);
                command.Parameters.AddWithValue("@RequestId", requestId);

                try
                {
                    connection.Open();
                    int rowsDeleted = command.ExecuteNonQuery();
                    return rowsDeleted > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting RequestExecution: {ex.Message}");
                    throw;
                }
            }
        }

        //Get all RequestExecutions
        public List<RequestExecution> GetAllRequestExecutions()
        {
            string query = @"
                SELECT ActualTime, WorkerRating, ClientRating, RequestStatus, ClientFeedback, WorkerFeedback, Worker_ID, Request_ID
                FROM RequestExecution";

            var executions = new List<RequestExecution>();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            executions.Add(new RequestExecution
                            {
                                ActualTime = reader.GetDateTime(0),
                                WorkerRating = reader.GetDecimal(1),
                                ClientRating = reader.GetDecimal(2),
                                Status = reader.GetString(3),
                                ClientFeedback = reader.IsDBNull(4) ? null : reader.GetString(4),
                                WorkerFeedback = reader.IsDBNull(5) ? null : reader.GetString(5),
                                WorkerId = reader.GetInt32(6),
                                RequestId = reader.GetInt32(7)
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving RequestExecutions: {ex.Message}");
                    throw;
                }
            }

            return executions;
        }
        public List<RequestExecution> GetPendingRequestExecutions()
        {
            string query = @"
                SELECT ActualTime, WorkerRating, ClientRating, RequestStatus, ClientFeedback, WorkerFeedback, Worker_ID, Request_ID
                FROM RequestExecution
                WHERE RequestStatus = 'Pending' ";

            var executions = new List<RequestExecution>();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            executions.Add(new RequestExecution
                            {
                                ActualTime = reader.GetDateTime(0),
                                WorkerRating = reader.GetDecimal(1),
                                ClientRating = reader.GetDecimal(2),
                                Status = reader.GetString(3),
                                ClientFeedback = reader.IsDBNull(4) ? null : reader.GetString(4),
                                WorkerFeedback = reader.IsDBNull(5) ? null : reader.GetString(5),
                                WorkerId = reader.GetInt32(6),
                                RequestId = reader.GetInt32(7)
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving RequestExecutions: {ex.Message}");
                    throw;
                }
            }

            return executions;
        }
        public List<RequestExecution> GetCompletedRequestExecutions()
        {
            string query = @"
                SELECT ActualTime, WorkerRating, ClientRating, RequestStatus, ClientFeedback, WorkerFeedback, Worker_ID, Request_ID
                FROM RequestExecution
                WHERE RequestStatus = 'Completed' ";

            var executions = new List<RequestExecution>();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            executions.Add(new RequestExecution
                            {
                                ActualTime = reader.GetDateTime(0),
                                WorkerRating = reader.GetDecimal(1),
                                ClientRating = reader.GetDecimal(2),
                                Status = reader.GetString(3),
                                ClientFeedback = reader.IsDBNull(4) ? null : reader.GetString(4),
                                WorkerFeedback = reader.IsDBNull(5) ? null : reader.GetString(5),
                                WorkerId = reader.GetInt32(6),
                                RequestId = reader.GetInt32(7)
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving RequestExecutions: {ex.Message}");
                    throw;
                }
            }

            return executions;
        }
    }
}
