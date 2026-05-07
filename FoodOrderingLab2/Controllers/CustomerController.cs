using Microsoft.AspNetCore.Mvc;
using FoodOrderingLab2.Repositories;

namespace FoodOrderingLab2.Controllers
{
    [Route("kupci")]
    public class CustomerController : Controller
    {
        private readonly CustomerRepository _customerRepository;

        public CustomerController(CustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [Route("")]
        public IActionResult Index()
        {
            var customers = _customerRepository.GetAll();
            return View(customers);
        }

        [Route("{id:int}")]
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
