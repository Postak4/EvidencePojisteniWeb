using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

using EvidencePojisteniWeb.Data;
using EvidencePojisteniWeb.Models;
using EvidencePojisteniWeb.Models.ViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EvidencePojisteniWeb.Controllers
{
    //[Authorize(Roles = "Admin, Pojistenec")]
    public class PojistnaUdalostController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PojistnaUdalostController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ---------- Helpers ----------
        private static string GetEnumDisplayName<T>(T? value) where T : struct, Enum
        {
            if (value == null) return "";
            var mem = typeof(T).GetMember(value.Value.ToString()).FirstOrDefault();
            var disp = mem?.GetCustomAttributes(typeof(DisplayAttribute), false)
                           .Cast<DisplayAttribute>().FirstOrDefault();
            return disp?.Name ?? value.Value.ToString();
        }

        private async Task<int?> GetCurrentPojistenecIdAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            return user?.PojistenecModelId;
        }

        private async Task<List<SelectListItem>> GetInsuranceOptionsForUserAsync(int osobaId, int? selectedId = null)
        {
            var smlouvy = await _context.Pojisteni
                .AsNoTracking()
                .Where(p => p.PojisteniOsoby.Any(po => po.OsobaId == osobaId))
                .Select(p => new
                {
                    p.Id,
                    Text = GetEnumDisplayName<TypPojisteni>(p.TypPojisteni) + " - " + p.PredmetPojisteni
                })
                .ToListAsync();

            return smlouvy
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Text, Selected = selectedId == x.Id })
                .ToList();
        }

        private async Task<bool> InsuranceBelongsToAsync(int pojisteniId, int osobaId)
        {
            return await _context.PojisteneOsoby.AsNoTracking()
                .AnyAsync(po => po.PojisteniId == pojisteniId && po.OsobaId == osobaId);
        }

        private bool IsAdmin() => User.IsInRole("Admin");

        // ---------- INDEX ----------
        public async Task<IActionResult> Index()
        {
            var events = await _context.PojistneUdalosti
                .Include(u => u.Pojisteni)
                .ThenInclude(p => p.PojisteniOsoby)
                .AsNoTracking()
                .ToListAsync();

            // statistiky jen pro události s vyplněným typem pojištění
            var stats = events
                .Where(e => e.Pojisteni != null && e.Pojisteni.TypPojisteni.HasValue)
                .GroupBy(e => e.Pojisteni!.TypPojisteni!.Value)
                .Select(g => new InsuranceTypeStats
                {
                    TypPojisteni = GetEnumDisplayName<TypPojisteni>(g.Key),
                    PocetUdalosti = g.Count(),
                    CelkovaCastka = g.Sum(x => x.Skoda)
                })
                .OrderByDescending(x => x.PocetUdalosti)
                .ToList();

            var totalClients = await _context.Pojistenci.AsNoTracking().CountAsync();
            var pojistniciCount = await _context.PojisteneOsoby.AsNoTracking()
                .Where(po => po.Role == RoleVuciPojisteni.Pojistnik).Select(po => po.OsobaId).Distinct().CountAsync();
            var pojistenciCount = await _context.PojisteneOsoby.AsNoTracking()
                .Where(po => po.Role == RoleVuciPojisteni.Pojisteny).Select(po => po.OsobaId).Distinct().CountAsync();

            var clients = await _context.Pojistenci
                .Include(p => p.User)
                .AsNoTracking()
                .ToListAsync();

            int? currentPojistenecId = null;
            if (User.Identity?.IsAuthenticated == true && User.IsInRole("Pojistenec"))
            {
                currentPojistenecId = await GetCurrentPojistenecIdAsync();
            }
            ViewBag.CurrentPojistenecId = currentPojistenecId;

            var vm = new PojistnaUdalostIndexViewModel
            {
                Events = events,
                StatsByInsuranceType = stats,
                SumByInsuranceType = stats, // v tvém view je to totéž
                Counts = new ClientCounts
                {
                    CelkovyPocetKlientu = totalClients,
                    PocetPojistniku = pojistniciCount,
                    PocetPojistencu = pojistenciCount
                },
                Pojistenci = clients
            };

            return View(vm);
        }

        // ---------- DETAILS ----------
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var model = await _context.PojistneUdalosti
                .Include(p => p.Pojisteni)
                .Include(p => p.Osoba)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (model == null) return NotFound();

            // přístup: Admin vždy; Pojistenec jen ke svým
            if (!IsAdmin())
            {
                var osobaId = await GetCurrentPojistenecIdAsync();
                if (osobaId == null || model.OsobaId != osobaId) return Forbid();
            }

            return View(model);
        }

        // ---------- CREATE ----------
        public async Task<IActionResult> Create()
        {
            var osobaId = await GetCurrentPojistenecIdAsync();
            if (osobaId == null) return RedirectToAction("Login", "Account");

            var options = await GetInsuranceOptionsForUserAsync(osobaId.Value);
            if (!options.Any())
            {
                TempData["Warn"] = "Nemáte žádné pojištění. Nejprve si prosím založte pojištění.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.PojisteniId = options;

            return View(new PojistnaUdalostModel
            {
                DatumUdalosti = DateTime.Today
                // OsobaId se NEPOSÍLÁ z UI, nastaví se v POST
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PopisUdalosti,DatumUdalosti,MistoUdalosti,Skoda,Vyreseno,Svedek,AdresaSvedka,Poznamka,PojisteniId")] PojistnaUdalostModel model)
        {
            var osobaId = await GetCurrentPojistenecIdAsync();
            if (osobaId == null) return RedirectToAction("Login", "Account");

            // ověř, že vybrané pojištění patří přihlášenému
            if (!await InsuranceBelongsToAsync(model.PojisteniId, osobaId.Value))
            {
                ModelState.AddModelError(nameof(model.PojisteniId), "Zvolené pojištění nepatří přihlášenému uživateli.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.PojisteniId = await GetInsuranceOptionsForUserAsync(osobaId.Value, model.PojisteniId);
                return View(model);
            }

            model.OsobaId = osobaId.Value; // kriticky: nastavíme na serveru
            _context.Add(model);
            await _context.SaveChangesAsync();

            TempData["Ok"] = "Událost byla vytvořena.";
            return RedirectToAction(nameof(Index));
        }

        // ---------- EDIT ----------
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var model = await _context.PojistneUdalosti.FindAsync(id);
            if (model == null) return NotFound();

            if (!IsAdmin())
            {
                var osobaId = await GetCurrentPojistenecIdAsync();
                if (osobaId == null || model.OsobaId != osobaId) return Forbid();
                ViewBag.PojisteniId = await GetInsuranceOptionsForUserAsync(osobaId.Value, model.PojisteniId);
            }
            else
            {
                // Admin může vybírat libovolné pojištění
                var all = await _context.Pojisteni
                    .AsNoTracking()
                    .Select(p => new
                    {
                        p.Id,
                        Text = GetEnumDisplayName<TypPojisteni>(p.TypPojisteni) + " - " + p.PredmetPojisteni
                    })
                    .ToListAsync();

                ViewBag.PojisteniId = all
                    .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Text, Selected = model.PojisteniId == x.Id })
                    .ToList();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PopisUdalosti,DatumUdalosti,MistoUdalosti,Skoda,Vyreseno,Svedek,AdresaSvedka,Poznamka,PojisteniId")] PojistnaUdalostModel model)
        {
            if (id != model.Id) return NotFound();

            var origin = await _context.PojistneUdalosti.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (origin == null) return NotFound();

            // autorizace vlastníka, pokud není Admin
            if (!IsAdmin())
            {
                var osobaId = await GetCurrentPojistenecIdAsync();
                if (osobaId == null || origin.OsobaId != osobaId) return Forbid();

                // pojisteni musí patřit uživateli (pokud změnil)
                if (!await InsuranceBelongsToAsync(model.PojisteniId, osobaId.Value))
                {
                    ModelState.AddModelError(nameof(model.PojisteniId), "Zvolené pojištění nepatří přihlášenému uživateli.");
                }
            }

            if (!ModelState.IsValid)
            {
                if (!IsAdmin())
                {
                    var osobaId = await GetCurrentPojistenecIdAsync();
                    ViewBag.PojisteniId = osobaId == null
                        ? new List<SelectListItem>()
                        : await GetInsuranceOptionsForUserAsync(osobaId.Value, model.PojisteniId);
                }
                else
                {
                    var all = await _context.Pojisteni
                        .AsNoTracking()
                        .Select(p => new
                        {
                            p.Id,
                            Text = GetEnumDisplayName<TypPojisteni>(p.TypPojisteni) + " - " + p.PredmetPojisteni
                        })
                        .ToListAsync();

                    ViewBag.PojisteniId = all
                        .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Text, Selected = model.PojisteniId == x.Id })
                        .ToList();
                }
                return View(model);
            }

            // zachovat vlastnictví (OsobaId) – nebindujeme z UI
            model.OsobaId = origin.OsobaId;

            try
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                TempData["Ok"] = "Událost byla upravena.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.PojistneUdalosti.Any(e => e.Id == id)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // ---------- DELETE ----------
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var model = await _context.PojistneUdalosti
                .Include(p => p.Pojisteni)
                .Include(p => p.Osoba)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (model == null) return NotFound();

            if (!IsAdmin())
            {
                var osobaId = await GetCurrentPojistenecIdAsync();
                if (osobaId == null || model.OsobaId != osobaId) return Forbid();
            }

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _context.PojistneUdalosti.FindAsync(id);
            if (model == null) return RedirectToAction(nameof(Index));

            if (!IsAdmin())
            {
                var osobaId = await GetCurrentPojistenecIdAsync();
                if (osobaId == null || model.OsobaId != osobaId) return Forbid();
            }

            _context.PojistneUdalosti.Remove(model);
            await _context.SaveChangesAsync();

            TempData["Ok"] = "Událost byla odstraněna.";
            return RedirectToAction(nameof(Index));
        }
    }
}
