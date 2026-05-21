using Microsoft.AspNetCore.Mvc;
using FoodOrderingLab2.Repositories;
using System.Linq;

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

        [HttpGet]
        [Route("search")]
        public IActionResult Search(string q)
        {
            q = q ?? string.Empty;
            var results = _customerRepository.GetAll()
                .Where(c => c.FullName.Contains(q, System.StringComparison.InvariantCultureIgnoreCase)
                            || c.Email.Contains(q, System.StringComparison.InvariantCultureIgnoreCase))
                .Select(c => new { id = c.CustomerId, text = c.FullName })
                .Take(10)
                .ToList();

            return Json(results);
        }
    }
}
