using Microsoft.AspNetCore.Mvc;
using FoodOrderingLab2.Repositories;
using FoodOrderingLab2.ViewModels;
using FoodOrderingLab2.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace FoodOrderingLab2.Controllers
{
    [Route("narudzbe")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly OrderRepository _orderRepository;
        private readonly RestaurantRepository _restaurant_repository;
        private readonly CustomerRepository _customerRepository;
        private readonly MenuItemRepository _menuItemRepository;
        private readonly UserManager<AppUser> _userManager;

        public OrderController(
            OrderRepository orderRepository,
            RestaurantRepository restaurantRepository,
            CustomerRepository customerRepository,
            MenuItemRepository menuItemRepository,
            UserManager<AppUser> userManager)
        {
            _orderRepository = orderRepository;
            _restaurant_repository = restaurantRepository;
            _customerRepository = customerRepository;
            _menuItemRepository = menuItemRepository;
            _userManager = userManager;
        }

        [Route("")]
        public IActionResult Index()
        {
            var orders = IsStaff
                ? _orderRepository.GetAll()
                : _orderRepository.GetByCustomerId(CurrentCustomer?.CustomerId ?? 0);
            return View(orders);
        }

        [HttpGet]
        [Route("search")]
        public IActionResult Search(string q)
        {
            q = q ?? string.Empty;
            var orders = IsStaff
                ? _orderRepository.Search(q)
                : _orderRepository.Search(q, CurrentCustomer?.CustomerId ?? 0);
            var results = orders
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
            if (!CanAccess(order)) return Forbid();

            var customer = _customerRepository.GetById(order.CustomerId);
            var restaurant = _restaurant_repository.GetById(order.RestaurantId);

            var viewModel = new OrderDetailViewModel
            {
                Order = order,
                Customer = customer!,
                Restaurant = restaurant!,
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
                OrderDate = DateTime.Now,
                CustomerId = IsStaff ? 0 : CurrentCustomer?.CustomerId ?? 0
            };
            if (!IsStaff && vm.CustomerId == 0) return RedirectToPage("/Account/Register", new { area = "Identity" });
            ViewBag.IsStaff = IsStaff;
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
        [ValidateAntiForgeryToken]
        public IActionResult Create(OrderCreateViewModel vm)
        {
            if (!IsStaff)
            {
                vm.CustomerId = CurrentCustomer?.CustomerId ?? 0;
                vm.Status = Models.Enums.OrderStatus.Pending;
            }
            if (_customerRepository.GetById(vm.CustomerId) == null)
                ModelState.AddModelError(nameof(vm.CustomerId), "Odabrani kupac ne postoji.");
            if (_restaurant_repository.GetById(vm.RestaurantId) == null)
                ModelState.AddModelError(nameof(vm.RestaurantId), "Odabrani restoran ne postoji.");

            if (!ModelState.IsValid)
            {
                ViewBag.IsStaff = IsStaff;
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
                if (menuItemObj == null || menuItemObj.RestaurantId != vm.RestaurantId || !menuItemObj.IsAvailable)
                {
                    ModelState.AddModelError(string.Empty, "Odabrani artikl mora biti dostupan i pripadati odabranom restoranu.");
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
                ViewBag.IsStaff = IsStaff;
                return View(vm);
            }

            order.TotalPrice = total;
            _orderRepository.Add(order);
            TempData["SuccessMessage"] = "Narudžba je spremljena.";
            return RedirectToAction(nameof(Details), new { id = order.OrderId });
        }

        [Route("edit/{id:int}")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
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
            var menuNames = new Dictionary<int, string>();

            foreach (var it in order.OrderItems)
            {
                var mi = _menuItemRepository.GetById(it.MenuItemId);
                if (mi != null)
                    menuNames[it.MenuItemId] = mi.Name;
            }

            ViewBag.MenuItemNames = menuNames;

            var customer = _customerRepository.GetById(order.CustomerId);
            var restaurant = _restaurant_repository.GetById(order.RestaurantId);
            ViewBag.Customer = customer;
            ViewBag.Restaurant = restaurant;

            ViewBag.OrderId = order.OrderId;

            return View(vm);
        }

        [Route("edit/{id:int}")]
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, OrderCreateViewModel vm)
        {
            var existing = _orderRepository.GetById(id);
            if (existing == null)
            {
                return NotFound();
            }

            if (_customerRepository.GetById(vm.CustomerId) == null)
                ModelState.AddModelError(nameof(vm.CustomerId), "Odabrani kupac ne postoji.");
            if (_restaurant_repository.GetById(vm.RestaurantId) == null)
                ModelState.AddModelError(nameof(vm.RestaurantId), "Odabrani restoran ne postoji.");

            if (!ModelState.IsValid)
            {
                ViewBag.Customer = _customerRepository.GetById(vm.CustomerId);
                ViewBag.Restaurant = _restaurant_repository.GetById(vm.RestaurantId);
                ViewBag.OrderId = id;
                return View(vm);
            }

            var updatedOrder = new Order
            {
                OrderId = id,
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
                var menuItemObj = _menuItemRepository.GetById(it.MenuItemId);
                if (menuItemObj == null || menuItemObj.RestaurantId != vm.RestaurantId || !menuItemObj.IsAvailable)
                {
                    ModelState.AddModelError(string.Empty, "Odabrani artikl mora biti dostupan i pripadati odabranom restoranu.");
                    break;
                }

                var orderItem = new OrderItem
                {
                    MenuItemId = menuItemObj.MenuItemId,
                    Quantity = it.Quantity,
                    UnitPrice = menuItemObj.Price,
                    SpecialRequests = it.SpecialRequests
                };
                updatedOrder.OrderItems.Add(orderItem);
                total += orderItem.UnitPrice * orderItem.Quantity;
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Customer = _customerRepository.GetById(vm.CustomerId);
                ViewBag.Restaurant = _restaurant_repository.GetById(vm.RestaurantId);
                ViewBag.OrderId = id;
                return View(vm);
            }

            updatedOrder.TotalPrice = total;

            _orderRepository.Update(updatedOrder);
            TempData["SuccessMessage"] = "Narudžba je spremljena.";
            return RedirectToAction(nameof(Details), new { id = id });
        }

        [Route("delete/{id:int}")]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                _orderRepository.Delete(id);
                TempData["SuccessMessage"] = "Narudžba obrisana.";
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = "Pogreška prilikom brisanja narudžbe: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        private bool IsStaff => User.IsInRole("Admin") || User.IsInRole("Manager");
        private Customer? CurrentCustomer
        {
            get
            {
                var userId = _userManager.GetUserId(User);
                return string.IsNullOrWhiteSpace(userId) ? null : _customerRepository.GetByAppUserId(userId);
            }
        }

        private bool CanAccess(Order order) => IsStaff || CurrentCustomer?.CustomerId == order.CustomerId;
    }
}
