using System.ComponentModel.DataAnnotations;
using FoodOrderingLab2.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FoodOrderingLab2.Areas.Identity.Pages.Account;

public class LoginModel(SignInManager<AppUser> signInManager) : PageModel
{
    [BindProperty] public InputModel Input { get; set; } = new();
    public IList<AuthenticationScheme> ExternalLogins { get; set; } = [];
    public string? ReturnUrl { get; set; }

    public class InputModel
    {
        [Required, EmailAddress] public string Email { get; set; } = null!;
        [Required, DataType(DataType.Password), Display(Name = "Lozinka")] public string Password { get; set; } = null!;
        [Display(Name = "Zapamti me")] public bool RememberMe { get; set; }
    }

    public async Task OnGetAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;
        ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        if (!ModelState.IsValid) return Page();

        var result = await signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, false);
        if (result.Succeeded) return LocalRedirect(returnUrl);

        ModelState.AddModelError(string.Empty, "Neispravan email ili lozinka.");
        return Page();
    }
}
