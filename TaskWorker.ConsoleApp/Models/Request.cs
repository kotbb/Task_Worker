using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskWorker.Models
{
    internal class Request
    {
        public int Id { get; set; }
        public DateTime RequestTime { get; set; }
        public DateTime PreferredTimeSlot { get; set; }

        // Foreign Keys
        public int ClientId { get; set; }
        public int TaskId { get; set; }

        // Relationships
        public List<RequestExecution> RequestExecutions { get; set; } = new();

        public void display()
        {
            Console.WriteLine("=========== Request Details ===========");
            Console.WriteLine($"Request ID         : {Id}");
            Console.WriteLine($"Request Time       : {RequestTime}");
            Console.WriteLine($"Preferred TimeSlot : {PreferredTimeSlot}");
            Console.WriteLine($"Client ID          : {ClientId}");
            Console.WriteLine($"Task ID            : {TaskId}");
            Console.WriteLine("------ Executions for this Request ------");
        }
    }
}

