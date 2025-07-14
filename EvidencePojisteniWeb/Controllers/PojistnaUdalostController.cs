using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EvidencePojisteniWeb.Data;
using EvidencePojisteniWeb.Models;
using EvidencePojisteniWeb.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

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
            // Základní seznam událostí
            var events = await _context.PojistneUdalosti
                .Include(u => u.Pojisteni)
                .ThenInclude(p => p.PojisteniOsoby)
                .ToListAsync();

            // Statistiky podle typu pojištění (počet událostí)
            var stats = events
                .GroupBy(e => e.Pojisteni.TypPojisteni)
                .Select(g => new InsuranceTypeStats
                {
                    TypPojisteni = g.Key.ToString(),
                    PocetUdalosti = g.Count(),
                    CelkovaCastka = g.Sum(x => x.Skoda)
                })
                .ToList();

            // Počty klientů: Celkový, pojistníci, pojištěnci
            var totalClients = await _context.Pojistenci.CountAsync();
            var pojistniciCount = await _context.PojisteneOsoby
                .Where(po => po.Role == RoleVuciPojisteni.Pojistnik)
                .Select(po => po.OsobaId)
                .Distinct()
                .CountAsync();
            var pojistenciCount = await _context.PojisteneOsoby
                .Where(po => po.Role == RoleVuciPojisteni.Pojisteny)
                .Select(po => po.OsobaId)
                .Distinct()
                .CountAsync();

            // Sumy částek k pojištěním (celková suma škod podle typu)
            var sumByType = stats;

            // načteme všechny Pojistenci s User (pro případ, že je budu dál potřebovat)
            var clients = await _context.Pojistenci
                .Include(p => p.User)      // pokud potřebujete navigační vlastnost k User
                .ToListAsync();

            //zajištění Id právě přihlášeného uživatele
            int? currentPojistenecId = null;
            if (User.Identity.IsAuthenticated && User.IsInRole("Pojistenec"))
            {
                var user = await _userManager.GetUserAsync(User);
                currentPojistenecId = user?.PojistenecModelId;
            }
            ViewBag.CurrentPojistenecId = currentPojistenecId;

            // Naplnění viewmodelu
            var vm = new PojistnaUdalostIndexViewModel
            {
                Events = events,
                StatsByInsuranceType = stats,
                Counts = new ClientCounts
                { 
                    CelkovyPocetKlientu = totalClients,
                    PocetPojistniku = pojistniciCount,
                    PocetPojistencu = pojistenciCount
                },
                SumByInsuranceType = sumByType,
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
