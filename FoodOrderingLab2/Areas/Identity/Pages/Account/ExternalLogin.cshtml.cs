using System.ComponentModel.DataAnnotations;
using FoodOrderingLab2.Models;
using FoodOrderingLab2.Data;
using FoodOrderingLab2.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderingLab2.Areas.Identity.Pages.Account;

public class ExternalLoginModel(
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager,
    ApplicationDbContext db) : PageModel
{
    [BindProperty] public InputModel Input { get; set; } = new();
    public string? ProviderDisplayName { get; set; }
    public string ReturnUrl { get; set; } = "/";

    public class InputModel
    {
        [Required, EmailAddress] public string Email { get; set; } = null!;
        [Required, StringLength(11, MinimumLength = 11), RegularExpression("^[0-9]*$")] public string OIB { get; set; } = null!;
        [Required, StringLength(13, MinimumLength = 13), RegularExpression("^[0-9]*$")] public string JMBG { get; set; } = null!;
        [Required, StringLength(100), Display(Name = "Ime")] public string FirstName { get; set; } = null!;
        [Required, StringLength(100), Display(Name = "Prezime")] public string LastName { get; set; } = null!;
        [Required, Phone, CroatianPhone, StringLength(50), Display(Name = "Telefon")] public string Phone { get; set; } = null!;
        [Required, StringLength(300), Display(Name = "Adresa")] public string Address { get; set; } = null!;
    }

    public IActionResult OnPost(string provider, string? returnUrl = null)
    {
        var redirectUrl = Url.Page("./ExternalLogin", "Callback", new { returnUrl });
        var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return new ChallengeResult(provider, properties);
    }

    public async Task<IActionResult> OnGetCallbackAsync(string? returnUrl = null, string? remoteError = null)
    {
        ReturnUrl = returnUrl ?? Url.Content("~/");
        if (remoteError != null)
        {
            ModelState.AddModelError(string.Empty, $"Vanjska prijava nije uspjela: {remoteError}");
            return Page();
        }

        var info = await signInManager.GetExternalLoginInfoAsync();
        if (info == null) return RedirectToPage("./Login");
        var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, true);
        if (result.Succeeded) return LocalRedirect(ReturnUrl);

        ProviderDisplayName = info.ProviderDisplayName;
        Input.Email = info.Principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? string.Empty;
        return Page();
    }

    public async Task<IActionResult> OnPostConfirmationAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl ?? Url.Content("~/");
        var info = await signInManager.GetExternalLoginInfoAsync();
        if (info == null) return RedirectToPage("./Login");
        if (!ModelState.IsValid) return Page();
        var normalizedEmail = Input.Email.Trim().ToUpper();
        if (await db.Customers.AnyAsync(x => x.Email.ToUpper() == normalizedEmail))
        {
            ModelState.AddModelError("Input.Email", "Kupac s ovim emailom već postoji.");
            ProviderDisplayName = info.ProviderDisplayName;
            return Page();
        }

        var user = new AppUser { UserName = Input.Email, Email = Input.Email, OIB = Input.OIB, JMBG = Input.JMBG };
        var result = await userManager.CreateAsync(user);
        if (result.Succeeded)
        {
            result = await userManager.AddLoginAsync(user, info);
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
                await signInManager.SignInAsync(user, false);
                return LocalRedirect(ReturnUrl);
            }
        }
        foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
        ProviderDisplayName = info.ProviderDisplayName;
        return Page();
    }
}
