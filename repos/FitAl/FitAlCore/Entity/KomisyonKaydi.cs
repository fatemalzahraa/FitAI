using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitAl.Core.Entity
{
    public class KomisyonKaydi
    {
        public int KomisyonId { get; set; }

        public int MagazaId { get; set; }

        public int UrunId { get; set; }

        public int? WidgetSorguId { get; set; }

        public string PlatformSiparisKodu { get; set; } = null!;

        public decimal SatisTutari { get; set; }

        public decimal KomisyonTutari { get; set; }

        public decimal KomisyonOrani { get; set; }

        public bool AIAttributionMi { get; set; }

        public DateTime? AttributionTarihi { get; set; }

        public DateTime IslemTarihi { get; set; }

        public int DonemYil { get; set; }

        public int DonemAy { get; set; }

        // Navigation
        public Magaza Magaza { get; set; } = null!;
        public Urun Urun { get; set; } = null!;
        public WidgetSorguLog? WidgetSorguLog { get; set; }
    }
}
