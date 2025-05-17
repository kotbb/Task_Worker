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
        
        public void addClient(Client client)
        {
            string commandText = @"
            INSERT INTO Client_ (Name, Email,Password, City, StreetName, Country, StreetNumber, ApartmentNumber)
            VALUES (@Name, @Email,@Password, @City, @StreetName, @Country, @StreetNumber, @ApartmentNumber)
            SELECT CAST(SCOPE_IDENTITY() AS INT);"; 

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(commandText, connection))
            {
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
                    var newId = (int)command.ExecuteScalar();  
                    client.Id = newId;
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
        private List<ClientPhone> getClientPhones(int clientId)
        {
            string commandText = @"
            SELECT Client_ID, PhoneNumber
            FROM Client_Phone
            WHERE Client_ID = @ClientId";

            var clientPhones = new List<ClientPhone>();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@ClientId", clientId);

                try
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            clientPhones.Add(new ClientPhone
                            {
                                ClientId = reader.GetInt32(0),
                                PhoneNumber = reader.GetString(1)
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

            return clientPhones;
        }
        
        public void addClientPaymentInfo(int clientId, ClientPaymentInfo paymentInfo)
        {
            
            string commandText = @"
            INSERT INTO Client_PaymentInfo (CardHolderName , CardNumber , CVV , ExpiryDate , Client_ID)
            VALUES (@CardHolderName, @CardNumber,@CVV,@ExpiryDate,@ClientId)";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@CardHolderName", paymentInfo.CardHolderName);
                command.Parameters.AddWithValue("@CardNumber", paymentInfo.CardNumber);
                command.Parameters.AddWithValue("@CVV", paymentInfo.CVV);  
                command.Parameters.AddWithValue("@ExpiryDate", paymentInfo.ExpiryDate);
                command.Parameters.AddWithValue("@ClientId", clientId);
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error add payment Info: {ex.Message}");
                    throw;
                }
                
            }
        }
        private List<ClientPaymentInfo> getPaymentInfo(int clientId)
        {
            string commandText = @"
            SELECT CardHolderName, CardNumber, CVV, ExpiryDate ,Client_ID
            FROM Client_PaymentInfo
            WHERE Client_ID = @ClientId";

            var clientPaymentInfos = new List<ClientPaymentInfo>();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@ClientId", clientId);

                try
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            clientPaymentInfos.Add(new ClientPaymentInfo
                            {
                                CardHolderName = reader.GetString(0),
                                CardNumber = reader.GetString(1),
                                CVV = reader.GetDecimal(2),
                                ExpiryDate = reader.GetDateTime(3),
                                ClientId = reader.GetInt32(4),
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving client payment info: {ex.Message}");
                    throw;
                }
            }

            return clientPaymentInfos;
        }
        
        public List<Client> getAllClients()
        {
            var clients = new List<Client>();
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT * FROM Client_;";
                var command = new SqlCommand(query, connection);

                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    clients.Add(new Client
                        {
                            Id = id,
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            Password = reader.GetString(reader.GetOrdinal("Password")),
                            City = reader.GetString(reader.GetOrdinal("City")),
                            StreetName = reader.GetString(reader.GetOrdinal("StreetName")),
                            Country = reader.GetString(reader.GetOrdinal("Country")),
                            StreetNumber = reader.GetInt32(reader.GetOrdinal("StreetNumber")),
                            ApartmentNumber = reader.GetInt32(reader.GetOrdinal("ApartmentNumber")),
                            // RelationShips
                            PaymentInfos = getPaymentInfo(id),
                            Phones = getClientPhones(id).ToHashSet()
                        }
                    );
                }
                return clients;
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
                            var id = reader.GetInt32(0);
                            return new Client
                            {
                                Id = id,
                                Name = reader.GetString(1),
                                Email = reader.GetString(2),
                                Password = reader.GetString(3),
                                City = reader.GetString(4),
                                StreetName = reader.GetString(5),
                                Country = reader.GetString(6),
                                StreetNumber = reader.GetInt32(7),
                                ApartmentNumber = reader.GetInt32(8),
                                // RelationShips
                                PaymentInfos = getPaymentInfo(id),
                                Phones = getClientPhones(id).ToHashSet()
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
                            var id = reader.GetInt32(0);
                            return new Client
                            {
                                Id = id,
                                Name = reader.GetString(1),
                                Email = reader.GetString(2),
                                Password = reader.GetString(3),
                                City = reader.GetString(4),
                                StreetName = reader.GetString(5),
                                Country = reader.GetString(6),
                                StreetNumber = reader.GetInt32(7),
                                ApartmentNumber = reader.GetInt32(8),
                                // RelationShips
                                PaymentInfos = getPaymentInfo(id),
                                Phones = getClientPhones(id).ToHashSet()
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
        

        public bool updateClients(Dictionary<string, object> updates, Dictionary<string, object> conditions) 
        {
            var setText = new List<string>();
            var sqlParameters = new List<SqlParameter>();

            foreach (var update in updates)
            {
                setText.Add($"{update.Key} = @{update.Key}");
                sqlParameters.Add(new SqlParameter($"{update.Key}", update.Value));
            }

            var whereText = new List<string>();
            foreach (var condition in conditions)
            {
                whereText.Add($"{condition.Key} = @{condition.Key}");
                sqlParameters.Add(new SqlParameter($"{condition.Key}", condition.Value));
            }

            string commandText = $@"
                UPDATE Client_ 
                SET {string.Join(", ", setText)} 
                WHERE {string.Join(" AND ", whereText)}";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(commandText, connection))
            {
                foreach (var param in sqlParameters)
                {
                    command.Parameters.Add(param);
                }
                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    Console.WriteLine($"Updated {rowsAffected} client/s matching conditions.");
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error updating clients: {e.Message}");
                    throw;
                }
            }
        }
        public bool DeleteClients(Dictionary<string, object> conditions)
        {
            var whereText = new List<string>();
            var sqlParameters = new List<SqlParameter>();

            foreach (var condition in conditions)
            {
                whereText.Add($"{condition.Key} = @{condition.Key}");
                sqlParameters.Add(new SqlParameter($"@{condition.Key}", condition.Value));
            }

            string commandText = $@"
            DELETE FROM Client_ 
            WHERE {string.Join(" AND ", whereText)}";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(commandText, connection))
            {
                foreach (var param in sqlParameters)
                {
                    command.Parameters.Add(param);
                }
                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    Console.WriteLine($"Deleted {rowsAffected} client/s matching conditions.");
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error deleting clients: {e.Message}");
                    throw;
                }
            }
        }
        
        public bool DeleteAllClients() {

            using (var connection = new SqlConnection(_connectionString)) {

                string query = "DELETE FROM Client_;" +
                               "DBCC CHECKIDENT ('Client_', RESEED, 0);";
                
                var command = new SqlCommand(query, connection);
                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
        }
        
    }
}
