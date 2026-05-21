using Microsoft.AspNetCore.Mvc;
using FoodOrderingLab2.Repositories;
using FoodOrderingLab2.ViewModels;
using FoodOrderingLab2.Models;
using System.Linq;

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

        [Route("create")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Route("create")]
        [HttpPost]
        public IActionResult Create(Order order)
        {
            if (ModelState.IsValid)
            {
                order.Status = Models.Enums.OrderStatus.Pending;
                order.TotalPrice = 0;
                _orderRepository.Add(order);
                return RedirectToAction(nameof(Details), new { id = order.OrderId });
            }

            return View(order);
        }

        [Route("edit/{id:int}")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var order = _orderRepository.GetById(id);
            if (order == null)
            {
                return NotFound();
            }

            var customer = _customerRepository.GetById(order.CustomerId);
            var restaurant = _restaurantRepository.GetById(order.RestaurantId);

            ViewBag.Customer = customer;
            ViewBag.Restaurant = restaurant;

            return View(order);
        }

        [Route("edit/{id:int}")]
        [HttpPost]
        public IActionResult Edit(int id, Order order)
        {
            var existing = _orderRepository.GetById(id);
            if (existing == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                existing.CustomerId = order.CustomerId;
                existing.RestaurantId = order.RestaurantId;
                existing.OrderDate = order.OrderDate;
                existing.Status = order.Status;
                _orderRepository.Update(existing);
                return RedirectToAction(nameof(Details), new { id = id });
            }

            return View(order);
        }

        [Route("delete/{id:int}")]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            _orderRepository.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
