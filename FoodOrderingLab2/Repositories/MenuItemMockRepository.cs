using FoodOrderingLab2.Data;
using FoodOrderingLab2.Models;

namespace FoodOrderingLab2.Repositories
{
    public class MenuItemMockRepository
    {
        private List<MenuItem> _menuItems;

        public MenuItemMockRepository()
        {
            var restaurants = MockDataInitializer.GetRestaurants();
            _menuItems = new List<MenuItem>();
            foreach (var restaurant in restaurants)
            {
                _menuItems.AddRange(restaurant.MenuItems);
            }
        }

        public List<MenuItem> GetAll()
        {
            return _menuItems;
        }

        public MenuItem GetById(int id)
        {
            return _menuItems.FirstOrDefault(m => m.MenuItemId == id);
        }

        public List<MenuItem> GetByRestaurantId(int restaurantId)
        {
            return _menuItems.Where(m => m.RestaurantId == restaurantId).ToList();
        }
    }
}
