using FoodOrderingLab2.Data;
using FoodOrderingLab2.Dtos;
using FoodOrderingLab2.Models;
using FoodOrderingLab2.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderingLab2.Controllers.Api;

[ApiController]
[Route("api/orders")]
public class OrdersApiController(ApplicationDbContext db) : ControllerBase
{
    [AllowAnonymous, HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll(
        [FromQuery] string? q, [FromQuery] OrderStatus? status, [FromQuery] int? customerId, [FromQuery] int? restaurantId)
    {
        var query = FullQuery().AsNoTracking();
        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(x => x.Customer.FirstName.Contains(q) || x.Customer.LastName.Contains(q) || x.Restaurant.Name.Contains(q));
        if (status.HasValue) query = query.Where(x => x.Status == status);
        if (customerId.HasValue) query = query.Where(x => x.CustomerId == customerId);
        if (restaurantId.HasValue) query = query.Where(x => x.RestaurantId == restaurantId);
        return Ok((await query.OrderByDescending(x => x.OrderDate).ToListAsync()).Select(x => x.ToDto()));
    }

    [Authorize, HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> GetById(int id)
    {
        var value = await FullQuery().AsNoTracking().FirstOrDefaultAsync(x => x.OrderId == id);
        return value == null ? NotFound() : Ok(value.ToDto());
    }

    [Authorize(Roles = "Admin,Manager"), HttpPost]
    public async Task<ActionResult<OrderDto>> Create(OrderRequest request)
    {
        var validationError = await ValidateRequest(request);
        if (validationError != null) return BadRequest(validationError);

        var value = new Order
        {
            CustomerId = request.CustomerId,
            RestaurantId = request.RestaurantId,
            OrderDate = request.OrderDate,
            Status = request.Status
        };
        await ReplaceItems(value, request.Items);
        db.Orders.Add(value);
        await db.SaveChangesAsync();
        var created = await FullQuery().AsNoTracking().FirstAsync(x => x.OrderId == value.OrderId);
        return CreatedAtAction(nameof(GetById), new { id = value.OrderId }, created.ToDto());
    }

    [Authorize(Roles = "Admin,Manager"), HttpPut("{id:int}")]
    public async Task<ActionResult<OrderDto>> Update(int id, OrderRequest request)
    {
        var value = await db.Orders.Include(x => x.OrderItems).FirstOrDefaultAsync(x => x.OrderId == id);
        if (value == null) return NotFound();
        var validationError = await ValidateRequest(request);
        if (validationError != null) return BadRequest(validationError);

        value.CustomerId = request.CustomerId;
        value.RestaurantId = request.RestaurantId;
        value.OrderDate = request.OrderDate;
        value.Status = request.Status;
        db.OrderItems.RemoveRange(value.OrderItems);
        value.OrderItems.Clear();
        await ReplaceItems(value, request.Items);
        await db.SaveChangesAsync();
        var updated = await FullQuery().AsNoTracking().FirstAsync(x => x.OrderId == id);
        return Ok(updated.ToDto());
    }

    [Authorize(Roles = "Admin"), HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var value = await db.Orders.Include(x => x.OrderItems).FirstOrDefaultAsync(x => x.OrderId == id);
        if (value == null) return NotFound();
        db.OrderItems.RemoveRange(value.OrderItems);
        db.Orders.Remove(value);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private IQueryable<Order> FullQuery() =>
        db.Orders.Include(x => x.Customer).Include(x => x.Restaurant).Include(x => x.OrderItems).ThenInclude(x => x.MenuItem);

    private async Task<string?> ValidateRequest(OrderRequest request)
    {
        if (!await db.Customers.AnyAsync(x => x.CustomerId == request.CustomerId)) return "Odabrani kupac ne postoji.";
        if (!await db.Restaurants.AnyAsync(x => x.RestaurantId == request.RestaurantId)) return "Odabrani restoran ne postoji.";
        var itemIds = request.Items.Select(x => x.MenuItemId).Distinct().ToList();
        var validCount = await db.MenuItems.CountAsync(x => itemIds.Contains(x.MenuItemId) && x.RestaurantId == request.RestaurantId && x.IsAvailable);
        return validCount == itemIds.Count ? null : "Sve stavke moraju postojati, biti dostupne i pripadati odabranom restoranu.";
    }

    private async Task ReplaceItems(Order order, IEnumerable<OrderItemRequest> requests)
    {
        var itemIds = requests.Select(x => x.MenuItemId).Distinct().ToList();
        var prices = await db.MenuItems.Where(x => itemIds.Contains(x.MenuItemId)).ToDictionaryAsync(x => x.MenuItemId, x => x.Price);
        foreach (var request in requests)
        {
            order.OrderItems.Add(new OrderItem
            {
                MenuItemId = request.MenuItemId,
                Quantity = request.Quantity,
                UnitPrice = prices[request.MenuItemId],
                SpecialRequests = request.SpecialRequests
            });
        }
        order.TotalPrice = order.OrderItems.Sum(x => x.TotalItemPrice);
    }
}
