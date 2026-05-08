using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
namespace FitAI.Web.Pages.Account;

public class RegisterModel : FitAI.Web.Pages.FitAIPageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new InputModel();   // ← default değer ata

    public class InputModel
    {
        [Required(ErrorMessage = "Ad soyad gereklidir.")]
        [Display(Name = "Ad Soyad")]
        public string? FullName { get; set; }   // ← nullable yap

        [Required(ErrorMessage = "E-posta adresi gereklidir.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string? Email { get; set; }   // ← nullable yap

        [Required(ErrorMessage = "Şifre gereklidir.")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Şifre en az 8 karakter olmalıdır.")]
        public string? Password { get; set; }   // ← nullable yap

        [Required(ErrorMessage = "Kullanım koşullarını kabul etmelisiniz.")]
        public bool AcceptTerms { get; set; }
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

        // Demo başarılı kayıt simülasyonu
        return RedirectToPage("/Account/Login", new { registered = true });
    }
}