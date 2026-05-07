using Microsoft.AspNetCore.Mvc;
using FoodOrderingLab2.Repositories;
using FoodOrderingLab2.ViewModels;

namespace FoodOrderingLab2.Controllers
{
    [Route("meni")]
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
                Restaurant = restaurant
            };

            return View(viewModel);
        }
    }
}
