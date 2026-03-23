using Microsoft.AspNetCore.Mvc;

public class AiTalimatController : Controller
{
    public IActionResult Index() => View();
    public IActionResult Gecmis() => View();

    [HttpPost]
    public IActionResult Gonder(string talimatMetni) => RedirectToAction("Index");
}
