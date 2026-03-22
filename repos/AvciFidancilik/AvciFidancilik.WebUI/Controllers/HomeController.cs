// Controllers/HomeController.cs
using Eticaret.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Eticaret.Core.Entities;

namespace AvciFidancilik.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(DatabaseContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Database baðlantýsýný test et
                var productCount = await _context.Products.CountAsync();
                var categoryCount = await _context.Categories.CountAsync();

                ViewBag.ProductCount = productCount;
                ViewBag.CategoryCount = categoryCount;
                ViewBag.DbStatus = "Database baðlantýsý baþarýlý!";

                // TÜM ÜRÜNLERÝ ÇEK (Product ve ProductImages iliþkisi ile)
                var products = await _context.Products
                    .Include(p => p.Category) // Kategori bilgisi
                    .Include(p => p.ProductImages) // Ürün görselleri
                    .Where(p => p.Stock > 0) // Sadece stokta olanlar
                    .OrderByDescending(p => p.Id) // En yeniler önce
                    .ToListAsync();

                // DEBUG: Konsola yazdýr
                _logger.LogInformation($"Database baðlantýsý baþarýlý. {productCount} ürün, {categoryCount} kategori bulundu.");

                // Ürünleri View'e gönder
                return View(products);
            }
            catch (Exception ex)
            {
                ViewBag.DbStatus = $"Database hatasý: {ex.Message}";
                _logger.LogError(ex, "Database baðlantý hatasý!");

                // Hata durumunda boþ liste gönder
                return View(new List<Product>());
            }
        }
        [HttpPost]
        public async Task<IActionResult> SendContactMessage([FromBody] Contact model)
        {
            try
            {
                // 1. Temel Doðrulama
                if (model == null)
                {
                    return Json(new { success = false, message = "Geçersiz veri gönderildi." });
                }

                if (string.IsNullOrEmpty(model.Name) || string.IsNullOrEmpty(model.Message))
                {
                    return Json(new { success = false, message = "Lütfen Ad Soyad ve Mesaj alanlarýný doldurun." });
                }

                // 2. Entity'deki [Required] alanlarý doldurma
                // Formda Email ve Subject almadýðýmýz için veritabaný hatasý almamak adýna dolduruyoruz.
                if (string.IsNullOrEmpty(model.Email))
                    model.Email = "web-form@avcifidancilik.com";

                if (string.IsNullOrEmpty(model.Subject))
                    model.Subject = "Web Sitesi Footer Ýletiþim Formu";

                model.CreatedDate = DateTime.Now;

                // 3. KRÝTÝK NOKTA: Foreign Key Hatasýný Çözme
                // Veritabanýnda Id'si 0 olan bir kullanýcý olmadýðý için 0 atamak hata verir.
                // Eðer UserId nullable (int?) ise null atýyoruz:
                model.UserId = null;

                /* Not: Eðer veritabanýnda UserId hala zorunluysa ve null kabul etmiyorsa,
                   geçici olarak model.UserId = 1; (var olan bir id) yazabilirsin.
                */

                // 4. Kayýt Ýþlemi
                _context.Contacts.Add(model);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Hatanýn detayýný (InnerException) yakalayýp kullanýcýya veya loga basýyoruz
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return Json(new { success = false, message = "Sunucu Hatasý: " + innerMessage });
            }
        }
        // Kullanýcýn kendi mesajlarýný listelemesi için (Opsiyonel)
        public async Task<IActionResult> MyMessages()
        {
            // Burada giriþ yapmýþ kullanýcýnýn mesajlarýný çekebilirsin
            // Örn: var userId = _userManager.GetUserId(User);
            var messages = await _context.Contacts.OrderByDescending(x => x.CreatedDate).ToListAsync();
            return View(messages);
        }

        // GÜNCELLEME (Edit)
        [HttpPost]
        public async Task<IActionResult> UpdateContactMessage([FromBody] Contact model)
        {
            try
            {
                var existing = await _context.Contacts.FindAsync(model.Id);
                if (existing == null) return Json(new { success = false, message = "Mesaj bulunamadý." });

                existing.Name = model.Name;
                existing.Email = model.Email;
                existing.Subject = model.Subject;
                existing.Message = model.Message;

                _context.Update(existing);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }); }
        }

        // SÝLME (Delete)
        [HttpPost]
        public async Task<IActionResult> DeleteContactMessage(int id)
        {
            try
            {
                var contact = await _context.Contacts.FindAsync(id);
                if (contact == null) return Json(new { success = false, message = "Kayýt bulunamadý." });

                _context.Contacts.Remove(contact);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }); }
        }
        public IActionResult ContactUs()
        {
            return View();
        }
    }
}