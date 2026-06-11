using FoodOrderingLab2.Data;
using FoodOrderingLab2.Dtos;
using FoodOrderingLab2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace FoodOrderingLab2.Controllers.Api;

[ApiController]
[Route("api/customers")]
public class CustomersApiController(ApplicationDbContext db, UserManager<AppUser> userManager) : ControllerBase
{
    [AllowAnonymous, HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAll([FromQuery] string? q)
    {
        var query = db.Customers.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(x => x.FirstName.Contains(q) || x.LastName.Contains(q) || x.Email.Contains(q));
        }
        return Ok((await query.OrderBy(x => x.LastName).ToListAsync()).Select(x => x.ToDto()));
    }

    [Authorize, HttpGet("{id:int}")]
    public async Task<ActionResult<CustomerDto>> GetById(int id)
    {
        var value = await db.Customers.AsNoTracking().FirstOrDefaultAsync(x => x.CustomerId == id);
        return value == null ? NotFound() : Ok(value.ToDto());
    }

    [Authorize(Roles = "Admin,Manager"), HttpPost]
    public async Task<ActionResult<CustomerDto>> Create(CustomerRequest request)
    {
        var value = new Customer();
        Apply(request, value);
        db.Customers.Add(value);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = value.CustomerId }, value.ToDto());
    }

    [Authorize(Roles = "Admin,Manager"), HttpPut("{id:int}")]
    public async Task<ActionResult<CustomerDto>> Update(int id, CustomerRequest request)
    {
        var value = await db.Customers.FindAsync(id);
        if (value == null) return NotFound();
        Apply(request, value);
        await db.SaveChangesAsync();
        return Ok(value.ToDto());
    }

    [Authorize(Roles = "Admin"), HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var value = await db.Customers.Include(x => x.Orders).FirstOrDefaultAsync(x => x.CustomerId == id);
        if (value == null) return NotFound();
        if (value.Orders.Count != 0) return Conflict("Kupac s narudžbama ne može se obrisati.");
        if (!string.IsNullOrWhiteSpace(value.AppUserId))
        {
            var appUser = await userManager.FindByIdAsync(value.AppUserId);
            if (appUser != null)
            {
                var result = await userManager.DeleteAsync(appUser);
                if (!result.Succeeded) return Problem("Korisnički račun nije moguće obrisati.");
            }
        }
        db.Customers.Remove(value);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private static void Apply(CustomerRequest request, Customer value)
    {
        value.FirstName = request.FirstName;
        value.LastName = request.LastName;
        value.Email = request.Email;
        value.Phone = request.Phone;
        value.Address = request.Address;
        value.RegisterDate = request.RegisterDate;
    }
}
