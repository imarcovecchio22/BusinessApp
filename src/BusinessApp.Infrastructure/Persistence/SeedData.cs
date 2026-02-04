using BusinessApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace BusinessApp.Infrastructure.Persistence;

public static class SeedData
{
    public static async Task InitializeAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        // Verificar si ya existe el usuario admin
        var adminUser = await userManager.FindByEmailAsync("admin@businessapp.com");
        
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = "admin@businessapp.com",
                Email = "admin@businessapp.com",
                FirstName = "Admin",
                LastName = "System",
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}
