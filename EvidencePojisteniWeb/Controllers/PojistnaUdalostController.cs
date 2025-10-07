using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using EvidencePojisteniWeb.Data;
using EvidencePojisteniWeb.Models;
using EvidencePojisteniWeb.Models.ViewModels;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EvidencePojisteniWeb.Controllers
{
    public class PojistnaUdalostController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PojistnaUdalostController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: PojistnaUdalost
        public async Task<IActionResult> Index()
        {
            var events = await _context.PojistneUdalosti
                .Include(u => u.Pojisteni)
                .ThenInclude(p => p.PojisteniOsoby)
                .AsNoTracking()
                .ToListAsync();

            // Helper pro display name enumu
            string EnumDisplay<T>(T value) where T : struct, Enum
            {
                var mem = typeof(T).GetMember(value.ToString()).FirstOrDefault();
                var disp = mem?.GetCustomAttributes(typeof(DisplayAttribute), false)
                              .Cast<DisplayAttribute>().FirstOrDefault();
                return disp?.Name ?? value.ToString();
            }

            // Jen události, které mají pojištění i typ pojištění
            var eventsWithType = events
                .Where(e => e.Pojisteni != null && e.Pojisteni.TypPojisteni.HasValue)
                .ToList();

            var stats = eventsWithType
                .GroupBy(e => e.Pojisteni!.TypPojisteni!.Value)
                .Select(g => new InsuranceTypeStats
                {
                    TypPojisteni = EnumDisplay(g.Key),       // hezký popisek do tabulky
                    PocetUdalosti = g.Count(),
                    CelkovaCastka = g.Sum(x => x.Skoda)     // Skoda je decimal (není nullable) ✔
                })
                .OrderByDescending(x => x.PocetUdalosti)
                .ToList();

            var totalClients = await _context.Pojistenci.AsNoTracking().CountAsync();

            var pojistniciCount = await _context.PojisteneOsoby.AsNoTracking()
                .Where(po => po.Role == RoleVuciPojisteni.Pojistnik)
                .Select(po => po.OsobaId).Distinct().CountAsync();

            var pojistenciCount = await _context.PojisteneOsoby.AsNoTracking()
                .Where(po => po.Role == RoleVuciPojisteni.Pojisteny)
                .Select(po => po.OsobaId).Distinct().CountAsync();

            var clients = await _context.Pojistenci
                .Include(p => p.User)
                .AsNoTracking()
                .ToListAsync();

            int? currentPojistenecId = null;
            if (User.Identity?.IsAuthenticated == true && User.IsInRole("Pojistenec"))
            {
                var user = await _userManager.GetUserAsync(User);
                currentPojistenecId = user?.PojistenecModelId;
            }
            ViewBag.CurrentPojistenecId = currentPojistenecId;

            var vm = new PojistnaUdalostIndexViewModel
            {
                Events = events,
                StatsByInsuranceType = stats,
                SumByInsuranceType = stats,   // používáš stejný typ -> recyklujeme
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

        // GET: PojistnaUdalost/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pojistnaUdalostModel = await _context.PojistneUdalosti
                .Include(p => p.Pojisteni)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pojistnaUdalostModel == null)
            {
                return NotFound();
            }

            return View(pojistnaUdalostModel);
        }

        // GET: PojistnaUdalost/Create
        public async Task<IActionResult> Create()
        {
            // Zjištění přihlášeného uživatele
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account"); // Přesměrování na přihlášení, pokud není uživatel přihlášen
            }

            // Načtení jeho pojištěnce (cizí klíč v ApplicationUser)
            var pojistenecId = user.PojistenecModelId;

            // vybrání jen těch pojistných smluv, kde je pojistníkem nebo pojištěncem 
            var mojeSmlouvy = await _context.Pojisteni
                .Where(p => p.PojisteniOsoby.Any(po => po.OsobaId == pojistenecId))
                .Select(p => new
                {
                    p.Id,
                    // zobrazíme název typu a předmět smlouvy
                    Text = p.TypPojisteni.ToString() + " - " + p.PredmetPojisteni
                })
                .ToListAsync();

            // Naplníme ViewBag pro SelectList
            ViewBag.PojisteniId = new SelectList(mojeSmlouvy, "Id", "Text");

            return View();
        }

        // POST: PojistnaUdalost/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PopisUdalosti,DatumUdalosti,MistoUdalosti,Skoda,Vyreseno,Svedek,AdresaSvedka,Poznamka,PojisteniId")] PojistnaUdalostModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var pojistenecId = user?.PojistenecModelId ?? 0;

            // Znovu naplníme ViewBag, pokud vracíme view kvůli chybě validace
            var mojeSmlouvy = await _context.Pojisteni
                .Where(p => p.PojisteniOsoby.Any(po => po.OsobaId == pojistenecId))
                .Select(p => new 
                {
                    p.Id, Text = p.TypPojisteni.ToString() + " - " + p.PredmetPojisteni
                })
                .ToListAsync();

            ViewBag.PojisteniId = new SelectList(mojeSmlouvy, "Id", "Text", model.PojisteniId);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // pokud je validní uložíme
            _context.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: PojistnaUdalost/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pojistnaUdalostModel = await _context.PojistneUdalosti.FindAsync(id);
            if (pojistnaUdalostModel == null)
            {
                return NotFound();
            }
            ViewData["PojisteniId"] = new SelectList(_context.Pojisteni, "Id", "PredmetPojisteni", pojistnaUdalostModel.PojisteniId);
            return View(pojistnaUdalostModel);
        }

        // POST: PojistnaUdalost/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PopisUdalosti,DatumUdalosti,MistoUdalosti,Skoda,Vyreseno,Svedek,AdresaSvedka,Poznamka,PojisteniId")] PojistnaUdalostModel pojistnaUdalostModel)
        {
            if (id != pojistnaUdalostModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pojistnaUdalostModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PojistnaUdalostModelExists(pojistnaUdalostModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PojisteniId"] = new SelectList(_context.Pojisteni, "Id", "PredmetPojisteni", pojistnaUdalostModel.PojisteniId);
            return View(pojistnaUdalostModel);
        }

        // GET: PojistnaUdalost/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pojistnaUdalostModel = await _context.PojistneUdalosti
                .Include(p => p.Pojisteni)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pojistnaUdalostModel == null)
            {
                return NotFound();
            }

            return View(pojistnaUdalostModel);
        }

        // POST: PojistnaUdalost/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pojistnaUdalostModel = await _context.PojistneUdalosti.FindAsync(id);
            if (pojistnaUdalostModel != null)
            {
                _context.PojistneUdalosti.Remove(pojistnaUdalostModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PojistnaUdalostModelExists(int id)
        {
            return _context.PojistneUdalosti.Any(e => e.Id == id);
        }
    }
}
