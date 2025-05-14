using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskWorker.Models
{
    internal class WorkerLocation
    {
        public string City { get; set; }
        public string Street { get; set; }
        public string Country { get; set; }
        public int WorkerId { get; set; }
    }
}
