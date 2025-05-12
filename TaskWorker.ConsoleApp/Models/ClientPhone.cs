using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskWorker.Models
{
    internal class ClientPhone
    {
        public int CountryCode { get; set; }
        public string LocalNumber { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; }
    }
}
