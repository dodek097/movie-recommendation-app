using System.ComponentModel.DataAnnotations;
using FoodOrderingLab2.Models.Enums;

namespace FoodOrderingLab2.ViewModels
{
    public class OrderCreateViewModel : IValidatableObject
    {
        [Range(1, int.MaxValue, ErrorMessage = "Odaberi kupca.")]
        public int CustomerId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Odaberi restoran.")]
        public int RestaurantId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public List<OrderItemCreateModel> Items { get; set; } = new List<OrderItemCreateModel>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Items == null || !Items.Any(i => i.MenuItemId > 0 && i.Quantity > 0))
            {
                yield return new ValidationResult(
                    "Dodaj barem jednu stavku u narudžbu.",
                    new[] { nameof(Items) });
            }
        }
    }
}
