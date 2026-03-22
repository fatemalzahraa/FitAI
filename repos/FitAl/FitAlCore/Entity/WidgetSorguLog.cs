using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitAl.Core.Entity
{
    public class WidgetSorguLog
    {
        public int SorguId { get; set; }

        public int? ProfilId { get; set; }

        public int UrunId { get; set; }

        public int MagazaId { get; set; }

        public string VucutTipi { get; set; } = null!;

        public decimal DonulenUyumSkoru { get; set; }

        public string SkorKaynagi { get; set; } = null!;

        public DateTime SorguTarihi { get; set; }

        public bool SatinAlmaGerceklesti { get; set; }

        public DateTime? SatinAlmaTarihi { get; set; }

        public string? IpAdresi { get; set; }

        // Navigation
        public KullaniciProfil? Profil { get; set; }
        public Urun Urun { get; set; } = null!;
        public Magaza Magaza { get; set; } = null!;
        public ICollection<KomisyonKaydi> KomisyonKayitlari { get; set; } = new List<KomisyonKaydi>();
    }
}
