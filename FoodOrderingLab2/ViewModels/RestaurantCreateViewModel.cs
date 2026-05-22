using System.ComponentModel.DataAnnotations;

namespace FoodOrderingLab2.ViewModels
{
    public class RestaurantCreateViewModel
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Address { get; set; } = null!;

        [Required]
        public string Phone { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Range(0, 5)]
        public decimal Rating { get; set; } = 4.5M;

        [Required]
        public string OpeningHours { get; set; } = "09:00 - 22:00";

        public List<RestaurantMenuItemCreateModel> MenuItems { get; set; } = new();
    }
}
