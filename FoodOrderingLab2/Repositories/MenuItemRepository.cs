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
    }
}
