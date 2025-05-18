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
    }
}

