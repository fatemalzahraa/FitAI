using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitAl.Core.Entity
{
    public class Urun
    {
        public int UrunId { get; set; }

        public int MagazaId { get; set; }

        public int BaglantiId { get; set; }

        public string PlatformUrunKodu { get; set; } = null!;

        public string Ad { get; set; } = null!;

        public string? Aciklama { get; set; }

        public string? KesimTuru { get; set; }

        public bool? KumasEsnek { get; set; }

        public string? VucutTipiEtiketi { get; set; }

        public string? EtiketKaynagi { get; set; }

        public DateTime? SonPushTarihi { get; set; }

        public string? PushDurumu { get; set; }

        public DateTime GuncellenmeTarihi { get; set; }

        public bool SilindiMi { get; set; }

        // Navigation
        public Magaza Magaza { get; set; } = null!;
        public PlatformBaglantisi PlatformBaglantisi { get; set; } = null!;
        public ICollection<VucutUyumSkoru> VucutUyumSkorlari { get; set; } = new List<VucutUyumSkoru>();
        public ICollection<NlpBulgusu> NlpBulgulari { get; set; } = new List<NlpBulgusu>();
        public ICollection<KomisyonKaydi> KomisyonKayitlari { get; set; } = new List<KomisyonKaydi>();
        public ICollection<Yorum> Yorumlar { get; set; } = new List<Yorum>();
        public ICollection<WidgetSorguLog> WidgetSorgulari { get; set; } = new List<WidgetSorguLog>();
    }
}
