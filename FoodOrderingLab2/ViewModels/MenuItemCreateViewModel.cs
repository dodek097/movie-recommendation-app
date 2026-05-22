using System.ComponentModel.DataAnnotations;
using FoodOrderingLab2.Models.Enums;

namespace FoodOrderingLab2.ViewModels
{
    public class MenuItemCreateViewModel
    {
        [Required]
        public int RestaurantId { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        [Range(0.01, 9999.99)]
        public decimal Price { get; set; }

        public FoodCategory Category { get; set; } = FoodCategory.MainCourse;

        [Range(0, 10000)]
        public int Calories { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}
