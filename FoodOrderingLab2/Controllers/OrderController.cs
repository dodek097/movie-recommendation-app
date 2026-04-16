using Microsoft.AspNetCore.Mvc;
using FoodOrderingLab2.Repositories;
using FoodOrderingLab2.ViewModels;

namespace FoodOrderingLab2.Controllers
{
    public class OrderController : Controller
    {
        private readonly OrderMockRepository _orderRepository;
        private readonly RestaurantMockRepository _restaurantRepository;
        private readonly CustomerMockRepository _customerRepository;

        public OrderController(OrderMockRepository orderRepository, RestaurantMockRepository restaurantRepository, CustomerMockRepository customerRepository)
        {
            _orderRepository = orderRepository;
            _restaurantRepository = restaurantRepository;
            _customerRepository = customerRepository;
        }

        public IActionResult Index()
        {
            var orders = _orderRepository.GetAll();
            return View(orders);
        }

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
                OrderItems = order.OrderItems
            };

            return View(viewModel);
        }
    }
}
