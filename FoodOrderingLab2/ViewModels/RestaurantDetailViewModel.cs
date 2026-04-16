using FoodOrderingLab2.Models;

namespace FoodOrderingLab2.ViewModels
{
    public class RestaurantDetailViewModel
    {
        public Restaurant Restaurant { get; set; }
        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
    }
}
