using System;
using System.Linq;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using TaskWorker.Models;
using TaskWorker.Services;
using TaskWorker.Validation;

namespace TaskWorker.ConsoleApp
{
    internal class Program
    {
        public const string connectionString = "Server=KOTB;Database=Task_Worker;Trusted_Connection=true;";
        static void Main(string[] args)
        {
            //mainMenu();
        }
        //--------------------------------  Menu Functions  --------------------------------
        public static void mainMenu()
        {
            while (true)
            {
                Console.WriteLine("===========  Task Worker Management  ===========");
                Console.WriteLine("=====  Main Menu  =====");
                Console.Write("1. Admin\n"+ "2. Client\n" + "3. Exit\n");
                int choice = int.Parse(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        Console.WriteLine("Admin selected.");
                        adminMenu();
                        break;
                    case 2:
                        Console.WriteLine("Client selected.");
                        MainClientMenu();
                        break;
                    case 3:
                        Console.WriteLine("Thank you for your time.");
                        Environment.Exit(0);
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please correct choice.");
                        break;
                }
            }
        }
        public static void adminMenu()
        {
            while (true)
            {
                Console.WriteLine("=====  Admin Menu  =====");
                Console.WriteLine("Enter username:");
                string username = Console.ReadLine();
                Console.WriteLine("Enter password:");
                string password = Console.ReadLine();
                if (username == null || password == null || username != "admin" || password != "123")
                {
                    Console.WriteLine("Invalid inputs.");
                    continue;
                }
                Console.WriteLine("Login successful.");
                break;
            }
            while (true)
            {
                Console.WriteLine("=====  Admin Menu  =====");
                Console.WriteLine("1. worker\n" +
                                  "2. Add worker\n"+
                                  "3. Edit worker\n"+
                                  "4. Delete worker\n"+
                                  "5. View client\n"+
                                  "6. Edit client\n"+
                                  "7. Delete client\n"+
                                  "8. Return to Main Menu");
                    int choice = int.Parse(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        
                    case 2:
                        // add worker
                        break;
                    case 3:
                        // edit worker
                        break;
                    case 4:
                        // delete worker
                        break;
                    case 5:
                        SelectClientMenu();
                        break;
                    case 6:
                        editClientMenu();
                        break;
                    case 7:
                        DeleteClientMenu();
                        break;
                    case 8:
                        mainMenu();
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }
        //--------------------------------  Clients Functions  --------------------------------
        public static void MainClientMenu()
        {
            var clientService = new ClientService(connectionString);
            while (true)
            {
                Console.WriteLine("=====  Client Menu  =====");
                Console.WriteLine("1. Sign Up\n" + 
                                  "2. Login in\n"+
                                  "3. Return to Main Menu"
                );
                int choice = int.Parse(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        signUpClient();
                        break;
                    case 2:
                        Console.Write("Email: ");
                        string email = Console.ReadLine();
                        if(!CheckInputs.isValidEmail(email))
                        {
                            Console.WriteLine("Invalid email");
                            return;
                        }
                        Console.Write("Password: ");
                        string pass = Console.ReadLine();
                        
                        Client logClient = clientService.getClientByEmail(email);
                        if (logClient.Password != pass || logClient.Email != email)
                        {
                            Console.WriteLine("Login failed , Invalid data.");
                        
                        }
                        Console.WriteLine("Login Successful.");
                        break;
                    case 3:
                        mainMenu();
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }

                while (true)
                {
                    Console.WriteLine("=====  Client Menu  =====");
                    Console.WriteLine("1. View all available tasks\n" +
                                      "2. Return to client Menu");
                    choice = int.Parse(Console.ReadLine());
                    switch (choice)
                    {
                        case 1:
                            // tasks view
                            break;
                        case 2:
                            MainClientMenu();
                            break;
                        default:
                            Console.WriteLine("Invalid choice.");
                            break;
                    }  
                }
            }
        }
        public static void SelectClientMenu() {
            var clientService = new ClientService(connectionString);
            List<Client> clients;
            Dictionary<string, object> conditions = new Dictionary<string, object>();
            List<string> columnsToSelect = new List<string>();
            Dictionary<string, string> choicesValues = new Dictionary<string, string>
            {
                { "1", "All" },
                { "2", "Id"},
                { "3", "Name" },
                { "4", "Email" },
                { "5", "Phone" },
                { "6", "Country" },
                { "7", "City" },
                { "8", "Return to admin menu" }
            };
            //-----------------------------------------------------------------------
            // Columns select menu
            Console.WriteLine("Select client/s by:\n" +
                              "1. All\n" +
                              "2. Id\n" +
                              "3. Name\n" +
                              "4. Email\n" +
                              "5. Phone\n" +
                              "6. Country\n" +
                              "7. City\n" +
                              "8. Return to admin menu");
            Console.WriteLine("Select column(s) to display (e.g., 1 2 4):");
            string columnSelectChoice = Console.ReadLine();
            
            var selectedColumns = columnSelectChoice.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (selectedColumns.Contains("8"))
            {
                return;
            }
            
            foreach (var col in selectedColumns)
            {
                if (choicesValues.ContainsKey(col))
                {
                    columnsToSelect.Add(choicesValues[col]);
                }
            }
            // Condition menu
            Console.WriteLine("Choose condition(s):\n" +
                              "1. All\n" +
                              "2. Id\n" +
                              "3. Name\n" +
                              "4. Email\n" +
                              "5. Phone\n" +
                              "6. Country\n" +
                              "7. City\n" +
                              "8. Return to admin menu");
            Console.WriteLine("Enter your condition choices (e.g., 2 5):");
            string conditionChoice = Console.ReadLine();
            
            var selectedConditions = conditionChoice.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (selectedConditions.Contains("8"))
            {
                return;
            }
            foreach (var cond in selectedConditions)
            {
                // All condition
                if (cond == "1")
                {
                    conditions[choicesValues[cond]] = "All";
                }
                else if (choicesValues.ContainsKey(cond))
                {
                    Console.Write($"Enter value for {choicesValues[cond]}: ");
                    if (cond == "2")
                    {
                        int valueInt = int.Parse(Console.ReadLine());
                        conditions[choicesValues[cond]] = valueInt;
                    }
                    else
                    {
                        string valueStr = Console.ReadLine();
                        conditions[choicesValues[cond]] = valueStr;
                    }
                }
            }
            // Call function selectClients to the database
            clients = clientService.getAllClients();
            printClients(clients); 
        }
        public static void DeleteClientMenu()
        {
            var clientService = new ClientService(connectionString);

            Console.WriteLine("Delete client(s) by condition:\n" +
                              "1. Name\n" +
                              "2. Id\n" +
                              "3. Email\n" +
                              "4. Phone\n" +
                              "5. Country\n" +
                              "6. City\n" +
                              "7. Return to admin menu");
            Console.WriteLine("Enter your condition choices (e.g., 2 4):");

            string conditionChoice = Console.ReadLine();

            Dictionary<string, string> choicesValues = new Dictionary<string, string>
            {
                { "1", "Id"},
                { "2", "Name" },
                { "3", "Email" },
                { "4", "Phone" },
                { "5", "Country" },
                { "6", "City" },
                { "7", "Return to admin menu" }
            };

            Dictionary<string, object> conditions = new Dictionary<string, object>();

            var selectedConditions = conditionChoice.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (selectedConditions.Contains("7"))
            {
                return;
            }
            foreach (var cond in selectedConditions)
            {
                if (choicesValues.ContainsKey(cond))
                {
                    Console.Write($"Enter value for {choicesValues[cond]}: ");
                    if (cond == "1")
                    {
                        int valueInt = int.Parse(Console.ReadLine());
                        conditions[choicesValues[cond]] = valueInt;
                    }
                    else
                    {
                        string valueStr = Console.ReadLine();
                        conditions[choicesValues[cond]] = valueStr;
                    }
                }
            }
            
            // Call function DeleteClients to the database
            bool success = clientService.DeleteClients(conditions);
            if (!success)
                Console.WriteLine("No clients matched the condition.");
        }
        public static void editClientMenu()
        {
            var clientService = new ClientService(connectionString);

            Dictionary<string, string> choicesValues = new Dictionary<string, string>
            {
                { "1", "Id" },
                { "2", "Name" },
                { "3", "Email" },
                { "4", "Phone" },
                { "5", "Country" },
                { "6", "City" },
                { "7", "Return to admin menu" }
            };
            
            Console.WriteLine("Update client(s) by condition:\n" +
                              "1. Id\n" +
                              "2. Name\n" +
                              "3. Email\n" +
                              "4. Phone\n" +
                              "5. Country\n" +
                              "6. City\n" +
                              "7. Return to admin menu");
            Console.WriteLine("Enter condition choices (e.g., 1 3):");
            string conditionChoice = Console.ReadLine();
            var selectedConditions = conditionChoice.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (selectedConditions.Contains("7"))
            {
                return;
            }

            Dictionary<string, object> conditions = new Dictionary<string, object>();

            foreach (var cond in selectedConditions)
            {
                if (choicesValues.ContainsKey(cond))
                {
                    Console.Write($"Enter value for {choicesValues[cond]}: ");
                    if (cond == "1")
                    {
                        int valueInt = int.Parse(Console.ReadLine());
                        conditions[choicesValues[cond]] = valueInt;
                    }
                    else
                    {
                        string valueStr = Console.ReadLine();
                        conditions[choicesValues[cond]] = valueStr;
                    }
                }
            }
            Console.WriteLine("\nSelect fields to update:\n" +
                              "2. Name\n" +
                              "3. Email\n" +
                              "4. Phone\n" +
                              "5. Country\n" +
                              "6. City\n" +
                              "7. Return to admin menu");
            Console.WriteLine("Enter field numbers to update (e.g., 2 5):");
            string updateChoice = Console.ReadLine();
            var selectedUpdates = updateChoice.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (selectedUpdates.Contains("7"))
            {
                return;
            }

            Dictionary<string, object> updates = new Dictionary<string, object>();

            foreach (var upd in selectedUpdates)
            {
                if (choicesValues.ContainsKey(upd))
                {
                    Console.Write($"Enter value for {choicesValues[upd]}: ");
                    string valueStr = Console.ReadLine();
                    conditions[choicesValues[upd]] = valueStr;
                }
            }
            bool success = clientService.updateClients(updates, conditions);
        }
        public static void signUpClient()
        {
            
            var client = new Client();
            Console.Write("Full Name: ");
            client.Name = Console.ReadLine();
            
            Console.Write("Email: ");
            client.Email = Console.ReadLine();
            while (!CheckInputs.isValidEmail(client.Email))
            {
                Console.WriteLine("Invalid email format. Please try again.");
                Console.Write("Email: ");
                client.Email = Console.ReadLine();
            }
            
            Console.Write("Password: ");
            client.Password = Console.ReadLine();
            
            Console.Write("Country: ");
            client.Country = Console.ReadLine();
            
            Console.Write("City: ");
            client.City = Console.ReadLine();
    
            Console.Write("Street Name: ");
            client.StreetName = Console.ReadLine();
    
            Console.Write("Street Number: ");
            int streetNumber;
            while (!int.TryParse(Console.ReadLine(), out streetNumber))
            {
                Console.WriteLine("Invalid input. Please enter a number:");
                Console.Write("Street Number: ");
            }
            
            client.StreetNumber = streetNumber;
    
            Console.Write("Apartment Number: ");
            int apartmentNumber;
            while (!int.TryParse(Console.ReadLine(), out apartmentNumber))
            {
                Console.WriteLine("Invalid input. Please enter a number:");
                Console.Write("Apartment Number: ");
            }
            client.ApartmentNumber = apartmentNumber;
            
            Console.Write("Phone Number: ");
            string phoneNumber = Console.ReadLine().Trim();
            
            try
            {
                var clientService = new ClientService(connectionString); 
                clientService.addClient(client);
                clientService.addClientPhone(client.Id,phoneNumber);
        
                Console.WriteLine("\nRegistration successful!");
                Console.WriteLine($"Your client ID is: {client.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nRegistration failed: {ex.Message}");
                MainClientMenu();
            }
        }
        public static void printClients(List<Client> clients)
        {
            if (clients.Count == 0)
            {
                Console.WriteLine("No data found.");
                return;
            }
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine($"{clients.Count} client(s) selected.");
            foreach (var client in clients)
            {
                client.display();
            }
        }
    }
    
}
