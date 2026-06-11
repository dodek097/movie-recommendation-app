using FoodOrderingLab2.Data;
using FoodOrderingLab2.Models;
using FoodOrderingLab2.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderingLab2.Repositories
{
    public class OrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Order> GetAll()
        {
            return _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Restaurant)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .AsNoTracking()
                .ToList();
        }

        public List<Order> GetByCustomerId(int customerId)
        {
            return _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Restaurant)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Where(o => o.CustomerId == customerId)
                .AsNoTracking()
                .ToList();
        }

        public Order? GetById(int id)
        {
            return _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Restaurant)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefault(o => o.OrderId == id);
        }

        public Order Add(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();

            return order;
        }

       public Order? Update(Order order)
        {
            var existing = _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefault(o => o.OrderId == order.OrderId);

            if (existing == null)
                return null;

            existing.CustomerId = order.CustomerId;
            existing.RestaurantId = order.RestaurantId;
            existing.OrderDate = order.OrderDate;
            existing.Status = order.Status;
            existing.TotalPrice = order.TotalPrice;

            var oldItems = existing.OrderItems.ToList();
            if (oldItems.Any())
            {
                _context.OrderItems.RemoveRange(oldItems);
                existing.OrderItems.Clear();
            }

            foreach (var item in order.OrderItems)
            {
                existing.OrderItems.Add(new OrderItem
                {
                    MenuItemId = item.MenuItemId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    SpecialRequests = item.SpecialRequests,
                    OrderId = existing.OrderId
                });
            }

            _context.SaveChanges();
            return existing;
        }
        public void Delete(int id)
        {
            var order = _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefault(o => o.OrderId == id);

            if (order != null)
            {
                // remove child order items first because FK has Restrict behavior
                if (order.OrderItems != null && order.OrderItems.Any())
                {
                    _context.OrderItems.RemoveRange(order.OrderItems);
                }

                _context.Orders.Remove(order);
                _context.SaveChanges();

                if (_context.Database.IsSqlServer() && !_context.Orders.Any())
                {
                    _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Orders', RESEED, 0);");
                }
            }
        }

        public List<Order> Search(string q)
        {
            return SearchQuery(q)
                .AsNoTracking()
                .ToList();
        }

        public List<Order> Search(string q, int customerId)
        {
            return SearchQuery(q)
                .Where(o => o.CustomerId == customerId)
                .AsNoTracking()
                .ToList();
        }

        private IQueryable<Order> SearchQuery(string q)
        {
            q = q?.Trim().ToLower() ?? string.Empty;
            var digits = new string(q.Where(char.IsDigit).ToArray());
            int.TryParse(digits, out var numericId);
            var hasStatus = Enum.TryParse<OrderStatus>(q, true, out var status);

            var query = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Restaurant);

            if (string.IsNullOrWhiteSpace(q))
            {
                return query;
            }

            if (numericId == 0 && (q.Contains("narudzba") || q.Contains("narudžba") || q.Contains("order")))
            {
                return query;
            }

            return query.Where(o =>
                o.Customer.FirstName.ToLower().Contains(q)
                || o.Customer.LastName.ToLower().Contains(q)
                || o.Restaurant.Name.ToLower().Contains(q)
                || (numericId > 0 && o.OrderId == numericId)
                || (hasStatus && o.Status == status));
        }
    }
}
