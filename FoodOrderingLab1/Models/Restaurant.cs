namespace FoodOrderingLab1.Models
{
    public class Restaurant
    {
        public int RestaurantId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public decimal Rating { get; set; }
        public string OpeningHours { get; set; }
        
        // Relationships
        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        public List<Order> Orders { get; set; } = new List<Order>();

        public Restaurant()
        {
            MenuItems = new List<MenuItem>();
            Orders = new List<Order>();
        }

        public override string ToString()
        {
            return $"🍽️  {Name} - Rating: {Rating}/5 - {Address}";
        }
    }
}
