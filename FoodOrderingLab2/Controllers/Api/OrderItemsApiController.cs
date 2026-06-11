using FoodOrderingLab2.Data;
using FoodOrderingLab2.Dtos;
using FoodOrderingLab2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderingLab2.Controllers.Api;

[ApiController]
[Route("api/orders/{orderId:int}/items")]
public class OrderItemsApiController(ApplicationDbContext db) : ControllerBase
{
    [AllowAnonymous, HttpGet]
    public async Task<ActionResult<IEnumerable<OrderItemDto>>> GetAll(int orderId)
    {
        if (!await db.Orders.AnyAsync(x => x.OrderId == orderId)) return NotFound();
        var values = await db.OrderItems.Include(x => x.MenuItem).AsNoTracking()
            .Where(x => x.OrderId == orderId).ToListAsync();
        return Ok(values.Select(x => x.ToDto()));
    }

    [Authorize, HttpGet("{id:int}")]
    public async Task<ActionResult<OrderItemDto>> GetById(int orderId, int id)
    {
        var value = await db.OrderItems.Include(x => x.MenuItem).AsNoTracking()
            .FirstOrDefaultAsync(x => x.OrderId == orderId && x.OrderItemId == id);
        return value == null ? NotFound() : Ok(value.ToDto());
    }

    [Authorize(Roles = "Admin,Manager"), HttpPost]
    public async Task<ActionResult<OrderItemDto>> Create(int orderId, OrderItemRequest request)
    {
        var order = await db.Orders.Include(x => x.OrderItems).FirstOrDefaultAsync(x => x.OrderId == orderId);
        if (order == null) return NotFound();
        var menuItem = await db.MenuItems.FirstOrDefaultAsync(x =>
            x.MenuItemId == request.MenuItemId && x.RestaurantId == order.RestaurantId && x.IsAvailable);
        if (menuItem == null) return BadRequest("Artikl mora biti dostupan i pripadati restoranu narudžbe.");
        var value = new OrderItem
        {
            OrderId = orderId,
            MenuItemId = menuItem.MenuItemId,
            MenuItem = menuItem,
            Quantity = request.Quantity,
            UnitPrice = menuItem.Price,
            SpecialRequests = request.SpecialRequests
        };
        db.OrderItems.Add(value);
        order.TotalPrice += value.TotalItemPrice;
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { orderId, id = value.OrderItemId }, value.ToDto());
    }

    [Authorize(Roles = "Admin,Manager"), HttpPut("{id:int}")]
    public async Task<ActionResult<OrderItemDto>> Update(int orderId, int id, OrderItemRequest request)
    {
        var order = await db.Orders.Include(x => x.OrderItems).FirstOrDefaultAsync(x => x.OrderId == orderId);
        if (order == null) return NotFound();
        var value = await db.OrderItems.Include(x => x.MenuItem)
            .FirstOrDefaultAsync(x => x.OrderId == orderId && x.OrderItemId == id);
        if (value == null) return NotFound();
        var menuItem = await db.MenuItems.FirstOrDefaultAsync(x =>
            x.MenuItemId == request.MenuItemId && x.RestaurantId == order.RestaurantId && x.IsAvailable);
        if (menuItem == null) return BadRequest("Artikl mora biti dostupan i pripadati restoranu narudžbe.");
        value.MenuItemId = menuItem.MenuItemId;
        value.MenuItem = menuItem;
        value.Quantity = request.Quantity;
        value.UnitPrice = menuItem.Price;
        value.SpecialRequests = request.SpecialRequests;
        order.TotalPrice = order.OrderItems.Sum(x => x.TotalItemPrice);
        await db.SaveChangesAsync();
        return Ok(value.ToDto());
    }

    [Authorize(Roles = "Admin"), HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int orderId, int id)
    {
        var order = await db.Orders.Include(x => x.OrderItems).FirstOrDefaultAsync(x => x.OrderId == orderId);
        if (order == null) return NotFound();
        var value = order.OrderItems.FirstOrDefault(x => x.OrderItemId == id);
        if (value == null) return NotFound();
        if (order.OrderItems.Count == 1) return Conflict("Narudžba mora imati barem jednu stavku.");
        db.OrderItems.Remove(value);
        order.TotalPrice = order.OrderItems.Where(x => x.OrderItemId != id).Sum(x => x.TotalItemPrice);
        await db.SaveChangesAsync();
        return NoContent();
    }

}
