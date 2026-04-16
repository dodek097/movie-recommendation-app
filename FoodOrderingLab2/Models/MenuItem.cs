using FoodOrderingLab2.Models.Enums;

namespace FoodOrderingLab2.Models
{
    public class MenuItem
    {
        public int MenuItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public FoodCategory Category { get; set; }
        public int Calories { get; set; }
        public bool IsAvailable { get; set; }
        
        // Foreign key
        public int RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; }

        // Relationships
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public MenuItem()
        {
            OrderItems = new List<OrderItem>();
        }

        public override string ToString()
        {
            return $"[ITEM] {Name} ({Category}) - {Price}EUR - {Calories}cal";
        }
    }
}
