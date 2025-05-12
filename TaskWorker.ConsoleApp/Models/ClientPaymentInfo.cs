using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskWorker.Models
{
    internal class ClientPaymentInfo
    {
        public string CardHolderName { get; set; }
        public long CardNumber { get; set; }
        public int CVV { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; }
    }
}
