using System.ComponentModel.DataAnnotations;

namespace Eticaret.Core.Entities
{
    public class City : IEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        // --- 🧠 KARAR DESTEK SİSTEMİ (KDS) ALANLARI ---

        [Display(Name = "Dikim Mesafesi Düzenleyici")]
        public int SpacingModifier { get; set; } = 0; // Örn: Malatya için -2, kurak yerler için +3

        [Display(Name = "Don Riski Seviyesi")]
        public string? FrostRiskLevel { get; set; } // Örn: "Yüksek", "Orta", "Düşük"

        [Display(Name = "Bölgesel Mühendislik Notu")]
        [StringLength(500)]
        public string? AdvisoryNote { get; set; } // Örn: "Bu bölge için geç çiçeklenen çeşitler önerilir."

        [Display(Name = "İklim Tipi")]
        public string? ClimateType { get; set; } // Örn: "Karasal", "Akdeniz"
    }
}