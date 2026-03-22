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
    public class OrderItemsController : BaseAdminController
    {
        private readonly DatabaseContext _context;

        public OrderItemsController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: admin/OrderItems
        public async Task<IActionResult> Index()
        {
            var orderItems = _context.OrderItems
                .Include(o => o.Order)
                .Include(o => o.Product);

            return View(await orderItems.ToListAsync());
        }

        // GET: admin/OrderItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var orderItem = await _context.OrderItems
                .Include(o => o.Order)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (orderItem == null)
                return NotFound();

            return View(orderItem);
        }

        // GET: admin/OrderItems/Create
        public IActionResult Create()
        {
            // Status KULLANMIYORUZ → sadece Id gösteriyoruz
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id");
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Description");
            return View();
        }

        // POST: admin/OrderItems/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,OrderId,ProductId,Quantity,UnitPrice")] OrderItem orderItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["OrderId"] = new SelectList(
                _context.Orders, "Id", "Id", orderItem.OrderId);

            ViewData["ProductId"] = new SelectList(
                _context.Products, "Id", "Description", orderItem.ProductId);

            return View(orderItem);
        }

        // GET: admin/OrderItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem == null)
                return NotFound();

            ViewData["OrderId"] = new SelectList(
                _context.Orders, "Id", "Id", orderItem.OrderId);

            ViewData["ProductId"] = new SelectList(
                _context.Products, "Id", "Description", orderItem.ProductId);

            return View(orderItem);
        }

        // POST: admin/OrderItems/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,OrderId,ProductId,Quantity,UnitPrice")] OrderItem orderItem)
        {
            if (id != orderItem.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderItemExists(orderItem.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["OrderId"] = new SelectList(
                _context.Orders, "Id", "Id", orderItem.OrderId);

            ViewData["ProductId"] = new SelectList(
                _context.Products, "Id", "Description", orderItem.ProductId);

            return View(orderItem);
        }

        // GET: admin/OrderItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var orderItem = await _context.OrderItems
                .Include(o => o.Order)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (orderItem == null)
                return NotFound();

            return View(orderItem);
        }

        // POST: admin/OrderItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem != null)
            {
                _context.OrderItems.Remove(orderItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool OrderItemExists(int id)
        {
            return _context.OrderItems.Any(e => e.Id == id);
        }
    }
}
