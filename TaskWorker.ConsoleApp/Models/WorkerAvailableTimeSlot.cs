using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskWorker.Models
{
    internal class WorkerAvailableTimeSlot
    {
        public DateTime Slot { get; set; }
        public int WorkerId { get; set; }
        public Worker Worker { get; set; }
    }
}
