using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Eticaret.Core.Entities;
using Eticaret.Data;
using Microsoft.Data.SqlClient;

namespace AvciFidancilik.WebUI.Areas.admin.Controllers
{
    [Area("admin")]
    public class ContactsController : BaseAdminController
    {
        private readonly DatabaseContext _context;

        public ContactsController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: admin/Contacts
        public async Task<IActionResult> Index()
        {
            var contacts = await _context.Contacts
                .Include(c => c.User)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();
            return View(contacts);
        }

        // GET: admin/Contacts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // GET: admin/Contacts/Create
        public IActionResult Create()
        {
            // Geçerli kullanıcıları getir (UserId 0 olmayan)
            var users = _context.AppUsers
                .Where(u => u.Id > 0)
                .Select(u => new { u.Id, u.Email })
                .ToList();

            // Boş seçenek ve kullanıcı listesi
            var userList = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "-- Kullanıcı Yok (Misafir) --" }
            };

            userList.AddRange(users.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.Email
            }));

            ViewBag.Users = userList;
            return View();
        }

        // POST: admin/Contacts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contact contact)
        {
            // UserId 0 ise, geçerli bir kullanıcı kontrolü yap
            if (contact.UserId == 0)
            {
                // UserId'yi 0 olarak bırak (misafir kullanıcı)
                // Bu, veritabanında olmayan bir kullanıcı demek
                // FOREIGN KEY hatası almamak için UserId'yi geçersiz değer yapma
            }
            else if (contact.UserId > 0)
            {
                // Kullanıcının var olup olmadığını kontrol et
                var userExists = await _context.AppUsers
                    .AnyAsync(u => u.Id == contact.UserId);

                if (!userExists)
                {
                    ModelState.AddModelError("UserId", "Geçersiz kullanıcı ID'si");
                    // ViewBag'i tekrar oluştur
                    await PrepareUserListForView();
                    return View(contact);
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // CreatedDate'i şimdi olarak ayarla
                    contact.CreatedDate = DateTime.UtcNow;

                    _context.Add(contact);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "İletişim mesajı başarıyla oluşturuldu!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    // FOREIGN KEY hatasını yakala
                    if (ex.InnerException is SqlException sqlEx && sqlEx.Number == 547)
                    {
                        ModelState.AddModelError("", "Geçersiz kullanıcı ID'si. Lütfen geçerli bir kullanıcı seçin.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Mesaj oluşturulurken bir hata oluştu: " + ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Mesaj oluşturulurken bir hata oluştu: " + ex.Message);
                }
            }

            // Hata varsa formu tekrar göster
            await PrepareUserListForView();
            return View(contact);
        }

        // GET: admin/Contacts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }

            await PrepareUserListForView();
            return View(contact);
        }

        // POST: admin/Contacts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Contact contact)
        {
            if (id != contact.Id)
            {
                return NotFound();
            }

            // UserId kontrolü
            if (contact.UserId > 0)
            {
                var userExists = await _context.AppUsers
                    .AnyAsync(u => u.Id == contact.UserId);

                if (!userExists)
                {
                    ModelState.AddModelError("UserId", "Geçersiz kullanıcı ID'si");
                    await PrepareUserListForView();
                    return View(contact);
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingContact = await _context.Contacts.FindAsync(id);
                    if (existingContact == null)
                    {
                        return NotFound();
                    }

                    // Sadece gerekli alanları güncelle
                    existingContact.Name = contact.Name;
                    existingContact.Email = contact.Email;
                    existingContact.Subject = contact.Subject;
                    existingContact.Message = contact.Message;
                    existingContact.UserId = contact.UserId;

                    _context.Update(existingContact);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "İletişim mesajı başarıyla güncellendi!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactExists(contact.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException is SqlException sqlEx && sqlEx.Number == 547)
                    {
                        ModelState.AddModelError("", "Geçersiz kullanıcı ID'si. Lütfen geçerli bir kullanıcı seçin.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Mesaj güncellenirken bir hata oluştu: " + ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Mesaj güncellenirken bir hata oluştu: " + ex.Message);
                }
            }

            await PrepareUserListForView();
            return View(contact);
        }

        // GET: admin/Contacts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // POST: admin/Contacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact != null)
            {
                try
                {
                    _context.Contacts.Remove(contact);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "İletişim mesajı başarıyla silindi!";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Mesaj silinirken bir hata oluştu: " + ex.Message;
                }
            }
            else
            {
                TempData["Error"] = "Silinecek mesaj bulunamadı!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ContactExists(int id)
        {
            return _context.Contacts.Any(e => e.Id == id);
        }

        private async Task PrepareUserListForView()
        {
            var users = await _context.AppUsers
                .Where(u => u.Id > 0)
                .Select(u => new { u.Id, u.Email })
                .ToListAsync();

            var userList = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "-- Kullanıcı Yok (Misafir) --" }
            };

            userList.AddRange(users.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.Email
            }));

            ViewBag.Users = userList;
        }
    }
}