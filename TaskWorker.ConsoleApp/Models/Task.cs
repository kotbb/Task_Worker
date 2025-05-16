using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskWorker.Models
{
    internal class Task_
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string RequiredSpecialty { get; set; }

        public Task_(string n,string reqspec) {
            this.Name = n;
            this.RequiredSpecialty = reqspec;
        }
        public Task_(int d,string n,string reqspec) {
            this.Id = d;
            this.Name = n;
            this.RequiredSpecialty = reqspec;
        }
        public void display() {
            Console.WriteLine($"{this.Id}.\t{this.Name}\t{this.RequiredSpecialty}\t");
            
        }
        // Relationships
        public List<Request> Requests { get; set; } = new();
        public HashSet<Perform> Performs { get; set; } = new();
    }
}
