using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace final.Models
{
    public class Employee
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }  // Encrypted or hashed password

        public string Role { get; set; }  // Admin, Manager, Employee

        public int? ManagerId { get; set; }

        // 🔹 New fields
        public string Position { get; set; }  // e.g., "Software Engineer"

        public string? Address { get; set; }   // e.g., "123 Main Street"

        public decimal? Salary { get; set; }  // e.g., 50000.00

        public string? ImagePath { get; set; }  // This stores the saved path
        [NotMapped]
        [JsonIgnore]
        public IFormFile? ImageFile { get; set; }  // This receives the uploaded file

    }
}
