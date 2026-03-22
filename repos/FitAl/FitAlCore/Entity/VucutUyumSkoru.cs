using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitAl.Core.Entity
{
    public class VucutUyumSkoru
    {
        public int SkorId { get; set; }

        public int UrunId { get; set; }

        public int MagazaId { get; set; }

        public string VucutTipi { get; set; } = null!;

        public decimal UyumSkoru { get; set; }

        public decimal IadeRiski { get; set; }

        public string? BedenTavsiyesi { get; set; }

        public string SkorKaynagi { get; set; } = null!;

        public string? ModelVersiyon { get; set; }

        public DateTime HesapTarihi { get; set; }

        public bool GecerliMi { get; set; }

        // Navigation
        public Urun Urun { get; set; } = null!;
        public Magaza Magaza { get; set; } = null!;
    }
}
