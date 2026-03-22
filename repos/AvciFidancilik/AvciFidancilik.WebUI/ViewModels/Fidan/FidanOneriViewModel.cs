using Eticaret.Data.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Eticaret.WebUI.ViewModels.Fidan
{
    public class FidanOneriViewModel
    {
        // 🔹 FORM ALANLARI (kullanıcı / üretici girer)

        [Display(Name = "İklim Tipi")]
        public string? IklimTipi { get; set; }

        [Display(Name = "Toprak Tipi")]
        public string? ToprakTipi { get; set; }

        [Display(Name = "Güneş İhtiyacı")]
        public string? GunesIhtiyaci { get; set; }

        [Display(Name = "Minimum Sıcaklık")]
        public int? MinSicaklik { get; set; }

        [Display(Name = "Maksimum Sıcaklık")]
        public int? MaxSicaklik { get; set; }

        [Display(Name = "Meyve Versin mi?")]
        public bool? MeyveVerir { get; set; }

        // 🔹 ASİSTAN SONUCU (dinamik)
      
    }
}
