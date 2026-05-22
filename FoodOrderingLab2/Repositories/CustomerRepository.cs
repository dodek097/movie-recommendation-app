using FoodOrderingLab2.Data;
using FoodOrderingLab2.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderingLab2.Repositories
{
    public class CustomerRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Customer> GetAll()
        {
            return _context.Customers
                .Include(c => c.Orders)
                .AsNoTracking()
                .ToList();
        }

        public Customer? GetById(int id)
        {
            return _context.Customers
                .Include(c => c.Orders)
                .FirstOrDefault(c => c.CustomerId == id);
        }

        public int GetNextId()
        {
            return _context.Customers.Any() ? _context.Customers.Max(c => c.CustomerId) + 1 : 1;
        }

        public void Add(Customer customer)
        {
            _context.Customers.Add(customer);
            _context.SaveChanges();
        }

        public void Update(Customer customer)
        {
            _context.Customers.Update(customer);
            _context.SaveChanges();
        }

        public void Delete(Customer customer)
        {
            _context.Customers.Remove(customer);
            _context.SaveChanges();
        }
    }
}
