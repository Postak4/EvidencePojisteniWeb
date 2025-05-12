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
    public class PojisteniController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PojisteniController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Pojisteni
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Pojisteni.Include(p => p.Pojistenec);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Pojisteni/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pojisteniModel = await _context.Pojisteni
                .Include(p => p.Pojistenec)
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
            ViewData["PojistenecId"] = new SelectList(_context.Pojistenci, "Id", "Email");
            return View();
        }

        // POST: Pojisteni/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PojistenecId,TypPojisteni,PredmetPojisteni,DatumZacatku,DatumKonce,Castka")] PojisteniModel pojisteniModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pojisteniModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PojistenecId"] = new SelectList(_context.Pojistenci, "Id", "Email", pojisteniModel.PojistenecId);
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
            ViewData["PojistenecId"] = new SelectList(_context.Pojistenci, "Id", "Email", pojisteniModel.PojistenecId);
            return View(pojisteniModel);
        }

        // POST: Pojisteni/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PojistenecId,TypPojisteni,PredmetPojisteni,DatumZacatku,DatumKonce,Castka")] PojisteniModel pojisteniModel)
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
            ViewData["PojistenecId"] = new SelectList(_context.Pojistenci, "Id", "Email", pojisteniModel.PojistenecId);
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
                .Include(p => p.Pojistenec)
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
    }
}
