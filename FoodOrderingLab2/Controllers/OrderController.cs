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
        private readonly RestaurantRepository _restaurant_repository;
        private readonly CustomerRepository _customerRepository;
        private readonly MenuItemRepository _menuItemRepository;

        public OrderController(OrderRepository orderRepository, RestaurantRepository restaurantRepository, CustomerRepository customerRepository, MenuItemRepository menuItemRepository)
        {
            _orderRepository = orderRepository;
            _restaurant_repository = restaurantRepository;
            _customerRepository = customerRepository;
            _menuItemRepository = menuItemRepository;
        }

        [Route("")]
        public IActionResult Index()
        {
            var orders = _orderRepository.GetAll();
            return View(orders);
        }

        [HttpGet]
        [Route("search")]
        public IActionResult Search(string q)
        {
            q = q ?? string.Empty;
            var results = _orderRepository.Search(q)
                .Select(o => new
                {
                    id = o.OrderId,
                    text = $"Narudžba #{o.OrderId}",
                    customerName = o.Customer?.FullName,
                    restaurantName = o.Restaurant?.Name,
                    status = o.Status.ToString(),
                    totalPrice = o.TotalPrice.ToString("F2"),
                    orderDateText = o.OrderDate.ToString("d.M.yyyy H:mm")
                })
                .ToList();

            return Json(results);
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
            var restaurant = _restaurant_repository.GetById(order.RestaurantId);

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
            var vm = new OrderCreateViewModel
            {
                OrderDate = DateTime.Now
            };
            // prepare menu item name lookup for selected items
            var menuNames = new Dictionary<int, string>();
            foreach (var it in vm.Items)
            {
                var mi = _menuItemRepository.GetById(it.MenuItemId);
                if (mi != null) menuNames[it.MenuItemId] = mi.Name;
            }
            ViewBag.MenuItemNames = menuNames;

            return View(vm);
        }

        [Route("create")]
        [HttpPost]
        public IActionResult Create(OrderCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var order = new Order
            {
                CustomerId = vm.CustomerId,
                RestaurantId = vm.RestaurantId,
                OrderDate = vm.OrderDate,
                Status = vm.Status,
                OrderItems = new List<OrderItem>()
            };

            decimal total = 0m;
            foreach (var it in vm.Items)
            {
                if (it.MenuItemId <= 0 || it.Quantity <= 0) continue;
                // We will use MenuItemRepository via RestaurantRepository.GetById? Instead, resolve via data context using MenuItemRepository.
                // To avoid adding new dependency here, fetch via _restaurantRepository.GetById for restaurant then find item.
                var menuItemObj = _menuItemRepository.GetById(it.MenuItemId);
                if (menuItemObj == null || menuItemObj.RestaurantId != vm.RestaurantId)
                {
                    ModelState.AddModelError(string.Empty, "Odabrani artikl mora pripadati odabranom restoranu.");
                    break;
                }

                var orderItem = new OrderItem
                {
                    MenuItemId = menuItemObj.MenuItemId,
                    Quantity = it.Quantity,
                    UnitPrice = menuItemObj.Price,
                    SpecialRequests = it.SpecialRequests
                };

                total += orderItem.UnitPrice * orderItem.Quantity;
                order.OrderItems.Add(orderItem);
            }

            if (!ModelState.IsValid)
            {
                var menuNames = new Dictionary<int, string>();
                foreach (var it in vm.Items)
                {
                    var mi = _menuItemRepository.GetById(it.MenuItemId);
                    if (mi != null) menuNames[it.MenuItemId] = mi.Name;
                }
                ViewBag.MenuItemNames = menuNames;
                return View(vm);
            }

            order.TotalPrice = total;

            _orderRepository.Add(order);
            TempData["SuccessMessage"] = "Narudžba je spremljena.";
            return RedirectToAction(nameof(Details), new { id = order.OrderId });
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

            var vm = new OrderCreateViewModel
            {
                CustomerId = order.CustomerId,
                RestaurantId = order.RestaurantId,
                OrderDate = order.OrderDate,
                Status = order.Status,
                Items = order.OrderItems.Select(oi => new OrderItemCreateModel
                {
                    MenuItemId = oi.MenuItemId,
                    Quantity = oi.Quantity,
                    SpecialRequests = oi.SpecialRequests
                }).ToList()
            };

            var customer = _customerRepository.GetById(order.CustomerId);
            var restaurant = _restaurant_repository.GetById(order.RestaurantId);
            ViewBag.Customer = customer;
            ViewBag.Restaurant = restaurant;

            ViewBag.OrderId = order.OrderId;

            return View(vm);
        }

        [Route("edit/{id:int}")]
        [HttpPost]
        public IActionResult Edit(int id, OrderCreateViewModel vm)
        {
            var existing = _orderRepository.GetById(id);
            if (existing == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Customer = _customerRepository.GetById(vm.CustomerId);
                ViewBag.Restaurant = _restaurant_repository.GetById(vm.RestaurantId);
                ViewBag.OrderId = id;
                return View(vm);
            }

            existing.CustomerId = vm.CustomerId;
            existing.RestaurantId = vm.RestaurantId;
            existing.OrderDate = vm.OrderDate;
            existing.Status = vm.Status;

            // Rebuild items
            existing.OrderItems.Clear();
            decimal total = 0m;
            foreach (var it in vm.Items)
            {
                if (it.MenuItemId <= 0 || it.Quantity <= 0) continue;
                var menuItemObj = _menuItemRepository.GetById(it.MenuItemId);
                if (menuItemObj == null || menuItemObj.RestaurantId != vm.RestaurantId)
                {
                    ModelState.AddModelError(string.Empty, "Odabrani artikl mora pripadati odabranom restoranu.");
                    break;
                }

                var orderItem = new OrderItem
                {
                    MenuItemId = menuItemObj.MenuItemId,
                    Quantity = it.Quantity,
                    UnitPrice = menuItemObj.Price,
                    SpecialRequests = it.SpecialRequests
                };
                existing.OrderItems.Add(orderItem);
                total += orderItem.UnitPrice * orderItem.Quantity;
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Customer = _customerRepository.GetById(vm.CustomerId);
                ViewBag.Restaurant = _restaurant_repository.GetById(vm.RestaurantId);
                ViewBag.OrderId = id;
                return View(vm);
            }

            existing.TotalPrice = total;

            _orderRepository.Update(existing);
            TempData["SuccessMessage"] = "Narudžba je spremljena.";
            return RedirectToAction(nameof(Details), new { id = id });
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
