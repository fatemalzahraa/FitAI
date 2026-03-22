using System;
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
    public class CartItemsController : BaseAdminController
    {
        private readonly DatabaseContext _context;

        public CartItemsController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: admin/CartItems
        public async Task<IActionResult> Index()
        {
            var databaseContext = _context.CartItems
                .Include(c => c.Cart)
                .Include(c => c.Product);
            return View(await databaseContext.ToListAsync());
        }

        // GET: admin/CartItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItems
                .Include(c => c.Cart)
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cartItem == null)
            {
                return NotFound();
            }

            return View(cartItem);
        }

        // GET: admin/CartItems/Create
        public IActionResult Create()
        {
            ViewData["CartId"] = new SelectList(_context.Carts, "Id", "Id");
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Description");
            return View();
        }

        // POST: admin/CartItems/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,CartId,Quantity")] CartItem cartItem)
        {
            // Entity'deki [Required] validasyonları otomatik çalışacak
            if (ModelState.IsValid)
            {
                // Entity'deki [Range] validasyonu için kontrol
                if (cartItem.Quantity < 1)
                {
                    ModelState.AddModelError("Quantity", "Adet en az 1 olmalıdır.");
                    ViewData["CartId"] = new SelectList(_context.Carts, "Id", "Id", cartItem.CartId);
                    ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Description", cartItem.ProductId);
                    return View(cartItem);
                }

                _context.Add(cartItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CartId"] = new SelectList(_context.Carts, "Id", "Id", cartItem.CartId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Description", cartItem.ProductId);
            return View(cartItem);
        }

        // GET: admin/CartItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }

            ViewData["CartId"] = new SelectList(_context.Carts, "Id", "Id", cartItem.CartId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Description", cartItem.ProductId);
            return View(cartItem);
        }

        // POST: admin/CartItems/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,CartId,Quantity")] CartItem cartItem)
        {
            if (id != cartItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Entity'deki [Range] validasyonu için kontrol
                if (cartItem.Quantity < 1)
                {
                    ModelState.AddModelError("Quantity", "Adet en az 1 olmalıdır.");
                    ViewData["CartId"] = new SelectList(_context.Carts, "Id", "Id", cartItem.CartId);
                    ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Description", cartItem.ProductId);
                    return View(cartItem);
                }

                try
                {
                    _context.Update(cartItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartItemExists(cartItem.Id))
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

            ViewData["CartId"] = new SelectList(_context.Carts, "Id", "Id", cartItem.CartId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Description", cartItem.ProductId);
            return View(cartItem);
        }

        // GET: admin/CartItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItems
                .Include(c => c.Cart)
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cartItem == null)
            {
                return NotFound();
            }

            return View(cartItem);
        }

        // POST: admin/CartItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CartItemExists(int id)
        {
            return _context.CartItems.Any(e => e.Id == id);
        }
    }
}