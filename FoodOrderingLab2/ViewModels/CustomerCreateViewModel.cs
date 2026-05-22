using System.ComponentModel.DataAnnotations;

namespace FoodOrderingLab2.ViewModels
{
    public class CustomerCreateViewModel
    {
        [Required]
        public string FirstName { get; set; } = null!;

        [Required]
        public string LastName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Phone { get; set; } = null!;

        [Required]
        public string Address { get; set; } = null!;

        [Required]
        public DateTime RegisterDate { get; set; } = DateTime.Now;
    }
}
