using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EvidencePojisteniWeb.Data;
using EvidencePojisteniWeb.Models;

namespace EvidencePojisteniWeb.Controllers
{
    public class PojistenecController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PojistenecController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Pojistenec
        public async Task<IActionResult> Index(int? pageNumber)
        {
            int pageSize = 3; // Počet záznamů na stránku
            var pojistenci = _context.Pojistenci
                // všechny vyzby PojistenciOsoby
                .Include(p => p.PojisteniOsoby)
                // všechny vyzby Pojisteni
                .ThenInclude(po => po.Pojisteni)
                .AsNoTracking();

            return View(await PaginatedList<PojistenecModel>.CreateAsync(pojistenci, pageNumber ?? 1, pageSize));
        }

        // GET: Pojistenec/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pojistenecModel = await _context.Pojistenci
                .Include(p => p.PojisteniOsoby)
                .ThenInclude(po => po.Pojisteni)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (pojistenecModel == null)
            {
                return NotFound();
            }

            return View(pojistenecModel);
        }

        // GET: Pojistenec/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Pojistenec/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Jmeno,Prijmeni,DatumNarozeni,UliceCpCe,Mesto,PSC,Stat,Telefon,Email")] PojistenecModel pojistenecModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pojistenecModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pojistenecModel);
        }

        // GET: Pojistenec/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pojistenecModel = await _context.Pojistenci.FindAsync(id);
            if (pojistenecModel == null)
            {
                return NotFound();
            }
            return View(pojistenecModel);
        }

        // POST: Pojistenec/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Jmeno,Prijmeni,DatumNarozeni,UliceCpCe,Mesto,PSC,Stat,Telefon,Email")] PojistenecModel pojistenecModel)
        {
            if (id != pojistenecModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pojistenecModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PojistenecModelExists(pojistenecModel.Id))
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
            return View(pojistenecModel);
        }

        // GET: Pojistenec/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pojistenecModel = await _context.Pojistenci
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pojistenecModel == null)
            {
                return NotFound();
            }

            return View(pojistenecModel);
        }

        // POST: Pojistenec/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pojistenecModel = await _context.Pojistenci.FindAsync(id);
            if (pojistenecModel != null)
            {
                _context.Pojistenci.Remove(pojistenecModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PojistenecModelExists(int id)
        {
            return _context.Pojistenci.Any(e => e.Id == id);
        }
    }
}
