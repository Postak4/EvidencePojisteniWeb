using EvidencePojisteniWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EvidencePojisteniWeb.Controllers
{
    public class DemoController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public DemoController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> LoginAsAdmin()
        {
            var admin = await _userManager.FindByEmailAsync("admin@localhost");
            if (admin == null)
            {
                return NotFound("Admin není nalezen.");
            }

            // Když je někdo přihlášený, odhlásíme ho
            await _signInManager.SignOutAsync();

            // Přihlásíme administrátora
            await _signInManager.SignInAsync(admin, isPersistent: false);

            // Přesměrujeme na domovskou stránku
            return RedirectToAction("Index", "Home");
        }
    }
}
