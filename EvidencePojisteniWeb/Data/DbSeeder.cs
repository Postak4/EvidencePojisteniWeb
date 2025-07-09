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
            var userMgr = services.GetRequiredService<UserManager<AplicationUser>>();

            string[] roles = new[] { "Admin", "Pojištěnec" };
            foreach (var r in roles)
            {
                // Kontrola, zda role již existuje
                if (!await roleMgr.RoleExistsAsync(r))
                {
                    // Vytvoření nové role
                    await roleMgr.CreateAsync(new IdentityRole(r));
                }

                // Vytvoření administrátorského uživatele, pokud ještě neexistuje
                const string adminEmail = "admin@localhost";
                var admin = await userMgr.FindByEmailAsync(adminEmail);
                if (admin == null)
                {
                    admin = new AplicationUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
                    await userMgr.CreateAsync(admin, "Admin!2345");
                    await userMgr.AddToRoleAsync(admin, "Admin");
                }
            }
        }
    }
}
