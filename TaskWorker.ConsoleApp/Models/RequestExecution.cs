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
        public decimal WorkerRating { get; set; }  // 1.0 to 5.0
        public decimal ClientRating { get; set; }  // 1.0 to 5.0
        public string Status { get; set; }         // "Pending", "Completed", "Failed"
        public string ClientFeedback { get; set; }
        public string WorkerFeedback { get; set; }

        // Foreign Keys
        public int WorkerId { get; set; }
        public int RequestId { get; set; }

        // Relationships
        public Worker Worker { get; set; }
        public Request Request { get; set; }
    }
}
