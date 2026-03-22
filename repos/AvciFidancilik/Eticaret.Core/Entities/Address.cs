using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eticaret.Core.Entities
{
    public class Address : IEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Adres başlığı zorunludur.")]
        [Display(Name = "Adres Başlığı")]
        [StringLength(100, ErrorMessage = "Adres başlığı en fazla 100 karakter olabilir.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Adres satırı zorunludur.")]
        [Display(Name = "Adres")]
        [StringLength(500, ErrorMessage = "Adres en fazla 500 karakter olabilir.")]
        public string Line { get; set; } = string.Empty;

        [Required(ErrorMessage = "İlçe zorunludur.")]
        [Display(Name = "İlçe")]
        [StringLength(100, ErrorMessage = "İlçe adı en fazla 100 karakter olabilir.")]
        public string District { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şehir zorunludur.")]
        [Display(Name = "Şehir")]
        [StringLength(50, ErrorMessage = "Şehir adı en fazla 50 karakter olabilir.")]
        public string City { get; set; } = string.Empty;

        [Display(Name = "Posta Kodu")]
        [StringLength(10, ErrorMessage = "Posta kodu en fazla 10 karakter olabilir.")]
        [RegularExpression(@"^\d{5}$", ErrorMessage = "Geçerli bir posta kodu giriniz (5 haneli).")]
        public string? PostalCode { get; set; }

        [Display(Name = "TC Kimlik No")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "TC Kimlik No 11 haneli olmalıdır.")]
        [RegularExpression(@"^[1-9]{1}[0-9]{9}[0,2,4,6,8]{1}$", ErrorMessage = "Geçerli bir TC Kimlik No giriniz.")]
        public string? IdentityNumber { get; set; }

        [Display(Name = "Aktif")]
        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = "Kullanıcı ID zorunludur.")]
        [Display(Name = "Kullanıcı ID")]
        public int UserId { get; set; }

        [Display(Name = "Kullanıcı")]
        [ForeignKey("UserId")]
        public AppUser? User { get; set; }
    }
}