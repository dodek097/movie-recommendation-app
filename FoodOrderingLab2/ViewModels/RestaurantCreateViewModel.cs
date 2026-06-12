using System.ComponentModel.DataAnnotations;
using FoodOrderingLab2.Validation;

namespace FoodOrderingLab2.ViewModels
{
    public class RestaurantCreateViewModel
    {
        [Required, StringLength(150)]
        [Display(Name = "Naziv restorana")]
        public string Name { get; set; } = null!;

        [Required, StringLength(300)]
        [Display(Name = "Adresa")]
        public string Address { get; set; } = null!;

        [Required, Phone, CroatianPhone, StringLength(50)]
        [Display(Name = "Telefon")]
        public string Phone { get; set; } = null!;

        [Required, EmailAddress, StringLength(256)]
        public string Email { get; set; } = null!;

        [Range(0, 5), Display(Name = "Ocjena")]
        public decimal Rating { get; set; } = 4.5M;

        [Required, StringLength(100)]
        [RegularExpression(@"^(?:[01]\d|2[0-3]):[0-5]\d\s*-\s*(?:[01]\d|2[0-3]):[0-5]\d$",
            ErrorMessage = "Koristi format HH:mm - HH:mm, npr. 09:00 - 22:00.")]
        [Display(Name = "Radno vrijeme")]
        public string OpeningHours { get; set; } = "09:00 - 22:00";

        public List<RestaurantMenuItemCreateModel> MenuItems { get; set; } = new();
    }
}
