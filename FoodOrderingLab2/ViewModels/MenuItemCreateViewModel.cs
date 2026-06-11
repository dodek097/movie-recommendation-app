using System.ComponentModel.DataAnnotations;
using FoodOrderingLab2.Models.Enums;

namespace FoodOrderingLab2.ViewModels
{
    public class MenuItemCreateViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Odaberi restoran.")]
        [Display(Name = "Restoran")]
        public int RestaurantId { get; set; }

        [Required, StringLength(150)]
        [Display(Name = "Naziv")]
        public string Name { get; set; } = null!;

        [Required, StringLength(1000)]
        [Display(Name = "Opis")]
        public string Description { get; set; } = null!;

        [Range(0.01, 9999.99), Display(Name = "Cijena")]
        public decimal Price { get; set; }

        [Display(Name = "Kategorija")]
        public FoodCategory Category { get; set; } = FoodCategory.MainCourse;

        [Range(0, 10000), Display(Name = "Kalorije")]
        public int Calories { get; set; }

        [Display(Name = "Dostupno")]
        public bool IsAvailable { get; set; } = true;
    }
}
