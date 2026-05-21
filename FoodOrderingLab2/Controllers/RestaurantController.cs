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
                .Select(r => new { id = r.RestaurantId, text = r.Name })
                .Take(10)
                .ToList();

            return Json(results);
        }
    }
}
