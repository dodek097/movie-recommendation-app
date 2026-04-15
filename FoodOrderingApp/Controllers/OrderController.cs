using FoodOrderingApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace FoodOrderingApp.Controllers
{
    public class OrderController : Controller
    {
        private readonly ILogger<OrderController> _logger;
        // In-memory list for demonstration. Replace with database context for production.
        private static List<Order> _orders = new List<Order>();

        public OrderController(ILogger<OrderController> logger)
        {
            _logger = logger;
        }

        // GET: Order/GetAllOrders
        [HttpGet]
        public IActionResult GetAllOrders()
        {
            _logger.LogInformation("Retrieving all food orders");
            return Ok(_orders);
        }

        // GET: Order
        public IActionResult Index()
        {
            _logger.LogInformation("Retrieving all orders");
            return View(_orders);
        }

        // GET: Order/Details/5
        public IActionResult Details(int id)
        {
            _logger.LogInformation($"Retrieving order details for OrderId: {id}");
            var order = _orders.FirstOrDefault(o => o.OrderId == id);
            
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Order/Create
        public IActionResult Create()
        {
            _logger.LogInformation("Opening Create order form");
            return View();
        }

        // POST: Order/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("OrderId,UserId,MovieId,OrderDate,TotalPrice")] Order order)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Set OrderId if not provided
                    if (order.OrderId == 0)
                    {
                        order.OrderId = _orders.Count > 0 ? _orders.Max(o => o.OrderId) + 1 : 1;
                    }

                    _orders.Add(order);
                    _logger.LogInformation($"Order created successfully with OrderId: {order.OrderId}");
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            return View(order);
        }

        // GET: Order/Edit/5
        public IActionResult Edit(int id)
        {
            _logger.LogInformation($"Opening Edit form for OrderId: {id}");
            var order = _orders.FirstOrDefault(o => o.OrderId == id);
            
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Order/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("OrderId,UserId,MovieId,OrderDate,TotalPrice")] Order order)
        {
            if (id != order.OrderId)
            {
                return BadRequest();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var existingOrder = _orders.FirstOrDefault(o => o.OrderId == id);
                    
                    if (existingOrder != null)
                    {
                        existingOrder.UserId = order.UserId;
                        existingOrder.MovieId = order.MovieId;
                        existingOrder.OrderDate = order.OrderDate;
                        existingOrder.TotalPrice = order.TotalPrice;

                        _logger.LogInformation($"Order updated successfully for OrderId: {id}");
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating order with OrderId: {id}");
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            return View(order);
        }

        // GET: Order/Delete/5
        public IActionResult Delete(int id)
        {
            _logger.LogInformation($"Opening Delete confirmation for OrderId: {id}");
            var order = _orders.FirstOrDefault(o => o.OrderId == id);
            
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var order = _orders.FirstOrDefault(o => o.OrderId == id);
                
                if (order != null)
                {
                    _orders.Remove(order);
                    _logger.LogInformation($"Order deleted successfully with OrderId: {id}");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting order with OrderId: {id}");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
