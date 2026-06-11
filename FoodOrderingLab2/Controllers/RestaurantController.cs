using Microsoft.AspNetCore.Mvc;
using FoodOrderingLab2.Repositories;
using FoodOrderingLab2.ViewModels;
using FoodOrderingLab2.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FoodOrderingLab2.Controllers
{
    [Route("restorani")]
    [Authorize]
    public class RestaurantController : Controller
    {
        private readonly RestaurantRepository _restaurantRepository;
        private readonly MenuItemRepository _menuItemRepository;
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;

        public RestaurantController(
            RestaurantRepository restaurantRepository,
            MenuItemRepository menuItemRepository,
            ApplicationDbContext dbContext,
            IWebHostEnvironment environment)
        {
            _restaurantRepository = restaurantRepository;
            _menuItemRepository = menuItemRepository;
            _dbContext = dbContext;
            _environment = environment;
        }

        [Route("")]
        [AllowAnonymous]
        public IActionResult Index()
        {
            var restaurants = _restaurantRepository.GetAll();
            return View(restaurants);
        }

        [HttpGet]
        [Route("create")]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Create()
        {
            var model = new RestaurantCreateViewModel();
            model.MenuItems.Add(new RestaurantMenuItemCreateModel());
            return View(model);
        }

        [HttpPost]
        [Route("create")]
        [Authorize(Roles = "Admin,Manager")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RestaurantCreateViewModel model)
        {
            model.MenuItems ??= [];
            if (!model.MenuItems.Any(mi => !string.IsNullOrWhiteSpace(mi.Name)))
            {
                ModelState.AddModelError(nameof(model.MenuItems), "Dodaj barem jedno jelo restoranu.");
            }

            if (!ModelState.IsValid)
            {
                if (!model.MenuItems.Any())
                {
                    model.MenuItems.Add(new RestaurantMenuItemCreateModel());
                }
                return View(model);
            }

            var restaurant = new Models.Restaurant
            {
                Name = model.Name,
                Address = model.Address,
                Phone = model.Phone,
                Email = model.Email,
                Rating = model.Rating,
                OpeningHours = model.OpeningHours
            };

            var validMenuItems = model.MenuItems
                .Where(mi => !string.IsNullOrWhiteSpace(mi.Name))
                .Select(mi => new Models.MenuItem
                {
                    Name = mi.Name,
                    Description = mi.Description,
                    Price = mi.Price,
                    Category = mi.Category,
                    Calories = mi.Calories,
                    IsAvailable = mi.IsAvailable,
                    Restaurant = restaurant
                })
                .ToList();

            foreach (var item in validMenuItems)
            {
                restaurant.MenuItems.Add(item);
            }

            _restaurantRepository.Add(restaurant);
            return RedirectToAction("Details", new { id = restaurant.RestaurantId });
        }

        [HttpGet]
        [Route("edit/{id:int}")]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Edit(int id)
        {
            var restaurant = _restaurantRepository.GetById(id);
            if (restaurant == null)
            {
                return NotFound();
            }

            var model = new RestaurantCreateViewModel
            {
                Name = restaurant.Name,
                Address = restaurant.Address,
                Phone = restaurant.Phone,
                Email = restaurant.Email,
                Rating = restaurant.Rating,
                OpeningHours = restaurant.OpeningHours
            };

            ViewBag.RestaurantId = id;
            return View(model);
        }

        [HttpPost]
        [Route("edit/{id:int}")]
        [Authorize(Roles = "Admin,Manager")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, RestaurantCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.RestaurantId = id;
                return View(model);
            }

            var restaurant = _restaurantRepository.GetById(id);
            if (restaurant == null)
            {
                return NotFound();
            }

            restaurant.Name = model.Name;
            restaurant.Address = model.Address;
            restaurant.Phone = model.Phone;
            restaurant.Email = model.Email;
            restaurant.Rating = model.Rating;
            restaurant.OpeningHours = model.OpeningHours;

            _restaurantRepository.Update(restaurant);
            return RedirectToAction("Details", new { id = restaurant.RestaurantId });
        }

        [HttpPost]
        [Route("delete/{id:int}")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var restaurant = _restaurantRepository.GetById(id);
            if (restaurant == null)
            {
                return NotFound();
            }

            if (restaurant.Orders.Any())
            {
                TempData["ErrorMessage"] = "Ne možete obrisati restoran koji ima narudžbe.";
                return RedirectToAction("Details", new { id });
            }

            try
            {
                _restaurantRepository.Delete(restaurant);
                TempData["SuccessMessage"] = "Restoran obrisan.";
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = "Ne mogu obrisati restoran: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        [Route("{id:int}")]
        public IActionResult Details(int id)
        {
            var restaurant = _restaurantRepository.GetById(id);
            if (restaurant == null)
            {
                return NotFound();
            }

            var viewModel = new RestaurantDetailViewModel
            {
                Restaurant = restaurant,
                MenuItems = _menuItemRepository.GetByRestaurantId(id),
                Attachments = _dbContext.RestaurantAttachments
                    .AsNoTracking()
                    .Where(x => x.RestaurantId == id)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToList()
            };

            ViewBag.ErrorMessage = TempData["ErrorMessage"];
            return View(viewModel);
        }

        [HttpGet]
        [Route("search")]
        [AllowAnonymous]
        public IActionResult Search(string q)
        {
            q = q ?? string.Empty;
            var results = _restaurantRepository.GetAll()
                .Where(r => r.Name.Contains(q, System.StringComparison.InvariantCultureIgnoreCase)
                            || r.Address.Contains(q, System.StringComparison.InvariantCultureIgnoreCase))
                .Select(r => new { id = r.RestaurantId, text = r.Name, name = r.Name, address = r.Address, rating = r.Rating.ToString("F2") })
                .Take(10)
                .ToList();

            return Json(results);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPost("attachments/upload/{restaurantId:int}")]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(10_000_000)]
        public async Task<IActionResult> UploadAttachment(int restaurantId, IFormFile file)
        {
            if (!await _dbContext.Restaurants.AnyAsync(x => x.RestaurantId == restaurantId)) return NotFound();
            if (file == null || file.Length == 0) return BadRequest("Datoteka je prazna.");
            if (file.Length > 10_000_000) return BadRequest("Datoteka smije imati najviše 10 MB.");

            var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp", ".pdf" };
            var extension = Path.GetExtension(file.FileName);
            if (!allowedExtensions.Contains(extension)) return BadRequest("Dopušteni formati su JPG, PNG, WEBP i PDF.");

            var relativeDirectory = Path.Combine("uploads", "restaurants", restaurantId.ToString());
            var physicalDirectory = Path.Combine(_environment.WebRootPath, relativeDirectory);
            Directory.CreateDirectory(physicalDirectory);
            var storedFileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
            var physicalPath = Path.Combine(physicalDirectory, storedFileName);

            await using (var stream = System.IO.File.Create(physicalPath))
            {
                await file.CopyToAsync(stream);
            }

            var attachment = new Models.RestaurantAttachment
            {
                RestaurantId = restaurantId,
                FileName = Path.GetFileName(file.FileName),
                FilePath = "/" + Path.Combine(relativeDirectory, storedFileName).Replace('\\', '/'),
                ContentType = file.ContentType,
                FileSize = file.Length,
                CreatedAt = DateTime.UtcNow
            };
            _dbContext.RestaurantAttachments.Add(attachment);
            await _dbContext.SaveChangesAsync();
            return Json(new { success = true, id = attachment.RestaurantAttachmentId });
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpGet("attachments/{restaurantId:int}")]
        public async Task<IActionResult> GetAttachments(int restaurantId)
        {
            var attachments = await _dbContext.RestaurantAttachments.AsNoTracking()
                .Where(x => x.RestaurantId == restaurantId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
            return PartialView("_AttachmentList", attachments);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPost("attachments/delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAttachment(int id)
        {
            var attachment = await _dbContext.RestaurantAttachments.FindAsync(id);
            if (attachment == null) return NotFound();

            var physicalPath = Path.GetFullPath(Path.Combine(_environment.WebRootPath, attachment.FilePath.TrimStart('/')));
            var uploadsRoot = Path.GetFullPath(Path.Combine(_environment.WebRootPath, "uploads", "restaurants"));
            if (physicalPath.StartsWith(uploadsRoot, StringComparison.OrdinalIgnoreCase) && System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Delete(physicalPath);
            }

            _dbContext.RestaurantAttachments.Remove(attachment);
            await _dbContext.SaveChangesAsync();
            return Json(new { success = true });
        }
    }
}
