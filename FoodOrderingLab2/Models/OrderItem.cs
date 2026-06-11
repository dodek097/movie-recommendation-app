using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderingLab2.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        public int OrderId { get; set; }
        public int MenuItemId { get; set; }
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        [Range(0.01, 9999.99)]
        public decimal UnitPrice { get; set; }
        [StringLength(500)]
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
