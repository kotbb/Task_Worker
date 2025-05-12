using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskWorker.Models
{
    internal class Worker
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Relationships
        public HashSet<WorkerSpecialty> Specialties { get; set; } = new();
        public List<WorkerAvailableTimeSlot> AvailableTimeSlots { get; set; } = new();
        public List<WorkerLocation> Locations { get; set; } = new();
        public HashSet<Perform> Performs { get; set; } = new();
        public List<RequestExecution> RequestExecutions { get; set; } = new();
    }
}
