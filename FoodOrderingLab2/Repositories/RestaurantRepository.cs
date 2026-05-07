using FoodOrderingLab2.Data;
using FoodOrderingLab2.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderingLab2.Repositories
{
    public class RestaurantRepository
    {
        private readonly ApplicationDbContext _context;

        public RestaurantRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Restaurant> GetAll()
        {
            return _context.Restaurants
                .Include(r => r.MenuItems)
                .AsNoTracking()
                .ToList();
        }

        public Restaurant? GetById(int id)
        {
            return _context.Restaurants
                .Include(r => r.MenuItems)
                .FirstOrDefault(r => r.RestaurantId == id);
        }
    }
}
