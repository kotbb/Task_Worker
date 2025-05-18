using System;
using System.Linq;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using TaskWorker.Models;
using Task = TaskWorker.Models.Task;
using TaskWorker.Services;
using TaskWorker.Validation;

namespace TaskWorker.ConsoleApp
{
    internal class Program
    {
        public const string connectionString = "Server=KOTB;Database=Task_Worker;Trusted_Connection=true;";
        public static ClientService clientService = new ClientService(connectionString);
        public static WorkerService workerService = new WorkerService(connectionString);
        public static TaskService taskService = new TaskService(connectionString);
        public static RequestService requestService = new RequestService(connectionString);
        public static RequestExecutionService requestExecService = new RequestExecutionService(connectionString);
        static void Main(string[] args)
        {
            
            mainMenu();
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
                Console.WriteLine("1. Workers\n" +
                                  "2. Tasks\n"+
                                  "3. Requests\n" +
                                  "4. Clients\n"+
                                  "5. Return to Main Menu");
                    int choice = int.Parse(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        MainWorkerMenu();
                        break;
                    case 2:
                        MainTaskMenu();
                        break;
                    case 3:
                        MainRequestMenu();
                        break;
                    case 4:
                        List<Client> clients = clientService.getAllClients();
                        foreach (Client client in clients)
                        {
                            client.display();
                        }
                        break;
                    case 5:
                        mainMenu();
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }
        //--------------------------------  Request Functions  --------------------------------
        public static void MainTaskMenu(){}
          //--------------------------------  Request Functions  --------------------------------
        public static void MainRequestMenu(){}
        //--------------------------------  Workers Functions  --------------------------------
        public static void MainWorkerMenu()
        {
            Worker logWorker = new Worker();
            while (true)
            {
                Console.WriteLine("=====  Worker Menu  =====");
                Console.WriteLine("1. Add\n" + 
                                  "2. Update\n" +
                                  "3. View\n" +
                                  "4. Delete\n" +
                                  "5. Return to Main Menu"
                );
                int choice = int.Parse(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        AddWorkerFunc();
                        break;
                    case 2:
                        UpdateWorkerFunc();
                        break;
                    case 3:
                        ViewWorkerFunc();
                        break;
                    case 4:
                        DeleteWorkerFunc();
                        break;
                    case 5:
                        mainMenu();
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        public static void AddWorkerFunc()
        {
            Worker newworker = new Worker();
            var workerLocation = new WorkerLocation();
            var workerSlots = new WorkerAvailableTimeSlot();
            var workerSpeciality = new WorkerSpecialty();
            Console.Write("Full Name: ");
            newworker.Name = Console.ReadLine();
            
            Console.Write("Speciality: ");
            workerSpeciality.Specialty = Console.ReadLine();
            
            Console.Write("Country: ");
            workerLocation.Country = Console.ReadLine();
            
            Console.Write("City: ");
            workerLocation.City = Console.ReadLine();
    
            Console.Write("Street Name: ");
            workerLocation.Street = Console.ReadLine();
            
            Console.Write("AvailableTimeSlot: ");
            string input = Console.ReadLine();
            if (!DateTime.TryParse(input, out DateTime slotTime))
            {
                Console.WriteLine("Invalid date-time format. Please try again.");
                return;
            }
            workerSlots.Slot = slotTime;
            try
            {
                workerService.AddWorker(newworker);
                workerLocation.WorkerId = newworker.Id;
                workerSpeciality.WorkerId = newworker.Id;
                workerSlots.WorkerId = newworker.Id;
                workerService.AddWorkerLocation(workerLocation);
                workerService.AddWorkerSpecialty(workerSpeciality);
                workerService.AddWorkerAvailableTimeSlot(workerSlots);
        
                Console.WriteLine("\nRegistration successful!");
                Console.WriteLine($"Your Worker ID is: {newworker.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nRegistration failed: {ex.Message}");
            }
            MainWorkerMenu();
        }

        public static void UpdateWorkerFunc()
        {
            Console.WriteLine("Enter Worker ID you want to update:");
            int workerId = int.Parse(Console.ReadLine());
            
            Console.WriteLine("Choose:\n" +
                              "1. Name\n" +
                              "2. Location\n" +
                              "3. Speciality\n" +
                              "4. AvailableSlots\n" +
                              "5. Return to admin menu");
            int choice = int.Parse(Console.ReadLine());
            switch (choice)
            {
                case 1:
                    Console.WriteLine("Enter new Name:");
                    string name = Console.ReadLine();
                    workerService.UpdateWorkerName(workerId, name);
                    Console.WriteLine("Name successfully updated!");
                    break;
                
                case 2:
                    WorkerLocation workerLocation = new WorkerLocation();
                    Console.WriteLine("Enter new Country:");
                    workerLocation.Country = Console.ReadLine();
                    Console.WriteLine("Enter new City:");
                    workerLocation.City = Console.ReadLine();
                    Console.WriteLine("Enter new Street:");
                    workerLocation.Street = Console.ReadLine();
                    workerService.UpdateWorkerLocations(workerId, workerLocation);
                    Console.WriteLine("Worker Location successfully updated!");
                    break;
                
                case 3:
                    WorkerSpecialty workerSpecialty = new WorkerSpecialty();
                    Console.WriteLine("Enter new Speciality:");
                    workerSpecialty.Specialty = Console.ReadLine();
                    workerService.UpdateWorkerSpecialties(workerId, workerSpecialty);
                    Console.WriteLine("Worker Speciality successfully updated!");
                    break;
                
                case 4:
                    WorkerAvailableTimeSlot workerSlot = new WorkerAvailableTimeSlot();
                    Console.WriteLine("Enter new Slot:");
                    string input = Console.ReadLine();
                    if (!DateTime.TryParse(input, out DateTime slotTime))
                    {
                        Console.WriteLine("Invalid date-time format. Please try again.");
                        return;
                    }
                    workerSlot.Slot = slotTime;
                    workerService.UpdateWorkerTimeSlots(workerId, workerSlot);
                    Console.WriteLine("Worker slotTime successfully updated!");
                    break;
                
                case 5:
                    MainWorkerMenu();
                    break;
                
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }

        public static void DeleteWorkerFunc()
        {
            List <Worker> workers = workerService.getAllWorkers();
            foreach (var worker in workers)
            {
                worker.display();
            }
            Console.WriteLine("Enter worker ID you want to delete:");
            int workerId = int.Parse(Console.ReadLine());
            
            Console.WriteLine("Are you sure you want to delete this worker y/n ?");
            string deleteCh = Console.ReadLine();
            if (deleteCh == "y")
            {
                workerService.DeleteWorkerById(workerId);
                Console.WriteLine("Deleted successfully.");
            }
        }
        public static void ViewWorkerFunc()
        {
            List <Worker> workers = workerService.getAllWorkers();
            foreach (var worker in workers)
            {
                worker.display();
            }
        }

        //--------------------------------  Clients Functions  --------------------------------
        public static void MainClientMenu()
        {
            Client logClient = new Client();
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
                        
                        logClient = clientService.getClientByEmail(email);
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
                    Console.WriteLine("1. Edit my account\n" +
                                      "2. View all available tasks\n" +
                                      "3. Delete my account\n" +
                                      "4. Return to client Menu\n");
                    choice = int.Parse(Console.ReadLine());
                    switch (choice)
                    {
                        case 1:
                            editClientMenu(logClient);
                            break;
                        case 2:
                            TaskViewMenu(logClient);
                            break;
                        case 3:
                            DeleteClientMenu(logClient);
                            MainClientMenu();
                            break;
                        case 4:
                            MainClientMenu();
                            break;
                        default:
                            Console.WriteLine("Invalid choice.");
                            break;
                    }  
                }
            }
        }

        public static void TaskViewMenu(Client logClient)
        {
          
            Request request = new Request();
            List <Task> tasks = taskService.getallTasks();
            foreach (Task task in tasks)
            {
                task.display();
            }
            Console.WriteLine("Enter task ID you want to request:");
            int taskId = int.Parse(Console.ReadLine());
            Console.Write("Enter your preferred time slot (e.g., 'yyyy-MM-dd HH:mm'): ");
            string input = Console.ReadLine();
            if (!DateTime.TryParse(input, out DateTime prefTime))
            {
                Console.WriteLine("Invalid date-time format. Please try again.");
                return;
            }
            request.ClientId = logClient.Id;
            request.TaskId = taskId;
            request.RequestTime = DateTime.Now;
            request.PreferredTimeSlot = prefTime;
            requestService.InsertRequest(request);
            
            Task selectedTask = taskService.getTaskById(taskId);
            executeRequest(request.Id, logClient, selectedTask);
            
        }

        public static bool executeRequest(int requestId, Client client, Task selectedTask)
        {
            Worker bestWorker = null;
            List<Worker> workers = workerService.selectWorkerBySpecialty(selectedTask.RequiredSpecialty);
            if (workers.Count == 0)
            {
                Console.WriteLine("No workers available.");
            }
            else if (workers.Count == 1)
            {
                bestWorker = workers[0];
            }
            else
            {
                foreach (Worker worker in workers)
                {
                    foreach (var workerLocation in worker.Locations)
                    {
                        if (client.City == workerLocation.City)
                        {
                            bestWorker = worker;
                            break;
                        }
                    }

                    if (bestWorker != null)
                        break;
                }
            }
            RequestExecution exec = new RequestExecution
            {
                ActualTime = DateTime.Now,
                Status = "Pending",
                RequestId = requestId,
                WorkerId = bestWorker.Id,
            };
            requestExecService.AddRequestExecution(exec);
            Console.WriteLine("Request is placed successfully.");
            bestWorker.display();
            return true;
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

        public static void DeleteClientMenu(Client logClient)
        {
            Console.WriteLine("Are you sure you want to delete your account y/n ?");
            string deleteCh = Console.ReadLine();
            if (deleteCh == "y")
            {
                clientService.DeleteClientById(logClient.Id);
                Console.WriteLine("Deleted successfully.");
            }
        }
        public static void editClientMenu(Client logClient)
        {
            int id = logClient.Id;
            logClient.display();
            Console.WriteLine("\nSelect fields to update:\n" +
                              "1. Name\n" +
                              "2. Password\n" +
                              "3. Country\n" +
                              "4. City\n" +
                              "5. Phone number\n" +
                              "6. Return to client menu");
            Console.WriteLine("Enter field you want to update:");
            string updateChoice = Console.ReadLine();
            switch (updateChoice)
            {
                case "1":
                    Console.WriteLine("Enter the updated name: ");
                    string name = Console.ReadLine();
                    clientService.UpdateClientName(id, name);
                    Console.WriteLine("Updated successfully.");
                    break;
                
                case "2":
                    Console.WriteLine("Enter the updated Password: ");
                    string Password = Console.ReadLine();
                    clientService.UpdateClientCountry(id, Password);
                    Console.WriteLine("Updated successfully.");
                    break;

                case "3":
                    Console.WriteLine("Enter the updated country: ");
                    string country = Console.ReadLine();
                    clientService.UpdateClientCountry(id, country);
                    Console.WriteLine("Updated successfully.");
                    break;

                case "4":
                    Console.WriteLine("Enter the updated city: ");
                    string city = Console.ReadLine();
                    clientService.UpdateClientCity(id, city);
                    Console.WriteLine("Updated successfully.");
                    break;
                
                case "5":
                    Console.WriteLine("Enter another phone Number: ");
                    string phone = Console.ReadLine();
                    clientService.addClientPhone(id, phone);
                    Console.WriteLine("Updated successfully.");
                    break;
                
                case "6":
                    MainClientMenu();
                    break;
                default:
                    Console.WriteLine("Invaild choice ");
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
            
            clientService.addClient(client);
            //-------------------------------------------------------
            Console.Write("Phone Number: ");
            string phoneNumber = Console.ReadLine().Trim();
            if (!CheckInputs.IsValidPhoneNumber(phoneNumber))
            {
                Console.WriteLine("Invalid phoneNumber.");
                return;
            }
            //-------------------------------------------------------
            Console.Write("Add payment info? y/n");
            string paymentChoice = Console.ReadLine();
            ClientPaymentInfo clientPaymentInfo = new ClientPaymentInfo();
            if (paymentChoice.ToLower() == "y")
            {
                Console.Write("CardHolderName: ");
                clientPaymentInfo.CardHolderName = Console.ReadLine();
            
                Console.Write("CardNumber: ");
                clientPaymentInfo.CardNumber = Console.ReadLine();
            
                Console.Write("CVV: ");
                clientPaymentInfo.CVV = int.Parse(Console.ReadLine());
    
                Console.Write("ExpiryDate: ");
                string input = Console.ReadLine();
                if (!DateTime.TryParse(input, out DateTime expiryDate))
                {
                    Console.WriteLine("Invalid expiryDate.");
                }
                else
                {
                    clientPaymentInfo.ExpiryDate = expiryDate;
                }
                clientService.addClientPaymentInfo(client.Id,clientPaymentInfo);
            }
            try
            {
                clientService.addClientPhone(client.Id,phoneNumber);
                Console.WriteLine("\nRegistration successful!");
                Console.WriteLine($"Your client ID is: {client.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nRegistration failed: {ex.Message}");
            }
            MainClientMenu();
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
