using AvciFidancilik.WebUI.Areas.admin.Controllers;
using Eticaret.Core.Entities;
using Eticaret.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eticaret.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriesController : BaseAdminController
    {
        private readonly DatabaseContext _context;

        public CategoriesController(DatabaseContext context)
        {
            _context = context;
        }

        // -----------------------
        // INDEX
        // -----------------------
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .Include(c => c.Products) // Alt tabloyu getir
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(categories); // @model IEnumerable<Category>
        }

        // -----------------------
        // DETAILS
        // -----------------------
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products) // Ürünleri de al
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                TempData["ErrorMessage"] = "Kategori bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            return View(category); // @model Category
        }

        // -----------------------
        // CREATE
        // -----------------------
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.Name = model.Name?.Trim();

            if (await _context.Categories.AnyAsync(c => c.Name.ToLower() == model.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "Bu kategori adı zaten mevcut.");
                return View(model);
            }

            _context.Categories.Add(model);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"'{model.Name}' kategorisi başarıyla oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }

        // -----------------------
        // EDIT
        // -----------------------
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                TempData["ErrorMessage"] = "Kategori bulunamadı.";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category model)
        {
            if (id != model.Id) return NotFound();
            if (!ModelState.IsValid) return View(model);

            var existingCategory = await _context.Categories.FindAsync(id);
            if (existingCategory == null)
            {
                TempData["ErrorMessage"] = "Kategori bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            existingCategory.Name = model.Name?.Trim();
            _context.Categories.Update(existingCategory);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"'{existingCategory.Name}' kategorisi başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        // -----------------------
        // DELETE
        // -----------------------
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products) // Alt tabloyu getir
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                TempData["ErrorMessage"] = "Kategori bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ProductCount = category.Products?.Count ?? 0;

            // Diğer kategorileri listele (taşıma için)
            ViewBag.OtherCategories = await _context.Categories
                .Where(c => c.Id != id)
                .ToListAsync();

            return View(category); // @model Category
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string deleteOption, int? targetCategoryId)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                TempData["ErrorMessage"] = "Kategori bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            var productCount = category.Products?.Count ?? 0;

            if (productCount > 0)
            {
                if (deleteOption == "all")
                {
                    // Tüm ürünleri sil
                    _context.Products.RemoveRange(category.Products);
                }
                else if (deleteOption == "move")
                {
                    if (targetCategoryId.HasValue)
                    {
                        var targetCategory = await _context.Categories.FindAsync(targetCategoryId.Value);
                        if (targetCategory != null)
                        {
                            foreach (var product in category.Products)
                            {
                                product.CategoryId = targetCategory.Id;
                            }
                            _context.Products.UpdateRange(category.Products);
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Ürünleri taşımak için hedef kategori seçmelisiniz.";
                        return RedirectToAction(nameof(Delete), new { id });
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Silme işlemi seçilmedi veya geçersiz.";
                    return RedirectToAction(nameof(Delete), new { id });
                }
            }

            // Kategori sil
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = productCount > 0
                ? $"'{category.Name}' kategorisi ve {productCount} ürün başarıyla işlendi."
                : $"'{category.Name}' kategorisi başarıyla silindi.";

            return RedirectToAction(nameof(Index));
        }
    }
}
