using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Eticaret.Data;
using Eticaret.Data.Entities;

namespace AvciFidancilik.WebUI.Areas.admin.Controllers.ChatbotControllers
{
    [Area("admin")]
    public class ArazisController : Controller
    {
        private readonly DatabaseContext _context;

        public ArazisController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: admin/Arazis
        public async Task<IActionResult> Index()
        {
            return View(await _context.Araziler.ToListAsync());
        }

        // GET: admin/Arazis/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var arazi = await _context.Araziler
                .FirstOrDefaultAsync(m => m.Id == id);
            if (arazi == null)
            {
                return NotFound();
            }

            return View(arazi);
        }

        // GET: admin/Arazis/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: admin/Arazis/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Donum,Sehir,ToprakTipi")] Arazi arazi)
        {
            if (ModelState.IsValid)
            {
                _context.Add(arazi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(arazi);
        }

        // GET: admin/Arazis/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var arazi = await _context.Araziler.FindAsync(id);
            if (arazi == null)
            {
                return NotFound();
            }
            return View(arazi);
        }

        // POST: admin/Arazis/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Donum,Sehir,ToprakTipi")] Arazi arazi)
        {
            if (id != arazi.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(arazi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AraziExists(arazi.Id))
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
            return View(arazi);
        }

        // GET: admin/Arazis/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var arazi = await _context.Araziler
                .FirstOrDefaultAsync(m => m.Id == id);
            if (arazi == null)
            {
                return NotFound();
            }

            return View(arazi);
        }

        // POST: admin/Arazis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var arazi = await _context.Araziler.FindAsync(id);
            if (arazi != null)
            {
                _context.Araziler.Remove(arazi);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AraziExists(int id)
        {
            return _context.Araziler.Any(e => e.Id == id);
        }
    }
}
