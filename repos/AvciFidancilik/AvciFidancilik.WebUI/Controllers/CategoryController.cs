using Eticaret.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Eticaret.WebUI.Controllers
{
    public class CategoryController : Controller
    {
        private readonly DatabaseContext _context;

        public CategoryController(DatabaseContext context)
        {
            _context = context;
        }

        // ÇALIŞAN URL:
        // /Category/Index?categoryId=2
        public IActionResult Index(int categoryId)
        {
            var category = _context.Categories
                .FirstOrDefault(c => c.Id == categoryId);

            if (category == null)
                return NotFound();

            var products = _context.Products
                .Include(p => p.ProductImages)
                .Where(p => p.CategoryId == categoryId && p.Stock > 0)
                .ToList();

            ViewBag.CategoryName = category.Name;

            return View(products);
        }
    }
}
