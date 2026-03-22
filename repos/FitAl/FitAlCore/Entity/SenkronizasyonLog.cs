using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitAl.Core.Entity
{
    public class SenkronizasyonLog
    {
        public int LogId { get; set; }

        public int BaglantiId { get; set; }

        public int MagazaId { get; set; }

        public string SyncTipi { get; set; } = null!;

        public DateTime BaslamaTarihi { get; set; }

        public DateTime? BitisTarihi { get; set; }

        public string Durum { get; set; } = null!;

        public string? HataMesaji { get; set; }

        public int IslemSayisi { get; set; }

        public int EklenenSayisi { get; set; }

        public int GuncellenenSayisi { get; set; }

        // Navigation
        public PlatformBaglantisi PlatformBaglantisi { get; set; } = null!;
        public Magaza Magaza { get; set; } = null!;
    }
}
