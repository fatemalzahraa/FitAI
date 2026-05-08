using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FitAI.Web.Pages;

namespace FitAI.Web.Pages.Account;

public class LoginModel : FitAIPageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();   // Varsayılan nesne oluştur

    public class InputModel
    {
        [Required(ErrorMessage = "E-posta veya kullanıcı adı gereklidir.")]
        public string? EmailOrUsername { get; set; }

        [Required(ErrorMessage = "Şifre gereklidir.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public bool RememberMe { get; set; }
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Demo doğrulama
        if ((Input.EmailOrUsername == "admin@fitai.com" || Input.EmailOrUsername == "admin") && Input.Password == "12345678")
        {
            return RedirectToPage("/Index");
        }

        ModelState.AddModelError(string.Empty, "E-posta veya şifre hatalı.");
        return Page();
    }
}