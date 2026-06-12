using System.ComponentModel.DataAnnotations;
using FoodOrderingLab2.Validation;

namespace FoodOrderingLab2.ViewModels;

public class CustomerProfileViewModel
{
    [Required, StringLength(100)]
    [Display(Name = "Ime")]
    public string FirstName { get; set; } = null!;

    [Required, StringLength(100)]
    [Display(Name = "Prezime")]
    public string LastName { get; set; } = null!;

    [Required, Phone, CroatianPhone, StringLength(50)]
    [Display(Name = "Telefon")]
    public string Phone { get; set; } = null!;

    [Required, StringLength(300)]
    [Display(Name = "Adresa")]
    public string Address { get; set; } = null!;
}
