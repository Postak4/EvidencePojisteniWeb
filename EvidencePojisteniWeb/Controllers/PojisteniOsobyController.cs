using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EvidencePojisteniWeb.Data;
using EvidencePojisteniWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace EvidencePojisteniWeb.Controllers
{
    // aby na stránku nezavítal nějaký nezvaný host, přidáme autorizaci
    [Authorize]
    public class PojisteniOsobyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public PojisteniOsobyController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // GET: PojisteniOsoby
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.PojisteneOsoby.Include(p => p.Osoba).Include(p => p.Pojisteni);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: PojisteniOsoby/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pojisteniOsobyModel = await _context.PojisteneOsoby
                .Include(p => p.Osoba)
                .Include(p => p.Pojisteni)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pojisteniOsobyModel == null)
            {
                return NotFound();
            }

            return View(pojisteniOsobyModel);
        }

        // GET: PojisteniOsoby/Create
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Create(TypPojisteni typPojisteni)
        {
            // ověříme uživatele a získáme jeho profil pojištěnce
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.PojistenecModelId == null)
            {
                // pokud nemá profil pošleme ho do oregistrace
                return RedirectToAction("Create", "Pojistenec");
            }
            int pojistenecId = user.PojistenecModelId.Value;

            //Připravíme ViewBag a select listy pro formulář
            var pojistenec = await _context.Pojistenci.FindAsync(pojistenecId);
            ViewBag.Pojistenec = pojistenec;
            ViewBag.Role = new SelectList(Enum.GetValues<RoleVuciPojisteni>());
            ViewBag.TypPojisteni = new SelectList(
                Enum.GetValues<TypPojisteni>()
                    .Cast<TypPojisteni>()
                    .Select(e => new { Id = e, Name = e.ToString() }),
                "Id", "Name", typPojisteni);

            // Vytvoříme model s předvyplněnými hodnotami
            var model = new PojisteniOsobyModel
            {
                OsobaId = pojistenecId,
                PlatnostOd = DateTime.Today,
                PlatnostDo = DateTime.Today.AddYears(1), // Přednastavení platnosti na 1 rok
                Pojisteni = new PojisteniModel
                {
                    TypPojisteni = typPojisteni,
                }
            };

            return View(model);
        }

        // POST: PojisteniOsoby/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PojisteniOsobyModel model)
        {
            // Always prepare select lists for validation errors
            ViewBag.Pojistenec = await _context.Pojistenci.FindAsync(model.OsobaId);
            ViewBag.TypPojisteni = new SelectList(Enum.GetValues<TypPojisteni>());
            ViewBag.Role = new SelectList(Enum.GetValues<RoleVuciPojisteni>());


            // Ensure nested object is not null (edge-case, almost redundant if view is OK)
            if (model.Pojisteni == null)
                model.Pojisteni = new PojisteniModel();

            if (!ModelState.IsValid)
            {
                // Return view with filled data and validation summary
                return View(model);
            }

            // Save new insurance
            _context.Pojisteni.Add(model.Pojisteni!);
            await _context.SaveChangesAsync();

            // Link to person
            model.PojisteniId = model.Pojisteni.Id;
            model.Pojisteni = null; // avoid circular reference

            _context.PojisteneOsoby.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Pojistenec", new { id = model.OsobaId });
        }


        // GET: PojisteniOsoby/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pojisteniOsobyModel = await _context.PojisteneOsoby
                .Include(p => p.Pojisteni)
                .Include(p => p.Osoba)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pojisteniOsobyModel == null)
            {
                return NotFound();
            }

            ViewBag.Pojistenec = pojisteniOsobyModel.Osoba;
            ViewBag.TypPojisteni = new SelectList(Enum.GetValues<TypPojisteni>());
            ViewBag.Role = new SelectList(Enum.GetValues<RoleVuciPojisteni>());

            return View(pojisteniOsobyModel);
        }

        // POST: PojisteniOsoby/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OsobaId,PojisteniId,Role")] PojisteniOsobyModel pojisteniOsobyModel)
        {
            if (id != pojisteniOsobyModel.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Pojistenec = await _context.Pojistenci.FindAsync(pojisteniOsobyModel.OsobaId);
                ViewBag.TypPojisteni = new SelectList(Enum.GetValues<TypPojisteni>());
                ViewBag.Role = new SelectList(Enum.GetValues<RoleVuciPojisteni>());

                return View(pojisteniOsobyModel);
            }

            try
            {
                _context.Update(pojisteniOsobyModel);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PojisteniOsobyModelExists(pojisteniOsobyModel.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction("Details", "Pojistenec", new { id = pojisteniOsobyModel.OsobaId });
        }

        // GET: PojisteniOsoby/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pojisteniOsobyModel = await _context.PojisteneOsoby
                .Include(p => p.Osoba)
                .Include(p => p.Pojisteni)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pojisteniOsobyModel == null)
            {
                return NotFound();
            }

            return View(pojisteniOsobyModel);
        }

        // POST: PojisteniOsoby/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pojisteniOsobyModel = await _context.PojisteneOsoby.FindAsync(id);
            if (pojisteniOsobyModel != null)
            {
                _context.PojisteneOsoby.Remove(pojisteniOsobyModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PojisteniOsobyModelExists(int id)
        {
            return _context.PojisteneOsoby.Any(e => e.Id == id);
        }
    }
}
