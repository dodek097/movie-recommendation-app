using FoodOrderingLab2.Models;

namespace FoodOrderingLab2.ViewModels
{
    public class OrderDetailViewModel
    {
        public Order Order { get; set; }
        public Customer Customer { get; set; }
        public Restaurant Restaurant { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
