using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace WebApplication1.Data
{
    public static class SeedData
    {
        public const string AdminRole = "Admin";
        private const string AdminEmail = "admin@shop.com";
        private const string AdminPassword = "Admin123";

        public static async Task InitializeAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

            // 1. Create the Admin role if it doesn't exist
            if (!await roleManager.RoleExistsAsync(AdminRole))
                await roleManager.CreateAsync(new IdentityRole(AdminRole));

            // 2. Create the admin user if it doesn't exist
            var admin = await userManager.FindByEmailAsync(AdminEmail);
            if (admin == null)
            {
                admin = new IdentityUser
                {
                    UserName = AdminEmail,
                    Email = AdminEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(admin, AdminPassword);
            }

            // 3. Ensure the admin user is in the Admin role
            if (!await userManager.IsInRoleAsync(admin, AdminRole))
                await userManager.AddToRoleAsync(admin, AdminRole);
        }
    }
}
