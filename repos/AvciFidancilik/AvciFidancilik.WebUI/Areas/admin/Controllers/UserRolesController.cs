using Eticaret.Core.Entities;
using Eticaret.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvciFidancilik.WebUI.Areas.admin.Controllers
{
    [Area("admin")]
    public class UserRolesController : BaseAdminController
    {
        private readonly DatabaseContext _context;

        public UserRolesController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: admin/UserRoles
        public async Task<IActionResult> Index()
        {
            var databaseContext = _context.UserRoles.Include(u => u.Role).Include(u => u.User);
            return View(await databaseContext.ToListAsync());
        }

        // GET: admin/UserRoles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userRole = await _context.UserRoles
                .Include(u => u.Role)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userRole == null)
            {
                return NotFound();
            }

            return View(userRole);
        }

        // GET: admin/UserRoles/Create
        public IActionResult Create()
        {
            // Basit ve direkt yöntem
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name");
            ViewData["UserId"] = new SelectList(_context.AppUsers, "Id", "Email");
            return View();
        }

        // POST: admin/UserRoles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,RoleId")] UserRole userRole)
        {
            // DEBUG: Gelen değerleri logla
            Console.WriteLine($"POST Create çağrıldı - UserId: {userRole.UserId}, RoleId: {userRole.RoleId}");

            // ÖNEMLİ: User ve Role navigation property'leri null olduğu için ModelState hata verebilir
            // Bu nedenle önce bu property'leri geçici olarak ayarlayalım
            var tempUserRole = new UserRole
            {
                UserId = userRole.UserId,
                RoleId = userRole.RoleId
            };

            // Navigation property'leri yükle (sadece validasyon için)
            try
            {
                tempUserRole.User = await _context.AppUsers.FindAsync(userRole.UserId);
                tempUserRole.Role = await _context.Roles.FindAsync(userRole.RoleId);
            }
            catch
            {
                // Navigation property yükleme hatası
            }

            // ModelState'i kontrol et
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(tempUserRole, null, null);
            bool isValid = Validator.TryValidateObject(tempUserRole, validationContext, validationResults, true);

            if (!isValid)
            {
                // Validation hatalarını ModelState'e ekle
                foreach (var validationResult in validationResults)
                {
                    foreach (var memberName in validationResult.MemberNames)
                    {
                        ModelState.AddModelError(memberName, validationResult.ErrorMessage);
                    }
                }

                // Hata mesajlarını logla
                foreach (var error in validationResults)
                {
                    Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                }
            }

            // Ek validasyon: Aynı kayıt var mı?
            if (isValid)
            {
                var existingRecord = await _context.UserRoles
                    .FirstOrDefaultAsync(ur => ur.UserId == userRole.UserId && ur.RoleId == userRole.RoleId);

                if (existingRecord != null)
                {
                    ModelState.AddModelError("", "Bu kullanıcı zaten bu role sahip!");
                    isValid = false;
                }
            }

            // Ek validasyon: UserId ve RoleId > 0 olmalı
            if (userRole.UserId <= 0)
            {
                ModelState.AddModelError("UserId", "Geçerli bir kullanıcı seçin.");
                isValid = false;
            }

            if (userRole.RoleId <= 0)
            {
                ModelState.AddModelError("RoleId", "Geçerli bir rol seçin.");
                isValid = false;
            }

            if (isValid)
            {
                try
                {
                    Console.WriteLine($"Kayıt ekleniyor - UserId: {userRole.UserId}, RoleId: {userRole.RoleId}");

                    // Navigation property'leri null yap (EF Core bunları kendisi yükleyecek)
                    var newUserRole = new UserRole
                    {
                        UserId = userRole.UserId,
                        RoleId = userRole.RoleId
                    };

                    _context.Add(newUserRole);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Kullanıcı rolü başarıyla oluşturuldu.";
                    Console.WriteLine("Kayıt başarıyla eklendi.");
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbEx)
                {
                    Console.WriteLine($"DbUpdateException: {dbEx.Message}");

                    // Foreign key hatası
                    if (dbEx.InnerException != null &&
                        (dbEx.InnerException.Message.Contains("FOREIGN KEY") ||
                         dbEx.InnerException.Message.Contains("constraint")))
                    {
                        ModelState.AddModelError("", "Seçilen kullanıcı veya rol geçerli değil. Lütfen tekrar seçin.");
                    }
                    else
                    {
                        ModelState.AddModelError("", $"Veritabanı hatası: {dbEx.Message}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                    ModelState.AddModelError("", $"Bir hata oluştu: {ex.Message}");
                }
            }

            // Hata durumunda dropdown'ları tekrar doldur
            // Seçili değerleri korumak için dördüncü parametreyi kullan
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", userRole.RoleId);
            ViewData["UserId"] = new SelectList(_context.AppUsers, "Id", "Email", userRole.UserId);

            // Hata mesajlarını topla ve TempData'ya ekle
            if (!isValid)
            {
                var errorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .Distinct()
                    .ToList();

                if (errorMessages.Any())
                {
                    TempData["ErrorMessage"] = string.Join("<br>", errorMessages);
                }

                // Debug için hataları logla
                Console.WriteLine("Form gönderimi başarısız. Hatalar:");
                foreach (var error in errorMessages)
                {
                    Console.WriteLine($"- {error}");
                }
            }

            return View(userRole);
        }

        // GET: admin/UserRoles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userRole = await _context.UserRoles.FindAsync(id);
            if (userRole == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", userRole.RoleId);
            ViewData["UserId"] = new SelectList(_context.AppUsers, "Id", "Email", userRole.UserId);
            return View(userRole);
        }

        // POST: admin/UserRoles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,RoleId")] UserRole userRole)
        {
            if (id != userRole.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userRole);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Kullanıcı rolü başarıyla güncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserRoleExists(userRole.Id))
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
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", userRole.RoleId);
            ViewData["UserId"] = new SelectList(_context.AppUsers, "Id", "Email", userRole.UserId);
            return View(userRole);
        }

        // GET: admin/UserRoles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userRole = await _context.UserRoles
                .Include(u => u.Role)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userRole == null)
            {
                return NotFound();
            }

            return View(userRole);
        }

        // POST: admin/UserRoles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userRole = await _context.UserRoles.FindAsync(id);
            if (userRole != null)
            {
                _context.UserRoles.Remove(userRole);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Kullanıcı rolü başarıyla silindi.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool UserRoleExists(int id)
        {
            return _context.UserRoles.Any(e => e.Id == id);
        }
    }
}