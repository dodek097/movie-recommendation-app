using System.ComponentModel.DataAnnotations;

namespace FoodOrderingLab2.Models
{
    public class Restaurant
    {
        [Key]
        public int RestaurantId { get; set; }

        [Required, StringLength(150)]
        public string Name { get; set; } = null!;
        [Required, StringLength(300)]
        public string Address { get; set; } = null!;
        [Required, Phone, StringLength(50)]
        public string Phone { get; set; } = null!;
        [Required, EmailAddress, StringLength(256)]
        public string Email { get; set; } = null!;
        [Range(0, 5)]
        public decimal Rating { get; set; }
        [Required, StringLength(100)]
        public string OpeningHours { get; set; } = null!;

        // Relationships
        public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<RestaurantAttachment> Attachments { get; set; } = new List<RestaurantAttachment>();

        public override string ToString()
        {
            return $"[RESTO] {Name} - Rating: {Rating}/5 - {Address}";
        }
    }
}
