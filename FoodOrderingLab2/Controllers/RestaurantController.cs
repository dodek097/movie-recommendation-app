using Microsoft.AspNetCore.Mvc;
using FoodOrderingLab2.Repositories;
using FoodOrderingLab2.ViewModels;
using System.Linq;

namespace FoodOrderingLab2.Controllers
{
    [Route("restorani")]
    public class RestaurantController : Controller
    {
        private readonly RestaurantRepository _restaurantRepository;
        private readonly MenuItemRepository _menuItemRepository;

        public RestaurantController(RestaurantRepository restaurantRepository, MenuItemRepository menuItemRepository)
        {
            _restaurantRepository = restaurantRepository;
            _menuItemRepository = menuItemRepository;
        }

        [Route("")]
        public IActionResult Index()
        {
            var restaurants = _restaurantRepository.GetAll();
            return View(restaurants);
        }

        [HttpGet]
        [Route("create")]
        public IActionResult Create()
        {
            var model = new RestaurantCreateViewModel();
            model.MenuItems.Add(new RestaurantMenuItemCreateModel());
            return View(model);
        }

        [HttpPost]
        [Route("create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RestaurantCreateViewModel model)
        {
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
                RestaurantId = _restaurantRepository.GetNextId(),
                Name = model.Name,
                Address = model.Address,
                Phone = model.Phone,
                Email = model.Email,
                Rating = model.Rating,
                OpeningHours = model.OpeningHours
            };

            var nextMenuItemId = _menuItemRepository.GetNextId();
            var validMenuItems = model.MenuItems
                .Where(mi => !string.IsNullOrWhiteSpace(mi.Name))
                .Select(mi => new Models.MenuItem
                {
                    MenuItemId = nextMenuItemId++,
                    Name = mi.Name,
                    Description = mi.Description,
                    Price = mi.Price,
                    Category = mi.Category,
                    Calories = mi.Calories,
                    IsAvailable = mi.IsAvailable,
                    RestaurantId = restaurant.RestaurantId
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

            _restaurantRepository.Delete(restaurant);
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
                MenuItems = _menuItemRepository.GetByRestaurantId(id)
            };

            ViewBag.ErrorMessage = TempData["ErrorMessage"];
            return View(viewModel);
        }

        [HttpGet]
        [Route("search")]
        public IActionResult Search(string q)
        {
            q = q ?? string.Empty;
            var results = _restaurantRepository.GetAll()
                .Where(r => r.Name.Contains(q, System.StringComparison.InvariantCultureIgnoreCase)
                            || r.Address.Contains(q, System.StringComparison.InvariantCultureIgnoreCase))
                .Select(r => new { id = r.RestaurantId, text = r.Name, name = r.Name, address = r.Address, rating = r.Rating })
                .Take(10)
                .ToList();

            return Json(results);
        }
    }
}
