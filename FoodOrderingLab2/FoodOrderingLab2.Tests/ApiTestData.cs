using FoodOrderingLab2.Data;
using FoodOrderingLab2.Models;
using FoodOrderingLab2.Models.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace FoodOrderingLab2.Tests;

public record SeededData(int CustomerId, int RestaurantId, int MenuItemId, int SecondMenuItemId, int OrderId, int OrderItemId);

public static class ApiTestData
{
    public static async Task<SeededData> SeedAsync(ApiWebApplicationFactory factory)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var customer = new Customer
        {
            AppUserId = "customer-user",
            FirstName = "Test", LastName = "Kupac", Email = "test@example.com", Phone = "+385 91 111 1111",
            Address = "Testna 1", RegisterDate = DateTime.UtcNow
        };
        var restaurant = new Restaurant
        {
            Name = "Test restoran", Address = "Testna 2", Phone = "+385 1 111 1111",
            Email = "restoran@example.com", Rating = 4.5m, OpeningHours = "09:00 - 22:00"
        };
        var firstItem = new MenuItem
        {
            Name = "Prvo jelo", Description = "Opis", Price = 10m, Category = FoodCategory.MainCourse,
            Calories = 500, IsAvailable = true, Restaurant = restaurant
        };
        var secondItem = new MenuItem
        {
            Name = "Drugo jelo", Description = "Opis", Price = 5m, Category = FoodCategory.Desserts,
            Calories = 250, IsAvailable = true, Restaurant = restaurant
        };
        var order = new Order
        {
            Customer = customer, Restaurant = restaurant, OrderDate = DateTime.UtcNow, Status = OrderStatus.Pending,
            TotalPrice = firstItem.Price,
            OrderItems = [new OrderItem { MenuItem = firstItem, Quantity = 1, UnitPrice = firstItem.Price }]
        };
        db.Orders.Add(order);
        db.MenuItems.Add(secondItem);
        await db.SaveChangesAsync();
        return new(customer.CustomerId, restaurant.RestaurantId, firstItem.MenuItemId, secondItem.MenuItemId, order.OrderId,
            order.OrderItems.Single().OrderItemId);
    }
}
