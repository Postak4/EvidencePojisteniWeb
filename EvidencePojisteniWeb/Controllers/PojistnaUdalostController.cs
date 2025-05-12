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
    public class PojistnaUdalostController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PojistnaUdalostController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PojistnaUdalost
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.PojistneUdalosti.Include(p => p.Pojisteni);
            return View(await applicationDbContext.ToListAsync());
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
        public IActionResult Create()
        {
            ViewData["PojisteniId"] = new SelectList(_context.Pojisteni, "Id", "PredmetPojisteni");
            return View();
        }

        // POST: PojistnaUdalost/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PopisUdalosti,DatumUdalosti,MistoUdalosti,Skoda,Vyreseno,Svedek,AdresaSvedka,Poznamka,PojisteniId")] PojistnaUdalostModel pojistnaUdalostModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pojistnaUdalostModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PojisteniId"] = new SelectList(_context.Pojisteni, "Id", "PredmetPojisteni", pojistnaUdalostModel.PojisteniId);
            return View(pojistnaUdalostModel);
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
