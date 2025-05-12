using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskWorker.Models
{
    internal class Task
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string RequiredSpecialty { get; set; }

        // Relationships
        public List<Request> Requests { get; set; } = new();
        public HashSet<Perform> Performs { get; set; } = new();
    }
}
