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
            try
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
                }
                return workers;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting all workers: {ex.Message}");
                throw;
            }
        }

        public void AddWorker(Worker worker)
        {
            try
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
                    connection.Open();
                    var newId = (int)command.ExecuteScalar();
                    worker.Id = newId;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding worker: {ex.Message}");
                throw;
            }
        }

        public void AddWorkerAvailableTimeSlot(WorkerAvailableTimeSlot timeSlot)
        {
            try
            {
                string commandText = @"
                INSERT INTO Worker_AvailableTimeSlots (Worker_ID, AvailableTimeSlots)
                VALUES (@WorkerId, @Slot)";

                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("@WorkerId", timeSlot.WorkerId);
                    command.Parameters.AddWithValue("@Slot", timeSlot.Slot);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding worker time slot: {ex.Message}");
                throw;
            }
        }

        public void AddWorkerLocation(WorkerLocation location)
        {
            try
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
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding worker location: {ex.Message}");
                throw;
            }
        }

        public void AddWorkerSpecialty(WorkerSpecialty specialty)
        {
            try
            {
                string commandText = @"
                INSERT INTO Worker_Specialties (Worker_ID, Specialties)
                VALUES (@WorkerId, @Specialty)";

                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("@WorkerId", specialty.WorkerId);
                    command.Parameters.AddWithValue("@Specialty", specialty.Specialty);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding worker specialty: {ex.Message}");
                throw;
            }
        }

        public List<WorkerAvailableTimeSlot> GetWorkerTimeSlots(int workerId)
        {
            try
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
                return timeSlots;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving worker time slots: {ex.Message}");
                throw;
            }
        }

        public List<WorkerLocation> GetWorkerLocations(int workerId)
        {
            try
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
                return locations;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving worker locations: {ex.Message}");
                throw;
            }
        }

        public List<WorkerSpecialty> GetWorkerSpecialties(int workerId)
        {
            try
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
                return specialties;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving worker specialties: {ex.Message}");
                throw;
            }
        }

        public Worker selectWorkerById(int workerId)
        {
            try
            {
                const string query = @"
                    SELECT ID, Name, OverallRating
                    FROM Worker_ 
                    WHERE ID = @WorkerId";

                var worker = new Worker();

                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@WorkerId", workerId);
                    connection.Open();
                    using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (reader.Read())
                        {
                            worker.Id = reader.GetInt32(reader.GetOrdinal("ID"));
                            worker.Name = reader.GetString(reader.GetOrdinal("Name"));
                            worker.overallRating = reader.GetDecimal(reader.GetOrdinal("OverallRating"));
                        }
                        else
                        {
                            return null;
                        }
                    }

                    worker.AvailableTimeSlots = GetWorkerTimeSlots(workerId);
                    worker.Locations = GetWorkerLocations(workerId);
                    worker.Specialties = GetWorkerSpecialties(workerId).ToHashSet();
                    worker.Performs = GetWorkerPerforms(workerId).ToHashSet();
                }
                return worker;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving worker by ID: {ex.Message}");
                throw;
            }
        }

        public Worker selectWorkerByName(string workerName)
        {
            try
            {
                const string query = @"
                    SELECT ID, Name, OverallRating
                    FROM Worker_ 
                    WHERE Name = @WorkerName";

                var worker = new Worker();

                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@WorkerName", workerName);
                    connection.Open();
                    using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (reader.Read())
                        {
                            worker.Id = reader.GetInt32(reader.GetOrdinal("ID"));
                            worker.Name = reader.GetString(reader.GetOrdinal("Name"));
                            worker.overallRating = reader.GetDecimal(reader.GetOrdinal("OverallRating"));
                        }
                        else
                        {
                            return null;
                        }
                    }
                    worker.AvailableTimeSlots = GetWorkerTimeSlots(worker.Id);
                    worker.Locations = GetWorkerLocations(worker.Id);
                    worker.Specialties = GetWorkerSpecialties(worker.Id).ToHashSet();
                    worker.Performs = GetWorkerPerforms(worker.Id).ToHashSet();
                }
                return worker;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving worker by name: {ex.Message}");
                throw;
            }
        }

        public List<Worker> selectWorkerBySpecialty(string specialty)
        {
            try
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
                            worker.Performs = GetWorkerPerforms(worker.Id).ToHashSet();
                            workers.Add(worker);
                        }
                    }
                }
                return workers;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving workers by specialty: {ex.Message}");
                throw;
            }
        }

        public bool DeleteWorkerById(int workerId)
        {
            try
            {
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
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting worker by ID: {ex.Message}");
                throw;
            }
        }

        public bool DeleteWorkerByName(int workerId)
        {
            try
            {
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
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting worker by name: {ex.Message}");
                throw;
            }
        }

        public bool DeleteAllCWorkers()
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting all workers: {ex.Message}");
                throw;
            }
        }

        public bool UpdateWorkerName(int workerId, string workerName)
        {
            try
            {
                string commandText = @"
                    UPDATE Worker_ 
                    SET Name = @name 
                    WHERE ID = @id;";

                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("@name", workerName);
                    command.Parameters.AddWithValue("@id", workerId);
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    Console.WriteLine($"Updated {rowsAffected} worker/s matching name.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating worker name: {ex.Message}");
                throw;
            }
        }

        public bool CalculateOverallRating(int workerId)
        {
            try
            {
                decimal? averageRating = null;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string selectCommandText = @"
                        SELECT AVG(CAST(WorkerRating AS DECIMAL(3,2))) as AverageRating
                        FROM RequestExecution 
                        WHERE Worker_ID = @workerId";

                    using (var selectCommand = new SqlCommand(selectCommandText, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@workerId", workerId);
                        var result = selectCommand.ExecuteScalar();

                        if (result != DBNull.Value)
                        {
                            averageRating = Convert.ToDecimal(result);
                        }
                    }

                    string updateCommandText = @"
                        UPDATE Worker_
                        SET OverallRating = @averageRating
                        WHERE ID = @workerId";

                    using (var updateCommand = new SqlCommand(updateCommandText, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@workerId", workerId);

                        if (averageRating.HasValue)
                        {
                            updateCommand.Parameters.AddWithValue("@averageRating", averageRating.Value);
                        }
                        else
                        {
                            updateCommand.Parameters.AddWithValue("@averageRating", DBNull.Value);
                        }

                        int rowsAffected = updateCommand.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating overall rating for worker {workerId}: {ex.Message}");
                throw;
            }
        }

        public bool UpdateWorkerTimeSlots(int workerId, WorkerAvailableTimeSlot newTimeSlot)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string insertQuery = @"INSERT INTO Worker_AvailableTimeSlots 
                                  (Worker_ID, AvailableTimeSlots)
                                  VALUES (@WorkerId, @Slot)";
                    using (var insertCommand = new SqlCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@WorkerId", workerId);
                        insertCommand.Parameters.AddWithValue("@Slot", newTimeSlot.Slot);
                        connection.Open();
                        return insertCommand.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating time slots: {ex.Message}");
                throw;
            }
        }

        public bool UpdateWorkerLocations(int workerId, WorkerLocation newLocation)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
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
                        connection.Open();
                        return insertCommand.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating locations: {ex.Message}");
                throw;
            }
        }

        public bool UpdateWorkerSpecialties(int workerId, WorkerSpecialty newSpeciality)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string insertQuery = @"INSERT INTO Worker_Specialties 
                                  (Worker_ID, Specialties)
                                  VALUES (@WorkerId, @Specialty)";
                    using (var insertCommand = new SqlCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@WorkerId", workerId);
                        insertCommand.Parameters.AddWithValue("@Specialty", newSpeciality.Specialty);
                        connection.Open();
                        return insertCommand.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating specialties: {ex.Message}");
                throw;
            }
        }

        public bool DeleteWorkerLocation(int workerId, string city, string street, string country)
        {
            try
            {
                string commandText = @"
                DELETE FROM Worker_Locations 
                WHERE Worker_ID = @WorkerId 
                AND City = @City 
                AND Street = @Street 
                AND Country = @Country";

                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("@WorkerId", workerId);
                    command.Parameters.AddWithValue("@City", city);
                    command.Parameters.AddWithValue("@Street", street);
                    command.Parameters.AddWithValue("@Country", country);
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting worker location: {ex.Message}");
                throw;
            }
        }

        public bool DeleteWorkerSpecialty(int workerId, string specialty)
        {
            try
            {
                string commandText = @"
                DELETE FROM Worker_Specialties 
                WHERE Worker_ID = @WorkerId 
                AND Specialties = @Specialty";

                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("@WorkerId", workerId);
                    command.Parameters.AddWithValue("@Specialty", specialty);
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting worker specialty: {ex.Message}");
                throw;
            }
        }

        public bool DeleteWorkerAvailableTimeSlot(int workerId, DateTime timeSlot)
        {
            try
            {
                string commandText = @"
                DELETE FROM Worker_AvailableTimeSlots 
                WHERE Worker_ID = @WorkerId 
                AND AvailableTimeSlots = @TimeSlot";

                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("@WorkerId", workerId);
                    command.Parameters.AddWithValue("@TimeSlot", timeSlot);
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting worker time slot: {ex.Message}");
                throw;
            }
        }

        public List<Perform> GetWorkerPerforms(int workerId)
        {
            try
            {
                string query = @"
                    SELECT Worker_ID, Task_ID
                    FROM Perform
                    WHERE Worker_ID = @WorkerId";

                var performs = new List<Perform>();

                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@WorkerId", workerId);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            performs.Add(new Perform
                            {
                                WorkerId = reader.GetInt32(0),
                                TaskId = reader.GetInt32(1)
                            });
                        }
                    }
                }
                return performs;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving worker performs: {ex.Message}");
                throw;
            }
        }

        public void AddWorkerPerform(int workerId, int taskId)
        {
            try
            {
                string commandText = @"
                    INSERT INTO Perform (Worker_ID, Task_ID)
                    VALUES (@WorkerId, @TaskId)";

                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("@WorkerId", workerId);
                    command.Parameters.AddWithValue("@TaskId", taskId);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding worker perform: {ex.Message}");
                throw;
            }
        }
    }
}
