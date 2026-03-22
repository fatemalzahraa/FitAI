using AvciFidancilik.WebUI.Areas.admin.Controllers;
using Eticaret.Core.Entities;
using Eticaret.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace AvciFidancilik.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductImagesController : BaseAdminController
    {
        private readonly DatabaseContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProductImagesController(DatabaseContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Admin/ProductImages
        public async Task<IActionResult> Index()
        {
            var productImages = await _context.ProductImages
                .Include(p => p.Product)
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            return View(productImages);
        }

        // GET: Admin/ProductImages/ProductImages/5
        public async Task<IActionResult> ProductImages(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                TempData["ErrorMessage"] = "Ürün bulunamadı!";
                return RedirectToAction("Index", "Products", new { area = "Admin" });
            }

            var images = await _context.ProductImages
                .Where(p => p.ProductId == productId)
                .Include(p => p.Product)
                .OrderByDescending(p => p.IsMain)
                .ThenByDescending(p => p.Id)
                .ToListAsync();

            ViewBag.ProductId = productId;
            ViewBag.ProductName = product.Name;

            return View("Index", images);
        }

        // GET: Admin/ProductImages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Resim ID belirtilmemiş!";
                return RedirectToAction(nameof(Index));
            }

            var productImage = await _context.ProductImages
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (productImage == null)
            {
                TempData["ErrorMessage"] = "Resim bulunamadı!";
                return RedirectToAction(nameof(Index));
            }

            // Aynı ürüne ait tüm resimleri al
            var allProductImages = await _context.ProductImages
                .Where(p => p.ProductId == productImage.ProductId)
                .Include(p => p.Product)
                .OrderByDescending(p => p.IsMain)
                .ThenByDescending(p => p.Id)
                .ToListAsync();

            ViewBag.AllProductImages = allProductImages;
            ViewBag.ProductId = productImage.ProductId;
            ViewBag.ProductName = productImage.Product?.Name;

            return View(productImage);
        }

        // GET: Admin/ProductImages/Create
        public IActionResult Create()
        {
            var products = _context.Products.OrderBy(p => p.Name).ToList();
            ViewBag.ProductList = new SelectList(products, "Id", "Name");
            return View();
        }

        // GET: Admin/ProductImages/CreateForProduct/5
        public async Task<IActionResult> CreateForProduct(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                TempData["ErrorMessage"] = "Ürün bulunamadı!";
                return RedirectToAction("Index", "Products", new { area = "Admin" });
            }

            var products = _context.Products.OrderBy(p => p.Name).ToList();
            ViewBag.SelectedProductId = productId;
            ViewBag.ProductName = product.Name;
            ViewBag.ProductList = new SelectList(products, "Id", "Name", productId);

            return View("Create");
        }

        // POST: Admin/ProductImages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,IsMain")] ProductImage productImage, List<IFormFile> files)
        {
            try
            {
                // 1. Ürün kontrolü
                if (productImage.ProductId <= 0)
                {
                    ModelState.AddModelError("ProductId", "Lütfen bir ürün seçin!");
                    await SetViewBagsForError(productImage.ProductId);
                    return View(productImage);
                }

                // 2. Dosya kontrolü
                if (files == null || files.Count == 0)
                {
                    ModelState.AddModelError("", "Lütfen en az bir resim dosyası seçin!");
                    await SetViewBagsForError(productImage.ProductId);
                    return View(productImage);
                }

                if (files.Count > 10)
                {
                    ModelState.AddModelError("", "Maksimum 10 resim yükleyebilirsiniz!");
                    await SetViewBagsForError(productImage.ProductId);
                    return View(productImage);
                }

                // 3. Uploads klasörünü kontrol et
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "products");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // 4. Dosya yükleme işlemi
                var uploadedImages = new List<ProductImage>();
                var errorMessages = new List<string>();
                var uploadSuccessCount = 0;

                foreach (var file in files)
                {
                    try
                    {
                        if (file.Length == 0) continue;

                        // Dosya tipi kontrolü
                        if (!file.ContentType.StartsWith("image/"))
                        {
                            errorMessages.Add($"{file.FileName} geçerli bir resim dosyası değil!");
                            continue;
                        }

                        // Dosya boyutu kontrolü (5MB)
                        if (file.Length > 5 * 1024 * 1024)
                        {
                            errorMessages.Add($"{file.FileName} 5MB'tan büyük olamaz!");
                            continue;
                        }

                        // Geçerli dosya uzantıları
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp" };
                        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                        if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
                        {
                            errorMessages.Add($"{file.FileName} desteklenmeyen dosya formatı!");
                            continue;
                        }

                        // Benzersiz dosya adı oluştur
                        string uniqueFileName = $"{Guid.NewGuid():N}{extension}";
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        // Dosyayı kaydet
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        // ProductImage nesnesi oluştur
                        var newImage = new ProductImage
                        {
                            ProductId = productImage.ProductId,
                            ImageUrl = $"/uploads/products/{uniqueFileName}",
                            IsMain = (uploadSuccessCount == 0 && productImage.IsMain),
                        };

                        uploadedImages.Add(newImage);
                        uploadSuccessCount++;
                    }
                    catch (Exception ex)
                    {
                        errorMessages.Add($"{file.FileName} yüklenirken hata oluştu: {ex.Message}");
                    }
                }

                // 5. Hata mesajlarını ekle
                foreach (var error in errorMessages)
                {
                    ModelState.AddModelError("", error);
                }

                // 6. Başarılı yüklenen dosya yoksa
                if (!uploadedImages.Any())
                {
                    ModelState.AddModelError("", "Hiçbir resim yüklenemedi!");
                    await SetViewBagsForError(productImage.ProductId);
                    return View(productImage);
                }

                // 7. Ana resim kontrolü ve güncellemesi
                if (productImage.IsMain && uploadedImages.Any(img => img.IsMain))
                {
                    // Önce bu ürüne ait tüm ana resimleri false yap
                    var existingMainImages = await _context.ProductImages
                        .Where(p => p.ProductId == productImage.ProductId && p.IsMain)
                        .ToListAsync();

                    if (existingMainImages.Any())
                    {
                        foreach (var img in existingMainImages)
                        {
                            img.IsMain = false;
                            _context.ProductImages.Update(img);
                        }
                        await _context.SaveChangesAsync();
                    }
                }

                // 8. Yeni resimleri veritabanına ekle
                await _context.ProductImages.AddRangeAsync(uploadedImages);
                await _context.SaveChangesAsync();

                // 9. Başarı mesajı
                TempData["SuccessMessage"] = $"{uploadSuccessCount} resim başarıyla yüklendi!";

                // 10. Ürün sayfasına yönlendir
                return RedirectToAction("ProductImages", new { productId = productImage.ProductId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Resim yüklenirken bir hata oluştu: {ex.Message}");
                await SetViewBagsForError(productImage.ProductId);
                return View(productImage);
            }
        }

        // GET: Admin/ProductImages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Resim ID belirtilmemiş!";
                return RedirectToAction(nameof(Index));
            }

            var productImage = await _context.ProductImages
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (productImage == null)
            {
                TempData["ErrorMessage"] = "Resim bulunamadı!";
                return RedirectToAction(nameof(Index));
            }

            var products = _context.Products.OrderBy(p => p.Name).ToList();
            ViewBag.ProductList = new SelectList(products, "Id", "Name", productImage.ProductId);

            ViewBag.SelectedProductId = productImage.ProductId;
            ViewBag.ProductName = productImage.Product?.Name;

            return View(productImage);
        }

        // POST: Admin/ProductImages/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,IsMain")] ProductImage productImage, IFormFile imageFile)
        {
            if (id != productImage.Id)
            {
                TempData["ErrorMessage"] = "Resim ID uyuşmazlığı!";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // Mevcut resmi al
                var existingImage = await _context.ProductImages.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                if (existingImage == null)
                {
                    TempData["ErrorMessage"] = "Resim bulunamadı!";
                    return RedirectToAction(nameof(Index));
                }

                // Eğer yeni dosya yüklenmişse, onu işle
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Dosya tipi kontrolü
                    if (!imageFile.ContentType.StartsWith("image/"))
                    {
                        ModelState.AddModelError("", "Geçerli bir resim dosyası değil!");
                        await SetViewBagsForError(productImage.ProductId);
                        return View(productImage);
                    }

                    // Dosya boyutu kontrolü (5MB)
                    if (imageFile.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("", "Dosya boyutu maksimum 5MB olmalıdır!");
                        await SetViewBagsForError(productImage.ProductId);
                        return View(productImage);
                    }

                    // Geçerli dosya uzantıları
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp" };
                    var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                    if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("", "Desteklenmeyen dosya formatı!");
                        await SetViewBagsForError(productImage.ProductId);
                        return View(productImage);
                    }

                    // Uploads klasörünü kontrol et
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "products");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Benzersiz dosya adı oluştur
                    string uniqueFileName = $"{Guid.NewGuid():N}{extension}";
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Dosyayı kaydet
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    // Eski dosyayı sil (opsiyonel)
                    if (!string.IsNullOrEmpty(existingImage.ImageUrl) && existingImage.ImageUrl.StartsWith("/uploads/"))
                    {
                        var oldFilePath = Path.Combine(_environment.WebRootPath, existingImage.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            try
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                            catch { /* Eski dosya silinemezse devam et */ }
                        }
                    }

                    // Yeni URL'yi entity'e ekle
                    productImage.ImageUrl = $"/uploads/products/{uniqueFileName}";
                }
                else
                {
                    // Dosya yüklenmemişse, mevcut URL'yi kullan
                    productImage.ImageUrl = existingImage.ImageUrl;
                }

                // Ana resim kontrolü
                if (productImage.IsMain)
                {
                    // Aynı ürüne ait diğer tüm resimleri ana resim olmaktan çıkar
                    var otherImages = await _context.ProductImages
                        .Where(p => p.ProductId == productImage.ProductId && p.Id != id)
                        .ToListAsync();

                    foreach (var img in otherImages)
                    {
                        img.IsMain = false;
                        _context.Update(img);
                    }
                }

                // ModelState'i temizle ve manuel kontrol et
                ModelState.Clear();
                TryValidateModel(productImage);

                if (ModelState.IsValid)
                {
                    _context.Update(productImage);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Resim başarıyla güncellendi!";
                    return RedirectToAction("Details", new { id = productImage.Id });
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductImageExists(productImage.Id))
                {
                    TempData["ErrorMessage"] = "Resim bulunamadı!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Resim güncellenirken bir hata oluştu: {ex.Message}");
            }

            // Hata durumunda ViewBag'leri yeniden doldur
            var products = _context.Products.OrderBy(p => p.Name).ToList();
            ViewBag.ProductList = new SelectList(products, "Id", "Name", productImage.ProductId);
            await SetViewBagsForError(productImage.ProductId);

            return View(productImage);
        }

        // GET: Admin/ProductImages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Resim ID belirtilmemiş!";
                return RedirectToAction(nameof(Index));
            }

            var productImage = await _context.ProductImages
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (productImage == null)
            {
                TempData["ErrorMessage"] = "Resim bulunamadı!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ProductId = productImage.ProductId;
            ViewBag.ProductName = productImage.Product?.Name;

            return View(productImage);
        }

        // POST: Admin/ProductImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var productImage = await _context.ProductImages
                    .Include(p => p.Product)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (productImage == null)
                {
                    TempData["ErrorMessage"] = "Resim bulunamadı!";
                    return RedirectToAction(nameof(Index));
                }

                var productId = productImage.ProductId;
                var wasMain = productImage.IsMain;

                // Fiziksel dosyayı da sil (opsiyonel)
                if (!string.IsNullOrEmpty(productImage.ImageUrl) && productImage.ImageUrl.StartsWith("/uploads/"))
                {
                    var filePath = Path.Combine(_environment.WebRootPath, productImage.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        try
                        {
                            System.IO.File.Delete(filePath);
                        }
                        catch (Exception ex)
                        {
                            // Dosya silinemedi ama devam et
                            TempData["WarningMessage"] = $"Resim dosyası silinemedi: {ex.Message}";
                        }
                    }
                }

                _context.ProductImages.Remove(productImage);
                await _context.SaveChangesAsync();

                // Eğer silinen resim ana resimse ve başka resim varsa, ilk resmi ana yap
                if (wasMain)
                {
                    var remainingImages = await _context.ProductImages
                        .Where(p => p.ProductId == productId)
                        .OrderByDescending(p => p.Id)
                        .FirstOrDefaultAsync();

                    if (remainingImages != null)
                    {
                        remainingImages.IsMain = true;
                        _context.Update(remainingImages);
                        await _context.SaveChangesAsync();
                        TempData["InfoMessage"] = "Ana resim silindiği için yeni bir ana resim seçildi.";
                    }
                }

                TempData["SuccessMessage"] = "Resim başarıyla silindi!";
                return RedirectToAction("ProductImages", new { productId = productId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Resim silinirken bir hata oluştu: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Admin/ProductImages/SetAsMain/5 (AJAX için)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetAsMain(int id)
        {
            try
            {
                var productImage = await _context.ProductImages
                    .Include(p => p.Product)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (productImage == null)
                {
                    return Json(new { success = false, message = "Resim bulunamadı!" });
                }

                // Aynı ürüne ait diğer tüm resimleri ana resim olmaktan çıkar
                var otherImages = await _context.ProductImages
                    .Where(p => p.ProductId == productImage.ProductId && p.Id != id)
                    .ToListAsync();

                foreach (var img in otherImages)
                {
                    img.IsMain = false;
                    _context.Update(img);
                }

                productImage.IsMain = true;
                _context.Update(productImage);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Ana resim başarıyla güncellendi!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Hata: {ex.Message}" });
            }
        }

        #region Helper Methods

        private bool ProductImageExists(int id)
        {
            return _context.ProductImages.Any(e => e.Id == id);
        }

        private async Task SetProductViewBag(int productId)
        {
            if (productId <= 0) return;

            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                ViewBag.SelectedProductId = productId;
                ViewBag.ProductName = product.Name;
            }
        }

        private async Task SetViewBagsForError(int productId)
        {
            var products = _context.Products.OrderBy(p => p.Name).ToList();
            ViewBag.ProductList = new SelectList(products, "Id", "Name", productId);

            if (productId > 0)
            {
                await SetProductViewBag(productId);
            }
        }

        #endregion
    }
}