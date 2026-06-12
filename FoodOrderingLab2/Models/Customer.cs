using System.ComponentModel.DataAnnotations;
using FoodOrderingLab2.Validation;

namespace FoodOrderingLab2.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [Required, StringLength(100)]
        public string FirstName { get; set; } = null!;
        [Required, StringLength(100)]
        public string LastName { get; set; } = null!;
        [Required, EmailAddress, StringLength(256)]
        public string Email { get; set; } = null!;
        [Required, Phone, CroatianPhone, StringLength(50)]
        public string Phone { get; set; } = null!;
        [Required, StringLength(300)]
        public string Address { get; set; } = null!;
        public DateTime RegisterDate { get; set; }
        public string? AppUserId { get; set; }

        // Relationships
        public virtual AppUser? AppUser { get; set; }
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        public string FullName => $"{FirstName} {LastName}";

        public override string ToString()
        {
            return $"[USER] {FullName} - {Email} - Registered: {RegisterDate:dd.MM.yyyy}";
        }
    }
}
