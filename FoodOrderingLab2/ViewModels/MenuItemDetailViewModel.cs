using FoodOrderingLab2.Models;

namespace FoodOrderingLab2.ViewModels
{
    public class MenuItemDetailViewModel
    {
        public required MenuItem MenuItem { get; set; }
        public required Restaurant Restaurant { get; set; }
    }
}
