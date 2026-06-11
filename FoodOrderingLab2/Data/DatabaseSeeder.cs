using System.Linq;
using FoodOrderingLab2.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderingLab2.Data
{
    public static class DatabaseSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            context.Database.Migrate();

            if (context.Restaurants.Any())
            {
                return;
            }

            var restaurants = MockDataInitializer.GetRestaurants();
            var customers = MockDataInitializer.GetCustomers();
            var orders = MockDataInitializer.GetOrders(restaurants, customers);

            ResetIdsForDatabaseGeneration(restaurants, customers, orders);
            context.Orders.AddRange(orders);
            context.SaveChanges();
        }

        private static void ResetIdsForDatabaseGeneration(
            IEnumerable<Restaurant> restaurants,
            IEnumerable<Customer> customers,
            IEnumerable<Order> orders)
        {
            foreach (var restaurant in restaurants)
            {
                restaurant.RestaurantId = 0;
                foreach (var menuItem in restaurant.MenuItems)
                {
                    menuItem.MenuItemId = 0;
                    menuItem.RestaurantId = 0;
                }
            }

            foreach (var customer in customers)
            {
                customer.CustomerId = 0;
            }

            foreach (var order in orders)
            {
                order.OrderId = 0;
                order.CustomerId = 0;
                order.RestaurantId = 0;
                foreach (var item in order.OrderItems)
                {
                    item.OrderItemId = 0;
                    item.OrderId = 0;
                    item.MenuItemId = 0;
                }
            }
        }
    }
}
