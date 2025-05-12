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
    public class PojisteniOsobyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PojisteniOsobyController(ApplicationDbContext context)
        {
            _context = context;
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
        public IActionResult Create()
        {
            ViewData["OsobaId"] = new SelectList(_context.Pojistenci, "Id", "Email");
            ViewData["PojisteniId"] = new SelectList(_context.Pojisteni, "Id", "PredmetPojisteni");
            return View();
        }

        // POST: PojisteniOsoby/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OsobaId,PojisteniId,Role")] PojisteniOsobyModel pojisteniOsobyModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pojisteniOsobyModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OsobaId"] = new SelectList(_context.Pojistenci, "Id", "Email", pojisteniOsobyModel.OsobaId);
            ViewData["PojisteniId"] = new SelectList(_context.Pojisteni, "Id", "PredmetPojisteni", pojisteniOsobyModel.PojisteniId);
            return View(pojisteniOsobyModel);
        }

        // GET: PojisteniOsoby/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pojisteniOsobyModel = await _context.PojisteneOsoby.FindAsync(id);
            if (pojisteniOsobyModel == null)
            {
                return NotFound();
            }
            ViewData["OsobaId"] = new SelectList(_context.Pojistenci, "Id", "Email", pojisteniOsobyModel.OsobaId);
            ViewData["PojisteniId"] = new SelectList(_context.Pojisteni, "Id", "PredmetPojisteni", pojisteniOsobyModel.PojisteniId);
            return View(pojisteniOsobyModel);
        }

        // POST: PojisteniOsoby/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OsobaId,PojisteniId,Role")] PojisteniOsobyModel pojisteniOsobyModel)
        {
            if (id != pojisteniOsobyModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["OsobaId"] = new SelectList(_context.Pojistenci, "Id", "Email", pojisteniOsobyModel.OsobaId);
            ViewData["PojisteniId"] = new SelectList(_context.Pojisteni, "Id", "PredmetPojisteni", pojisteniOsobyModel.PojisteniId);
            return View(pojisteniOsobyModel);
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
