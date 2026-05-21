using System.ComponentModel.DataAnnotations;
using FoodOrderingLab2.Models.Enums;

namespace FoodOrderingLab2.ViewModels
{
    public class OrderCreateViewModel
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int RestaurantId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public List<OrderItemCreateModel> Items { get; set; } = new List<OrderItemCreateModel>();
    }
}