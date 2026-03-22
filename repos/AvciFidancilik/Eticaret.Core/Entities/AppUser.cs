using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Eticaret.Core.Entities
{
    public class AppUser : IEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        [Display(Name = "Adı")]
        [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        [Display(Name = "Soyadı")]
        [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir.")]
        public string SurName { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "E-posta")]
        [StringLength(100, ErrorMessage = "E-posta en fazla 100 karakter olabilir.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon numarası zorunludur.")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        [Display(Name = "Telefon")]
        [StringLength(15, ErrorMessage = "Telefon numarası en fazla 15 karakter olabilir.")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6, en fazla 100 karakter olabilir.")]
        public string PasswordHash { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        [Display(Name = "Kullanıcı Adı")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Kullanıcı adı en az 3, en fazla 50 karakter olabilir.")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Kullanıcı adı sadece harf, rakam ve alt çizgi içerebilir.")]
        public string UserName { get; set; } = string.Empty;

        [Display(Name = "Aktif")]
        public bool IsActive { get; set; } = true;

        // ⚠️ NOT:
        // Bu alan KULLANILMAMALIDIR.
        // Admin yetkisi UserRoles -> Role.Name == "Admin" üzerinden kontrol edilmelidir.

        [Display(Name = "Yönetici")]
        public bool IsAdmin { get; set; } = false;

        [Display(Name = "Oluşturulma Tarihi")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Kullanıcı GUID")]
        public Guid UserGuid { get; set; } = Guid.NewGuid();

        // Navigasyon özellikleri
        [Display(Name = "Adresler")]
        public ICollection<Address> Addresses { get; set; } = new List<Address>();

        [Display(Name = "Siparişler")]
        public ICollection<Order> Orders { get; set; } = new List<Order>();

        [Display(Name = "Sepet")]
        public Cart? Cart { get; set; }

        [Display(Name = "Favoriler")]
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

        [Display(Name = "Kullanıcı Rolleri")]
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        [Display(Name = "İletişimler")]
        public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
    }

}