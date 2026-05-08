using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace FitAI.Domain.Products
{
    public class Yorum : AuditedAggregateRoot<int>
    {
        public int UrunId { get; set; }

        public int MagazaId { get; set; }

        public string YorumMetni { get; set; } = null!;

        public int? Puan { get; set; }

        public bool NlpIslendi { get; set; }
    }
}