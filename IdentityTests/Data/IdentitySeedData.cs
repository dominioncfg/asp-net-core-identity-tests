using IdentityTests.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace IdentityTests.Data
{
    public class IdentitySeedData
    {
        public static void CreateIdentitySeedData(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            CreateRoles(serviceProvider).GetAwaiter().GetResult();
            CreateAdminAccountAsync(serviceProvider, configuration).GetAwaiter().GetResult();
        }

        private static async Task CreateRoles(IServiceProvider serviceProvider)
        {
            serviceProvider = serviceProvider.CreateScope().ServiceProvider;
            RoleManager<IdentityRole> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var rolesNames = Enum.GetNames(typeof(Roles));
            foreach (string role in rolesNames)
            {
                if (await roleManager.FindByNameAsync(role) == null)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        public static async Task CreateAdminAccountAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            serviceProvider = serviceProvider.CreateScope().ServiceProvider;
            UserManager<IdentityUser> userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            IOptions<IdentitySeedConfiguration> settings = serviceProvider.GetRequiredService<IOptions<IdentitySeedConfiguration>>();

            string username = settings.Value.AdminEmail;
            string email = settings.Value.AdminEmail;
            string password = settings.Value.AdminEmailPassword;

            if (await userManager.FindByNameAsync(username) == null)
            {
                IdentityUser user = new IdentityUser
                {
                    UserName = username,
                    Email = email,
                    EmailConfirmed = true,
                };

                IdentityResult result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    string adminRole = Roles.Admin.ToString();
                    await userManager.AddToRoleAsync(user, adminRole);
                }
            }
        }
    }
}
