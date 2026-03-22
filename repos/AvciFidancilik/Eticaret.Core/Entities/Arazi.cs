using Eticaret.Core.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eticaret.Data.Entities
{
    public class Arazi : IEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Arazi büyüklüğü (dönüm) zorunludur.")]
        [Display(Name = "Dönüm")]
        public decimal Donum { get; set; }

        // Kullanıcının chat sırasında yazdığı ham metinleri burada yedekliyoruz
        [Display(Name = "Şehir Adı")]
        public string? Sehir { get; set; }

        [Display(Name = "Toprak Tipi Açıklaması")]
        public string? ToprakTipi { get; set; }

        // --- KARAR DESTEK SİSTEMİ İLİŞKİLERİ ---

        // Sistemin belirlediği İklim (Örn: Malatya seçilince arka planda İklimId = 1 olur)
        public int? IklimId { get; set; }

      

        // Sistemin belirlediği Toprak Tipi (Örn: "Killi" seçilince SuTutmaKatsayısı'na buradan ulaşırız)
        public int? ToprakTipiId { get; set; }

        [ForeignKey("ToprakTipiId")]
      

        // Bu araziyi sistemdeki bir kullanıcıya bağlamak istersen (Giriş yapılmışsa)
        public int? UserId { get; set; }

        [ForeignKey("UserId")]
        public AppUser? User { get; set; }
    }
}