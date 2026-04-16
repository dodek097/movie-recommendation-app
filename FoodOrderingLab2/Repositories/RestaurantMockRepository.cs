using FoodOrderingLab2.Data;
using FoodOrderingLab2.Models;

namespace FoodOrderingLab2.Repositories
{
    public class RestaurantMockRepository
    {
        private List<Restaurant> _restaurants;

        public RestaurantMockRepository()
        {
            _restaurants = MockDataInitializer.GetRestaurants();
        }

        public List<Restaurant> GetAll()
        {
            return _restaurants;
        }

        public Restaurant GetById(int id)
        {
            return _restaurants.FirstOrDefault(r => r.RestaurantId == id);
        }
    }
}
