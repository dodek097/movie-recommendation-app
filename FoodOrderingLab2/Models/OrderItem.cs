using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderingLab2.Models
{
    public class OrderItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int OrderItemId { get; set; }

        public int OrderId { get; set; }
        public int MenuItemId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? SpecialRequests { get; set; }

        // Relationships
        [ForeignKey(nameof(OrderId))]
        public virtual Order Order { get; set; } = null!;

        [ForeignKey(nameof(MenuItemId))]
        public virtual MenuItem MenuItem { get; set; } = null!;

        public decimal TotalItemPrice => UnitPrice * Quantity;

        public override string ToString()
        {
            return $"  - {MenuItem?.Name} x{Quantity} - {TotalItemPrice:F2}EUR" +
                   (string.IsNullOrEmpty(SpecialRequests) ? string.Empty : $" (Note: {SpecialRequests})");
        }
    }
}
