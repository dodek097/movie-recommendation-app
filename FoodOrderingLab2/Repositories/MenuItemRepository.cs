using FoodOrderingLab2.Data;
using FoodOrderingLab2.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderingLab2.Repositories
{
    public class MenuItemRepository
    {
        private readonly ApplicationDbContext _context;

        public MenuItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<MenuItem> GetAll()
        {
            return _context.MenuItems
                .Include(m => m.Restaurant)
                .AsNoTracking()
                .ToList();
        }

        public MenuItem? GetById(int id)
        {
            return _context.MenuItems
                .Include(m => m.Restaurant)
                .Include(m => m.OrderItems)
                .FirstOrDefault(m => m.MenuItemId == id);
        }

        public List<MenuItem> GetByRestaurantId(int restaurantId)
        {
            return _context.MenuItems
                .Where(m => m.RestaurantId == restaurantId)
                .Include(m => m.Restaurant)
                .AsNoTracking()
                .ToList();
        }

        public int GetNextId()
        {
            return _context.MenuItems.Any() ? _context.MenuItems.Max(m => m.MenuItemId) + 1 : 1;
        }

        public void AddRange(IEnumerable<MenuItem> items)
        {
            _context.MenuItems.AddRange(items);
            _context.SaveChanges();
        }

        public MenuItem Add(MenuItem item)
        {
            if (item.MenuItemId == 0)
            {
                item.MenuItemId = GetNextId();
            }

            _context.MenuItems.Add(item);
            _context.SaveChanges();
            return item;
        }

        public void Update(MenuItem item)
        {
            _context.MenuItems.Update(item);
            _context.SaveChanges();
        }

        public void Delete(MenuItem item)
        {
            _context.MenuItems.Remove(item);
            _context.SaveChanges();
        }
    }
}
