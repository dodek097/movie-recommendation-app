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

            context.Restaurants.AddRange(restaurants);
            context.Customers.AddRange(customers);
            context.Orders.AddRange(orders);
            context.SaveChanges();
        }
    }
}
