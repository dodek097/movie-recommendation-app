namespace FoodOrderingLab2.ViewModels
{
    public class OrderItemCreateModel
    {
        public int MenuItemId { get; set; }
        public int Quantity { get; set; }
        public string? SpecialRequests { get; set; }
    }
}