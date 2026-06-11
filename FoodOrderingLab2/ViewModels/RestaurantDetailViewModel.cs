using FoodOrderingLab2.Models;

namespace FoodOrderingLab2.ViewModels
{
    public class RestaurantDetailViewModel
    {
        public required Restaurant Restaurant { get; set; }
        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        public List<RestaurantAttachment> Attachments { get; set; } = new List<RestaurantAttachment>();
    }
}
