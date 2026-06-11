using System.ComponentModel.DataAnnotations;

namespace FoodOrderingLab2.ViewModels
{
    public class CustomerCreateViewModel
    {
        [Required, StringLength(100)]
        [Display(Name = "Ime")]
        public string FirstName { get; set; } = null!;

        [Required, StringLength(100)]
        [Display(Name = "Prezime")]
        public string LastName { get; set; } = null!;

        [Required, EmailAddress, StringLength(256)]
        public string Email { get; set; } = null!;

        [Required, Phone, StringLength(50)]
        [RegularExpression(@"^\+?[0-9][0-9\s()\-]{6,49}$", ErrorMessage = "Unesi valjan broj telefona.")]
        [Display(Name = "Telefon")]
        public string Phone { get; set; } = null!;

        [Required, StringLength(300)]
        [Display(Name = "Adresa")]
        public string Address { get; set; } = null!;

        [Required]
        public DateTime RegisterDate { get; set; } = DateTime.Now;
    }
}
