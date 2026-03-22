using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitAl.Core.Entity
{
    public class Kullanici
    {
        public int KullaniciId { get; set; }

        public int MagazaId { get; set; }

        public string Ad { get; set; } = null!;

        public string Eposta { get; set; } = null!;

        public string SifreHash { get; set; } = null!;

        public string Rol { get; set; } = null!;

        public bool AktifMi { get; set; } = true;

        public DateTime? SonGirisTarihi { get; set; }

        public DateTime OlusturmaTarihi { get; set; }

        // Navigation
        public Magaza Magaza { get; set; } = null!;
        public ICollection<AiTalimat> AiTalimatlar { get; set; } = new List<AiTalimat>();
    }
}
