using System;
using System.Linq;
using System.Data.SqlClient;
using TaskWorker.Models;
using TaskWorker.Services;
namespace TaskWorker.ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Server=KOTB;Database=Task_Worker;Trusted_Connection=true;";
            
            int choice = mainMenu();
            switch (choice)
            {
                case 1:
                    workerMenu(connectionString);
                    break;

                case 2:
                    clientMenu(connectionString);
                    break;

                default:
                    break;
            }
        }
        public static int mainMenu()
        {
            while (true)
            {
                Console.WriteLine("===========  Task Worker Management  ===========");
                Console.WriteLine("=====  Main Menu  =====");
                Console.Write("1. Worker\n" + "2. Client\n");

                string input = Console.ReadLine();

                if (input == null || !input.All(char.IsDigit))
                {
                    Console.WriteLine("Invalid input.");
                }
                int choice = int.Parse(input);
                switch (choice)
                {
                    case 1:
                        Console.WriteLine("Worker selected.");
                        return choice;
                    case 2:
                        Console.WriteLine("Client selected.");
                        return choice;
                    default:
                        Console.WriteLine("Invalid choice. Please enter 1 or 2.");
                        break;
                }
            }
        }
        public static void workerMenu(string connectionString)
        {

        }
        public static void clientMenu(string connectionString)
        {
            var clientService = new ClientService(connectionString);
            var clients = clientService.GetAllClients();
            foreach (var client in clients) { 
                client.Print();
            }
        }
    }
    
}
