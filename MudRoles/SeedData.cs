using Microsoft.AspNetCore.Identity;
using MudRoles.Data;

namespace MudRoles
{
    public class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, UserManager<ApplicationUser> userManager)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var logger = serviceProvider.GetRequiredService<ILogger<SeedData>>();

            string[] roleNames = { "Admin", "User" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    // Create the roles and seed them to the database
                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                    if (!roleResult.Succeeded)
                    {
                        logger.LogError($"Error creating role {roleName}");
                    }
                }
            }

            // Create demo users
            await CreateDemoUser(userManager, "admin@example.com", "Admin123!", "Admin");
            await CreateDemoUser(userManager, "user@example.com", "User123!", "User");
        }

        private static async Task CreateDemoUser(UserManager<ApplicationUser> userManager, string email, string password, string role)
        {
            if (userManager.Users.All(u => u.Email != email))
            {
                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }
    }
}
