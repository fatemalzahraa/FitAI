using System;
using System.ComponentModel.DataAnnotations;

namespace Eticaret.Core.Entities
{
    public class Contact : IEntity // Kullanıcıların iletişim veya destek mesajlarını tutar.
                                   // Eğer kullanıcı giriş yaptıysa UserId ile bağlanır.
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad Soyad zorunludur.")]
        [Display(Name = "Ad Soyad")]
        [StringLength(100, ErrorMessage = "Ad Soyad en fazla 100 karakter olabilir.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "E-posta")]
        [StringLength(100, ErrorMessage = "E-posta en fazla 100 karakter olabilir.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Konu zorunludur.")]
        [Display(Name = "Konu")]
        [StringLength(200, ErrorMessage = "Konu en fazla 200 karakter olabilir.")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mesaj zorunludur.")]
        [Display(Name = "Mesaj")]
        [StringLength(5000, MinimumLength = 10, ErrorMessage = "Mesaj en az 10, en fazla 5000 karakter olabilir.")]
        public string Message { get; set; } = string.Empty;

        [Display(Name = "Oluşturulma Tarihi")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Kullanıcı ID")]
        public int? UserId { get; set; }

        [Display(Name = "Kullanıcı")]
        public AppUser? User { get; set; }
    }
}