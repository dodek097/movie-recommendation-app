using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FoodOrderingLab2.Models.Enums;

namespace FoodOrderingLab2.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Kupac je obavezan.")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Restoran je obavezan.")]
        public int RestaurantId { get; set; }

        [Required(ErrorMessage = "Datum narudžbe je obavezan.")]
        public DateTime OrderDate { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Cijena mora biti veća od 0.")]
        public decimal TotalPrice { get; set; }

        public OrderStatus Status { get; set; }

        // Relationships
        [ForeignKey(nameof(CustomerId))]
        public virtual Customer Customer { get; set; } = null!;

        [ForeignKey(nameof(RestaurantId))]
        public virtual Restaurant Restaurant { get; set; } = null!;

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public override string ToString()
        {
            return $"[ORDER] Order #{OrderId} - {TotalPrice}EUR - Status: {Status} - Date: {OrderDate:dd.MM.yyyy HH:mm}";
        }
    }
}
