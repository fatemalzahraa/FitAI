using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Eticaret.Core.Entities;
using Eticaret.Data;
using Microsoft.AspNetCore.Authorization;

namespace AvciFidancilik.WebUI.Areas.admin.Controllers.ChatbotControllers
{
    [Area("admin")]
    [Authorize(Roles = "Admin")] // Sadece admin yetkisi olanlar erişebilmeli
    public class CitiesController : Controller
    {
        private readonly DatabaseContext _context;

        public CitiesController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: admin/Cities
        // Şehirleri alfabetik sırayla getirmek admin paneli kullanımını kolaylaştırır.
        public async Task<IActionResult> Index()
        {
            var cities = await _context.Cities
                .OrderBy(c => c.Name)
                .ToListAsync();
            return View(cities);
        }

        // GET: admin/Cities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var city = await _context.Cities
                .FirstOrDefaultAsync(m => m.Id == id);

            if (city == null) return NotFound();

            return View(city);
        }

        // GET: admin/Cities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: admin/Cities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] City city)
        {
            if (ModelState.IsValid)
            {
                _context.Add(city);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(city);
        }

        // GET: admin/Cities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var city = await _context.Cities.FindAsync(id);
            if (city == null) return NotFound();

            return View(city);
        }

        // POST: admin/Cities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] City city)
        {
            if (id != city.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(city);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CityExists(city.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(city);
        }

        // GET: admin/Cities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var city = await _context.Cities
                .FirstOrDefaultAsync(m => m.Id == id);

            if (city == null) return NotFound();

            return View(city);
        }

        // POST: admin/Cities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city != null)
            {
                _context.Cities.Remove(city);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CityExists(int id)
        {
            return _context.Cities.Any(e => e.Id == id);
        }
    }
}