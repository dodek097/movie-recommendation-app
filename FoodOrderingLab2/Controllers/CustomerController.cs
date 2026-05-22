using Microsoft.AspNetCore.Mvc;
using FoodOrderingLab2.Repositories;
using FoodOrderingLab2.ViewModels;
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

        [HttpGet]
        [Route("create")]
        public IActionResult Create()
        {
            var model = new CustomerCreateViewModel
            {
                RegisterDate = DateTime.Now
            };
            return View(model);
        }

        [HttpPost]
        [Route("create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CustomerCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var customer = new Models.Customer
            {
                CustomerId = _customerRepository.GetNextId(),
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Phone = model.Phone,
                Address = model.Address,
                RegisterDate = model.RegisterDate
            };

            _customerRepository.Add(customer);
            return RedirectToAction("Details", new { id = customer.CustomerId });
        }

        [HttpGet]
        [Route("edit/{id:int}")]
        public IActionResult Edit(int id)
        {
            var customer = _customerRepository.GetById(id);
            if (customer == null)
            {
                return NotFound();
            }

            var model = new CustomerCreateViewModel
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address,
                RegisterDate = customer.RegisterDate
            };

            ViewBag.CustomerId = customer.CustomerId;
            return View(model);
        }

        [HttpPost]
        [Route("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, CustomerCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CustomerId = id;
                return View(model);
            }

            var customer = _customerRepository.GetById(id);
            if (customer == null)
            {
                return NotFound();
            }

            customer.FirstName = model.FirstName;
            customer.LastName = model.LastName;
            customer.Email = model.Email;
            customer.Phone = model.Phone;
            customer.Address = model.Address;
            customer.RegisterDate = model.RegisterDate;

            _customerRepository.Update(customer);
            return RedirectToAction("Details", new { id = customer.CustomerId });
        }

        [HttpPost]
        [Route("delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var customer = _customerRepository.GetById(id);
            if (customer == null)
            {
                return NotFound();
            }

            if (customer.Orders.Any())
            {
                TempData["ErrorMessage"] = "Ne možete obrisati kupca koji ima narudžbe.";
                return RedirectToAction("Details", new { id });
            }

            _customerRepository.Delete(customer);
            return RedirectToAction("Index");
        }

        [Route("{id:int}")]
        public IActionResult Details(int id)
        {
            var customer = _customerRepository.GetById(id);
            if (customer == null)
            {
                return NotFound();
            }

            ViewBag.ErrorMessage = TempData["ErrorMessage"];
            return View(customer);
        }

        [HttpGet]
        [Route("search")]
        public IActionResult Search(string q)
        {
            q = q ?? string.Empty;
            var results = _customerRepository.GetAll()
                .Where(c => c.FullName.Contains(q, System.StringComparison.InvariantCultureIgnoreCase)
                            || c.Email.Contains(q, System.StringComparison.InvariantCultureIgnoreCase)
                            || c.Phone.Contains(q, System.StringComparison.InvariantCultureIgnoreCase))
                .Select(c => new { id = c.CustomerId, text = c.FullName, fullName = c.FullName, email = c.Email, phone = c.Phone })
                .Take(10)
                .ToList();

            return Json(results);
        }
    }
}
