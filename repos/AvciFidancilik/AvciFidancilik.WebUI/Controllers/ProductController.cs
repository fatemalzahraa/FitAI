using Eticaret.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Eticaret.WebUI.Controllers
{
    public class ProductController : Controller
    {
        private readonly DatabaseContext _context;

        public ProductController(DatabaseContext context)
        {
            _context = context;
        }

        // 🔹 Ürün Listesi
        public IActionResult Index(string searchTerm)
        {
            var products = _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Where(p => p.Stock > 0); // Başlangıçta stok > 0 filtreli

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                products = products.Where(p => p.Name.Contains(searchTerm));
            }

            return View(products.ToList());
        }


        // 🔹 Ürün Detay
        public IActionResult Detail(int id)
        {
            // Ana ürün
            var product = _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
                return NotFound();

            // 🔹 Aynı kategoriden rastgele 5 ürün
            var relatedProducts = _context.Products
                .Include(p => p.ProductImages)
                .Where(p =>
                    p.CategoryId == product.CategoryId &&
                    p.Id != product.Id &&
                    p.Stock > 0
                )
                .OrderBy(x => Guid.NewGuid()) // 🎲 RANDOM
                .Take(5)
                .ToList();

            // View’a gönder
            ViewBag.RelatedProducts = relatedProducts;

            return View(product);
        }
    }
}
