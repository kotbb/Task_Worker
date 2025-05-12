using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskWorker.Models
{
    internal class Perform
    {
        public int WorkerId { get; set; }
        public int TaskId { get; set; }

        public Worker Worker { get; set; }
        public Task Task { get; set; }
    }
}
