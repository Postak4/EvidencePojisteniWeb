using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using EvidencePojisteniWeb.Models;
using Microsoft.AspNetCore.Identity;

namespace EvidencePojisteniWeb.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRoleAndAdminAsync(IServiceProvider services)
        {
            // Získání RoleManager a UserManager z DI kontejneru
            var roleMgr = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userMgr = services.GetRequiredService<UserManager<ApplicationUser>>();

            // vytvoř všechny potřebné role
            string[] roles = new[] { "Admin", "Pojištěnec" };
            foreach (var role in roles)
            {
                // Kontrola, zda role již existuje
                if (!await roleMgr.RoleExistsAsync(role))
                {
                    // Vytvoření nové role
                    await roleMgr.CreateAsync(new IdentityRole(role));
                }
            }

            // Vytvoření administrátorského uživatele, pokud ještě neexistuje jen jednou
            const string adminEmail = "admin@localhost";
            var admin = await userMgr.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                await userMgr.CreateAsync(admin, "Admin!2345");
                await userMgr.AddToRoleAsync(admin, "Admin");
            }

        }
    }
}
