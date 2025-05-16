using System;
using System.Data.SqlClient;

class Program
{
    static string connectionString = "Server=DESKTOP-KD10KEJ\\MSSQLSERVER1;Database=master;Trusted_Connection=True;";

    static void Main(string[] args)  
    {
        while (true)
        {
            Console.WriteLine("\n--- Request Table Menu ---");
            Console.WriteLine("1. Insert Request");
            Console.WriteLine("2. Delete Request");
            Console.WriteLine("3. Update Request");
            Console.WriteLine("4. Exit");

            Console.Write("Choose an option: ");
            string input = Console.ReadLine();

            switch (input)
            {
                case "1": InsertRequest(); break;
                case "2": DeleteRequest(); break;
                case "3": UpdateRequest(); break;
                case "4": return;
                default: Console.WriteLine("Invalid option."); break;
            }
        }
    }

    static void InsertRequest()
    {
        Console.Write("Enter Request Time (yyyy-mm-dd): ");
        string requestTime = Console.ReadLine();

        Console.Write("Enter Preferred Time Slot (yyyy-mm-dd): ");
        string timeSlot = Console.ReadLine();

        Console.Write("Enter Client ID: ");
        string clientId = Console.ReadLine();

        Console.Write("Enter Task ID: ");
        string taskId = Console.ReadLine();

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string sql = "INSERT INTO Request_ (RequestTime, PreferredTimeSlot, Client_ID, Task_ID) VALUES (@RequestTime, @TimeSlot, @ClientID, @TaskID)";
            SqlCommand cmd = new SqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@RequestTime", requestTime);
            cmd.Parameters.AddWithValue("@TimeSlot", timeSlot);
            cmd.Parameters.AddWithValue("@ClientID", clientId);
            cmd.Parameters.AddWithValue("@TaskID", taskId);

            conn.Open();
            int rows = cmd.ExecuteNonQuery();
            Console.WriteLine($"{rows} row(s) inserted.");
        }
    }

    static void DeleteRequest()
    {
        Console.Write("Enter Request ID to delete: ");
        string id = Console.ReadLine();

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string sql = "DELETE FROM Request_ WHERE ID = @ID";
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ID", id);

            conn.Open();
            int rows = cmd.ExecuteNonQuery();
            Console.WriteLine($"{rows} row(s) deleted.");
        }
    }

    static void UpdateRequest()
    {
        Console.Write("Enter Request ID to update: ");
        string id = Console.ReadLine();

        Console.Write("Enter new Preferred Time Slot (yyyy-mm-dd): ");
        string newTimeSlot = Console.ReadLine();

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string sql = "UPDATE Request_ SET PreferredTimeSlot = @TimeSlot WHERE ID = @ID";
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@TimeSlot", newTimeSlot);
            cmd.Parameters.AddWithValue("@ID", id);

            conn.Open();
            int rows = cmd.ExecuteNonQuery();
            Console.WriteLine($"{rows} row(s) updated.");
        }
    }
}
