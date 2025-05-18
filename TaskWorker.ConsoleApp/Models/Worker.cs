using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskWorker.Models;
using TaskWorker.Services;
using TaskWorker.Validation;

namespace TaskWorker.Models
{
    internal class Worker
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal overallRating { get; set; }
        
        
        // Relationships
        public HashSet<WorkerSpecialty> Specialties { get; set; } = new();
        public List<WorkerAvailableTimeSlot> AvailableTimeSlots { get; set; } = new();
        public List<WorkerLocation> Locations { get; set; } = new();
        public HashSet<Perform> Performs { get; set; } = new();
        public List<RequestExecution> RequestExecutions { get; set; } = new();
        
        public void display()
        {
            Console.WriteLine("---------------------");
            Console.WriteLine($"ID: {this.Id}");
            Console.WriteLine($"Name: {this.Name}");
            Console.WriteLine($"Specialties:");
            foreach (var specialty in Specialties)
                Console.WriteLine($"- {specialty.Specialty}");
            
            Console.Write("\n");
            Console.WriteLine($"AvailableTimeSlots:");
            foreach (var slot in AvailableTimeSlots)
                Console.WriteLine($"- {slot.Slot}");
            
            Console.Write("\n");
            Console.WriteLine($"Locations:");
            foreach (var loc in Locations)
                Console.WriteLine($"- {loc.Country}, {loc.City} , {loc.Street}");
            
            Console.Write("\n");
            Console.WriteLine($"OverallRating: {this.overallRating}");
            Console.WriteLine("---------------------");
        }
    }
}