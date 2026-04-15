using FoodOrderingLab1.Models;
using FoodOrderingLab1.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║     FOOD ORDERING APP - Lab 1 (C#, LINQ, Async)           ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════╝\n");

        // ============================================
        // 1️⃣  INICIJALIZACIJA PODATAKA
        // ============================================
        Console.WriteLine("\n📊 INICIJALIZACIJA PODATAKA:\n");

        var restaurants = InitializeRestaurants();
        var customers = InitializeCustomers();
        var orders = InitializeOrders(restaurants, customers);

        // ============================================
        // 2️⃣  LINQ UPITI
        // ============================================
        Console.WriteLine("\n\n📋 LINQ UPITI - DEMONSTRACIJA:\n");

        ExecuteLINQQueries(restaurants, customers, orders);

        // ============================================
        // 3️⃣  ASYNC/AWAIT DEMONSTRACIJA
        // ============================================
        Console.WriteLine("\n\n⚙️  ASYNC/AWAIT DEMONSTRACIJA:\n");
        
        await ExecuteAsyncOperations(orders);

        Console.WriteLine("\n\n✅ Lab 1 - Završeno!");
        Console.ReadKey();
    }

    // ============================================
    // INICIJALIZACIJA - RESTORANI
    // ============================================
    static List<Restaurant> InitializeRestaurants()
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

        Console.WriteLine("✅ 3 Restorana inicijalizirano:");
        foreach (var r in restaurants)
        {
            Console.WriteLine($"   {r} - {r.MenuItems.Count} jela");
        }

        return restaurants;
    }

    // ============================================
    // INICIJALIZACIJA - CUSTOMERI
    // ============================================
    static List<Customer> InitializeCustomers()
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

        Console.WriteLine("\n✅ 3 Customera inicijalizirano:");
        foreach (var c in customers)
        {
            Console.WriteLine($"   {c}");
        }

        return customers;
    }

    // ============================================
    // INICIJALIZACIJA - ORDERI
    // ============================================
    static List<Order> InitializeOrders(List<Restaurant> restaurants, List<Customer> customers)
    {
        var orders = new List<Order>();
        int orderId = 1;

        // Marko - 3 ordere
        // Order 1 - Pizza Paradise
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

        // Order 2 - Asian Fusion
        var order2 = new Order
        {
            OrderId = orderId++,
            CustomerId = customers[0].CustomerId,
            RestaurantId = restaurants[1].RestaurantId,
            Customer = customers[0],
            Restaurant = restaurants[1],
            OrderDate = DateTime.Now.AddDays(-2),
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
                    SpecialRequests = "No wasabi"
                }
            }
        };
        order2.TotalPrice = order2.OrderItems.Sum(x => x.TotalItemPrice);
        orders.Add(order2);

        // Order 3 - Healthy Bites
        var order3 = new Order
        {
            OrderId = orderId++,
            CustomerId = customers[0].CustomerId,
            RestaurantId = restaurants[2].RestaurantId,
            Customer = customers[0],
            Restaurant = restaurants[2],
            OrderDate = DateTime.Now.AddHours(-1),
            Status = OrderStatus.Pending,
            OrderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    OrderItemId = 4,
                    MenuItemId = 8,
                    MenuItem = restaurants[2].MenuItems[1],
                    Quantity = 2,
                    UnitPrice = restaurants[2].MenuItems[1].Price,
                    SpecialRequests = "Grilled, not fried"
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
            OrderDate = DateTime.Now.AddDays(-3),
            Status = OrderStatus.Delivered,
            OrderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    OrderItemId = 5,
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
            RestaurantId = restaurants[2].RestaurantId,
            Customer = customers[1],
            Restaurant = restaurants[2],
            OrderDate = DateTime.Now.AddHours(-5),
            Status = OrderStatus.Preparing,
            OrderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    OrderItemId = 6,
                    MenuItemId = 7,
                    MenuItem = restaurants[2].MenuItems[0],
                    Quantity = 2,
                    UnitPrice = restaurants[2].MenuItems[0].Price,
                    SpecialRequests = "Dressing on the side"
                }
            }
        };
        order5.TotalPrice = order5.OrderItems.Sum(x => x.TotalItemPrice);
        orders.Add(order5);

        // Ivan - 1 order
        var order6 = new Order
        {
            OrderId = orderId++,
            CustomerId = customers[2].CustomerId,
            RestaurantId = restaurants[1].RestaurantId,
            Customer = customers[2],
            Restaurant = restaurants[1],
            OrderDate = DateTime.Now.AddDays(-1),
            Status = OrderStatus.Delivered,
            OrderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    OrderItemId = 7,
                    MenuItemId = 5,
                    MenuItem = restaurants[1].MenuItems[1],
                    Quantity = 1,
                    UnitPrice = restaurants[1].MenuItems[1].Price,
                    SpecialRequests = "Extra spicy"
                }
            }
        };
        order6.TotalPrice = order6.OrderItems.Sum(x => x.TotalItemPrice);
        orders.Add(order6);

        Console.WriteLine("\n✅ 6 Ordere inicijalizirano (Marko: 3, Ana: 2, Ivan: 1)");

        return orders;
    }

    // ============================================
    // LINQ UPITI
    // ============================================
    static void ExecuteLINQQueries(List<Restaurant> restaurants, List<Customer> customers, List<Order> orders)
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════");

        // 1️⃣  WHERE - Filtriranje
        Console.WriteLine("\n1️⃣  WHERE - Pronađi sve ordere s statusom 'Delivered':");
        var deliveredOrders = orders.Where(o => o.Status == OrderStatus.Delivered).ToList();
        foreach (var order in deliveredOrders)
        {
            Console.WriteLine($"   {order}");
        }

        // 2️⃣  OrderBy / OrderByDescending
        Console.WriteLine("\n2️⃣  OrderBy - Sortiraj ordere po cijeni (opadajuće):");
        var ordersByPrice = orders.OrderByDescending(o => o.TotalPrice).ToList();
        foreach (var order in ordersByPrice)
        {
            Console.WriteLine($"   Order #{order.OrderId} - €{order.TotalPrice} - {order.Customer.FirstName}");
        }

        // 3️⃣  Count
        Console.WriteLine("\n3️⃣  Count - Broji ordere po restaurantu:");
        var orderCountByRestaurant = orders.GroupBy(o => o.Restaurant.Name)
                                           .Select(g => new { Restaurant = g.Key, Count = g.Count() });
        foreach (var group in orderCountByRestaurant)
        {
            Console.WriteLine($"   {group.Restaurant}: {group.Count} ordere");
        }

        // 4️⃣  Sum - Ukupna vrijednost ordere
        Console.WriteLine("\n4️⃣  Sum - Ukupna vrijednost svih ordere:");
        var totalRevenue = orders.Sum(o => o.TotalPrice);
        Console.WriteLine($"   Ukupno: €{totalRevenue:F2}");

        // 5️⃣  Average
        Console.WriteLine("\n5️⃣  Average - Prosječna vrijednost ordere:");
        var avgOrderValue = orders.Average(o => o.TotalPrice);
        Console.WriteLine($"   Prosječno: €{avgOrderValue:F2}");

        // 6️⃣  First / FirstOrDefault
        Console.WriteLine("\n6️⃣  First - Pronađi prvi pending order:");
        var firstPending = orders.FirstOrDefault(o => o.Status == OrderStatus.Pending);
        if (firstPending != null)
            Console.WriteLine($"   {firstPending}");

        // 7️⃣  Any
        Console.WriteLine("\n7️⃣  Any - Postoji li restauran s rating > 4.7?");
        bool hasHighRated = restaurants.Any(r => r.Rating > 4.7m);
        Console.WriteLine($"   Rezultat: {(hasHighRated ? "DA ✓" : "NE ✗")}");

        // 8️⃣  Select / Select Many
        Console.WriteLine("\n8️⃣  SelectMany - Ispis svih stavki iz svih ordere:");
        var allOrderItems = orders.SelectMany(o => o.OrderItems)
                                  .OrderByDescending(oi => oi.TotalItemPrice);
        foreach (var item in allOrderItems.Take(5))
        {
            Console.WriteLine($"   {item} (€{item.TotalItemPrice:F2})");
        }

        // 9️⃣  GroupBy - Broji ordere po customeru
        Console.WriteLine("\n9️⃣  GroupBy - Broji ordere po customeru:");
        var ordersByCustomer = orders.GroupBy(o => o.Customer.FullName)
                                     .Select(g => new { Customer = g.Key, OrderCount = g.Count(), TotalSpent = g.Sum(x => x.TotalPrice) });
        foreach (var group in ordersByCustomer.OrderByDescending(g => g.TotalSpent))
        {
            Console.WriteLine($"   {group.Customer}: {group.OrderCount} ordere - €{group.TotalSpent:F2}");
        }

        // 🔟 Kompleksniji upit - Top 3 najskupnije stavke
        Console.WriteLine("\n🔟 Top 3 najskupnije stavke iz menija:");
        var topExpensiveItems = restaurants.SelectMany(r => r.MenuItems)
                                           .OrderByDescending(m => m.Price)
                                           .Take(3);
        foreach (var item in topExpensiveItems)
        {
            Console.WriteLine($"   {item.Name} - €{item.Price}");
        }

        // 1️⃣1️⃣  Pronađi sve ordere s više od 1 stavke
        Console.WriteLine("\n1️⃣1️⃣  Orderi s više od 1 stavke:");
        var complexOrders = orders.Where(o => o.OrderItems.Count > 1)
                                 .Select(o => new { o.OrderId, o.Customer.FullName, ItemCount = o.OrderItems.Count });
        foreach (var order in complexOrders)
        {
            Console.WriteLine($"   Order #{order.OrderId} ({order.FullName}): {order.ItemCount} stavki");
        }

        // 1️⃣2️⃣  Pronađi menuItems pod €10
        Console.WriteLine("\n1️⃣2️⃣  Jela ispod €10:");
        var cheapItems = restaurants.SelectMany(r => r.MenuItems)
                                   .Where(m => m.Price < 10)
                                   .OrderBy(m => m.Price);
        foreach (var item in cheapItems)
        {
            Console.WriteLine($"   {item.Name} ({item.Category}) - €{item.Price}");
        }

        Console.WriteLine("\n═══════════════════════════════════════════════════════════");
    }

    // ============================================
    // ASYNC / AWAIT DEMONSTRACIJA
    // ============================================
    static async Task ExecuteAsyncOperations(List<Order> orders)
    {
        Console.WriteLine("Starting async operations...\n");

        // Task 1 - Simulacija slanja ordere
        await SendOrderAsync(orders.First());

        // Task 2 - Simulacija dohvaćanja podataka
        await FetchOrderStatusAsync(orders.First());

        // Task 3 - Više taskova paralelno
        Console.WriteLine("\n⚡ Obrađujem više ordere paralelno:");
        var tasks = new List<Task>();
        foreach (var order in orders.Take(3))
        {
            tasks.Add(ProcessOrderAsync(order));
        }
        await Task.WhenAll(tasks);

        Console.WriteLine("\n✅ Svi async taskovi su završeni!");
    }

    // Async metoda 1 - Slanje ordere
    static async Task SendOrderAsync(Order order)
    {
        Console.WriteLine($"📤 Slanje ordere #{order.OrderId}...");
        await Task.Delay(1000); // Simulacija slanja
        Console.WriteLine($"✅ Oredr #{order.OrderId} poslana uspješno!");
    }

    // Async metoda 2 - Dohvat statusa
    static async Task FetchOrderStatusAsync(Order order)
    {
        Console.WriteLine($"\n📊 Dohvaćam status ordere #{order.OrderId}...");
        await Task.Delay(800); // Simulacija dohvata
        Console.WriteLine($"✅ Status ordere #{order.OrderId}: {order.Status}");
    }

    // Async metoda 3 - Obrada ordere
    static async Task ProcessOrderAsync(Order order)
    {
        Console.WriteLine($"   ⏳ Obrađujem order #{order.OrderId} za {order.Customer.FirstName}...");
        await Task.Delay(Random.Shared.Next(500, 1500));
        Console.WriteLine($"   ✅ Order #{order.OrderId} obrađen!");
    }
}
