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
        
        public int AverageTimeNeeded { get; set; }
        
        public decimal AverageTaskFee { get; set; }

        public Task() {}
        public Task(string n,string reqspec) {
            this.Name = n;
            this.RequiredSpecialty = reqspec;
        }
        public Task(int d,string n,string reqspec,int averageTimeNeeded,decimal averageTaskFee) {
            this.Id = d;
            this.Name = n;
            this.RequiredSpecialty = reqspec;
            this.AverageTimeNeeded = averageTimeNeeded;
            this.AverageTaskFee = averageTaskFee;
        }
        public void display() {
            Console.WriteLine("------------- Tasks --------------");
            Console.WriteLine($"| {"ID",-5} | {"Name",-20} | {"Required Specialty",-20} |");
            Console.WriteLine("-------------------------------------");
            Console.WriteLine($"| {this.Id,-5} | {this.Name,-20} | {this.RequiredSpecialty,-20} |");
            Console.WriteLine("-------------------------------------");
        }
        // Relationships
        public List<Request> Requests { get; set; } = new();
        public HashSet<Perform> Performs { get; set; } = new();
    }
}
