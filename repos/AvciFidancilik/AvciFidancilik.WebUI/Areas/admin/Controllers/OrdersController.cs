using AvciFidancilik.WebUI.Areas.admin.Controllers;
using Eticaret.Core.Entities;
using Eticaret.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AvciFidancilik.WebUI.Areas.Admin.Controllers
{
    [Area("admin")]
    public class OrdersController : BaseAdminController
    {
        private readonly DatabaseContext _context;

        public OrdersController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: admin/Orders
        public async Task<IActionResult> Index(string search, string sortBy, string sortOrder)
        {
            ViewData["CurrentSearch"] = search;
            ViewData["SortBy"] = sortBy;
            ViewData["SortOrder"] = sortOrder;

            var orders = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .AsQueryable();

            // Arama (SADE)
            if (!string.IsNullOrEmpty(search))
            {
                orders = orders.Where(o =>
                    o.Id.ToString().Contains(search) ||
                    o.User.Email.Contains(search));
            }

            // Sıralama
            switch (sortBy)
            {
                case "Id":
                    orders = sortOrder == "desc"
                        ? orders.OrderByDescending(o => o.Id)
                        : orders.OrderBy(o => o.Id);
                    break;

                case "User":
                    orders = sortOrder == "desc"
                        ? orders.OrderByDescending(o => o.User.Email)
                        : orders.OrderBy(o => o.User.Email);
                    break;

                case "OrderDate":
                    orders = sortOrder == "desc"
                        ? orders.OrderByDescending(o => o.OrderDate)
                        : orders.OrderBy(o => o.OrderDate);
                    break;

                default:
                    orders = orders.OrderByDescending(o => o.OrderDate);
                    break;
            }

            return View(await orders.ToListAsync());
        }

        // GET: admin/Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return RedirectToAction(nameof(Index));

            return View(order);
        }

        // GET: admin/Orders/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.AppUsers, "Id", "Email");
            return View();
        }

        // POST: admin/Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId")] Order order)
        {
            if (ModelState.IsValid)
            {
                order.OrderDate = DateTime.UtcNow;

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Sipariş oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["UserId"] = new SelectList(_context.AppUsers, "Id", "Email", order.UserId);
            return View(order);
        }

        // GET: admin/Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return RedirectToAction(nameof(Index));

            ViewData["UserId"] = new SelectList(_context.AppUsers, "Id", "Email", order.UserId);
            return View(order);
        }

        // POST: admin/Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,OrderDate")] Order order)
        {
            if (id != order.Id)
                return RedirectToAction(nameof(Index));

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Sipariş güncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
                        return RedirectToAction(nameof(Index));
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["UserId"] = new SelectList(_context.AppUsers, "Id", "Email", order.UserId);
            return View(order);
        }

        // GET: admin/Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return RedirectToAction(nameof(Index));

            ViewBag.TotalAmount = order.OrderItems?.Sum(x => x.Quantity * x.UnitPrice) ?? 0;
            return View(order);
        }

        // POST: admin/Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order != null)
            {
                if (order.OrderItems != null && order.OrderItems.Any())
                    _context.OrderItems.RemoveRange(order.OrderItems);

                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
