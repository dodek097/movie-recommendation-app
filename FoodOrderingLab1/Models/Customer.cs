namespace FoodOrderingLab1.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime RegisterDate { get; set; }
        
        // Relationships
        public List<Order> Orders { get; set; } = new List<Order>();

        public Customer()
        {
            Orders = new List<Order>();
        }

        public string FullName => $"{FirstName} {LastName}";

        public override string ToString()
        {
            return $"[USER] {FullName} - {Email} - Registered: {RegisterDate:dd.MM.yyyy}";
        }
    }
}
