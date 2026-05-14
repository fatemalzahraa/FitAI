using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;
using FitAI.Domain.Products;
using FitAI.Domain.Commerce;

namespace FitAI.Domain.Products 
{
    public class Yorum : AuditedAggregateRoot<int>
    {
        public int UrunId { get; set; }

        [ForeignKey("UrunId")]
        public virtual Urun Urun { get; set; } = null!;

        public int MagazaId { get; set; }

        [ForeignKey("MagazaId")]
        public virtual Magaza Magaza { get; set; } = null!;

        public string YorumMetni { get; set; } = null!;

        public int? Puan { get; set; }

        public bool NlpIslendi { get; set; } // Hafta 2'nin kilit noktası
    }
}