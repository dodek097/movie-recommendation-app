using FoodOrderingLab2.Models;
using Microsoft.AspNetCore.Identity;

namespace FoodOrderingLab2.Data;

public static class IdentitySeeder
{
    public const string AdminEmail = "admin@foodorder.local";
    public const string AdminPassword = "Admin123!";
    public const string ManagerEmail = "manager@foodorder.local";
    public const string ManagerPassword = "Manager123!";

    public static async Task SeedAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<AppUser>>();

        foreach (var role in new[] { "Admin", "Manager" })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        await EnsureUserAsync(userManager, AdminEmail, AdminPassword, "Admin", "12345678901", "1234567890123");
        await EnsureUserAsync(userManager, ManagerEmail, ManagerPassword, "Manager", "10987654321", "3210987654321");
    }

    private static async Task EnsureUserAsync(
        UserManager<AppUser> userManager,
        string email,
        string password,
        string role,
        string oib,
        string jmbg)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new AppUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                OIB = oib,
                JMBG = jmbg
            };

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));
            }
        }

        if (!await userManager.IsInRoleAsync(user, role))
        {
            await userManager.AddToRoleAsync(user, role);
        }
    }
}
