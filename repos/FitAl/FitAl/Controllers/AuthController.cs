using Microsoft.AspNetCore.Mvc;

public class AuthController : Controller
{
    public IActionResult Login() => View();

    [HttpPost]
    public IActionResult Login(string eposta, string sifre) => RedirectToAction("Index", "Dashboard");

    public IActionResult Register() => View();

    [HttpPost]
    public IActionResult Register(string magazaAdi, string eposta, string sifre) => RedirectToAction("Index", "Dashboard");
}
