using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitAl.Core.Entity
{
    public class Bildirim
    {
        public int BildirimId { get; set; }

        public int MagazaId { get; set; }

        public int? KullaniciId { get; set; }

        public string Tip { get; set; } = null!;

        public string Baslik { get; set; } = null!;

        public string Icerik { get; set; } = null!;

        public int? IlgiliKayitId { get; set; }

        public string? IlgiliKayitTipi { get; set; }

        public string Kanal { get; set; } = null!;

        public bool OkunduMu { get; set; }

        public DateTime GonderimTarihi { get; set; }

        public DateTime? OkunmaTarihi { get; set; }

        // Navigation
        public Magaza Magaza { get; set; } = null!;
        public Kullanici? Kullanici { get; set; }
    }
}
