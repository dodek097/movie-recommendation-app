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
                .Include(r => r.Orders)
                .FirstOrDefault(r => r.RestaurantId == id);
        }

        public void Add(Restaurant restaurant)
        {
            _context.Restaurants.Add(restaurant);
            _context.SaveChanges();
        }

        public void Update(Restaurant restaurant)
        {
            _context.Restaurants.Update(restaurant);
            _context.SaveChanges();
        }

        public void Delete(Restaurant restaurant)
        {
            _context.Restaurants.Remove(restaurant);
            _context.SaveChanges();
        }
    }
}
