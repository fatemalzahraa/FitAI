using AvciFidancilik.WebUI.Areas.admin.Controllers;
using Eticaret.Core.Entities;
using Eticaret.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AvciFidancilik.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SlidersController : BaseAdminController
    {
        private readonly DatabaseContext _context;
        private readonly IWebHostEnvironment _environment;

        public SlidersController(DatabaseContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        /* =======================
           INDEX
        ======================= */
        public async Task<IActionResult> Index()
        {
            var sliders = await _context.Sliders
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            return View(sliders);
        }

        /* =======================
           DETAILS
        ======================= */
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var slider = await _context.Sliders.FirstOrDefaultAsync(x => x.Id == id);
            if (slider == null) return NotFound();

            return View(slider);
        }

        /* =======================
           CREATE - GET
        ======================= */
        public IActionResult Create()
        {
            return View();
        }

        /* =======================
           CREATE - POST
        ======================= */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Slider slider, IFormFile ImageFile)
        {
            // ImageUrl model validation hatasını temizle
            ModelState.Remove("ImageUrl");

            if (ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "Slider görseli zorunludur.");
            }

            if (!ModelState.IsValid)
            {
                return View(slider);
            }

            slider.ImageUrl = await SaveImage(ImageFile);

            _context.Sliders.Add(slider);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Slider başarıyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        /* =======================
           EDIT - GET
        ======================= */
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var slider = await _context.Sliders.FindAsync(id);
            if (slider == null) return NotFound();

            return View(slider);
        }

        /* =======================
           EDIT - POST
        ======================= */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Slider slider, IFormFile ImageFile)
        {
            if (id != slider.Id) return NotFound();

            ModelState.Remove("ImageUrl");

            if (!ModelState.IsValid)
            {
                return View(slider);
            }

            var oldSlider = await _context.Sliders
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (oldSlider == null) return NotFound();

            if (ImageFile != null)
            {
                DeletePhysicalImage(oldSlider.ImageUrl);
                slider.ImageUrl = await SaveImage(ImageFile);
            }
            else
            {
                slider.ImageUrl = oldSlider.ImageUrl;
            }

            _context.Update(slider);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Slider başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        /* =======================
           DELETE - GET
        ======================= */
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var slider = await _context.Sliders.FirstOrDefaultAsync(x => x.Id == id);
            if (slider == null) return NotFound();

            return View(slider);
        }

        /* =======================
           DELETE - POST
        ======================= */
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var slider = await _context.Sliders.FindAsync(id);
            if (slider != null)
            {
                DeletePhysicalImage(slider.ImageUrl);
                _context.Sliders.Remove(slider);
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Slider silindi.";
            return RedirectToAction(nameof(Index));
        }

        /* =======================
           IMAGE SAVE
        ======================= */
        private async Task<string> SaveImage(IFormFile file)
        {
            string folder = Path.Combine(_environment.WebRootPath, "admin", "img", "slider");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            string path = Path.Combine(folder, fileName);

            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);

            return "/admin/img/slider/" + fileName;
        }

        /* =======================
           IMAGE DELETE
        ======================= */
        private void DeletePhysicalImage(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;

            string path = Path.Combine(_environment.WebRootPath, imageUrl.TrimStart('/'));

            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
        }
    }
}
