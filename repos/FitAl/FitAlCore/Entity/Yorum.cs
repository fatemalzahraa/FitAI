using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitAl.Core.Entity
{
    public class Yorum
    {
        public int YorumId { get; set; }

        public int UrunId { get; set; }

        public int MagazaId { get; set; }

        public string PlatformYorumKodu { get; set; } = null!;

        public string? YazarAdi { get; set; }

        public int? Puan { get; set; }

        public string YorumMetni { get; set; } = null!;

        public bool NlpIslendi { get; set; }

        public DateTime? YorumTarihi { get; set; }

        public DateTime CekilmeTarihi { get; set; }

        // Navigation
        public Urun Urun { get; set; } = null!;
        public Magaza Magaza { get; set; } = null!;
    }
}
