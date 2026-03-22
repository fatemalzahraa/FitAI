using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitAl.Core.Entity
{
    public class Magaza
    {
        public int MagazaId { get; set; }

        public string MagazaAdi { get; set; } = null!;

        public string Eposta { get; set; } = null!;

        public string SifreHash { get; set; } = null!;

        public decimal KomisyonOrani { get; set; }

        public decimal? MinimumKomisyonEsigi { get; set; }

        public string? PaketTuru { get; set; }

        public bool AktifMi { get; set; } = true;

        public DateTime OlusturmaTarihi { get; set; }

        public DateTime? SonGirisTarihi { get; set; }

        // Navigation
        public ICollection<PlatformBaglantisi> PlatformBaglantilari { get; set; } = new List<PlatformBaglantisi>();
        public ICollection<Kullanici> Kullanicilar { get; set; } = new List<Kullanici>();
        public ICollection<Urun> Urunler { get; set; } = new List<Urun>();
        public ICollection<AiTalimat> AiTalimatlar { get; set; } = new List<AiTalimat>();
        public ICollection<KomisyonKaydi> KomisyonKayitlari { get; set; } = new List<KomisyonKaydi>();
        public ICollection<Bildirim> Bildirimler { get; set; } = new List<Bildirim>();
        public ICollection<OnboardingAdim> OnboardingAdimlari { get; set; } = new List<OnboardingAdim>();
    }
}
