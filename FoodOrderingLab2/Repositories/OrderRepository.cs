using FoodOrderingLab2.Data;
using FoodOrderingLab2.Models;
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

        public Order Update(Order order)
        {
            _context.Orders.Update(order);
            _context.SaveChanges();
            return order;
        }

        public void Delete(int id)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                _context.SaveChanges();
            }
        }

        public List<Order> Search(string q)
        {
            q = q?.ToLower() ?? string.Empty;
            return _context.Orders
                .Where(o => o.Customer.FirstName.ToLower().Contains(q)
                         || o.Customer.LastName.ToLower().Contains(q)
                         || o.Status.ToString().ToLower().Contains(q))
                .Include(o => o.Customer)
                .Include(o => o.Restaurant)
                .AsNoTracking()
                .ToList();
        }
    }
}
