using ATLManager.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace ATLManager
{
    public static class Configurations
    {
        public static async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ATLManagerUser>>();
            string[] roleNames = { "Administrador", "Coordenador", "Funcionário", "EE", "Default" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist) await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            var admin = new ATLManagerUser
            {
                FirstName = "Admin",
                LastName = "Account",
                UserName = "admin@atlmanager.pt",
                Email = "admin@atlmanager.pt"
            };

            var _user = await userManager.FindByEmailAsync(admin.Email);
            if (_user != null) return;

            var createPowerUser = await userManager.CreateAsync(admin, "Password_123");
            if (createPowerUser.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Administrador");
                var code = await userManager.GenerateEmailConfirmationTokenAsync(admin);
                await userManager.ConfirmEmailAsync(admin, code);
            }
        }
    }
}
