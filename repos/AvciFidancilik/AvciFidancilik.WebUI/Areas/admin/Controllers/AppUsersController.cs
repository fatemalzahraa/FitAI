using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Eticaret.Core.Entities;
using Eticaret.Data;

namespace AvciFidancilik.WebUI.Areas.admin.Controllers
{
    [Area("admin")]
    public class AppUsersController : BaseAdminController
    {
        private readonly DatabaseContext _context;

        public AppUsersController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: admin/AppUsers
        public async Task<IActionResult> Index()
        {
            // İlişkili verileri Include et (opsiyonel ama faydalı)
            var appUsers = await _context.AppUsers
                .Include(u => u.Addresses)
                .Include(u => u.Orders)
                .Include(u => u.Cart)
                .ToListAsync();
            return View(appUsers);
        }

        // GET: admin/AppUsers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appUser = await _context.AppUsers
                .Include(u => u.Addresses)
                .Include(u => u.Orders)
                .Include(u => u.Cart)
                .Include(u => u.Favorites)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .Include(u => u.Contacts)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (appUser == null)
            {
                return NotFound();
            }

            return View(appUser);
        }

        // GET: admin/AppUsers/Create
        public IActionResult Create()
        {
            // Rol seçimi için dropdown (opsiyonel)
            ViewData["Roles"] = new SelectList(_context.Roles, "Id", "Name");
            return View();
        }

        // POST: admin/AppUsers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppUser appUser)
        {
            // GÜVENLİK: Kritik alanları manuel set et
            appUser.CreatedDate = DateTime.UtcNow;
            appUser.UserGuid = Guid.NewGuid();
            appUser.PasswordHash = "temp_password"; // Gerçek projede hash'le
            appUser.IsAdmin = false; // Varsayılan değer

            if (ModelState.IsValid)
            {
                _context.Add(appUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Roles"] = new SelectList(_context.Roles, "Id", "Name");
            return View(appUser);
        }

        // GET: admin/AppUsers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appUser = await _context.AppUsers.FindAsync(id);
            if (appUser == null)
            {
                return NotFound();
            }

            ViewData["Roles"] = new SelectList(_context.Roles, "Id", "Name");
            return View(appUser);
        }

        // POST: admin/AppUsers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AppUser appUser)
        {
            if (id != appUser.Id)
            {
                return NotFound();
            }

            // GÜVENLİK: Veritabanından mevcut kullanıcıyı çek
            var existingUser = await _context.AppUsers.FindAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            // Sadece değiştirilmesine izin verilen alanları güncelle
            existingUser.Name = appUser.Name;
            existingUser.SurName = appUser.SurName;
            existingUser.Email = appUser.Email;
            existingUser.Phone = appUser.Phone;
            existingUser.UserName = appUser.UserName;
            existingUser.IsActive = appUser.IsActive;
            // IsAdmin, CreatedDate, PasswordHash, UserGuid DEĞİŞTİRİLEMEZ

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(existingUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppUserExists(appUser.Id))
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

            ViewData["Roles"] = new SelectList(_context.Roles, "Id", "Name");
            return View(appUser);
        }

        // GET: admin/AppUsers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appUser = await _context.AppUsers
                .Include(u => u.Addresses)
                .Include(u => u.Orders)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (appUser == null)
            {
                return NotFound();
            }

            return View(appUser);
        }

        // POST: admin/AppUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appUser = await _context.AppUsers.FindAsync(id);
            if (appUser != null)
            {
                _context.AppUsers.Remove(appUser);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppUserExists(int id)
        {
            return _context.AppUsers.Any(e => e.Id == id);
        }
    }
}