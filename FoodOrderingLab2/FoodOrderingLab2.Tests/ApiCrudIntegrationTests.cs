using System.Net;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using FoodOrderingLab2.Dtos;
using FoodOrderingLab2.Models.Enums;
using FoodOrderingLab2.Models;
using FoodOrderingLab2.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace FoodOrderingLab2.Tests;

public class ApiCrudIntegrationTests(ApiWebApplicationFactory factory) : IClassFixture<ApiWebApplicationFactory>
{
    [Fact]
    public async Task CustomersApi_CoversCrudValidationAndMissingIds()
    {
        await factory.ResetAsync();
        using var client = factory.CreateAdminClient();
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/customers?q=test")).StatusCode);

        var request = new CustomerRequest
        {
            FirstName = "Ana", LastName = "Test", Email = "ana@example.com", Phone = "+385 91 222 2222",
            Address = "Nova 1", RegisterDate = DateTime.UtcNow
        };
        var create = await client.PostAsJsonAsync("/api/customers", request);
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var created = await create.Content.ReadFromJsonAsync<CustomerDto>();
        Assert.NotNull(created);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync($"/api/customers/{created.CustomerId}")).StatusCode);

        request.LastName = "Promijenjeno";
        Assert.Equal(HttpStatusCode.OK, (await client.PutAsJsonAsync($"/api/customers/{created.CustomerId}", request)).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/api/customers", new CustomerRequest())).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/api/customers/999999")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.PutAsJsonAsync("/api/customers/999999", request)).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync($"/api/customers/{created.CustomerId}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.DeleteAsync($"/api/customers/{created.CustomerId}")).StatusCode);
    }

    [Fact]
    public async Task RestaurantsApi_CoversCrudValidationAndMissingIds()
    {
        await factory.ResetAsync();
        using var client = factory.CreateAdminClient();
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/restaurants?minRating=4")).StatusCode);
        var request = new RestaurantRequest
        {
            Name = "Novi restoran", Address = "Nova 2", Phone = "+385 1 222 2222",
            Email = "novi@example.com", Rating = 4.2m, OpeningHours = "10:00 - 23:00"
        };
        var create = await client.PostAsJsonAsync("/api/restaurants", request);
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var created = await create.Content.ReadFromJsonAsync<RestaurantDto>();
        Assert.NotNull(created);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync($"/api/restaurants/{created.RestaurantId}")).StatusCode);
        request.Rating = 4.8m;
        Assert.Equal(HttpStatusCode.OK, (await client.PutAsJsonAsync($"/api/restaurants/{created.RestaurantId}", request)).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/api/restaurants", new RestaurantRequest())).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/api/restaurants/999999")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.PutAsJsonAsync("/api/restaurants/999999", request)).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync($"/api/restaurants/{created.RestaurantId}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.DeleteAsync($"/api/restaurants/{created.RestaurantId}")).StatusCode);
    }

    [Fact]
    public async Task MenuItemsApi_CoversCrudValidationAndMissingIds()
    {
        await factory.ResetAsync();
        var seed = await ApiTestData.SeedAsync(factory);
        using var client = factory.CreateAdminClient();
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync($"/api/menu-items?restaurantId={seed.RestaurantId}&available=true")).StatusCode);
        var request = new MenuItemRequest
        {
            RestaurantId = seed.RestaurantId, Name = "Novo jelo", Description = "Opis novog jela", Price = 7m,
            Category = FoodCategory.Salads, Calories = 300, IsAvailable = true
        };
        var create = await client.PostAsJsonAsync("/api/menu-items", request);
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var created = await create.Content.ReadFromJsonAsync<MenuItemDto>();
        Assert.NotNull(created);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync($"/api/menu-items/{created.MenuItemId}")).StatusCode);
        request.Price = 8m;
        Assert.Equal(HttpStatusCode.OK, (await client.PutAsJsonAsync($"/api/menu-items/{created.MenuItemId}", request)).StatusCode);
        request.RestaurantId = 999999;
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/api/menu-items", request)).StatusCode);
        request.RestaurantId = seed.RestaurantId;
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/api/menu-items/999999")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.PutAsJsonAsync("/api/menu-items/999999", request)).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync($"/api/menu-items/{created.MenuItemId}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.DeleteAsync($"/api/menu-items/{created.MenuItemId}")).StatusCode);
    }

    [Fact]
    public async Task OrdersApi_CoversCrudValidationAndMissingIds()
    {
        await factory.ResetAsync();
        var seed = await ApiTestData.SeedAsync(factory);
        using var client = factory.CreateAdminClient();
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync($"/api/orders?customerId={seed.CustomerId}")).StatusCode);
        var request = new OrderRequest
        {
            CustomerId = seed.CustomerId, RestaurantId = seed.RestaurantId, OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Confirmed, Items = [new OrderItemRequest { MenuItemId = seed.SecondMenuItemId, Quantity = 2 }]
        };
        var create = await client.PostAsJsonAsync("/api/orders", request);
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var created = await create.Content.ReadFromJsonAsync<OrderDto>();
        Assert.NotNull(created);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync($"/api/orders/{created.OrderId}")).StatusCode);
        request.Status = OrderStatus.Preparing;
        Assert.Equal(HttpStatusCode.OK, (await client.PutAsJsonAsync($"/api/orders/{created.OrderId}", request)).StatusCode);
        var invalid = new OrderRequest { CustomerId = seed.CustomerId, RestaurantId = seed.RestaurantId, OrderDate = DateTime.UtcNow, Items = [] };
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/api/orders", invalid)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/api/orders/999999")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.PutAsJsonAsync("/api/orders/999999", request)).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync($"/api/orders/{created.OrderId}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.DeleteAsync($"/api/orders/{created.OrderId}")).StatusCode);
    }

    [Fact]
    public async Task OrderItemsApi_CoversCrudValidationAndMissingIds()
    {
        await factory.ResetAsync();
        var seed = await ApiTestData.SeedAsync(factory);
        using var client = factory.CreateAdminClient();
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync($"/api/orders/{seed.OrderId}/items")).StatusCode);
        var request = new OrderItemRequest { MenuItemId = seed.SecondMenuItemId, Quantity = 2, SpecialRequests = "Bez umaka" };
        var create = await client.PostAsJsonAsync($"/api/orders/{seed.OrderId}/items", request);
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var created = await create.Content.ReadFromJsonAsync<OrderItemDto>();
        Assert.NotNull(created);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync($"/api/orders/{seed.OrderId}/items/{created.OrderItemId}")).StatusCode);
        request.Quantity = 3;
        Assert.Equal(HttpStatusCode.OK, (await client.PutAsJsonAsync($"/api/orders/{seed.OrderId}/items/{created.OrderItemId}", request)).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync($"/api/orders/{seed.OrderId}/items", new OrderItemRequest())).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/api/orders/{seed.OrderId}/items/999999")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.PutAsJsonAsync($"/api/orders/{seed.OrderId}/items/999999", request)).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync($"/api/orders/{seed.OrderId}/items/{created.OrderItemId}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.DeleteAsync($"/api/orders/{seed.OrderId}/items/{created.OrderItemId}")).StatusCode);
    }

    [Fact]
    public async Task RestaurantAttachmentsApi_CoversCrudValidationAndMissingIds()
    {
        await factory.ResetAsync();
        var seed = await ApiTestData.SeedAsync(factory);
        using var client = factory.CreateAdminClient();

        Assert.Equal(HttpStatusCode.OK,
            (await client.GetAsync($"/api/restaurant-attachments?restaurantId={seed.RestaurantId}&q=menu")).StatusCode);

        using var createContent = CreateFileContent(seed.RestaurantId, "menu.pdf", "test-pdf-content");
        var create = await client.PostAsync("/api/restaurant-attachments", createContent);
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var created = await create.Content.ReadFromJsonAsync<RestaurantAttachmentDto>();
        Assert.NotNull(created);
        Assert.Equal(seed.RestaurantId, created.Restaurant.RestaurantId);
        Assert.Equal(HttpStatusCode.OK,
            (await client.GetAsync($"/api/restaurant-attachments/{created.RestaurantAttachmentId}")).StatusCode);

        var update = new RestaurantAttachmentUpdateRequest { FileName = "novi-menu.pdf" };
        Assert.Equal(HttpStatusCode.OK,
            (await client.PutAsJsonAsync($"/api/restaurant-attachments/{created.RestaurantAttachmentId}", update)).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest,
            (await client.PutAsJsonAsync($"/api/restaurant-attachments/{created.RestaurantAttachmentId}",
                new RestaurantAttachmentUpdateRequest())).StatusCode);

        using var invalidContent = CreateFileContent(seed.RestaurantId, "skripta.exe", "invalid");
        Assert.Equal(HttpStatusCode.BadRequest,
            (await client.PostAsync("/api/restaurant-attachments", invalidContent)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/api/restaurant-attachments/999999")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound,
            (await client.PutAsJsonAsync("/api/restaurant-attachments/999999", update)).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent,
            (await client.DeleteAsync($"/api/restaurant-attachments/{created.RestaurantAttachmentId}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound,
            (await client.DeleteAsync($"/api/restaurant-attachments/{created.RestaurantAttachmentId}")).StatusCode);
    }

    [Fact]
    public async Task RestaurantEdit_UploadsListsAndDeletesAttachmentWithAjaxEndpoints()
    {
        await factory.ResetAsync();
        var seed = await ApiTestData.SeedAsync(factory);
        using var client = factory.CreateAdminClient();

        var editPage = await client.GetStringAsync($"/restorani/edit/{seed.RestaurantId}");
        var token = Regex.Match(editPage, "name=\"__RequestVerificationToken\" type=\"hidden\" value=\"([^\"]+)\"")
            .Groups[1].Value;
        Assert.NotEmpty(token);

        using var uploadContent = new MultipartFormDataContent();
        uploadContent.Add(new ByteArrayContent("mvc-upload"u8.ToArray()), "file", "jelovnik.pdf");
        using var uploadRequest = new HttpRequestMessage(HttpMethod.Post,
            $"/restorani/attachments/upload/{seed.RestaurantId}") { Content = uploadContent };
        uploadRequest.Headers.Add("RequestVerificationToken", token);
        var upload = await client.SendAsync(uploadRequest);
        Assert.Equal(HttpStatusCode.OK, upload.StatusCode);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FoodOrderingLab2.Data.ApplicationDbContext>();
        var attachment = Assert.Single(db.RestaurantAttachments);
        var list = await client.GetStringAsync($"/restorani/attachments/{seed.RestaurantId}");
        Assert.Contains("jelovnik.pdf", list);

        using var deleteRequest = new HttpRequestMessage(HttpMethod.Post,
            $"/restorani/attachments/delete/{attachment.RestaurantAttachmentId}");
        deleteRequest.Headers.Add("RequestVerificationToken", token);
        Assert.Equal(HttpStatusCode.OK, (await client.SendAsync(deleteRequest)).StatusCode);
        Assert.Empty(db.RestaurantAttachments);
    }

    [Fact]
    public async Task RestaurantDetails_ShowsPublicAttachmentsAndEmptyState()
    {
        await factory.ResetAsync();
        var seed = await ApiTestData.SeedAsync(factory);
        using var client = factory.CreateCustomerClient();

        var emptyPage = await client.GetStringAsync($"/restorani/{seed.RestaurantId}");
        Assert.Contains("No files available yet", emptyPage);

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.RestaurantAttachments.Add(new RestaurantAttachment
            {
                RestaurantId = seed.RestaurantId,
                FileName = "javni-jelovnik.pdf",
                FilePath = "/uploads/restaurants/javni-jelovnik.pdf",
                ContentType = "application/pdf",
                FileSize = 2048,
                CreatedAt = DateTime.UtcNow
            });
            await db.SaveChangesAsync();
        }

        var page = await client.GetStringAsync($"/restorani/{seed.RestaurantId}");
        Assert.Contains("Menus & Documents", page);
        Assert.Contains("javni-jelovnik.pdf", page);
        Assert.Contains("Open file", page);
        Assert.DoesNotContain("No files available yet", page);
        Assert.DoesNotContain("delete-attachment", page);
    }

    [Fact]
    public async Task Authorization_ReturnsExpectedStatuses()
    {
        await factory.ResetAsync();
        var seed = await ApiTestData.SeedAsync(factory);
        int attachmentId;
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var attachment = new RestaurantAttachment
            {
                RestaurantId = seed.RestaurantId,
                FileName = "test.pdf",
                FilePath = "/uploads/restaurants/test.pdf",
                ContentType = "application/pdf",
                FileSize = 10,
                CreatedAt = DateTime.UtcNow
            };
            db.RestaurantAttachments.Add(attachment);
            await db.SaveChangesAsync();
            attachmentId = attachment.RestaurantAttachmentId;
        }

        using var anonymous = factory.CreateClient();
        var publicLists = new[]
        {
            "/api/customers",
            "/api/restaurants",
            "/api/menu-items",
            "/api/orders",
            $"/api/orders/{seed.OrderId}/items",
            "/api/restaurant-attachments"
        };
        foreach (var path in publicLists)
        {
            Assert.Equal(HttpStatusCode.OK, (await anonymous.GetAsync(path)).StatusCode);
        }

        var protectedDetails = new[]
        {
            $"/api/customers/{seed.CustomerId}",
            $"/api/restaurants/{seed.RestaurantId}",
            $"/api/menu-items/{seed.MenuItemId}",
            $"/api/orders/{seed.OrderId}",
            $"/api/orders/{seed.OrderId}/items/{seed.OrderItemId}",
            $"/api/restaurant-attachments/{attachmentId}"
        };
        foreach (var path in protectedDetails)
        {
            Assert.Equal(HttpStatusCode.Unauthorized, (await anonymous.GetAsync(path)).StatusCode);
        }

        using var manager = factory.CreateManagerClient();
        var adminOnlyDeletes = new[]
        {
            $"/api/customers/{seed.CustomerId}",
            $"/api/restaurants/{seed.RestaurantId}",
            $"/api/menu-items/{seed.MenuItemId}",
            $"/api/orders/{seed.OrderId}",
            $"/api/orders/{seed.OrderId}/items/{seed.OrderItemId}",
            $"/api/restaurant-attachments/{attachmentId}"
        };
        foreach (var path in adminOnlyDeletes)
        {
            Assert.Equal(HttpStatusCode.Forbidden, (await manager.DeleteAsync(path)).StatusCode);
        }

        Assert.Equal(HttpStatusCode.Created,
            (await manager.PostAsJsonAsync("/api/customers", new CustomerRequest
            {
                FirstName = "Manager", LastName = "Test", Email = "manager-test@example.com", Phone = "+385 91 555 5555",
                Address = "Testna 5", RegisterDate = DateTime.UtcNow
            })).StatusCode);
    }

    [Fact]
    public async Task ApiDtos_ExposeNestedDataWithoutIdentityInternals()
    {
        await factory.ResetAsync();
        var seed = await ApiTestData.SeedAsync(factory);
        using var authenticated = factory.CreateCustomerClient();

        var customerJson = await authenticated.GetStringAsync($"/api/customers/{seed.CustomerId}");
        Assert.Contains("\"customerId\"", customerJson);
        Assert.DoesNotContain("appUserId", customerJson, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("passwordHash", customerJson, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("\"oib\"", customerJson, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("\"jmbg\"", customerJson, StringComparison.OrdinalIgnoreCase);

        var order = await authenticated.GetFromJsonAsync<OrderDto>($"/api/orders/{seed.OrderId}");
        Assert.NotNull(order);
        Assert.Equal(seed.CustomerId, order.Customer.CustomerId);
        Assert.Equal(seed.RestaurantId, order.Restaurant.RestaurantId);
        Assert.NotEmpty(order.Items);
        Assert.Equal(seed.MenuItemId, order.Items.First().MenuItem.MenuItemId);
    }

    [Fact]
    public async Task Registration_CreatesLinkedCustomer()
    {
        await factory.ResetAsync();
        using var client = factory.CreateClient();
        var registerPage = await client.GetStringAsync("/Identity/Account/Register");
        var token = ExtractAntiForgeryToken(registerPage);
        var email = $"customer-{Guid.NewGuid():N}@example.com";

        var response = await client.PostAsync("/Identity/Account/Register", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Input.FirstName"] = "Novi",
            ["Input.LastName"] = "Kupac",
            ["Input.Email"] = email,
            ["Input.Phone"] = "+385 91 444 4444",
            ["Input.Address"] = "Nova ulica 4",
            ["Input.OIB"] = "12345678901",
            ["Input.JMBG"] = "1234567890123",
            ["Input.Password"] = "Customer123!",
            ["Input.ConfirmPassword"] = "Customer123!"
        }));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FoodOrderingLab2.Data.ApplicationDbContext>();
        var customer = Assert.Single(db.Customers);
        Assert.Equal(email, customer.Email);
        Assert.NotNull(customer.AppUserId);
    }

    [Fact]
    public async Task Customer_CreatesOrderForSelfAndCannotSeeAnotherCustomersOrder()
    {
        await factory.ResetAsync();
        var seed = await ApiTestData.SeedAsync(factory);
        int otherOrderId;
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<FoodOrderingLab2.Data.ApplicationDbContext>();
            var otherCustomer = new FoodOrderingLab2.Models.Customer
            {
                AppUserId = "other-user", FirstName = "Drugi", LastName = "Kupac", Email = "drugi@example.com",
                Phone = "+385 91 333 3333", Address = "Druga 3", RegisterDate = DateTime.UtcNow
            };
            db.Customers.Add(otherCustomer);
            var otherOrder = new FoodOrderingLab2.Models.Order
            {
                Customer = otherCustomer,
                RestaurantId = seed.RestaurantId,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                TotalPrice = 5m,
                OrderItems =
                [
                    new FoodOrderingLab2.Models.OrderItem
                    {
                        MenuItemId = seed.SecondMenuItemId, Quantity = 1, UnitPrice = 5m
                    }
                ]
            };
            db.Orders.Add(otherOrder);
            await db.SaveChangesAsync();
            otherOrderId = otherOrder.OrderId;
        }

        using var client = factory.CreateCustomerClient();
        var index = await client.GetStringAsync("/narudzbe");
        Assert.Contains($"Narudžba #{seed.OrderId}", index);
        Assert.DoesNotContain("Drugi Kupac", index);
        Assert.Equal(HttpStatusCode.Forbidden, (await client.GetAsync($"/narudzbe/{otherOrderId}")).StatusCode);

        var search = await client.GetStringAsync($"/narudzbe/search?q=Narud%C5%BEba%20%23{seed.OrderId}");
        Assert.Contains($"\"id\":{seed.OrderId}", search);
        Assert.DoesNotContain($"\"id\":{otherOrderId}", search);

        var restaurants = await client.GetStringAsync("/restorani");
        Assert.DoesNotContain("+ Novi restoran", restaurants);
        Assert.DoesNotContain("/restorani/edit/", restaurants);

        var createPage = await client.GetStringAsync("/narudzbe/create");
        Assert.DoesNotContain("customer-autocomplete", createPage);
        var token = ExtractAntiForgeryToken(createPage);
        var response = await client.PostAsync("/narudzbe/create", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["CustomerId"] = "999999",
            ["RestaurantId"] = seed.RestaurantId.ToString(),
            ["OrderDate"] = DateTime.UtcNow.ToString("O"),
            ["Items[0].MenuItemId"] = seed.SecondMenuItemId.ToString(),
            ["Items[0].Quantity"] = "2"
        }));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var verifyScope = factory.Services.CreateScope();
        var verifyDb = verifyScope.ServiceProvider.GetRequiredService<FoodOrderingLab2.Data.ApplicationDbContext>();
        var newest = verifyDb.Orders.OrderByDescending(x => x.OrderId).First();
        Assert.Equal(seed.CustomerId, newest.CustomerId);
        Assert.Equal(OrderStatus.Pending, newest.Status);
    }

    [Fact]
    public async Task Forms_RenderExpectedUxAndRejectInvalidRelationships()
    {
        await factory.ResetAsync();
        using var anonymous = factory.CreateClient();
        var login = await anonymous.GetStringAsync("/Identity/Account/Login");
        Assert.Contains("Input.RememberMe", login);
        Assert.DoesNotContain("Resend", login, StringComparison.OrdinalIgnoreCase);

        var register = await anonymous.GetStringAsync("/Identity/Account/Register");
        Assert.Contains("type=\"tel\"", register);
        Assert.Contains("+385 91 123 4567", register);

        using var admin = factory.CreateAdminClient();
        var restaurantPage = await admin.GetStringAsync("/restorani/create");
        Assert.Contains("class=\"table-scroll\"", restaurantPage);
        var restaurantToken = ExtractAntiForgeryToken(restaurantPage);
        var invalidRestaurant = await admin.PostAsync("/restorani/create", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = restaurantToken,
            ["Name"] = "Restoran bez jela",
            ["Address"] = "Testna 1",
            ["Phone"] = "+385 1 123 4567",
            ["Email"] = "restoran@example.com",
            ["Rating"] = "4.5",
            ["OpeningHours"] = "09:00 - 22:00"
        }));
        Assert.Equal(HttpStatusCode.OK, invalidRestaurant.StatusCode);
        Assert.Contains("Dodaj barem jedno jelo restoranu", await invalidRestaurant.Content.ReadAsStringAsync());

        var menuItemPage = await admin.GetStringAsync("/meni/create");
        var menuItemToken = ExtractAntiForgeryToken(menuItemPage);
        var invalidMenuItem = await admin.PostAsync("/meni/create", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = menuItemToken,
            ["RestaurantId"] = "999999",
            ["Name"] = "Test jelo",
            ["Description"] = "Valjani opis",
            ["Price"] = "10",
            ["Category"] = "2",
            ["Calories"] = "300",
            ["IsAvailable"] = "true"
        }));
        Assert.Equal(HttpStatusCode.OK, invalidMenuItem.StatusCode);
        Assert.Contains("Odabrani restoran ne postoji", await invalidMenuItem.Content.ReadAsStringAsync());

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FoodOrderingLab2.Data.ApplicationDbContext>();
        Assert.Empty(db.Restaurants);
        Assert.Empty(db.MenuItems);
    }

    [Fact]
    public async Task ExistingIdentityUserWithoutCustomer_CanRecoverCustomerProfile()
    {
        await factory.ResetAsync();
        using (var scope = factory.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var result = await userManager.CreateAsync(new AppUser
            {
                Id = "orphan-user",
                UserName = "orphan@example.com",
                Email = "orphan@example.com",
                OIB = "12345678901",
                JMBG = "1234567890123"
            }, "Customer123!");
            Assert.True(result.Succeeded);
        }

        using var client = factory.CreateCustomerClient("orphan-user");
        var page = await client.GetStringAsync("/Identity/Account/Register");
        Assert.Contains("Obnovi profil kupca", page);
        Assert.DoesNotContain("Input.Password", page);

        var response = await client.PostAsync("/Identity/Account/Register", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = ExtractAntiForgeryToken(page),
            ["Input.FirstName"] = "Obnovljeni",
            ["Input.LastName"] = "Kupac",
            ["Input.Phone"] = "+385 91 555 5555",
            ["Input.Address"] = "Nova adresa 5"
        }));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var verifyScope = factory.Services.CreateScope();
        var db = verifyScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var customer = Assert.Single(db.Customers);
        Assert.Equal("orphan-user", customer.AppUserId);
        Assert.Equal("orphan@example.com", customer.Email);
    }

    [Fact]
    public async Task Customer_CanEditOnlyOwnProfile()
    {
        await factory.ResetAsync();
        var seed = await ApiTestData.SeedAsync(factory);
        int otherCustomerId;
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var other = new Customer
            {
                AppUserId = "other-user",
                FirstName = "Drugi",
                LastName = "Kupac",
                Email = "other@example.com",
                Phone = "+385 91 333 3333",
                Address = "Druga adresa 3",
                RegisterDate = DateTime.UtcNow
            };
            db.Customers.Add(other);
            await db.SaveChangesAsync();
            otherCustomerId = other.CustomerId;
        }

        using var client = factory.CreateCustomerClient();
        var page = await client.GetStringAsync("/kupci/moj-profil");
        Assert.Contains("My Profile", page);
        Assert.Contains("test@example.com", page);
        Assert.Equal(HttpStatusCode.Forbidden, (await client.GetAsync($"/kupci/edit/{otherCustomerId}")).StatusCode);

        var response = await client.PostAsync("/kupci/moj-profil", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = ExtractAntiForgeryToken(page),
            ["CustomerId"] = otherCustomerId.ToString(),
            ["FirstName"] = "Promijenjeni",
            ["LastName"] = "Kupac",
            ["Phone"] = "+385 91 777 7777",
            ["Address"] = "Nova vlastita adresa"
        }));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var verifyScope = factory.Services.CreateScope();
        var verifyDb = verifyScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var own = verifyDb.Customers.Single(x => x.CustomerId == seed.CustomerId);
        var otherCustomer = verifyDb.Customers.Single(x => x.CustomerId == otherCustomerId);
        Assert.Equal("Promijenjeni", own.FirstName);
        Assert.Equal("Nova vlastita adresa", own.Address);
        Assert.Equal("Drugi", otherCustomer.FirstName);
        Assert.Equal("Druga adresa 3", otherCustomer.Address);
    }

    [Fact]
    public async Task DeletingLinkedCustomer_AlsoDeletesIdentityAccount()
    {
        await factory.ResetAsync();
        int customerId;
        using (var scope = factory.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var user = new AppUser
            {
                UserName = "delete-me@example.com",
                Email = "delete-me@example.com",
                OIB = "12345678901",
                JMBG = "1234567890123"
            };
            Assert.True((await userManager.CreateAsync(user, "Customer123!")).Succeeded);

            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var customer = new Customer
            {
                AppUserId = user.Id,
                FirstName = "Delete",
                LastName = "Me",
                Email = user.Email!,
                Phone = "+385 91 222 2222",
                Address = "Adresa 2",
                RegisterDate = DateTime.UtcNow
            };
            db.Customers.Add(customer);
            await db.SaveChangesAsync();
            customerId = customer.CustomerId;
        }

        using var admin = factory.CreateAdminClient();
        Assert.Equal(HttpStatusCode.NoContent, (await admin.DeleteAsync($"/api/customers/{customerId}")).StatusCode);

        using var verifyScope = factory.Services.CreateScope();
        var verifyDb = verifyScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var verifyUserManager = verifyScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        Assert.Empty(verifyDb.Customers);
        Assert.Null(await verifyUserManager.FindByEmailAsync("delete-me@example.com"));
    }

    private static MultipartFormDataContent CreateFileContent(int restaurantId, string fileName, string content)
    {
        var multipart = new MultipartFormDataContent();
        multipart.Add(new StringContent(restaurantId.ToString()), "restaurantId");
        multipart.Add(new ByteArrayContent(System.Text.Encoding.UTF8.GetBytes(content)), "file", fileName);
        return multipart;
    }

    private static string ExtractAntiForgeryToken(string html) =>
        Regex.Match(html, "name=\"__RequestVerificationToken\" type=\"hidden\" value=\"([^\"]+)\"").Groups[1].Value;
}
