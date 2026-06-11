using System.ComponentModel.DataAnnotations;

namespace FoodOrderingLab2.ViewModels
{
    public class OrderItemCreateModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Odaberi artikl.")]
        public int MenuItemId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Količina mora biti veća od 0.")]
        public int Quantity { get; set; }

        [StringLength(500)]
        public string? SpecialRequests { get; set; }
    }
}
