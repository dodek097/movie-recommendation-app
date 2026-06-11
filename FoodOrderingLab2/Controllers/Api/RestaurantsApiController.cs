using FoodOrderingLab2.Data;
using FoodOrderingLab2.Dtos;
using FoodOrderingLab2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderingLab2.Controllers.Api;

[ApiController]
[Route("api/restaurants")]
public class RestaurantsApiController(ApplicationDbContext db) : ControllerBase
{
    [AllowAnonymous, HttpGet]
    public async Task<ActionResult<IEnumerable<RestaurantDto>>> GetAll([FromQuery] string? q, [FromQuery] decimal? minRating)
    {
        var query = db.Restaurants.Include(x => x.MenuItems).AsNoTracking();
        if (!string.IsNullOrWhiteSpace(q)) query = query.Where(x => x.Name.Contains(q) || x.Address.Contains(q));
        if (minRating.HasValue) query = query.Where(x => x.Rating >= minRating);
        return Ok((await query.OrderBy(x => x.Name).ToListAsync()).Select(x => x.ToDto()));
    }

    [Authorize, HttpGet("{id:int}")]
    public async Task<ActionResult<RestaurantDto>> GetById(int id)
    {
        var value = await db.Restaurants.Include(x => x.MenuItems).AsNoTracking()
            .FirstOrDefaultAsync(x => x.RestaurantId == id);
        return value == null ? NotFound() : Ok(value.ToDto());
    }

    [Authorize(Roles = "Admin,Manager"), HttpPost]
    public async Task<ActionResult<RestaurantDto>> Create(RestaurantRequest request)
    {
        var value = new Restaurant();
        Apply(request, value);
        db.Restaurants.Add(value);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = value.RestaurantId }, value.ToDto());
    }

    [Authorize(Roles = "Admin,Manager"), HttpPut("{id:int}")]
    public async Task<ActionResult<RestaurantDto>> Update(int id, RestaurantRequest request)
    {
        var value = await db.Restaurants.Include(x => x.MenuItems).FirstOrDefaultAsync(x => x.RestaurantId == id);
        if (value == null) return NotFound();
        Apply(request, value);
        await db.SaveChangesAsync();
        return Ok(value.ToDto());
    }

    [Authorize(Roles = "Admin"), HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var value = await db.Restaurants.Include(x => x.Orders).FirstOrDefaultAsync(x => x.RestaurantId == id);
        if (value == null) return NotFound();
        if (value.Orders.Count != 0) return Conflict("Restoran s narudžbama ne može se obrisati.");
        db.Restaurants.Remove(value);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private static void Apply(RestaurantRequest request, Restaurant value)
    {
        value.Name = request.Name;
        value.Address = request.Address;
        value.Phone = request.Phone;
        value.Email = request.Email;
        value.Rating = request.Rating;
        value.OpeningHours = request.OpeningHours;
    }
}
