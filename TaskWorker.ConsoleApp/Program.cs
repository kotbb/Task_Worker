using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using TaskWorker.Models;
using Task = TaskWorker.Models.Task;
using TaskWorker.Services;
using TaskWorker.Validation;
using System.Data;

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

        public static void report()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                Console.WriteLine("--- Matching Workers for Request ID 1 ---");
                ExecuteQuery(conn, @"
                    SELECT DISTINCT W.ID, W.Name
                    FROM Request_ R
                    JOIN Task_ T ON R.Task_ID = T.ID
                    JOIN Worker_Specialties WS ON WS.Specialties = T.RequiredSpecialty
                    JOIN Worker_AvailableTimeSlots TS ON CAST(TS.AvailableTimeSlots AS DATE) = R.PreferredTimeSlot
                    JOIN Worker_Locations WL ON WL.City = (SELECT City FROM Client_ WHERE ID = R.Client_ID)
                    JOIN Worker_ W ON W.ID = WS.Worker_ID
                    WHERE R.ID = 1
                ");

                Console.WriteLine("--- Total Due Wage per Worker (May 2025) ---");
                ExecuteQuery(conn, @"
                    SELECT W.ID, W.Name,
                           SUM(T.AverageTaskFee * (RE.ClientRating / 5.0)) AS TotalWage
                    FROM Worker_ W
                    JOIN RequestExecution RE ON W.ID = RE.Worker_ID
                    JOIN Request_ R ON RE.Request_ID = R.ID
                    JOIN Task_ T ON R.Task_ID = T.ID
                    WHERE R.RequestTime BETWEEN '2025-05-01' AND '2025-05-31'
                      AND RE.RequestStatus = 'Completed'
                    GROUP BY W.ID, W.Name
                ");

                Console.WriteLine("--- Most Requested Task ---");
                ExecuteQuery(conn, @"
                    SELECT TOP 1 T.Name, COUNT(*) AS RequestCount
                    FROM Request_ R
                    JOIN Task_ T ON R.Task_ID = T.ID
                    GROUP BY T.Name
                    ORDER BY RequestCount DESC
                ");

                Console.WriteLine("--- Best Worker per Specialty (May 2025) ---");
                ExecuteQuery(conn, @"
                    SELECT T.RequiredSpecialty, W.Name, AVG(RE.ClientRating) AS AvgRating
                    FROM RequestExecution RE
                    JOIN Worker_ W ON RE.Worker_ID = W.ID
                    JOIN Request_ R ON RE.Request_ID = R.ID
                    JOIN Task_ T ON R.Task_ID = T.ID
                    WHERE R.RequestTime BETWEEN '2025-05-01' AND '2025-05-31'
                    GROUP BY T.RequiredSpecialty, W.Name
                    HAVING AVG(RE.ClientRating) = (
                      SELECT MAX(AvgR) FROM (
                        SELECT AVG(RE2.ClientRating) AS AvgR
                        FROM RequestExecution RE2
                        JOIN Request_ R2 ON RE2.Request_ID = R2.ID
                        JOIN Task_ T2 ON R2.Task_ID = T2.ID
                        WHERE R2.RequestTime BETWEEN '2025-05-01' AND '2025-05-31'
                          AND T2.RequiredSpecialty = T.RequiredSpecialty
                        GROUP BY RE2.Worker_ID
                      ) AS Ratings
                    )
                ");

                Console.WriteLine("--- Specialties with No Requests in May 2025 ---");
                ExecuteQuery(conn, @"
                    SELECT DISTINCT T.RequiredSpecialty
                    FROM Task_ T
                    WHERE T.ID NOT IN (
                      SELECT Task_ID FROM Request_
                      WHERE RequestTime BETWEEN '2025-05-01' AND '2025-05-31'
                    )
                ");

                Console.WriteLine("--- Workers With All Ratings >= 4.5 ---");
                ExecuteQuery(conn, @"
                    SELECT W.ID, W.Name
                    FROM Worker_ W
                    WHERE NOT EXISTS (
                      SELECT 1 FROM RequestExecution RE
                      WHERE RE.Worker_ID = W.ID AND RE.ClientRating < 4.5
                    )
                ");
            }
        }

        static void ExecuteQuery(SqlConnection conn, string query)
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                DataTable schemaTable = reader.GetSchemaTable();
                foreach (DataRow col in schemaTable.Rows)
                {
                    Console.Write("{0}\t", col["ColumnName"]);
                }
                Console.WriteLine("\n--------------------------------------------------");

                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Console.Write("{0}\t", reader[i]);
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }
        }
        //--------------------------------  Menu Functions  --------------------------------
        public static void mainMenu()
        {
            while (true)
            {
                Console.WriteLine("===========  Task Worker Management  ===========");
                Console.WriteLine("=====  Main Menu  =====");
                Console.Write("1. Admin\n"+ "2. Client\n" +"3. Report\n" +"4. Exit\n");
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
                        report();
                        break;
                    case 4:
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
                int adminChoice = int.Parse(Console.ReadLine());
                switch (adminChoice)
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
                        Console.WriteLine("Choose:\n" +
                                          "1. View\n" +
                                          "2. Delete\n" +
                                          "3. Return to admin Menu\n");
                        int clientChoice = int.Parse(Console.ReadLine());
                        switch (clientChoice)
                        {
                            case 1:
                                ViewClientFunc();
                                break;
                            case 2:
                                DeleteClientFunc();
                                break;
                            case 3:
                                mainMenu();
                                break;
                            default:
                                Console.WriteLine("Invalid choice.");
                                break;
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
        //--------------------------------  Task Functions  --------------------------------
        public static void MainTaskMenu()
        {
            Console.WriteLine("=====  Task Menu  =====");
            Console.WriteLine("1. Add\n" + 
                              "2. View\n" +
                              "3. Delete\n" +
                              "4. Return to admin Menu\n");
            int choice = int.Parse(Console.ReadLine());
            switch (choice)
            {
                case 1:
                    AddTaskFunc();
                    break;
                case 2:
                    ViewTaskFunc();
                    break;
                case 3:
                    DeleteTaskFunc();
                    break;
                case 4:
                    adminMenu();
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }
        public static void AddTaskFunc()
        {
            Task newTask = new Task();
            Console.Write("Task Name: ");
            newTask.Name = Console.ReadLine();
            
            Console.Write("Required Speciality: ");
            newTask.RequiredSpecialty = Console.ReadLine();
            
            Console.Write("AverageTimeNeeded in minutes: ");
            newTask.AverageTimeNeeded = int.Parse(Console.ReadLine());
            
            Console.Write("AverageTaskFee: ");
            newTask.AverageTaskFee = int.Parse(Console.ReadLine());
            
            try
            {
                taskService.addTask(newTask);
                Console.WriteLine("\nAdd successful!");
                Console.WriteLine($"Task ID is: {newTask.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n Add failed: {ex.Message}");
                MainTaskMenu();
            }
            MainTaskMenu();
        }
        public static void ViewTaskFunc()
        {
            Console.WriteLine("View:");
            Console.WriteLine("1. All\n" + 
                              "2. Id\n" +
                              "3. Required Speciality\n" +
                              "4. Return to task Menu\n");
            int choice = int.Parse(Console.ReadLine());
            switch (choice)
            {
                case 1:
                    List<Task> tasks = taskService.getallTasks();
                    if (tasks.Count == 0)
                    {
                        Console.WriteLine("There are no tasks to delete.");
                    }
                    foreach (Task task in tasks)
                    {
                        task.display();
                    }
                    break;
                case 2:
                    Console.WriteLine("Enter the task id you want to view:");
                    int taskId = int.Parse(Console.ReadLine());
                    try{
                        Task taskWithId = taskService.getTaskById(taskId);
                        taskWithId.display();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error getting task by id: {ex.Message}");
                    }
                    break;
                case 3:
                    Console.WriteLine("Enter the required speciality you want to view:");
                    string speciality = Console.ReadLine();
                    try{
                        Task taskBySpecialty = taskService.getTaskBySpecialty(speciality);
                        taskBySpecialty.display();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error getting task by specialty: {ex.Message}");
                    }
                    break;
                case 4:
                    MainTaskMenu();
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
            MainTaskMenu();

        }
        public static void DeleteTaskFunc()
        {
            List <Task> tasks = taskService.getallTasks();
            foreach (var task in tasks)
            {
                task.display();
            }
            if (tasks.Count == 0)
            {
                Console.WriteLine("There are no tasks to delete.");
                MainTaskMenu();
            }
            Console.WriteLine("Enter task ID you want to delete:");
            int taskId = int.Parse(Console.ReadLine());
            
            Console.WriteLine("Are you sure you want to delete this task y/n ?");
            string deleteCh = Console.ReadLine();
            if (deleteCh == "y")
            {
                try{
                    taskService.deleteTaskById(taskId);
                    Console.WriteLine("Deleted successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting task: {ex.Message}");
                }
            }
            MainTaskMenu();
        }
        //--------------------------------  Request Functions  --------------------------------
        public static void MainRequestMenu()
        {
            while (true)
            {
                Console.WriteLine("=====  Request Menu  =====");
                Console.WriteLine("1. View\n" +
                                  "2. Delete\n" +
                                  "3. Edit\n" +
                                  "4. Return to admin Menu\n");
                int requestChoice = int.Parse(Console.ReadLine());
                switch (requestChoice)
                {
                    case 1:
                        ViewRequestFunc();
                        break;
                    case 2:
                        DeleteRequestFunc();
                        break;
                    case 3:
                        EditRequestFunc();
                        break;
                    case 4:
                        adminMenu();
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }
        public static void ViewRequestFunc()
        {
            Console.WriteLine("Choose:\n" +
                              "1. View Pending Requests\n" +
                              "2. View Completed Requests\n" +
                              "3. View Specific Request\n" +
                              "4. Return to Request Menu\n");
            int viewChoice = int.Parse(Console.ReadLine());
            switch (viewChoice)
            {
                case 1:
                    List<RequestExecution> pendingRequests = requestExecService.GetPendingRequestExecutions();
                    foreach (RequestExecution requestExec in pendingRequests)
                    {
                        requestExec.display();
                    }
                    if(pendingRequests.Count == 0)
                    {
                        Console.WriteLine("There are no pending requests.");
                    }
                    break;
                case 2:
                    List<RequestExecution> completedRequests = requestExecService.GetCompletedRequestExecutions();
                    foreach (RequestExecution requestExec in completedRequests)
                    {
                        requestExec.display();
                    }
                    if(completedRequests.Count == 0)
                    {
                        Console.WriteLine("There are no completed requests.");
                    }
                    break;
                case 3:
                    Console.WriteLine("Enter Worker ID:");
                    int workerId = int.Parse(Console.ReadLine());
                    Console.WriteLine("Enter Request ID:");
                    int requestId = int.Parse(Console.ReadLine());
                    var requestExecution = requestExecService.GetRequestExecution(workerId,requestId);
                    if (requestExecution != null)
                    {
                        requestExecution.display();
                    }
                    else{
                        Console.WriteLine("RequestExecution not found.");
                    }
                    break;
                case 4:
                    MainRequestMenu();
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }
        public static void DeleteRequestFunc()
        {
            List <RequestExecution> requests = requestExecService.GetAllRequestExecutions();
            foreach (RequestExecution req in requests)
            {
                req.display();
            }
            Console.WriteLine("Enter Request ID:");
            int requestId = int.Parse(Console.ReadLine());
            
            Console.WriteLine("Enter Worker ID:");
            int workerId = int.Parse(Console.ReadLine());
            
            Console.WriteLine("Are you sure you want to delete this request y/n ?");
            string deleteCh = Console.ReadLine();
            if (deleteCh == "y")
            {
                if(requestExecService.DeleteRequestExecution(workerId,requestId))
                    Console.WriteLine("Deleted successfully.");
                else
                    Console.WriteLine(("Delete failed."));
            }
            MainRequestMenu();
        }
        public static void EditRequestFunc()
        {
            List<RequestExecution> requests = requestExecService.GetAllRequestExecutions();
            foreach (RequestExecution req in requests)
            {
                req.display();
            }
            if(requests.Count == 0)
            {
                Console.WriteLine("There are no requests to edit.");    
                MainRequestMenu();
            }
               
            Console.WriteLine("Enter the request ID:");
            int requestId = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter the worker ID:");
            int workerId = int.Parse(Console.ReadLine());
            RequestExecution requestExec;
            try{        
                requestExec = requestExecService.GetRequestExecution(workerId, requestId);
                if (requestExec == null)
                {
                    Console.WriteLine("RequestExecution not found.");
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting request execution: {ex.Message}");
                return;
            }

            requestExec.display(); 

            Console.Write("Enter worker rating to the client (0.0 - 5.0): ");
            if(!CheckInputs.IsValidRating(Console.ReadLine()))
            {
                Console.WriteLine("Invalid rating.");
                return;
            }
            requestExec.ClientRating = decimal.Parse(Console.ReadLine());

            Console.Write("Enter worker feedback to the client: ");
            requestExec.WorkerFeedback = Console.ReadLine();

            Console.Write("Enter the new Status (Pending / Completed / Failed): ");
            string statusInput = Console.ReadLine();
            if(!CheckInputs.IsValidStatus(statusInput))
            {
                Console.WriteLine("Invalid status entered.");
                return;
            }
            requestExec.Status = statusInput;
            var request = requestService.GetRequestById(requestId);
            if (request == null)
            {
                Console.WriteLine("Error: Request not found.");
                return;
            }
            int clientId = request.ClientId;
            try{
                bool updated = requestExecService.UpdateRequestExecution(requestExec);
                if (updated)
                {
                    clientService.CalculateClientOverallRating(clientId);
                    Console.WriteLine("Request updated successfully.");
                }
                else
                {
                    Console.WriteLine("Update failed.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating request execution: {ex.Message}");
            }
        }
        
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
                                  "5. Return to admin Menu"
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
                        adminMenu();
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
            if (!CheckInputs.IsValidTimeSlot(input))
            {
                Console.WriteLine("Invalid date-time format. Please try again.");
                return;
            }
            DateTime slotTime = DateTime.Parse(input);
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
        
                Console.WriteLine("\nAdd successfully!");
                Console.WriteLine($"Your Worker ID is: {newworker.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nAdd failed: {ex.Message}");
                MainWorkerMenu();
            }
            MainWorkerMenu();
        }

        public static void UpdateWorkerFunc()
        {
            List <Worker> workers = workerService.getAllWorkers();
            foreach (var w in workers)
            {
                w.display();
            }
            if (workers.Count == 0)
            {
                Console.WriteLine("No workers found.");
                return;
            }
            Console.WriteLine("Enter Worker ID you want to update:");
            int workerId = int.Parse(Console.ReadLine());
            Worker worker = workerService.selectWorkerById(workerId);
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
                    worker.display();
                    Console.WriteLine("Enter new Name:");
                    string name = Console.ReadLine();
                    try
                    {
                        workerService.UpdateWorkerName(workerId, name);
                        Console.WriteLine("Name successfully updated!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error updating worker name: {ex.Message}");
                    }
                    break;
                
                case 2:
                    worker.display();
                    Console.WriteLine("if you want to delete the current location y/n");
                    string deleteLocationChoice = Console.ReadLine();
                    if (deleteLocationChoice.ToLower() == "y")
                    {
                        Console.WriteLine("Enter city:");
                        string city = Console.ReadLine();
                        Console.WriteLine("Enter street:");
                        string street = Console.ReadLine();
                        Console.WriteLine("Enter country:");
                        string country = Console.ReadLine();
                        workerService.DeleteWorkerLocation(workerId, city, street, country);
                        worker.Locations = workerService.GetWorkerLocations(workerId);
                    }
                    WorkerLocation workerLocation = new WorkerLocation();
                    Console.WriteLine("Enter new Country:");
                    workerLocation.Country = Console.ReadLine();
                    Console.WriteLine("Enter new City:");
                    workerLocation.City = Console.ReadLine();
                    Console.WriteLine("Enter new Street:");
                    workerLocation.Street = Console.ReadLine();
                    try
                    {
                        workerService.UpdateWorkerLocations(workerId, workerLocation);
                        worker.Locations = workerService.GetWorkerLocations(workerId);
                        Console.WriteLine("Worker Location successfully updated!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error updating worker location: {ex.Message}");
                    }
                    break;
                
                case 3:
                    worker.display();
                    Console.WriteLine("if you want to delete the current speciality y/n");
                    string deleteSpecialtyChoice = Console.ReadLine();
                    if (deleteSpecialtyChoice.ToLower() == "y")
                    {
                        Console.WriteLine("Enter specialty to delete:");
                        string specialty = Console.ReadLine();
                        workerService.DeleteWorkerSpecialty(workerId, specialty);
                        worker.Specialties = workerService.GetWorkerSpecialties(workerId).ToHashSet();
                    }
                    WorkerSpecialty workerSpecialty = new WorkerSpecialty();
                    Console.WriteLine("Enter new Speciality:");
                    workerSpecialty.Specialty = Console.ReadLine();
                    try
                    {
                        workerService.UpdateWorkerSpecialties(workerId, workerSpecialty);
                        worker.Specialties = workerService.GetWorkerSpecialties(workerId).ToHashSet();
                        Console.WriteLine("Worker Speciality successfully updated!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error updating worker speciality: {ex.Message}");
                    }
                    break;
                
                case 4:
                    worker.display();
                    Console.WriteLine("if you want to delete the current available slots y/n");
                    string deleteTimeSlotChoice = Console.ReadLine();
                    if (deleteTimeSlotChoice.ToLower() == "y")
                    {
                        Console.WriteLine("Enter time slot to delete (yyyy-MM-dd HH:mm):");
                        string timeSlotStr = Console.ReadLine();
                        if(!CheckInputs.IsValidTimeSlot(timeSlotStr))
                        {
                            Console.WriteLine("Invalid date-time format. Please try again.");
                            return;
                        }
                        DateTime timeSlot = DateTime.Parse(timeSlotStr);
                        workerService.DeleteWorkerAvailableTimeSlot(workerId, timeSlot);
                        worker.AvailableTimeSlots = workerService.GetWorkerTimeSlots(workerId);
                    }
                    WorkerAvailableTimeSlot workerSlot = new WorkerAvailableTimeSlot();
                    Console.WriteLine("Enter new Slot:");
                    string input = Console.ReadLine();
                    if(!CheckInputs.IsValidTimeSlot(input))
                    {
                        Console.WriteLine("Invalid date-time format. Please try again.");
                        return;
                    }

                    DateTime slotTime = DateTime.Parse(input);
                    workerSlot.Slot = slotTime;
                    try
                    {
                        workerService.UpdateWorkerTimeSlots(workerId, workerSlot);
                        worker.AvailableTimeSlots = workerService.GetWorkerTimeSlots(workerId);
                        Console.WriteLine("Worker slotTime successfully updated!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error updating worker time slot: {ex.Message}");
                    }
                    break;
                
                case 5:
                    MainWorkerMenu();
                    break;
                
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
            MainWorkerMenu();
        }

        public static void DeleteWorkerFunc()
        {
            List <Worker> workers = workerService.getAllWorkers();
            if (workers.Count == 0)
            {
                Console.WriteLine("No workers found.");
                return;
            }
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
                try
                {
                    workerService.DeleteWorkerById(workerId);
                    Console.WriteLine("Deleted successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting worker: {ex.Message}");
                }
            }
            MainWorkerMenu();
        }
        public static void ViewWorkerFunc()
        {
             Console.WriteLine("Choose:\n" +
                              "1. View all workers\n" +
                              "2. View worker by ID\n" +
                              "3. View worker by specialty\n" +
                              "4. Return to admin menu");
            int choice = int.Parse(Console.ReadLine());
            switch (choice)
            {
                case 1:
                    List <Worker> workers = workerService.getAllWorkers();
                    if (workers.Count == 0)
                    {
                        Console.WriteLine("No workers found.");
                    }
                    foreach (var w in workers)
                    {
                        w.display();
                    }
                    break;
                case 2:
                    Console.WriteLine("Enter worker ID:");
                    int workerId = int.Parse(Console.ReadLine());
                    try{
                        Worker worker = workerService.selectWorkerById(workerId);
                        worker.display();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error selecting worker by ID");
                    }
                    break;
                case 3:
                    Console.WriteLine("Enter specialty:");
                    string specialty = Console.ReadLine();
                    try{
                        List <Worker> workersBySpecialty = workerService.selectWorkerBySpecialty(specialty);
                        if (workersBySpecialty.Count == 0)
                        {
                            Console.WriteLine("No workers found.");
                        }
                        foreach (var w in workersBySpecialty)
                        {
                                w.display();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error selecting workers by specialty:");
                    }
                    break;
                case 4:
                    MainWorkerMenu();
                    break;
                default:
                    Console.WriteLine("Invalid choice.");   
                    break;
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
                        try
                        {
                            logClient = clientService.getClientByEmail(email);
                            if (logClient == null)
                            {
                                Console.WriteLine("Error: Invalid email address. Please try again.");
                                continue;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                            continue;
                        }
                        if (logClient.Password != pass || logClient.Email != email)
                        {
                            Console.WriteLine("Login failed , Invalid data.");
                            continue;
                        }
                        Console.WriteLine("Login Successful.");
                        loginClientMenu(logClient);
                        break;
                    case 3:
                        mainMenu();
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        public static void loginClientMenu(Client logClient)
        {
            while (true)
            {
                Console.WriteLine("=====  Client Menu  =====");
                Console.WriteLine("1. Edit my account\n" +
                                  "2. View all available tasks\n" +
                                  "3. View all completed requests\n" +
                                  "4. Delete my account\n" +
                                  "5. Return to client Menu");
                int choice = int.Parse(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        editClientMenu(logClient);
                        break;
                    case 2:
                        TaskViewMenu(logClient);
                        break;
                    case 3:
                        List <RequestExecution> completedRequests = requestExecService.GetCompletedRequestExecutions();
                        foreach (RequestExecution exec in completedRequests)
                        {
                            exec.display();
                        }
                        if (completedRequests.Count > 0)
                        {
                            Console.WriteLine("If you want to edit request? y/n:");
                            string ch = Console.ReadLine();
                            if (ch.ToLower() == "y")
                            {
                                Console.WriteLine("Enter the request ID:");
                                int requestId = int.Parse(Console.ReadLine());

                                Console.WriteLine("Enter the worker ID:");
                                int workerId = int.Parse(Console.ReadLine());

                                RequestExecution requestExec = requestExecService.GetRequestExecution(workerId, requestId);

                                if (requestExec == null)
                                {
                                    Console.WriteLine("RequestExecution not found.");
                                    return;
                                }
                                requestExec.display();

                                Console.Write("Enter rating to the worker (0.0 - 5.0): ");
                                if(!CheckInputs.IsValidRating(Console.ReadLine()))
                                {
                                    Console.WriteLine("Invalid rating.");
                                    return;
                                }
                                requestExec.WorkerRating = decimal.Parse(Console.ReadLine());

                                Console.Write("Enter feedback about the worker: ");
                                requestExec.ClientFeedback = Console.ReadLine();
                                bool updated = requestExecService.UpdateRequestExecution(requestExec);
                                if (updated)
                                {
                                    workerService.CalculateOverallRating(requestExec.WorkerId);
                                    Console.WriteLine("Request updated successfully.");
                                }
                                else
                                {
                                    Console.WriteLine("Update failed.");
                                }
                            }  
                        }
                        else
                        {
                            Console.WriteLine("No requests found.");
                        }
                        break;
                    case 4:
                        if(DeleteMyAccount(logClient))
                        {
                            MainClientMenu();
                        }
                        break;
                    case 5:
                        MainClientMenu();
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
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
            if (tasks.Count == 0)
            {
                Console.WriteLine("No tasks found.");
                return;
            }
            Console.WriteLine("If you want to request task? y/n:");
            string ch = Console.ReadLine();
            if (ch.ToLower() == "y")
            {
                Console.WriteLine("Enter task ID you want to request:");
                int taskId = int.Parse(Console.ReadLine());
                Console.Write("Enter your preferred time slot (e.g., 'yyyy-MM-dd HH:mm'): ");
                string input = Console.ReadLine();
                if(!CheckInputs.IsValidTimeSlot(input))
                {
                    Console.WriteLine("Invalid date-time format. Please try again.");
                    return;
                }
                DateTime prefTime = DateTime.Parse(input);
                request.ClientId = logClient.Id;
                request.TaskId = taskId;
                request.RequestTime = DateTime.Now;
                request.PreferredTimeSlot = prefTime;
                requestService.InsertRequest(request);
                try
                {
                    Task selectedTask = taskService.getTaskById(taskId);
                    executeRequest(request.Id, logClient, selectedTask);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error executing request: {ex.Message}");
                }
            }
        }

        public static void executeRequest(int requestId, Client client, Task selectedTask)
        {
            Worker bestWorker = null;
            
            List<Worker> workers = workerService.selectWorkerBySpecialty(selectedTask.RequiredSpecialty);
            if (workers.Count == 0)
            {
                Console.WriteLine("No workers available for this task.");
                return;
            }
            bestWorker = workers[0];             
            if (workers.Count > 1)
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
                }
            }
            if(!bestWorker.isPerforming(selectedTask.Id))
            {
                workerService.AddWorkerPerform(bestWorker.Id, selectedTask.Id);
            }
            RequestExecution exec = new RequestExecution
            {
                ActualTime = DateTime.Now,
                Status = "Pending",
                RequestId = requestId,
                WorkerId = bestWorker.Id,
            };
            try
            {
                requestExecService.AddRequestExecution(exec);
                Console.WriteLine("Request is placed successfully.");
                Console.WriteLine("Your Worker details:");
                bestWorker.display();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing request: {ex.Message}");
            }
        }
        
        public static bool DeleteMyAccount(Client logClient)
        {
            Console.WriteLine("Are you sure you want to delete your account y/n ?");
            string deleteCh = Console.ReadLine();
            if (deleteCh == "y")
            {
                try
                {
                    clientService.DeleteClientById(logClient.Id);
                    Console.WriteLine("Deleted successfully.");
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting client: {ex.Message}");
                }
            }
            return false;
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
                    try
                    {
                        clientService.UpdateClientName(id, name);
                        Console.WriteLine("Updated successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error updating client name: {ex.Message}");
                    }
                    break;
                
                case "2":
                    Console.WriteLine("Enter the updated Password: ");
                    string Password = Console.ReadLine();
                    try
                    {
                        clientService.UpdateClientPassword(id, Password);
                        Console.WriteLine("Updated successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error updating client password: {ex.Message}");
                    }
                    break;

                case "3":
                    Console.WriteLine("Enter the updated country: ");
                    string country = Console.ReadLine();
                    try
                    {
                        clientService.UpdateClientCountry(id, country);
                        Console.WriteLine("Updated successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error updating client country: {ex.Message}");
                    }
                    break;

                case "4":
                    Console.WriteLine("Enter the updated city: ");
                    string city = Console.ReadLine();
                    try
                    {
                        clientService.UpdateClientCity(id, city);
                        Console.WriteLine("Updated successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error updating client city: {ex.Message}");
                    }
                    break;
                
                case "5":
                    Console.WriteLine("if you want to delete current phone Number y/n");
                    string deleteCh = Console.ReadLine();
                    if (deleteCh.ToLower() == "y")
                    {
                        Console.WriteLine("Enter the phone Number you want to delete.");
                        string phoneNumber = Console.ReadLine();
                        if (clientService.DeleteClientPhone(id, phoneNumber))
                        {
                            logClient.Phones = clientService.getClientPhones(id).ToHashSet();
                            Console.WriteLine("Deleted successfully.");
                        }
                    }
                    Console.WriteLine("Enter a new phone Number: ");
                    string phone = Console.ReadLine();
                    try
                    {
                        if(clientService.addClientPhone(id, phone))
                        {
                            logClient.Phones = clientService.getClientPhones(id).ToHashSet();
                            Console.WriteLine("Updated successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Update failed.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error updating client phone: {ex.Message}");
                    }
                    break;
                case "6":
                    loginClientMenu(logClient);
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

            try
            {
                clientService.addClient(client);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error adding client: {e.Message}");
                MainClientMenu();
            }
            //-------------------------------------------------------
            Console.Write("Phone Number: ");
            string phoneNumber = Console.ReadLine().Trim();
            if (!CheckInputs.IsValidPhoneNumber(phoneNumber))
            {
                Console.WriteLine("Invalid phoneNumber.");
                return;
            }
            //-------------------------------------------------------
            Console.WriteLine("Add payment info? y/n");
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
                try
                {
                    clientService.addClientPaymentInfo(client.Id,clientPaymentInfo);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    MainClientMenu();
                }
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
                MainClientMenu();
            }
            MainClientMenu();
        }

        public static void ViewClientFunc()
        {
            var clients = clientService.getAllClients();
            if (clients.Count == 0)
            {
                Console.WriteLine("No clients found.");
                return;
            }

            Console.WriteLine("\nAll Clients:");
            foreach (var client in clients)
            {
                client.display();
            }
        }

        public static void DeleteClientFunc()
        {
            List<Client> clients = clientService.getAllClients();
            foreach (var client in clients)
            {
                client.display();
            }
            if(clients.Count == 0) 
            {
                Console.WriteLine("There are no clients to delete.");
                MainClientMenu();
            }
            
            Console.WriteLine("Enter client ID to delete:");
            int clientId = int.Parse(Console.ReadLine());
            Console.WriteLine("Are you sure you want to delete this client y/n ?");
            string deleteCh = Console.ReadLine();
            if (deleteCh == "y")
            {
                if (clientService.DeleteClientById(clientId))
                {
                    Console.WriteLine($"Client with ID {clientId} deleted successfully.");
                }
                else
                {
                    Console.WriteLine($"Failed to delete client with ID {clientId}.");
                }
            }
        }
    }
}
