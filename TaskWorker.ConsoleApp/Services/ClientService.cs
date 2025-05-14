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
    internal class ClientService
    {
        private string _connectionString;
        public ClientService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Client> selectByName(string name)
        {
            var clients = new List<Client>();
            string commandText = @"
                      SELECT * FROM Client_ WHERE Name = @Name"; 
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@Name", name);
                try
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read()) 
                        {
                            clients.Add(new Client
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ID")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Password = reader.GetString(reader.GetOrdinal("Password")),
                                City = reader.GetString(reader.GetOrdinal("City")),
                                StreetName = reader.GetString(reader.GetOrdinal("StreetName")),
                                Country = reader.GetString(reader.GetOrdinal("Country")),
                                StreetNumber = reader.GetInt32(reader.GetOrdinal("StreetNumber")),
                                ApartmentNumber = reader.GetInt32(reader.GetOrdinal("ApartmentNumber"))
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving clients: {ex.Message}");
                    throw;
                }
            }

            return clients;
        }
        public List<Client> selectByCity(string city)
        {
            var clients = new List<Client>();
            string commandText = @"
                      SELECT * FROM Client_ WHERE City = @City"; 
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@City", city);

                try
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read()) 
                        {
                            clients.Add(new Client
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ID")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Password = reader.GetString(reader.GetOrdinal("Password")),
                                City = reader.GetString(reader.GetOrdinal("City")),
                                StreetName = reader.GetString(reader.GetOrdinal("StreetName")),
                                Country = reader.GetString(reader.GetOrdinal("Country")),
                                StreetNumber = reader.GetInt32(reader.GetOrdinal("StreetNumber")),
                                ApartmentNumber = reader.GetInt32(reader.GetOrdinal("ApartmentNumber"))
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving clients: {ex.Message}");
                    throw;
                }
            }

            return clients;
        }
        public List<Client> selectByName_City(string name,string city)
        {
            var clients = new List<Client>();
            string commandText = @"
                      SELECT * FROM Client_ WHERE Name = @Name AND City = @City"; 
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@City", city);
                try
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read()) 
                        {
                            clients.Add(new Client
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ID")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Password = reader.GetString(reader.GetOrdinal("Password")),
                                City = reader.GetString(reader.GetOrdinal("City")),
                                StreetName = reader.GetString(reader.GetOrdinal("StreetName")),
                                Country = reader.GetString(reader.GetOrdinal("Country")),
                                StreetNumber = reader.GetInt32(reader.GetOrdinal("StreetNumber")),
                                ApartmentNumber = reader.GetInt32(reader.GetOrdinal("ApartmentNumber"))
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving clients: {ex.Message}");
                    throw;
                }
            }

            return clients;
        }
        public List<Client> getAllClients()
        {
            var clients = new List<Client>();
            string commandText = @"
                      SELECT * FROM Client_"; 
            
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(commandText, connection))
            {
                try
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read()) 
                        {
                            clients.Add(new Client
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ID")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Password = reader.GetString(reader.GetOrdinal("Password")),
                                City = reader.GetString(reader.GetOrdinal("City")),
                                StreetName = reader.GetString(reader.GetOrdinal("StreetName")),
                                Country = reader.GetString(reader.GetOrdinal("Country")),
                                StreetNumber = reader.GetInt32(reader.GetOrdinal("StreetNumber")),
                                ApartmentNumber = reader.GetInt32(reader.GetOrdinal("ApartmentNumber"))
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving clients: {ex.Message}");
                    throw;
                }
            }
    
            return clients; 
        }

        public void deleteClientById(int clientId)
        {
            string commandText = @"
            DELETE FROM Client_ WHERE ID = @Id";
            using(var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@Id", clientId);
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    Console.WriteLine($"Client whose id {clientId} deleted successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting client: {ex.Message}");
                    throw;
                }
            }
        }
        public void deleteClientByName(string name)
        {
            string commandText = @"
            DELETE FROM Client_ WHERE Name = @Name";
            using(var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@Name", name);
                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if(rowsAffected == 1)
                        Console.WriteLine($"Client whose name {name} deleted successfully.");
                    else
                    {
                        Console.WriteLine($"{rowsAffected} Clients whose name is {name} deleted successfully.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting client: {ex.Message}");
                    throw;
                }
            }
        }
        public void deleteClientById_Name(int clientId,string name)
        {
            string commandText = @"
            DELETE FROM Client_ WHERE ID = @Id AND Name = @Name";
            using(var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@Id", clientId);
                command.Parameters.AddWithValue("@Name", name);
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery(); 
                    Console.WriteLine($"Client whose id {clientId} deleted successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting client: {ex.Message}");
                    throw;
                }
            }
        }
        public void addClient(Client client)
        {
            string commandText = @"
            INSERT INTO Client_ (Name, Email,Password, City, StreetName, Country, StreetNumber, ApartmentNumber)
            VALUES (@Name, @Email,@Password, @City, @StreetName, @Country, @StreetNumber, @ApartmentNumber);
            SELECT SCOPE_IDENTITY();"; 

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(commandText, connection))
            {
                // Add parameters to prevent SQL injection
                command.Parameters.AddWithValue("@Name", client.Name);
                command.Parameters.AddWithValue("@Email", client.Email);
                command.Parameters.AddWithValue("@Password", client.Password);
                command.Parameters.AddWithValue("@City", client.City);
                command.Parameters.AddWithValue("@StreetName", client.StreetName);
                command.Parameters.AddWithValue("@Country", client.Country);
                command.Parameters.AddWithValue("@StreetNumber", client.StreetNumber);
                command.Parameters.AddWithValue("@ApartmentNumber", client.ApartmentNumber);

                try
                {
                    connection.Open();
        
                    // Execute and get the auto-generated ID
                    var newId = command.ExecuteScalar();
        
                    // Set the ID back to the client object if needed
                    if (newId != null && newId != DBNull.Value)
                    {
                        client.Id = Convert.ToInt32(newId);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error adding client: {ex.Message}");
                    throw;
                }
            }
        }
        public void addClientPhone(int clientId,string phoneNumber)
        {
            string commandText = @"
            INSERT INTO Client_Phone (PhoneNumber , Client_ID)
            VALUES (@PhoneNumber, @Client_ID)";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                command.Parameters.AddWithValue("@Client_ID", clientId);
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error add phone: {ex.Message}");
                    throw;
                }
                
            }
        }
        public Client getClientById(int clientId)
        {
            const string query = @"
        SELECT ID, Name, Email, Password , City, StreetName, Country, StreetNumber, ApartmentNumber 
        FROM Client_ 
        WHERE ID = @ClientId";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ClientId", clientId);
                try
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (reader.Read())
                        {
                            return new Client
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Email = reader.GetString(2),
                                Password = reader.GetString(3),
                                City = reader.GetString(4),
                                StreetName = reader.GetString(5),
                                Country = reader.GetString(6),
                                StreetNumber = reader.GetInt32(7),
                                ApartmentNumber = reader.GetInt32(8)
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving client by ID: {ex.Message}");
                    throw;
                }
            }

            return null; 
        }
        public Client getClientByEmail(string email)
        {
            const string query = @"
            SELECT ID, Name, Email,Password, City, StreetName, Country, StreetNumber, ApartmentNumber 
            FROM Client_ 
            WHERE Email = @Email";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Email", email);

                try
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (reader.Read())
                        {
                            return new Client
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Email = reader.GetString(2),
                                Password = reader.GetString(3),
                                City = reader.GetString(4),
                                StreetName = reader.GetString(5),
                                Country = reader.GetString(6),
                                StreetNumber = reader.GetInt32(7),
                                ApartmentNumber = reader.GetInt32(8)
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving client by email: {ex.Message}");
                    throw;
                }
            }
            return null; 
        }
    }
}
