using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitAl.Core.Entity
{
    public class OnboardingAdim
    {
        public int AdimId { get; set; }

        public int MagazaId { get; set; }

        public int SiraNo { get; set; }

        public string AdimKodu { get; set; } = null!;

        public string AdimAdi { get; set; } = null!;

        public bool Tamamlandi { get; set; }

        public DateTime? TamamlanmaTarihi { get; set; }

        public int? GecenSureSaniye { get; set; }

        // Navigation
        public Magaza Magaza { get; set; } = null!;
    }
}
