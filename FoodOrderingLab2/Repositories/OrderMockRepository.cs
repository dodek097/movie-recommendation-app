using FoodOrderingLab2.Data;
using FoodOrderingLab2.Models;

namespace FoodOrderingLab2.Repositories
{
    public class OrderMockRepository
    {
        private List<Order> _orders;

        public OrderMockRepository()
        {
            var restaurants = MockDataInitializer.GetRestaurants();
            var customers = MockDataInitializer.GetCustomers();
            _orders = MockDataInitializer.GetOrders(restaurants, customers);
        }

        public List<Order> GetAll()
        {
            return _orders;
        }

        public Order GetById(int id)
        {
            return _orders.FirstOrDefault(o => o.OrderId == id);
        }
    }
}
