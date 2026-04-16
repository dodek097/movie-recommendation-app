namespace FoodOrderingLab2.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int MenuItemId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string SpecialRequests { get; set; }
        
        // Relationships
        public Order Order { get; set; }
        public MenuItem MenuItem { get; set; }

        public decimal TotalItemPrice => UnitPrice * Quantity;

        public override string ToString()
        {
            return $"  - {MenuItem?.Name} x{Quantity} - {TotalItemPrice:F2}EUR" + 
                   (string.IsNullOrEmpty(SpecialRequests) ? "" : $" (Note: {SpecialRequests})");
        }
    }
}
