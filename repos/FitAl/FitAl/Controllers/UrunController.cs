using Microsoft.AspNetCore.Mvc;

public class UrunController : Controller
{
    public IActionResult Index() => View();
    public IActionResult Ekle() => View();
    public IActionResult Analiz() => View();
    public IActionResult Detay(int id) => View();
}
