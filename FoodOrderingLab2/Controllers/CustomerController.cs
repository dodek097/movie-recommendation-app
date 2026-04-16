using Microsoft.AspNetCore.Mvc;
using FoodOrderingLab2.Repositories;

namespace FoodOrderingLab2.Controllers
{
    public class CustomerController : Controller
    {
        private readonly CustomerMockRepository _customerRepository;

        public CustomerController(CustomerMockRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public IActionResult Index()
        {
            var customers = _customerRepository.GetAll();
            return View(customers);
        }

        public IActionResult Details(int id)
        {
            var customer = _customerRepository.GetById(id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }
    }
}
