using FoodOrderingLab2.Data;
using FoodOrderingLab2.Dtos;
using FoodOrderingLab2.Models;
using FoodOrderingLab2.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderingLab2.Controllers.Api;

[ApiController]
[Route("api/menu-items")]
public class MenuItemsApiController(ApplicationDbContext db) : ControllerBase
{
    [AllowAnonymous, HttpGet]
    public async Task<ActionResult<IEnumerable<MenuItemDto>>> GetAll(
        [FromQuery] string? q, [FromQuery] int? restaurantId, [FromQuery] FoodCategory? category, [FromQuery] bool? available)
    {
        var query = db.MenuItems.Include(x => x.Restaurant).AsNoTracking();
        if (!string.IsNullOrWhiteSpace(q)) query = query.Where(x => x.Name.Contains(q) || x.Description.Contains(q));
        if (restaurantId.HasValue) query = query.Where(x => x.RestaurantId == restaurantId);
        if (category.HasValue) query = query.Where(x => x.Category == category);
        if (available.HasValue) query = query.Where(x => x.IsAvailable == available);
        return Ok((await query.OrderBy(x => x.Name).ToListAsync()).Select(x => x.ToDto()));
    }

    [Authorize, HttpGet("{id:int}")]
    public async Task<ActionResult<MenuItemDto>> GetById(int id)
    {
        var value = await db.MenuItems.Include(x => x.Restaurant).AsNoTracking()
            .FirstOrDefaultAsync(x => x.MenuItemId == id);
        return value == null ? NotFound() : Ok(value.ToDto());
    }

    [Authorize(Roles = "Admin,Manager"), HttpPost]
    public async Task<ActionResult<MenuItemDto>> Create(MenuItemRequest request)
    {
        if (!await db.Restaurants.AnyAsync(x => x.RestaurantId == request.RestaurantId))
            return BadRequest("Odabrani restoran ne postoji.");
        var value = new MenuItem();
        Apply(request, value);
        db.MenuItems.Add(value);
        await db.SaveChangesAsync();
        await db.Entry(value).Reference(x => x.Restaurant).LoadAsync();
        return CreatedAtAction(nameof(GetById), new { id = value.MenuItemId }, value.ToDto());
    }

    [Authorize(Roles = "Admin,Manager"), HttpPut("{id:int}")]
    public async Task<ActionResult<MenuItemDto>> Update(int id, MenuItemRequest request)
    {
        var value = await db.MenuItems.Include(x => x.Restaurant).FirstOrDefaultAsync(x => x.MenuItemId == id);
        if (value == null) return NotFound();
        if (!await db.Restaurants.AnyAsync(x => x.RestaurantId == request.RestaurantId))
            return BadRequest("Odabrani restoran ne postoji.");
        Apply(request, value);
        await db.SaveChangesAsync();
        await db.Entry(value).Reference(x => x.Restaurant).LoadAsync();
        return Ok(value.ToDto());
    }

    [Authorize(Roles = "Admin"), HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var value = await db.MenuItems.Include(x => x.OrderItems).FirstOrDefaultAsync(x => x.MenuItemId == id);
        if (value == null) return NotFound();
        if (value.OrderItems.Count != 0) return Conflict("Artikl korišten u narudžbi ne može se obrisati.");
        db.MenuItems.Remove(value);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private static void Apply(MenuItemRequest request, MenuItem value)
    {
        value.RestaurantId = request.RestaurantId;
        value.Name = request.Name;
        value.Description = request.Description;
        value.Price = request.Price;
        value.Category = request.Category;
        value.Calories = request.Calories;
        value.IsAvailable = request.IsAvailable;
    }
}
