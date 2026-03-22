using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Eticaret.Core.Entities;
using Eticaret.Data;

namespace AvciFidancilik.WebUI.Areas.admin.Controllers
{
    [Area("admin")]
    public class AddressesController : BaseAdminController
    {
        private readonly DatabaseContext _context;

        public AddressesController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: admin/Addresses
        public async Task<IActionResult> Index()
        {
            var databaseContext = _context.Addresses.Include(a => a.User);
            return View(await databaseContext.ToListAsync());
        }

        // GET: admin/Addresses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var address = await _context.Addresses
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (address == null)
                return NotFound();

            return View(address);
        }

        // GET: admin/Addresses/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.AppUsers, "Id", "Email");
            return View();
        }

        // POST: admin/Addresses/Create
        [HttpPost]
        public async Task<IActionResult> Create(Address address)
        {
            // 1) UserId zorunlu
            if (address.UserId <= 0)
            {
                ModelState.AddModelError("UserId", "Kullanıcı seçilmelidir.");
                return View(address);
            }

            // 2) Diğer zorunlu alanlar (Title, City vs.)
            if (string.IsNullOrWhiteSpace(address.Title))
                ModelState.AddModelError("Title", "Başlık boş olamaz.");

            if (string.IsNullOrWhiteSpace(address.City))
                ModelState.AddModelError("City", "Şehir boş olamaz.");

            if (!ModelState.IsValid)
                return View(address);

            // 3) Veri tabanına ekle
            _context.Add(address);
            await _context.SaveChangesAsync();

            // 4) Listeye yönlendir
            return RedirectToAction(nameof(Index));
        }



        // GET: admin/Addresses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var address = await _context.Addresses.FindAsync(id);
            if (address == null)
                return NotFound();

            ViewData["UserId"] = new SelectList(_context.AppUsers, "Id", "Email", address.UserId);
            return View(address);
        }

        // POST: admin/Addresses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Address address)
        {
            if (id != address.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["UserId"] = new SelectList(_context.AppUsers, "Id", "Email", address.UserId);
                return View(address);
            }

            try
            {
                _context.Update(address);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Addresses.Any(e => e.Id == address.Id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: admin/Addresses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var address = await _context.Addresses
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (address == null)
                return NotFound();

            return View(address);
        }

        // POST: admin/Addresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var address = await _context.Addresses.FindAsync(id);

            if (address != null)
                _context.Addresses.Remove(address);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
