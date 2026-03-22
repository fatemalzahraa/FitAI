using Eticaret.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AvciFidancilik.WebUI.Areas.admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = "Admin")]
    public class MainController : BaseAdminController
    {
        private readonly IAdminKdsService _kdsService;

        // Dependency Injection ile Karar Destek Sistemi servisini içeri alıyoruz
        public MainController(IAdminKdsService kdsService)
        {
            _kdsService = kdsService;
        }

        public async Task<IActionResult> Index()
        {
            // 🚀 Karar Destek Sistemi'nin tüm analizlerini (Sepet, Stok, Trend, Mevsimsel) tek seferde çekiyoruz
            var kdsResult = await _kdsService.GetDashboardSummaryAsync();

            // Yeni DTO yapısındaki metrikleri View'da kolayca kullanabilmek için ViewBag'e atıyoruz
            ViewBag.UserName = User.Identity?.Name;
            ViewBag.IsAdmin = User.IsInRole("Admin");

            // Dashboard kartlarında kullanılacak ana metrikler
            ViewBag.MonthlySales = kdsResult.MonthlySales;
            ViewBag.SalesGrowth = kdsResult.SalesGrowth;
            ViewBag.ActiveCustomers = kdsResult.ActiveCustomers;
            ViewBag.CriticalCount = kdsResult.CriticalStockCount;

            // KDS sonuçlarını model olarak View'a gönderiyoruz
            return View(kdsResult);
        }

        // AJAX üzerinden verileri manuel yenilemek istersen bu API ucunu kullanabilirsin
        [HttpGet]
        public async Task<JsonResult> RefreshKdsData()
        {
            var data = await _kdsService.GetDashboardSummaryAsync();
            return Json(data);
        }

        public IActionResult Dashboard()
        {
            return View();
        }
    }
}