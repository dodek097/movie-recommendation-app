using Microsoft.AspNetCore.Mvc;
using FoodOrderingLab2.Repositories;
using FoodOrderingLab2.ViewModels;

namespace FoodOrderingLab2.Controllers
{
    public class RestaurantController : Controller
    {
        private readonly RestaurantMockRepository _restaurantRepository;
        private readonly MenuItemMockRepository _menuItemRepository;

        public RestaurantController(RestaurantMockRepository restaurantRepository, MenuItemMockRepository menuItemRepository)
        {
            _restaurantRepository = restaurantRepository;
            _menuItemRepository = menuItemRepository;
        }

        public IActionResult Index()
        {
            var restaurants = _restaurantRepository.GetAll();
            return View(restaurants);
        }

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
    }
}
