using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskWorker.Models
{
    internal class RequestExecution
    {
        public DateTime ActualTime { get; set; }
        public decimal WorkerRating { get; set; }  // 0.0 to 5.0
        public decimal ClientRating { get; set; }  // 0.0 to 5.0
        public string Status { get; set; }         // "Pending", "Completed", "Failed"
        public string ClientFeedback { get; set; }
        public string WorkerFeedback { get; set; }

        // Foreign Keys
        public int WorkerId { get; set; }
        public int RequestId { get; set; }
        
        public void display()
        {
            Console.WriteLine("----- Request Execution Details -----");
            Console.WriteLine($"Actual Time                : {ActualTime}");
            Console.WriteLine($"Client Rating to Worker    : {WorkerRating}/5.0");
            Console.WriteLine($"Worker Rating to Client    : {ClientRating}/5.0");
            Console.WriteLine($"Status                     : {Status}");
            Console.WriteLine($"Client Feedback            : {ClientFeedback}");
            Console.WriteLine($"Worker Feedback            : {WorkerFeedback}");
            Console.WriteLine($"Worker ID                  : {WorkerId}");
            Console.WriteLine($"Request ID                 : {RequestId}");
            Console.WriteLine("-------------------------------------");
        }
        
    }
}
