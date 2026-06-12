using System.ComponentModel.DataAnnotations;
using FoodOrderingLab2.Models;
using FoodOrderingLab2.Models.Enums;
using FoodOrderingLab2.Validation;

namespace FoodOrderingLab2.Dtos;

public record CustomerSummaryDto(int CustomerId, string FullName, string Email);
public record RestaurantSummaryDto(int RestaurantId, string Name, string Address, decimal Rating);
public record MenuItemSummaryDto(int MenuItemId, string Name, decimal Price, FoodCategory Category, bool IsAvailable);

public record CustomerDto(
    int CustomerId,
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string Address,
    DateTime RegisterDate);

public class CustomerRequest
{
    [Required, StringLength(100)] public string FirstName { get; set; } = null!;
    [Required, StringLength(100)] public string LastName { get; set; } = null!;
    [Required, EmailAddress, StringLength(256)] public string Email { get; set; } = null!;
    [Required, Phone, CroatianPhone, StringLength(50)] public string Phone { get; set; } = null!;
    [Required, StringLength(300)] public string Address { get; set; } = null!;
    [Required] public DateTime RegisterDate { get; set; }
}

public record RestaurantDto(
    int RestaurantId,
    string Name,
    string Address,
    string Phone,
    string Email,
    decimal Rating,
    string OpeningHours,
    IReadOnlyCollection<MenuItemSummaryDto> MenuItems);

public class RestaurantRequest
{
    [Required, StringLength(150)] public string Name { get; set; } = null!;
    [Required, StringLength(300)] public string Address { get; set; } = null!;
    [Required, Phone, CroatianPhone, StringLength(50)] public string Phone { get; set; } = null!;
    [Required, EmailAddress, StringLength(256)] public string Email { get; set; } = null!;
    [Range(0, 5)] public decimal Rating { get; set; }
    [Required, StringLength(100)] public string OpeningHours { get; set; } = null!;
}

public record MenuItemDto(
    int MenuItemId,
    string Name,
    string Description,
    decimal Price,
    FoodCategory Category,
    int Calories,
    bool IsAvailable,
    RestaurantSummaryDto Restaurant);

public class MenuItemRequest
{
    [Range(1, int.MaxValue)] public int RestaurantId { get; set; }
    [Required, StringLength(150)] public string Name { get; set; } = null!;
    [Required, StringLength(1000)] public string Description { get; set; } = null!;
    [Range(0.01, 9999.99)] public decimal Price { get; set; }
    [EnumDataType(typeof(FoodCategory))] public FoodCategory Category { get; set; }
    [Range(0, 10000)] public int Calories { get; set; }
    public bool IsAvailable { get; set; }
}

public record OrderItemDto(
    int OrderItemId,
    int Quantity,
    decimal UnitPrice,
    decimal TotalItemPrice,
    string? SpecialRequests,
    MenuItemSummaryDto MenuItem);

public class OrderItemRequest
{
    [Range(1, int.MaxValue)] public int MenuItemId { get; set; }
    [Range(1, int.MaxValue)] public int Quantity { get; set; }
    [StringLength(500)] public string? SpecialRequests { get; set; }
}

public record OrderDto(
    int OrderId,
    DateTime OrderDate,
    decimal TotalPrice,
    OrderStatus Status,
    CustomerSummaryDto Customer,
    RestaurantSummaryDto Restaurant,
    IReadOnlyCollection<OrderItemDto> Items);

public class OrderRequest
{
    [Range(1, int.MaxValue)] public int CustomerId { get; set; }
    [Range(1, int.MaxValue)] public int RestaurantId { get; set; }
    [Required] public DateTime OrderDate { get; set; }
    [EnumDataType(typeof(OrderStatus))] public OrderStatus Status { get; set; }
    [MinLength(1)] public List<OrderItemRequest> Items { get; set; } = [];
}

public record RestaurantAttachmentDto(
    int RestaurantAttachmentId,
    string FileName,
    string FilePath,
    string ContentType,
    long FileSize,
    DateTime CreatedAt,
    RestaurantSummaryDto Restaurant);

public class RestaurantAttachmentUpdateRequest
{
    [Required, StringLength(260)]
    public string FileName { get; set; } = null!;
}

public static class DtoMappings
{
    public static CustomerSummaryDto ToSummaryDto(this Customer value) =>
        new(value.CustomerId, value.FullName, value.Email);

    public static RestaurantSummaryDto ToSummaryDto(this Restaurant value) =>
        new(value.RestaurantId, value.Name, value.Address, value.Rating);

    public static MenuItemSummaryDto ToSummaryDto(this MenuItem value) =>
        new(value.MenuItemId, value.Name, value.Price, value.Category, value.IsAvailable);

    public static CustomerDto ToDto(this Customer value) =>
        new(value.CustomerId, value.FirstName, value.LastName, value.Email, value.Phone, value.Address, value.RegisterDate);

    public static RestaurantDto ToDto(this Restaurant value) =>
        new(value.RestaurantId, value.Name, value.Address, value.Phone, value.Email, value.Rating, value.OpeningHours,
            value.MenuItems.Select(x => x.ToSummaryDto()).ToList());

    public static MenuItemDto ToDto(this MenuItem value) =>
        new(value.MenuItemId, value.Name, value.Description, value.Price, value.Category, value.Calories,
            value.IsAvailable, value.Restaurant.ToSummaryDto());

    public static OrderItemDto ToDto(this OrderItem value) =>
        new(value.OrderItemId, value.Quantity, value.UnitPrice, value.TotalItemPrice, value.SpecialRequests,
            value.MenuItem.ToSummaryDto());

    public static OrderDto ToDto(this Order value) =>
        new(value.OrderId, value.OrderDate, value.TotalPrice, value.Status, value.Customer.ToSummaryDto(),
            value.Restaurant.ToSummaryDto(), value.OrderItems.Select(x => x.ToDto()).ToList());

    public static RestaurantAttachmentDto ToDto(this RestaurantAttachment value) =>
        new(value.RestaurantAttachmentId, value.FileName, value.FilePath, value.ContentType, value.FileSize, value.CreatedAt,
            value.Restaurant.ToSummaryDto());
}
