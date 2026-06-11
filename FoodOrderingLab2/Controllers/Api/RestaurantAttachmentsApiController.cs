using FoodOrderingLab2.Data;
using FoodOrderingLab2.Dtos;
using FoodOrderingLab2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderingLab2.Controllers.Api;

[ApiController]
[Route("api/restaurant-attachments")]
public class RestaurantAttachmentsApiController(ApplicationDbContext db, IWebHostEnvironment environment) : ControllerBase
{
    private const long MaximumFileSize = 10_000_000;
    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp", ".pdf" };

    [AllowAnonymous, HttpGet]
    public async Task<ActionResult<IEnumerable<RestaurantAttachmentDto>>> GetAll(
        [FromQuery] string? q,
        [FromQuery] int? restaurantId)
    {
        var query = FullQuery().AsNoTracking();
        if (!string.IsNullOrWhiteSpace(q)) query = query.Where(x => x.FileName.Contains(q));
        if (restaurantId.HasValue) query = query.Where(x => x.RestaurantId == restaurantId);

        return Ok((await query.OrderByDescending(x => x.CreatedAt).ToListAsync()).Select(x => x.ToDto()));
    }

    [Authorize, HttpGet("{id:int}")]
    public async Task<ActionResult<RestaurantAttachmentDto>> GetById(int id)
    {
        var value = await FullQuery().AsNoTracking()
            .FirstOrDefaultAsync(x => x.RestaurantAttachmentId == id);
        return value == null ? NotFound() : Ok(value.ToDto());
    }

    [Authorize(Roles = "Admin,Manager"), HttpPost]
    [RequestSizeLimit(MaximumFileSize)]
    public async Task<ActionResult<RestaurantAttachmentDto>> Create([FromForm] int restaurantId, [FromForm] IFormFile file)
    {
        var restaurant = await db.Restaurants.FirstOrDefaultAsync(x => x.RestaurantId == restaurantId);
        if (restaurant == null) return BadRequest("Odabrani restoran ne postoji.");

        var validationError = ValidateFile(file);
        if (validationError != null) return BadRequest(validationError);

        var relativeDirectory = Path.Combine("uploads", "restaurants", restaurantId.ToString());
        var physicalDirectory = Path.Combine(environment.WebRootPath, relativeDirectory);
        Directory.CreateDirectory(physicalDirectory);
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var storedFileName = $"{Guid.NewGuid():N}{extension}";
        var physicalPath = Path.Combine(physicalDirectory, storedFileName);

        await using (var stream = System.IO.File.Create(physicalPath))
        {
            await file.CopyToAsync(stream);
        }

        var value = new RestaurantAttachment
        {
            RestaurantId = restaurantId,
            Restaurant = restaurant,
            FileName = Path.GetFileName(file.FileName),
            FilePath = "/" + Path.Combine(relativeDirectory, storedFileName).Replace('\\', '/'),
            ContentType = file.ContentType,
            FileSize = file.Length,
            CreatedAt = DateTime.UtcNow
        };
        db.RestaurantAttachments.Add(value);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = value.RestaurantAttachmentId }, value.ToDto());
    }

    [Authorize(Roles = "Admin,Manager"), HttpPut("{id:int}")]
    public async Task<ActionResult<RestaurantAttachmentDto>> Update(int id, RestaurantAttachmentUpdateRequest request)
    {
        var value = await FullQuery().FirstOrDefaultAsync(x => x.RestaurantAttachmentId == id);
        if (value == null) return NotFound();

        value.FileName = Path.GetFileName(request.FileName);
        await db.SaveChangesAsync();
        return Ok(value.ToDto());
    }

    [Authorize(Roles = "Admin"), HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var value = await db.RestaurantAttachments.FindAsync(id);
        if (value == null) return NotFound();

        DeletePhysicalFile(value.FilePath);
        db.RestaurantAttachments.Remove(value);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private IQueryable<RestaurantAttachment> FullQuery() =>
        db.RestaurantAttachments.Include(x => x.Restaurant);

    private static string? ValidateFile(IFormFile? file)
    {
        if (file == null || file.Length == 0) return "Datoteka je prazna.";
        if (file.Length > MaximumFileSize) return "Datoteka smije imati najviše 10 MB.";
        return AllowedExtensions.Contains(Path.GetExtension(file.FileName))
            ? null
            : "Dopušteni formati su JPG, PNG, WEBP i PDF.";
    }

    private void DeletePhysicalFile(string relativePath)
    {
        var physicalPath = Path.GetFullPath(Path.Combine(environment.WebRootPath, relativePath.TrimStart('/')));
        var uploadsRoot = Path.GetFullPath(Path.Combine(environment.WebRootPath, "uploads", "restaurants")) +
                          Path.DirectorySeparatorChar;
        if (physicalPath.StartsWith(uploadsRoot, StringComparison.OrdinalIgnoreCase) && System.IO.File.Exists(physicalPath))
        {
            System.IO.File.Delete(physicalPath);
        }
    }
}
