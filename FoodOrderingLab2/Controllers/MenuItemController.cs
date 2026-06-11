using Microsoft.AspNetCore.Mvc;
using FoodOrderingLab2.Repositories;
using FoodOrderingLab2.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace FoodOrderingLab2.Controllers
{
    [Route("meni")]
    [Authorize]
    public class MenuItemController : Controller
    {
        private readonly MenuItemRepository _menuItemRepository;
        private readonly RestaurantRepository _restaurantRepository;

        public MenuItemController(MenuItemRepository menuItemRepository, RestaurantRepository restaurantRepository)
        {
            _menuItemRepository = menuItemRepository;
            _restaurantRepository = restaurantRepository;
        }

        [Route("")]
        [Route("restoran/{restaurantId:int}")]
        [AllowAnonymous]
        public IActionResult Index(int restaurantId = 0)
        {
            List<Models.MenuItem> menuItems;

            if (restaurantId > 0)
            {
                menuItems = _menuItemRepository.GetByRestaurantId(restaurantId);
            }
            else
            {
                menuItems = _menuItemRepository.GetAll();
            }

            return View(menuItems);
        }

        [Route("{id:int}")]
        public IActionResult Details(int id)
        {
            var menuItem = _menuItemRepository.GetById(id);
            if (menuItem == null)
            {
                return NotFound();
            }

            var restaurant = _restaurantRepository.GetById(menuItem.RestaurantId);

            var viewModel = new MenuItemDetailViewModel
            {
                MenuItem = menuItem,
                Restaurant = restaurant!
            };

            return View(viewModel);
        }

        [HttpGet]
        [Route("create")]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Create()
        {
            ViewBag.Restaurants = _restaurantRepository.GetAll();
            return View(new ViewModels.MenuItemCreateViewModel());
        }

        [HttpPost]
        [Route("create")]
        [Authorize(Roles = "Admin,Manager")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ViewModels.MenuItemCreateViewModel model)
        {
            if (_restaurantRepository.GetById(model.RestaurantId) == null)
            {
                ModelState.AddModelError(nameof(model.RestaurantId), "Odabrani restoran ne postoji.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Restaurants = _restaurantRepository.GetAll();
                return View(model);
            }

            var menuItem = new Models.MenuItem
            {
                Name = model.Name,
                Description = model.Description,
                Category = model.Category,
                Price = model.Price,
                Calories = model.Calories,
                IsAvailable = model.IsAvailable,
                RestaurantId = model.RestaurantId
            };

            _menuItemRepository.Add(menuItem);
            return RedirectToAction("Details", new { id = menuItem.MenuItemId });
        }

        [HttpGet]
        [Route("edit/{id:int}")]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Edit(int id)
        {
            var menuItem = _menuItemRepository.GetById(id);
            if (menuItem == null)
            {
                return NotFound();
            }

            ViewBag.Restaurants = _restaurantRepository.GetAll();
            var model = new ViewModels.MenuItemCreateViewModel
            {
                Name = menuItem.Name,
                Description = menuItem.Description,
                Category = menuItem.Category,
                Price = menuItem.Price,
                Calories = menuItem.Calories,
                IsAvailable = menuItem.IsAvailable,
                RestaurantId = menuItem.RestaurantId
            };

            return View(model);
        }

        [HttpPost]
        [Route("edit/{id:int}")]
        [Authorize(Roles = "Admin,Manager")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ViewModels.MenuItemCreateViewModel model)
        {
            var menuItem = _menuItemRepository.GetById(id);
            if (menuItem == null)
            {
                return NotFound();
            }

            if (_restaurantRepository.GetById(model.RestaurantId) == null)
            {
                ModelState.AddModelError(nameof(model.RestaurantId), "Odabrani restoran ne postoji.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Restaurants = _restaurantRepository.GetAll();
                return View(model);
            }

            menuItem.Name = model.Name;
            menuItem.Description = model.Description;
            menuItem.Category = model.Category;
            menuItem.Price = model.Price;
            menuItem.Calories = model.Calories;
            menuItem.IsAvailable = model.IsAvailable;
            menuItem.RestaurantId = model.RestaurantId;

            _menuItemRepository.Update(menuItem);
            return RedirectToAction("Details", new { id = menuItem.MenuItemId });
        }

        [HttpPost]
        [Route("delete/{id:int}")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var menuItem = _menuItemRepository.GetById(id);
            if (menuItem == null)
            {
                return NotFound();
            }

            if (menuItem.OrderItems != null && menuItem.OrderItems.Any())
            {
                TempData["ErrorMessage"] = "Ne mogu obrisati stavku menija koja je već uključena u postojeću narudžbu.";
                return RedirectToAction("Index");
            }

            _menuItemRepository.Delete(menuItem);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("search")]
        [AllowAnonymous]
        public IActionResult Search(string q, int? restaurantId)
        {
            q = q ?? string.Empty;
            var items = restaurantId.GetValueOrDefault() > 0
            ? _menuItemRepository.GetByRestaurantId(restaurantId.GetValueOrDefault())
            : _menuItemRepository.GetAll();

            var results = items
            .Where(m =>
                (m.Name ?? "").Contains(q, StringComparison.OrdinalIgnoreCase) ||
                (m.Description ?? "").Contains(q, StringComparison.OrdinalIgnoreCase))
            .Select(m => new
            {
                id = m.MenuItemId,
                text = m.Name,
                name = m.Name,
                category = m.Category.ToString(),
                price = m.Price.ToString("F2"),
                description = m.Description,
                restaurantName = m.Restaurant?.Name
            })
            .Take(10)
            .ToList();

            return Json(results);
        }
    }
}
