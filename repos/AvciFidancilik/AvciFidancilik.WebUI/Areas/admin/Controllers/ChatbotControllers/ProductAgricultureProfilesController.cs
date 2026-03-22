using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Eticaret.Core.Entities;
using Eticaret.Data;

namespace AvciFidancilik.WebUI.Areas.admin.Controllers.ChatbotControllers
{
    [Area("admin")]
    public class ProductAgricultureProfilesController : Controller
    {
        private readonly DatabaseContext _context;

        public ProductAgricultureProfilesController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: admin/ProductAgricultureProfiles
        public async Task<IActionResult> Index()
        {
            var profiles = await _context.ProductAgricultureProfiles
                .Include(p => p.Category)
                .Include(p => p.Product)
                .OrderByDescending(p => p.Id)
                .ToListAsync();
            return View(profiles);
        }

        // GET: admin/ProductAgricultureProfiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var profile = await _context.ProductAgricultureProfiles
                .Include(p => p.Category)
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (profile == null) return NotFound();

            return View(profile);
        }

        // GET: admin/ProductAgricultureProfiles/Create
        public IActionResult Create()
        {
            PrepareViewBags();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductAgricultureProfile profile)
        {
            // 1. KRİTİK KONTROL: Aynı ürün için mükerrer kayıt engelleme
            bool exists = await _context.ProductAgricultureProfiles
                .AnyAsync(p => p.ProductId == profile.ProductId);

            if (exists)
            {
                ModelState.AddModelError("ProductId", "Bu ürün için zaten bir tarım profili mevcut.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(profile);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Veritabanı hatası: " + ex.Message);
                }
            }

            PrepareViewBags(profile.CategoryId, profile.ProductId);
            return View(profile);
        }

        // GET: admin/ProductAgricultureProfiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var profile = await _context.ProductAgricultureProfiles.FindAsync(id);
            if (profile == null) return NotFound();

            PrepareViewBags(profile.CategoryId, profile.ProductId);
            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductAgricultureProfile profile)
        {
            if (id != profile.Id) return NotFound();

            // 2. KRİTİK KONTROL: Başka bir profilin ürününü çalmasını engelleme
            bool otherExists = await _context.ProductAgricultureProfiles
                .AnyAsync(p => p.ProductId == profile.ProductId && p.Id != id);

            if (otherExists)
            {
                ModelState.AddModelError("ProductId", "Seçtiğiniz ürün için başka bir profil zaten tanımlanmış.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(profile);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductAgricultureProfileExists(profile.Id)) return NotFound();
                    else throw;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Güncelleme sırasında hata oluştu: " + ex.Message);
                }
            }

            PrepareViewBags(profile.CategoryId, profile.ProductId);
            return View(profile);
        }

        // GET: admin/ProductAgricultureProfiles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var profile = await _context.ProductAgricultureProfiles
                .Include(p => p.Category)
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (profile == null) return NotFound();

            return View(profile);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var profile = await _context.ProductAgricultureProfiles.FindAsync(id);
            if (profile != null)
            {
                _context.ProductAgricultureProfiles.Remove(profile);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // 3. DÜZELTİLMİŞ YARDIMCI METOT
        private void PrepareViewBags(int? categoryId = null, int? productId = null)
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories.OrderBy(x => x.Name), "Id", "Name", categoryId);

            // Null kontrolü eklenmiş ürün listesi (Substring hatası giderildi)
            var productList = _context.Products
                .AsNoTracking() // Performans artışı
                .OrderBy(x => x.Name)
                .ToList() // Memory'ye alarak string işlemlerini güvenli yapıyoruz
                .Select(s => new
                {
                    Id = s.Id,
                    DisplayName = s.Name + (string.IsNullOrEmpty(s.Description)
                        ? ""
                        : " - " + (s.Description.Length > 30 ? s.Description.Substring(0, 30) + "..." : s.Description))
                })
                .ToList();

            ViewData["ProductId"] = new SelectList(productList, "Id", "DisplayName", productId);
        }

        private bool ProductAgricultureProfileExists(int id)
        {
            return _context.ProductAgricultureProfiles.Any(e => e.Id == id);
        }
    }
}