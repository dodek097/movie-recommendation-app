using Microsoft.AspNetCore.Mvc;
using FoodOrderingLab2.Repositories;

namespace FoodOrderingLab2.Controllers
{
    public class HomeController : Controller
    {
        private readonly RestaurantRepository _restaurantRepository;

        public HomeController(RestaurantRepository restaurantRepository)
        {
            _restaurantRepository = restaurantRepository;
        }

        public IActionResult Index()
        {
            var restaurants = _restaurantRepository.GetAll();
            return View(restaurants);
        }
    }
}
