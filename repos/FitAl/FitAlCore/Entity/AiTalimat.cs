using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitAl.Core.Entity
{
    public class AiTalimat
    {
        public int TalimatId { get; set; }

        public int MagazaId { get; set; }

        public int? KullaniciId { get; set; }

        public string TalimatMetni { get; set; } = null!;

        public string? AnlasilanSenaryo { get; set; }

        public string? IslenenAgirliklar { get; set; }

        public string? OncekiAgirliklar { get; set; }

        public string OnayDurumu { get; set; } = "Onaylandi";

        public string? EtkiRaporu { get; set; }

        public bool EtkiRaporuOlusturulduMu { get; set; }

        public DateTime? PlanlananEtkiTarihi { get; set; }

        public DateTime OlusturmaTarihi { get; set; }

        // Navigation
        public Magaza Magaza { get; set; } = null!;
        public Kullanici? Kullanici { get; set; }
    }
}
