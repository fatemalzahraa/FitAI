using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitAl.Core.Entity
{
    public class PlatformBaglantisi
    {
        public int BaglantiId { get; set; }

        public int MagazaId { get; set; }

        public string PlatformTuru { get; set; } = null!;

        public string ApiAnahtari { get; set; } = null!;

        public string? ApiGizliAnahtari { get; set; }

        public bool AktifMi { get; set; } = true;

        public DateTime? SonSenkronizasyon { get; set; }

        public string? WebhookUrl { get; set; }

        public string? WebhookGizliAnahtari { get; set; }

        public DateTime OlusturmaTarihi { get; set; }

        // Navigation
        public Magaza Magaza { get; set; } = null!;
        public ICollection<Urun> Urunler { get; set; } = new List<Urun>();
        public ICollection<SenkronizasyonLog> SenkronizasyonLoglari { get; set; } = new List<SenkronizasyonLog>();
    }
}
