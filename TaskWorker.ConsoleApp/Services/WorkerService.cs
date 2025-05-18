using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using TaskWorker.Models;
using System.Data;

namespace TaskWorker.Services
{
    internal class WorkerService
    {
        private string _connectionString;

        public WorkerService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Worker> getAllWorkers()
        {
            var workers = new List<Worker>();
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT * FROM Worker_;";
                var command = new SqlCommand(query, connection);

                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(reader.GetOrdinal("Id"));
                    workers.Add(new Worker
                        {
                            Id = id,
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            overallRating = reader.GetDecimal(reader.GetOrdinal("OverallRating")),
                            Specialties = GetWorkerSpecialties(id).ToHashSet(),
                            AvailableTimeSlots = GetWorkerTimeSlots(id),
                            Locations = GetWorkerLocations(id),
                            Performs = GetWorkerPerforms(id).ToHashSet(),
                            
                        }
                    );
                }

                return workers;
            }
        }

        public void AddWorker(Worker worker)
        {
            string commandText = @"
            INSERT INTO Worker_ (Name,OverallRating)
            VALUES (@Name,@OverallRating);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@Name", worker.Name);
                command.Parameters.AddWithValue("@OverallRating", worker.overallRating);
                try
                {
                    connection.Open();
                    var newId = (int)command.ExecuteScalar();
                    worker.Id = newId;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error adding worker: {ex.Message}");
                    throw;
                }
            }
        }

        public void AddWorkerAvailableTimeSlot(WorkerAvailableTimeSlot timeSlot)
        {
            string commandText = @"
            INSERT INTO Worker_AvailableTimeSlots (Worker_ID, AvailableTimeSlots)
            VALUES (@WorkerId, @Slot)";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@WorkerId", timeSlot.WorkerId);
                command.Parameters.AddWithValue("@Slot", timeSlot.Slot);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error adding worker time slot: {ex.Message}");
                    throw;
                }
            }
        }

        public void AddWorkerLocation(WorkerLocation location)
        {
            string commandText = @"
            INSERT INTO Worker_Locations (Worker_ID, City, Street, Country)
            VALUES (@WorkerId, @City, @Street, @Country)";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@WorkerId", location.WorkerId);
                command.Parameters.AddWithValue("@City", location.City);
                command.Parameters.AddWithValue("@Street", location.Street);
                command.Parameters.AddWithValue("@Country", location.Country);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error adding worker location: {ex.Message}");
                    throw;
                }
            }
        }

        public void AddWorkerSpecialty(WorkerSpecialty specialty)
        {
            string commandText = @"
            INSERT INTO Worker_Specialties (Worker_ID, Specialties)
            VALUES (@WorkerId, @Specialty)";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@WorkerId", specialty.WorkerId);
                command.Parameters.AddWithValue("@Specialty", specialty.Specialty);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error adding worker specialty: {ex.Message}");
                    throw;
                }
            }
        }

        private List<WorkerAvailableTimeSlot> GetWorkerTimeSlots(int workerId)
        {
            string commandText = @"
            SELECT Worker_ID, AvailableTimeSlots
            FROM Worker_AvailableTimeSlots
            WHERE Worker_ID = @WorkerId";

            var timeSlots = new List<WorkerAvailableTimeSlot>();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@WorkerId", workerId);

                try
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            timeSlots.Add(new WorkerAvailableTimeSlot
                            {
                                WorkerId = reader.GetInt32(0),
                                Slot = reader.GetDateTime(1)
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving worker time slots: {ex.Message}");
                    throw;
                }
            }

            return timeSlots;
        }

        private List<WorkerLocation> GetWorkerLocations(int workerId)
        {
            string commandText = @"
            SELECT Worker_ID, City, Street, Country
            FROM Worker_Locations
            WHERE Worker_ID = @WorkerId";

            var locations = new List<WorkerLocation>();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@WorkerId", workerId);

                try
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            locations.Add(new WorkerLocation
                            {
                                WorkerId = reader.GetInt32(0),
                                City = reader.GetString(1),
                                Street = reader.GetString(2),
                                Country = reader.GetString(3)
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving worker locations: {ex.Message}");
                    throw;
                }
            }

            return locations;
        }

        private List<WorkerSpecialty> GetWorkerSpecialties(int workerId)
        {
            string commandText = @"
            SELECT Specialties,Worker_ID 
            FROM Worker_Specialties
            WHERE Worker_ID = @WorkerId";

            var specialties = new List<WorkerSpecialty>();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@WorkerId", workerId);

                try
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            specialties.Add(new WorkerSpecialty
                            {
                                Specialty = reader.GetString(0),
                                WorkerId = reader.GetInt32(1)
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving worker specialties: {ex.Message}");
                    throw;
                }
            }

            return specialties;
        }

        private List<Perform> GetWorkerPerforms(int workerId)
        {
            string commandText = @"
            SELECT Worker_ID, Task_ID
            FROM Perform
            WHERE Worker_ID = @WorkerId";

            var performs = new List<Perform>();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@WorkerId", workerId);

                try
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            performs.Add(new Perform
                            {
                                WorkerId = reader.GetInt32(0),
                                TaskId = reader.GetInt32(1),
                                
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving worker performs: {ex.Message}");
                    throw;
                }
            }

            return performs;
        }

        public Worker selectWorkerById(int workerId)
        {
            const string query = @"
                SELECT ID, Name 
                FROM Worker_ 
                WHERE ID = @WorkerId";

            var worker = new Worker();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@WorkerId", workerId);
                try
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (reader.Read())
                        {
                            worker.Id = reader.GetInt32(0);
                            worker.Name = reader.GetString(1);
                            worker.overallRating = reader.GetDecimal(2);
                        }
                        else
                        {
                            return null;
                        }
                    }

                    worker.AvailableTimeSlots = GetWorkerTimeSlots(workerId);
                    worker.Locations = GetWorkerLocations(workerId);
                    worker.Specialties = GetWorkerSpecialties(workerId).ToHashSet();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving worker by ID: {ex.Message}");
                    throw;
                }
            }
            return worker;
        }

        public Worker selectWorkerByName(string workerName)
        {
            const string query = @"
                SELECT ID, Name 
                FROM Worker_ 
                WHERE Name = @WorkerName";

            var worker = new Worker();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@WorkerName", workerName);
                try
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (reader.Read())
                        {
                            worker.Id = reader.GetInt32(0);
                            worker.Name = reader.GetString(1);
                            worker.overallRating = reader.GetDecimal(2);
                        }
                        else
                        {
                            return null;
                        }
                    }
                    worker.AvailableTimeSlots = GetWorkerTimeSlots(worker.Id);
                    worker.Locations = GetWorkerLocations(worker.Id);
                    worker.Specialties = GetWorkerSpecialties(worker.Id).ToHashSet();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving worker by ID: {ex.Message}");
                    throw;
                }
            }
            return worker;
        }

        // Inner join requirement
        public List<Worker> selectWorkerBySpecialty(string specialty)
        {
            const string query = @"
            SELECT w.ID, w.Name, w.OverallRating
            FROM Worker_ w
            INNER JOIN Worker_Specialties ws ON w.ID = ws.Worker_ID
            WHERE ws.Specialties = @Specialty";

            var workers = new List<Worker>();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Specialty", specialty);

                try
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var worker = new Worker
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                overallRating = reader.GetDecimal(2)
                            };

                            worker.Specialties = GetWorkerSpecialties(worker.Id).ToHashSet();
                            worker.AvailableTimeSlots = GetWorkerTimeSlots(worker.Id);
                            worker.Locations = GetWorkerLocations(worker.Id);

                            workers.Add(worker);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving workers by specialty: {ex.Message}");
                    throw;
                }
            }

            return workers;
        }

        public bool DeleteWorkerById(int workerId) {

            using (var connection = new SqlConnection(_connectionString))
            {
                string query = @"DELETE FROM Worker_
                                 WHERE ID = @Id;";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", workerId);
                connection.Open();
                
                return command.ExecuteNonQuery() > 0;
            }
        }
        public bool DeleteWorkerByName(int workerId) {

            using (var connection = new SqlConnection(_connectionString))
            {
                string query = @"DELETE FROM Worker_
                                 WHERE Name = @name;";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@name", workerId);
                connection.Open();
                
                return command.ExecuteNonQuery() > 0;
            }
        }
        public bool DeleteAllCWorkers()
        {

            using (var connection = new SqlConnection(_connectionString))
            {

                string query = "DELETE FROM Worker_;" +
                               "DBCC CHECKIDENT ('Worker_', RESEED, 0);";

                var command = new SqlCommand(query, connection);
                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
        }
        
        public bool UpdateWorkerName(int workerId,string workerName) 
        {

            string commandText = $@"
                UPDATE Worker_ 
                SET Name = @name 
                WHERE ID = @id;";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(commandText, connection))
            {
                
                command.Parameters.AddWithValue("@name", workerName);
                command.Parameters.AddWithValue("@id", workerId);
                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    Console.WriteLine($"Updated {rowsAffected} worker/s matching name.");
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error updating worker: {e.Message}");
                    throw;
                }
            }
        }
        
        public bool CalculateOverallRating(int workerId)
        {
            // First get the average rating
            string selectCommandText = @"
                SELECT AVG(CAST(WorkerRating AS DECIMAL(3,2)))
                FROM RequestExecution 
                WHERE Worker_ID = @workerId";
    
            // Then update the worker's overall rating
            string updateCommandText = @"
                UPDATE Worker_
                SET OverallRating = @averageRating
                WHERE ID = @workerId";

            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    // First calculate the average rating
                    decimal? averageRating = null;
                    using (var selectCommand = new SqlCommand(selectCommandText, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@workerId", workerId);
                        var result = selectCommand.ExecuteScalar();
                        
                        if (result != DBNull.Value)
                        {
                            averageRating = Convert.ToDecimal(result);
                        }
                    }

                    // Then update the worker's record
                    using (var updateCommand = new SqlCommand(updateCommandText, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@workerId", workerId);
                        
                        if (averageRating.HasValue)
                        {
                            updateCommand.Parameters.AddWithValue("@averageRating", averageRating.Value);
                        }
                        else
                        {
                            // Handle case where worker has no ratings yet
                            updateCommand.Parameters.AddWithValue("@averageRating", DBNull.Value);
                        }

                        int rowsAffected = updateCommand.ExecuteNonQuery();
                        Console.WriteLine($"Updated overall rating for worker ID {workerId}. Rows affected: {rowsAffected}");
                        return rowsAffected > 0;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error calculating overall rating: {e.Message}");
                    throw;
                }
            }
        }

        public bool UpdateWorkerTimeSlots(int workerId, WorkerAvailableTimeSlot newTimeSlot)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string insertQuery = @"INSERT INTO Worker_AvailableTimeSlots 
                              (Worker_ID, AvailableTimeSlots)
                              VALUES (@WorkerId, @Slot)";
                try
                {
                    using (var insertCommand = new SqlCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@WorkerId", workerId);
                        insertCommand.Parameters.AddWithValue("@Slot", newTimeSlot.Slot);
                        return insertCommand.ExecuteNonQuery() > 0;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error updating time slots: {e.Message}");
                    throw;
                }
            }
        }

        public bool UpdateWorkerLocations(int workerId, WorkerLocation newLocation)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    string insertQuery = @"INSERT INTO Worker_Locations 
                                  (Worker_ID, City, Street, Country)
                                  VALUES (@WorkerId, @City, @Street, @Country)";
                    using (var insertCommand = new SqlCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@WorkerId", workerId);
                        insertCommand.Parameters.AddWithValue("@City", newLocation.City);
                        insertCommand.Parameters.AddWithValue("@Street", newLocation.Street);
                        insertCommand.Parameters.AddWithValue("@Country", newLocation.Country);
                        return insertCommand.ExecuteNonQuery() > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating locations: {ex.Message}");
                    throw;
                }
            }
        }
        public bool UpdateWorkerSpecialties(int workerId, WorkerSpecialty newSpeciality)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                try{
                    string insertQuery = @"INSERT INTO Worker_Specialties 
                                  (Worker_ID, Specialties)
                                  VALUES (@WorkerId, @Specialty)";
                    using (var insertCommand = new SqlCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@WorkerId", workerId);
                        insertCommand.Parameters.AddWithValue("@Specialty", newSpeciality.Specialty);
                        return insertCommand.ExecuteNonQuery() > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating specialties: {ex.Message}");
                    throw;
                }
            }
        }
    }
}