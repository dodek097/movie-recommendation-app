using FoodOrderingLab2.Models;

namespace FoodOrderingLab2.ViewModels
{
    public class OrderDetailViewModel
    {
        public required Order Order { get; set; }
        public required Customer Customer { get; set; }
        public required Restaurant Restaurant { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
