using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
            // Už nebudeme includovat Pojistenec, protože model to nemá
            var pojisteniList = await _context.Pojisteni.ToListAsync();
            return View(pojisteniList);
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
    }
}
