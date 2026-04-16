using FoodOrderingLab2.Models;
using FoodOrderingLab2.Models.Enums;

namespace FoodOrderingLab2.Data
{
    public static class MockDataInitializer
    {
        public static List<Restaurant> GetRestaurants()
        {
            var restaurants = new List<Restaurant>
            {
                new Restaurant
                {
                    RestaurantId = 1,
                    Name = "Pizza Paradise",
                    Address = "Glavna ulica 10, Zagreb",
                    Phone = "+385 1 123 4567",
                    Email = "info@pizzaparadise.hr",
                    Rating = 4.8m,
                    OpeningHours = "11:00 - 23:00",
                    MenuItems = new List<MenuItem>
                    {
                        new MenuItem
                        {
                            MenuItemId = 1,
                            Name = "Margherita Pizza",
                            Description = "Classic pizza with mozzarella and tomato",
                            Price = 12.99m,
                            Category = FoodCategory.MainCourse,
                            Calories = 850,
                            IsAvailable = true,
                            RestaurantId = 1
                        },
                        new MenuItem
                        {
                            MenuItemId = 2,
                            Name = "Pepperoni Pizza",
                            Description = "Pizza with pepperoni and extra cheese",
                            Price = 14.99m,
                            Category = FoodCategory.MainCourse,
                            Calories = 950,
                            IsAvailable = true,
                            RestaurantId = 1
                        },
                        new MenuItem
                        {
                            MenuItemId = 3,
                            Name = "Tiramisu",
                            Description = "Classic Italian dessert",
                            Price = 8.99m,
                            Category = FoodCategory.Desserts,
                            Calories = 450,
                            IsAvailable = true,
                            RestaurantId = 1
                        }
                    }
                },
                new Restaurant
                {
                    RestaurantId = 2,
                    Name = "Asian Fusion",
                    Address = "Vukovarska 50, Zagreb",
                    Phone = "+385 1 234 5678",
                    Email = "info@asianfusion.hr",
                    Rating = 4.6m,
                    OpeningHours = "12:00 - 22:00",
                    MenuItems = new List<MenuItem>
                    {
                        new MenuItem
                        {
                            MenuItemId = 4,
                            Name = "Sushi Roll Combo",
                            Description = "Assorted fresh sushi rolls",
                            Price = 16.99m,
                            Category = FoodCategory.MainCourse,
                            Calories = 600,
                            IsAvailable = true,
                            RestaurantId = 2
                        },
                        new MenuItem
                        {
                            MenuItemId = 5,
                            Name = "Pad Thai",
                            Description = "Thai noodles with shrimp and vegetables",
                            Price = 13.99m,
                            Category = FoodCategory.MainCourse,
                            Calories = 750,
                            IsAvailable = true,
                            RestaurantId = 2
                        },
                        new MenuItem
                        {
                            MenuItemId = 6,
                            Name = "Spring Rolls",
                            Description = "Crispy vegetables spring rolls",
                            Price = 7.99m,
                            Category = FoodCategory.Appetizers,
                            Calories = 300,
                            IsAvailable = true,
                            RestaurantId = 2
                        }
                    }
                },
                new Restaurant
                {
                    RestaurantId = 3,
                    Name = "Healthy Bites",
                    Address = "Ulica zdravlja 5, Zagreb",
                    Phone = "+385 1 345 6789",
                    Email = "info@healthybites.hr",
                    Rating = 4.4m,
                    OpeningHours = "10:00 - 20:00",
                    MenuItems = new List<MenuItem>
                    {
                        new MenuItem
                        {
                            MenuItemId = 7,
                            Name = "Caesar Salad",
                            Description = "Fresh lettuce with caesar dressing",
                            Price = 11.99m,
                            Category = FoodCategory.Salads,
                            Calories = 350,
                            IsAvailable = true,
                            RestaurantId = 3
                        },
                        new MenuItem
                        {
                            MenuItemId = 8,
                            Name = "Grilled Chicken Breast",
                            Description = "Healthy grilled chicken with vegetables",
                            Price = 15.99m,
                            Category = FoodCategory.MainCourse,
                            Calories = 500,
                            IsAvailable = true,
                            RestaurantId = 3
                        },
                        new MenuItem
                        {
                            MenuItemId = 9,
                            Name = "Fresh Orange Juice",
                            Description = "Freshly squeezed orange juice",
                            Price = 5.99m,
                            Category = FoodCategory.Beverages,
                            Calories = 120,
                            IsAvailable = true,
                            RestaurantId = 3
                        }
                    }
                }
            };

            return restaurants;
        }

        public static List<Customer> GetCustomers()
        {
            var customers = new List<Customer>
            {
                new Customer
                {
                    CustomerId = 1,
                    FirstName = "Marko",
                    LastName = "Horvat",
                    Email = "marko.horvat@mail.com",
                    Phone = "+385 91 123 4567",
                    Address = "Novska 20, Zagreb",
                    RegisterDate = DateTime.Now.AddMonths(-6)
                },
                new Customer
                {
                    CustomerId = 2,
                    FirstName = "Ana",
                    LastName = "Petrov",
                    Email = "ana.petrov@mail.com",
                    Phone = "+385 91 234 5678",
                    Address = "Gajeva 15, Zagreb",
                    RegisterDate = DateTime.Now.AddMonths(-3)
                },
                new Customer
                {
                    CustomerId = 3,
                    FirstName = "Ivan",
                    LastName = "Novak",
                    Email = "ivan.novak@mail.com",
                    Phone = "+385 91 345 6789",
                    Address = "Ilica 100, Zagreb",
                    RegisterDate = DateTime.Now.AddDays(-15)
                }
            };

            return customers;
        }

        public static List<Order> GetOrders(List<Restaurant> restaurants, List<Customer> customers)
        {
            var orders = new List<Order>();
            int orderId = 1;

            // Marko - 3 ordere
            var order1 = new Order
            {
                OrderId = orderId++,
                CustomerId = customers[0].CustomerId,
                RestaurantId = restaurants[0].RestaurantId,
                Customer = customers[0],
                Restaurant = restaurants[0],
                OrderDate = DateTime.Now.AddDays(-5),
                Status = OrderStatus.Delivered,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem
                    {
                        OrderItemId = 1,
                        MenuItemId = 1,
                        MenuItem = restaurants[0].MenuItems[0],
                        Quantity = 2,
                        UnitPrice = restaurants[0].MenuItems[0].Price,
                        SpecialRequests = "Extra cheese"
                    },
                    new OrderItem
                    {
                        OrderItemId = 2,
                        MenuItemId = 3,
                        MenuItem = restaurants[0].MenuItems[2],
                        Quantity = 1,
                        UnitPrice = restaurants[0].MenuItems[2].Price,
                        SpecialRequests = null
                    }
                }
            };
            order1.TotalPrice = order1.OrderItems.Sum(x => x.TotalItemPrice);
            orders.Add(order1);

            var order2 = new Order
            {
                OrderId = orderId++,
                CustomerId = customers[0].CustomerId,
                RestaurantId = restaurants[1].RestaurantId,
                Customer = customers[0],
                Restaurant = restaurants[1],
                OrderDate = DateTime.Now.AddDays(-3),
                Status = OrderStatus.Confirmed,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem
                    {
                        OrderItemId = 3,
                        MenuItemId = 4,
                        MenuItem = restaurants[1].MenuItems[0],
                        Quantity = 1,
                        UnitPrice = restaurants[1].MenuItems[0].Price,
                        SpecialRequests = null
                    }
                }
            };
            order2.TotalPrice = order2.OrderItems.Sum(x => x.TotalItemPrice);
            orders.Add(order2);

            var order3 = new Order
            {
                OrderId = orderId++,
                CustomerId = customers[0].CustomerId,
                RestaurantId = restaurants[2].RestaurantId,
                Customer = customers[0],
                Restaurant = restaurants[2],
                OrderDate = DateTime.Now.AddDays(-1),
                Status = OrderStatus.ReadyForPickup,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem
                    {
                        OrderItemId = 4,
                        MenuItemId = 7,
                        MenuItem = restaurants[2].MenuItems[0],
                        Quantity = 2,
                        UnitPrice = restaurants[2].MenuItems[0].Price,
                        SpecialRequests = "No dressing"
                    },
                    new OrderItem
                    {
                        OrderItemId = 5,
                        MenuItemId = 9,
                        MenuItem = restaurants[2].MenuItems[2],
                        Quantity = 1,
                        UnitPrice = restaurants[2].MenuItems[2].Price,
                        SpecialRequests = null
                    }
                }
            };
            order3.TotalPrice = order3.OrderItems.Sum(x => x.TotalItemPrice);
            orders.Add(order3);

            // Ana - 2 ordere
            var order4 = new Order
            {
                OrderId = orderId++,
                CustomerId = customers[1].CustomerId,
                RestaurantId = restaurants[0].RestaurantId,
                Customer = customers[1],
                Restaurant = restaurants[0],
                OrderDate = DateTime.Now.AddDays(-10),
                Status = OrderStatus.Delivered,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem
                    {
                        OrderItemId = 6,
                        MenuItemId = 2,
                        MenuItem = restaurants[0].MenuItems[1],
                        Quantity = 1,
                        UnitPrice = restaurants[0].MenuItems[1].Price,
                        SpecialRequests = null
                    }
                }
            };
            order4.TotalPrice = order4.OrderItems.Sum(x => x.TotalItemPrice);
            orders.Add(order4);

            var order5 = new Order
            {
                OrderId = orderId++,
                CustomerId = customers[1].CustomerId,
                RestaurantId = restaurants[1].RestaurantId,
                Customer = customers[1],
                Restaurant = restaurants[1],
                OrderDate = DateTime.Now.AddDays(-2),
                Status = OrderStatus.Preparing,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem
                    {
                        OrderItemId = 7,
                        MenuItemId = 5,
                        MenuItem = restaurants[1].MenuItems[1],
                        Quantity = 2,
                        UnitPrice = restaurants[1].MenuItems[1].Price,
                        SpecialRequests = "Spicy"
                    },
                    new OrderItem
                    {
                        OrderItemId = 8,
                        MenuItemId = 6,
                        MenuItem = restaurants[1].MenuItems[2],
                        Quantity = 2,
                        UnitPrice = restaurants[1].MenuItems[2].Price,
                        SpecialRequests = null
                    }
                }
            };
            order5.TotalPrice = order5.OrderItems.Sum(x => x.TotalItemPrice);
            orders.Add(order5);

            // Ivan - 2 ordere
            var order6 = new Order
            {
                OrderId = orderId++,
                CustomerId = customers[2].CustomerId,
                RestaurantId = restaurants[2].RestaurantId,
                Customer = customers[2],
                Restaurant = restaurants[2],
                OrderDate = DateTime.Now.AddDays(-7),
                Status = OrderStatus.Delivered,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem
                    {
                        OrderItemId = 9,
                        MenuItemId = 8,
                        MenuItem = restaurants[2].MenuItems[1],
                        Quantity = 1,
                        UnitPrice = restaurants[2].MenuItems[1].Price,
                        SpecialRequests = null
                    }
                }
            };
            order6.TotalPrice = order6.OrderItems.Sum(x => x.TotalItemPrice);
            orders.Add(order6);

            var order7 = new Order
            {
                OrderId = orderId++,
                CustomerId = customers[2].CustomerId,
                RestaurantId = restaurants[0].RestaurantId,
                Customer = customers[2],
                Restaurant = restaurants[0],
                OrderDate = DateTime.Now.AddHours(-2),
                Status = OrderStatus.Pending,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem
                    {
                        OrderItemId = 10,
                        MenuItemId = 1,
                        MenuItem = restaurants[0].MenuItems[0],
                        Quantity = 1,
                        UnitPrice = restaurants[0].MenuItems[0].Price,
                        SpecialRequests = "Well done"
                    }
                }
            };
            order7.TotalPrice = order7.OrderItems.Sum(x => x.TotalItemPrice);
            orders.Add(order7);

            return orders;
        }
    }
}
