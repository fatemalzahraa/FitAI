using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace FitAI.Domain.Products
{
    public class Urun : AuditedAggregateRoot<int>
    {
        public int MagazaId { get; set; }

        public string Ad { get; set; } = null!;

        public string? Aciklama { get; set; }

        public string? KesimTuru { get; set; }

        public bool? KumasEsnek { get; set; }

        public bool SilindiMi { get; set; }
    }
}