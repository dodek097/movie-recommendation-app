using Microsoft.AspNetCore.Mvc;
using FoodOrderingLab2.Repositories;
using FoodOrderingLab2.ViewModels;

namespace FoodOrderingLab2.Controllers
{
    [Route("narudzbe")]
    public class OrderController : Controller
    {
        private readonly OrderRepository _orderRepository;
        private readonly RestaurantRepository _restaurantRepository;
        private readonly CustomerRepository _customerRepository;

        public OrderController(OrderRepository orderRepository, RestaurantRepository restaurantRepository, CustomerRepository customerRepository)
        {
            _orderRepository = orderRepository;
            _restaurantRepository = restaurantRepository;
            _customerRepository = customerRepository;
        }

        [Route("")]
        public IActionResult Index()
        {
            var orders = _orderRepository.GetAll();
            return View(orders);
        }

        [Route("{id:int}")]
        public IActionResult Details(int id)
        {
            var order = _orderRepository.GetById(id);
            if (order == null)
            {
                return NotFound();
            }

            var customer = _customerRepository.GetById(order.CustomerId);
            var restaurant = _restaurantRepository.GetById(order.RestaurantId);

            var viewModel = new OrderDetailViewModel
            {
                Order = order,
                Customer = customer,
                Restaurant = restaurant,
                OrderItems = order.OrderItems.ToList()
            };

            return View(viewModel);
        }
    }
}
