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
        public const string connectionString = "Server=DESKTOP-TGGO93S;Database=Task_Worker;Trusted_Connection=true;";
        static void Main(string[] args)
        {
            mainMenu();
            var taskservice = new TaskService(connectionString);
            taskservice.addTask(Task_("Task 1","Carpenter"));
            taskservice.addTask(Task_("Task 2","Plumber"));
            taskservice.addTask(Task_("Task 3","Engineer"));
            var tasks = taskservice.getallTasks();
            Console.WriteLine("ID\tName\tReq\n");
            foreach (var tsk in tasks)
            {
                tsk.display();

            }
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
                        Console.WriteLine("Worker selected.");
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
                Console.WriteLine("1. View worker\n" +
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
                        // view worker
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
                        selectClientMenu();
                        break;
                    case 6:
                        // edit client
                        break;
                    case 7:
                        deleteClientMenu();
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
        public static void selectClientMenu()
        {
            var clientService = new ClientService(connectionString);
            Console.WriteLine("Select client/s with:\n" +
                              "1. All\n" +
                              "2. Name\n" +
                              "3. City\n" +
                              "4. Name and city\n" +
                              "5. Return to admin menu");
            int choiceSelect = int.Parse(Console.ReadLine());
            string name, city;
            List<Client> clients;
            switch (choiceSelect)
            {
                case 1:
                    clients = clientService.getAllClients();
                    printClients(clients);
                    break;
                case 2:
                    Console.Write("Enter the name:");
                    name = Console.ReadLine();
                    clients = clientService.selectByName(name);
                    printClients(clients);
                    break;
                case 3:
                    Console.Write("Enter the city:");
                    city = Console.ReadLine();
                    clients = clientService.selectByCity(city);
                    printClients(clients);
                    break;
                case 4:
                    Console.Write("Enter the name:");
                    name = Console.ReadLine();
                    Console.Write("Enter the city:");
                    city = Console.ReadLine();
                    clients = clientService.selectByName_City(name,city);
                    printClients(clients);
                    break;
                case 5:
                    adminMenu();
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }
        public static void deleteClientMenu()
        {
            var clientService = new ClientService(connectionString);
            Console.WriteLine("Delete client/s with:\n" +
                              "1. Id\n" +
                              "2. Name\n" +
                              "3. Id and Name\n" +
                              "5. Return to admin menu");
            int choiceSelect = int.Parse(Console.ReadLine());
            string name;
            int id;
            switch (choiceSelect)
            {
                case 1:
                    Console.Write("Enter the name:");
                    name = Console.ReadLine();
                    clientService.deleteClientByName(name);
                    break;
                case 2:
                    Console.Write("Enter the city:");
                    id = int.Parse(Console.ReadLine());
                    clientService.deleteClientById(id);
                    break;
                case 3:
                    Console.Write("Enter the name:");
                    name = Console.ReadLine();
                    Console.Write("Enter the id:");
                    id = int.Parse(Console.ReadLine());
                    clientService.deleteClientById_Name(id,name);
                    break;
                case 4:
                    adminMenu();
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
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
                client.Print();
            }
        }
    }
    
}
