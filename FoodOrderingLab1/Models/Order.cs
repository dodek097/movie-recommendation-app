using FoodOrderingLab1.Models.Enums;

namespace FoodOrderingLab1.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int RestaurantId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public OrderStatus Status { get; set; }
        
        // Relationships
        public Customer Customer { get; set; }
        public Restaurant Restaurant { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public Order()
        {
            OrderItems = new List<OrderItem>();
        }

        public override string ToString()
        {
            return $"[ORDER] Order #{OrderId} - {TotalPrice}EUR - Status: {Status} - Date: {OrderDate:dd.MM.yyyy HH:mm}";
        }
    }
}
