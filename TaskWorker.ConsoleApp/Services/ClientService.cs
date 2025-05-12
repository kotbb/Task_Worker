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
        public List<Client> GetAllClients()
        {
            var clients = new List<Client>();
            using (var connection = new SqlConnection(_connectionString))
            using (var adapter = new SqlDataAdapter("SELECT * FROM Client_", connection))
            {
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                foreach (DataRow row in dataTable.Rows)
                {
                    var client = new Client
                    {
                        Id = Convert.ToInt32(row["ID"]),
                        Name = row["Name"].ToString(),
                        Email = row["Email"].ToString(),
                        City = row["City"].ToString(),
                        StreetName = row["StreetName"].ToString(),
                        Country = row["Country"].ToString(),
                        StreetNumber = Convert.ToInt32(row["StreetNumber"]),
                        ApartmentNumber = Convert.ToInt32(row["ApartmentNumber"])
                    };

                    clients.Add(client);
                    Console.WriteLine($"Added successfully {client.Name} (ID: {client.Id})");
                }
            }
            return clients;
        }
    }
}
