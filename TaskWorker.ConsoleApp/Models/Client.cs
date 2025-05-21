using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace TaskWorker.Models
{   
    internal class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        
        public string Password { get; set; }
        public string City { get; set; }
        public string StreetName { get; set; }
        public string Country { get; set; }
        public int StreetNumber { get; set; }
        public int ApartmentNumber { get; set; }
        
        public decimal overallRating { get; set; }
        
        // Relationships
        public HashSet<ClientPhone> Phones { get; set; } = new();
        public List<ClientPaymentInfo> PaymentInfos { get; set; } = new();
        public List<Request> Requests { get; set; } = new();
        
        public void display()
        {
            Console.WriteLine("---------------------");
            Console.WriteLine($"ID: {this.Id}");
            Console.WriteLine($"Name: {this.Name}");
            Console.WriteLine($"Email: {this.Email}");
            Console.WriteLine($"Country: {this.Country}");
            Console.WriteLine($"City: {this.City}");
            Console.WriteLine($"StreetName: {this.StreetName}");
            Console.WriteLine($"StreetNumber: {this.StreetNumber}");
            Console.WriteLine($"Phones:");
            foreach (var phone in Phones)
                Console.WriteLine($"-{phone.PhoneNumber}");
            
            Console.Write("\n");
            Console.WriteLine($"PaymentInfos:");
            foreach (var pi in PaymentInfos)
                Console.WriteLine($"-{pi.CardHolderName} , {pi.CardNumber} , {pi.CVV}");
            
            Console.Write("\n");
            Console.WriteLine($"OverallRating: {this.overallRating}");
            Console.WriteLine("---------------------");
        }
    }
}