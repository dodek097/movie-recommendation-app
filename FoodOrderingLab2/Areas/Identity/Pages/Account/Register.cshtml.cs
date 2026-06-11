using System.ComponentModel.DataAnnotations;
using FoodOrderingLab2.Models;
using FoodOrderingLab2.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FoodOrderingLab2.Areas.Identity.Pages.Account;

public class RegisterModel(
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager,
    ApplicationDbContext db,
    ILogger<RegisterModel> logger) : PageModel
{
    [BindProperty] public InputModel Input { get; set; } = new();
    public string? ReturnUrl { get; set; }
    public IList<AuthenticationScheme> ExternalLogins { get; set; } = [];
    public bool IsProfileRecovery { get; set; }

    public class InputModel
    {
        [Required, EmailAddress] public string Email { get; set; } = null!;
        [Required, StringLength(100, MinimumLength = 6), DataType(DataType.Password)] public string Password { get; set; } = null!;
        [DataType(DataType.Password), Compare(nameof(Password))] public string ConfirmPassword { get; set; } = null!;
        [Required, StringLength(11, MinimumLength = 11), RegularExpression("^[0-9]*$"), Display(Name = "OIB")] public string OIB { get; set; } = null!;
        [Required, StringLength(13, MinimumLength = 13), RegularExpression("^[0-9]*$"), Display(Name = "JMBG")] public string JMBG { get; set; } = null!;
        [Required, StringLength(100), Display(Name = "Ime")] public string FirstName { get; set; } = null!;
        [Required, StringLength(100), Display(Name = "Prezime")] public string LastName { get; set; } = null!;
        [Required, Phone, StringLength(50), RegularExpression(@"^\+?[0-9][0-9\s()\-]{6,49}$", ErrorMessage = "Unesi valjan broj telefona."), Display(Name = "Telefon")] public string Phone { get; set; } = null!;
        [Required, StringLength(300), Display(Name = "Adresa")] public string Address { get; set; } = null!;
    }

    public async Task OnGetAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;
        ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        await PrepareProfileRecoveryAsync();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        var currentUser = await userManager.GetUserAsync(User);
        if (currentUser != null && !db.Customers.Any(c => c.AppUserId == currentUser.Id))
        {
            IsProfileRecovery = true;
            foreach (var key in new[] { "Input.Email", "Input.Password", "Input.ConfirmPassword", "Input.OIB", "Input.JMBG" })
            {
                ModelState.Remove(key);
            }
            if (!ModelState.IsValid) return Page();

            db.Customers.Add(new Customer
            {
                AppUserId = currentUser.Id,
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                Email = currentUser.Email ?? currentUser.UserName ?? string.Empty,
                Phone = Input.Phone,
                Address = Input.Address,
                RegisterDate = DateTime.UtcNow
            });
            await db.SaveChangesAsync();
            logger.LogInformation("Obnovljen je profil kupca za postojeći korisnički račun.");
            return LocalRedirect(returnUrl);
        }

        if (!ModelState.IsValid) return Page();

        var user = new AppUser { UserName = Input.Email, Email = Input.Email, OIB = Input.OIB, JMBG = Input.JMBG };
        var result = await userManager.CreateAsync(user, Input.Password);
        if (result.Succeeded)
        {
            db.Customers.Add(new Customer
            {
                AppUserId = user.Id,
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                Email = Input.Email,
                Phone = Input.Phone,
                Address = Input.Address,
                RegisterDate = DateTime.UtcNow
            });
            await db.SaveChangesAsync();
            logger.LogInformation("Kreiran je novi lokalni korisnički račun.");
            await signInManager.SignInAsync(user, isPersistent: false);
            return LocalRedirect(returnUrl);
        }

        foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
        return Page();
    }

    private async Task PrepareProfileRecoveryAsync()
    {
        var currentUser = await userManager.GetUserAsync(User);
        if (currentUser == null || db.Customers.Any(c => c.AppUserId == currentUser.Id)) return;

        IsProfileRecovery = true;
        Input.Email = currentUser.Email ?? currentUser.UserName ?? string.Empty;
        Input.OIB = currentUser.OIB;
        Input.JMBG = currentUser.JMBG;
    }
}
