using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FoodOrderingLab2.Models.Enums;

namespace FoodOrderingLab2.Models
{
    public class MenuItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MenuItemId { get; set; }

        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public FoodCategory Category { get; set; }
        public int Calories { get; set; }
        public bool IsAvailable { get; set; }

        // Foreign key
        public int RestaurantId { get; set; }

        [ForeignKey(nameof(RestaurantId))]
        public virtual Restaurant Restaurant { get; set; } = null!;

        // Relationships
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public override string ToString()
        {
            return $"[ITEM] {Name} ({Category}) - {Price}EUR - {Calories}cal";
        }
    }
}
