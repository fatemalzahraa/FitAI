using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitAl.Core.Entity
{
    public class KullaniciProfil
    {
        public int ProfilId { get; set; }

        public string CihazId { get; set; } = null!;

        public string? VucutTipi { get; set; }

        public decimal? Boy { get; set; }

        public decimal? Kilo { get; set; }

        public string? TercihleriJSON { get; set; }

        public DateTime OlusturmaTarihi { get; set; }

        public DateTime GuncellenmeTarihi { get; set; }

        // Navigation
        public ICollection<WidgetSorguLog> WidgetSorgulari { get; set; } = new List<WidgetSorguLog>();
    }
}
