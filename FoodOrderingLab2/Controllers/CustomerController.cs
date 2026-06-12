using Microsoft.AspNetCore.Mvc;
using FoodOrderingLab2.Repositories;
using FoodOrderingLab2.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using FoodOrderingLab2.Models;

namespace FoodOrderingLab2.Controllers
{
    [Route("kupci")]
    [Authorize]
    public class CustomerController : Controller
    {
        private readonly CustomerRepository _customerRepository;
        private readonly UserManager<AppUser> _userManager;

        public CustomerController(CustomerRepository customerRepository, UserManager<AppUser> userManager)
        {
            _customerRepository = customerRepository;
            _userManager = userManager;
        }

        [Route("")]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Index()
        {
            var customers = _customerRepository.GetAll();
            return View(customers);
        }

        [HttpGet]
        [Route("create")]
        [Authorize(Roles = "Admin,Manager")]
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
        [Authorize(Roles = "Admin,Manager")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CustomerCreateViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Email) && _customerRepository.EmailExists(model.Email))
            {
                ModelState.AddModelError(nameof(model.Email), "Kupac s ovim emailom već postoji.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var customer = new Models.Customer
            {
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
        [Authorize(Roles = "Admin,Manager")]
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
        [Authorize(Roles = "Admin,Manager")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, CustomerCreateViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Email) && _customerRepository.EmailExists(model.Email, id))
            {
                ModelState.AddModelError(nameof(model.Email), "Kupac s ovim emailom već postoji.");
            }

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
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
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

            if (!string.IsNullOrWhiteSpace(customer.AppUserId))
            {
                var appUser = await _userManager.FindByIdAsync(customer.AppUserId);
                if (appUser != null)
                {
                    var result = await _userManager.DeleteAsync(appUser);
                    if (!result.Succeeded)
                    {
                        TempData["ErrorMessage"] = "Korisnički račun nije moguće obrisati.";
                        return RedirectToAction("Details", new { id });
                    }
                }
            }

            _customerRepository.Delete(customer);
            return RedirectToAction("Index");
        }

        [Route("{id:int}")]
        [Authorize(Roles = "Admin,Manager")]
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
        [Authorize(Roles = "Admin,Manager")]
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

        [HttpGet]
        [Route("moj-profil")]
        public IActionResult MyProfile()
        {
            var customer = CurrentCustomer;
            if (customer == null)
            {
                return RedirectToPage("/Account/Register", new { area = "Identity" });
            }

            ViewBag.Email = customer.Email;
            return View(new CustomerProfileViewModel
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Phone = customer.Phone,
                Address = customer.Address
            });
        }

        [HttpPost]
        [Route("moj-profil")]
        [ValidateAntiForgeryToken]
        public IActionResult MyProfile(CustomerProfileViewModel model)
        {
            var customer = CurrentCustomer;
            if (customer == null)
            {
                return RedirectToPage("/Account/Register", new { area = "Identity" });
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Email = customer.Email;
                return View(model);
            }

            customer.FirstName = model.FirstName;
            customer.LastName = model.LastName;
            customer.Phone = model.Phone;
            customer.Address = model.Address;
            _customerRepository.Update(customer);
            TempData["SuccessMessage"] = "Profil je spremljen.";
            return RedirectToAction(nameof(MyProfile));
        }

        private Customer? CurrentCustomer
        {
            get
            {
                var userId = _userManager.GetUserId(User);
                return string.IsNullOrWhiteSpace(userId) ? null : _customerRepository.GetByAppUserId(userId);
            }
        }
    }
}
