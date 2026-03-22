using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Eticaret.Data;
using Eticaret.Data.Entities;
using Microsoft.AspNetCore.Authorization;

namespace AvciFidancilik.WebUI.Areas.admin.Controllers.ChatbotControllers
{
    [Area("admin")]
    [Authorize(Roles = "Admin")] // Güvenlik için yetkilendirme eklendi
    public class ChatOptionsController : Controller
    {
        private readonly DatabaseContext _context;

        public ChatOptionsController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: admin/ChatOptions
        // Listeleme sayfasında senaryo ve adım sırasına göre dizmek yönetimi kolaylaştırır.
        public async Task<IActionResult> Index()
        {
            var options = await _context.ChatOptions
                .OrderBy(x => x.Scenario)
                .ThenBy(x => x.StepIndex)
                .ToListAsync();
            return View(options);
        }

        // GET: admin/ChatOptions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var chatOption = await _context.ChatOptions
                .FirstOrDefaultAsync(m => m.Id == id);

            if (chatOption == null) return NotFound();

            return View(chatOption);
        }

        // GET: admin/ChatOptions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: admin/ChatOptions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Scenario,StepIndex,ButtonText,ActionValue,NextStepIndex")] ChatOption chatOption)
        {
            if (ModelState.IsValid)
            {
                _context.Add(chatOption);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(chatOption);
        }

        // GET: admin/ChatOptions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var chatOption = await _context.ChatOptions.FindAsync(id);
            if (chatOption == null) return NotFound();

            return View(chatOption);
        }

        // POST: admin/ChatOptions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Scenario,StepIndex,ButtonText,ActionValue,NextStepIndex")] ChatOption chatOption)
        {
            if (id != chatOption.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chatOption);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChatOptionExists(chatOption.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(chatOption);
        }

        // GET: admin/ChatOptions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var chatOption = await _context.ChatOptions
                .FirstOrDefaultAsync(m => m.Id == id);

            if (chatOption == null) return NotFound();

            return View(chatOption);
        }

        // POST: admin/ChatOptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chatOption = await _context.ChatOptions.FindAsync(id);
            if (chatOption != null)
            {
                _context.ChatOptions.Remove(chatOption);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ChatOptionExists(int id)
        {
            return _context.ChatOptions.Any(e => e.Id == id);
        }
    }
}