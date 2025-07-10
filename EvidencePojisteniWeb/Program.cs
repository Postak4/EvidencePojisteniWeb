using System.Threading.Tasks;
using EvidencePojisteniWeb.Data;
using EvidencePojisteniWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EvidencePojisteniWeb
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1) Načtení connection stringu z appsettings.json
            var connectionString = builder.Configuration
                .GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            // 2) Registrace EF Core DbContextu
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString)); 

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // 3) Registrace EF Core DbContextu a ASP NET Identity
            // místi AddDefaultIdentity, protože chceme přidat vlastní ApplicationUser
            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                // sem pak případně další nastavení Identity hesla, lockout atd.
            })
            .AddRoles<IdentityRole>()                           // přidá služby RoleManageru a rolí
            .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // 4) Inicializace databáze a seedování dat
            using (var scope = app.Services.CreateScope())
            {
                await DbSeeder.SeedRoleAndAdminAsync(scope.ServiceProvider);
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // Mapování a routování pro MVC
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Vypnutí Razor Pages s původními statickými soubory
            app.MapRazorPages();

            app.Run();
        }
    }
}
