using AvciFidancilik.WebUI.Areas.admin.Controllers;
using Eticaret.Core.Entities;
using Eticaret.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Eticaret.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : BaseAdminController
    {
        private readonly DatabaseContext _context;

        public ProductsController(DatabaseContext context)
        {
            _context = context;
        }

        // -----------------------
        // INDEX
        // -----------------------
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .OrderBy(p => p.Name)
                .ToListAsync();

            return View(products);
        }

        // -----------------------
        // DETAILS
        // -----------------------
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        // -----------------------
        // CREATE (GET)
        // -----------------------
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(
                _context.Categories
                    .OrderBy(c => c.Name)
                    .ToList(),
                "Id",
                "Name"
            );

            return View();
        }

        // -----------------------
        // CREATE (POST)
        // -----------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CategoryId = new SelectList(
                    _context.Categories.OrderBy(c => c.Name),
                    "Id",
                    "Name",
                    product.CategoryId
                );

                return View(product);
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Ürün başarıyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        // -----------------------
        // EDIT (GET)
        // -----------------------
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            ViewBag.CategoryId = new SelectList(
                _context.Categories.OrderBy(c => c.Name),
                "Id",
                "Name",
                product.CategoryId
            );

            return View(product);
        }

        // -----------------------
        // EDIT (POST)
        // -----------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.CategoryId = new SelectList(
                    _context.Categories.OrderBy(c => c.Name),
                    "Id",
                    "Name",
                    product.CategoryId
                );

                return View(product);
            }

            try
            {
                _context.Update(product);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Ürün başarıyla güncellendi.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.Id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // -----------------------
        // DELETE (GET)
        // -----------------------
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        // -----------------------
        // DELETE (POST)
        // -----------------------
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Ürün başarıyla silindi.";
            }

            return RedirectToAction(nameof(Index));
        }

        // -----------------------
        // HELPER
        // -----------------------
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
