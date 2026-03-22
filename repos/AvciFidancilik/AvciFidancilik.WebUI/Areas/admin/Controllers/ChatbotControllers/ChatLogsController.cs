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
    public class ChatLogsController : Controller
    {
        private readonly DatabaseContext _context;

        public ChatLogsController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: admin/ChatLogs
        public async Task<IActionResult> Index()
        {
            return View(await _context.ChatLogs.ToListAsync());
        }

        // GET: admin/ChatLogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chatLog = await _context.ChatLogs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chatLog == null)
            {
                return NotFound();
            }

            return View(chatLog);
        }

        // GET: admin/ChatLogs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: admin/ChatLogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,KullaniciMesaj,BotCevap,Tarih")] ChatLog chatLog)
        {
            if (ModelState.IsValid)
            {
                _context.Add(chatLog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(chatLog);
        }

        // GET: admin/ChatLogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chatLog = await _context.ChatLogs.FindAsync(id);
            if (chatLog == null)
            {
                return NotFound();
            }
            return View(chatLog);
        }

        // POST: admin/ChatLogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,KullaniciMesaj,BotCevap,Tarih")] ChatLog chatLog)
        {
            if (id != chatLog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chatLog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChatLogExists(chatLog.Id))
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
            return View(chatLog);
        }

        // GET: admin/ChatLogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chatLog = await _context.ChatLogs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chatLog == null)
            {
                return NotFound();
            }

            return View(chatLog);
        }

        // POST: admin/ChatLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chatLog = await _context.ChatLogs.FindAsync(id);
            if (chatLog != null)
            {
                _context.ChatLogs.Remove(chatLog);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChatLogExists(int id)
        {
            return _context.ChatLogs.Any(e => e.Id == id);
        }
    }
}
