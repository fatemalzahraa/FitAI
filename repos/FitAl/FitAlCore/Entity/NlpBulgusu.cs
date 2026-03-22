using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitAl.Core.Entity
{
    public class NlpBulgusu
    {
        public int BulguId { get; set; }

        public int UrunId { get; set; }

        public int MagazaId { get; set; }

        public string Tema { get; set; } = null!;

        public int TekrarSayisi { get; set; }

        public decimal? DuyguSkoru { get; set; }

        public string? DuyguEtiketi { get; set; }

        public string? OneriMetni { get; set; }

        public string PlatformOnayDurumu { get; set; } = "Bekliyor";

        public string Durum { get; set; } = "Açık";

        public DateTime OlusturmaTarihi { get; set; }

        public DateTime? CozulmeTarihi { get; set; }

        // Navigation
        public Urun Urun { get; set; } = null!;
        public Magaza Magaza { get; set; } = null!;
    }
}
