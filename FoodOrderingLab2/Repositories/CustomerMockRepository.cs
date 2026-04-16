using FoodOrderingLab2.Data;
using FoodOrderingLab2.Models;

namespace FoodOrderingLab2.Repositories
{
    public class CustomerMockRepository
    {
        private List<Customer> _customers;
        private List<Order> _orders;

        public CustomerMockRepository()
        {
            _customers = MockDataInitializer.GetCustomers();
            var restaurants = MockDataInitializer.GetRestaurants();
            _orders = MockDataInitializer.GetOrders(restaurants, _customers);
            
            // Popuni Orders kolekciju za svakog customera
            foreach (var customer in _customers)
            {
                customer.Orders = _orders.Where(o => o.CustomerId == customer.CustomerId).ToList();
            }
        }

        public List<Customer> GetAll()
        {
            return _customers;
        }

        public Customer GetById(int id)
        {
            return _customers.FirstOrDefault(c => c.CustomerId == id);
        }
    }
}
