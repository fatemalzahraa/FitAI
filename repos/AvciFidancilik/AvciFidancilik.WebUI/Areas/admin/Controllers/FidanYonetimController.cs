using Microsoft.AspNetCore.Mvc;

namespace AvciFidancilik.WebUI.Controllers
{
    [Area("Admin")] // Admin paneli içinde olduğu için Area eklemeyi unutma
    public class FidanYönetimController : Controller
    {
        // GET: /Admin/Fidan/Index
        public IActionResult Index()
        {
            // Bu sayfa sadece bir 'Dashboard' (Panonun) görevi görür.
            // Diğer controller'ların (Fidans, Iklims, FidanKurals vb.) listelendiği ana ekrandır.
            return View();
        }
    }
}