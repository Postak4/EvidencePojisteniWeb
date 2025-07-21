using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EvidencePojisteniWeb.Data;
using EvidencePojisteniWeb.Models;
using EvidencePojisteniWeb.Models.ViewModels;
using EvidencePojisteniWeb.Helpers;
using Microsoft.AspNetCore.Identity;

namespace EvidencePojisteniWeb.Controllers
{
    public class PojisteniController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public PojisteniController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Pojisteni
        public async Task<IActionResult> Index()
        {
            var karty = Enum.GetValues<TypPojisteni>().Select(typ => new TypPojisteniCardViewModel
            {
                Typ = typ,
                Nazev = typ.GetDisplayName(),
                Popis = typ switch
                {
                    TypPojisteni.Zivotni => "Pojištění pro případ úmrtí nebo dožití.",
                    TypPojisteni.Urazove => "Kryje úraz, trvalé následky či hospitalizaci.",
                    TypPojisteni.Cestovni => "Zajištění pro cesty do zahraničí.",
                    TypPojisteni.Majetkove => "Pojištění domu, bytu a domácnosti.",
                    TypPojisteni.Odpovednost => "Krytí škod způsobených jiným osobám.",
                    _ => "Popis není k dispozici."
                },
                ObrazekUrl = $"/images/typyPojisteni/{typ.ToString().ToLower()}.jpg"
            }).ToList();

            return View(karty);
        }

        // GET: Pojisteni/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pojisteniModel = await _context.Pojisteni
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pojisteniModel == null)
            {
                return NotFound();
            }

            return View(pojisteniModel);
        }

        // GET: Pojisteni/Create
        public IActionResult Create()
        {
            // Už žádné ViewData["PojistenecId"]
            return View();
        }

        // POST: Pojisteni/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TypPojisteni,PredmetPojisteni,Castka")] PojisteniModel pojisteniModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pojisteniModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pojisteniModel);
        }

        // GET: Pojisteni/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pojisteniModel = await _context.Pojisteni.FindAsync(id);
            if (pojisteniModel == null)
            {
                return NotFound();
            }
            return View(pojisteniModel);
        }

        // POST: Pojisteni/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TypPojisteni,PredmetPojisteni,Castka")] PojisteniModel pojisteniModel)
        {
            if (id != pojisteniModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pojisteniModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PojisteniModelExists(pojisteniModel.Id))
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
            return View(pojisteniModel);
        }

        // GET: Pojisteni/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pojisteniModel = await _context.Pojisteni
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pojisteniModel == null)
            {
                return NotFound();
            }

            return View(pojisteniModel);
        }

        // POST: Pojisteni/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pojisteniModel = await _context.Pojisteni.FindAsync(id);
            if (pojisteniModel != null)
            {
                _context.Pojisteni.Remove(pojisteniModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PojisteniModelExists(int id)
        {
            return _context.Pojisteni.Any(e => e.Id == id);
        }

        // GET: Pojisteni/TypDetail/5
        [HttpGet]
        public async Task<IActionResult> TypDetail(int id)
        {
            // 1) Převedeme `id` na váš enum
            if (!Enum.IsDefined(typeof(TypPojisteni), id))
                return NotFound();
            var typ = (TypPojisteni)id;

            // 2) Spočteme počet smluv
            var pocetSmluv = await _context.Pojisteni
                .CountAsync(p => p.TypPojisteni == typ);

            // 3) Sestavíme ViewModel
            var vm = new TypPojisteniCardViewModel
            {
                Typ = typ,
                Nazev = typ.GetDisplayName(),
                Popis = typ switch
                {
                    TypPojisteni.Zivotni => "Pojištění pro případ úmrtí nebo dožití.",
                    TypPojisteni.Urazove => "Kryje úraz, trvalé následky či hospitalizaci.",
                    TypPojisteni.Cestovni => "Zajištění pro cesty do zahraničí.",
                    TypPojisteni.Majetkove => "Pojištění domu, bytu a domácnosti.",
                    TypPojisteni.Odpovednost => "Krytí škod způsobených jiným osobám.",
                    _ => "Popis není k dispozici."
                },
                DlouhyPopis = typ switch
                {
                    TypPojisteni.Zivotni => "Životní pojištění je určeno pro osoby, které chtějí zajistit své blízké …",
                    TypPojisteni.Urazove => "Úrazové pojištění pokrývá škody způsobené úrazy včetně trvalých následků…",
                    TypPojisteni.Cestovni => "Cestovní pojištění zahrnuje krytí léčebných výloh, repatriaci…",
                    TypPojisteni.Majetkove => "Toto pojištění chrání vaši nemovitost, domácnost a cennosti…",
                    TypPojisteni.Odpovednost => "Pojištění odpovědnosti vás chrání, pokud neúmyslně způsobíte škodu…",
                    _ => string.Empty
                },
                ObrazekUrl = $"/images/typyPojisteni/{typ.ToString().ToLower()}.jpg"
            };

            // 4) Zjistíme přihlášeného pojištěnce
            int? pojistenecId = null;
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                pojistenecId = user?.PojistenecModelId;
            }

            ViewBag.Pocet = pocetSmluv;
            ViewBag.PojistenecId = pojistenecId;
            return View(vm);
        }
    }
}