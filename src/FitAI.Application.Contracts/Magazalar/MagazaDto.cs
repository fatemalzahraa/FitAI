using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace FitAI.Magazalar
{
    /// <summary>
    /// Mağaza veri transfer objesi (response)
    /// Id alanı int olduğu için EntityDto<int> kullanıyoruz
    /// </summary>
    public class MagazaDto : EntityDto<int>
    {
        /// <summary>Mağaza adı (gerekli)</summary>
        [Required(ErrorMessage = "Mağaza adı gereklidir")]
        [StringLength(200, MinimumLength = 3)]
        public required string MagazaAdi { get; set; }

        /// <summary>Mağaza yöneticisinin e-posta (gerekli)</summary>
        [Required(ErrorMessage = "E-posta adresi gereklidir")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin")]
        public required string Eposta { get; set; }

        /// <summary>Komisyon oranı (%)</summary>
        [Range(0, 100, ErrorMessage = "Komisyon oranı 0-100 arasında olmalıdır")]
        public decimal KomisyonOrani { get; set; }

        /// <summary>Mağaza aktif durumu</summary>
        public bool AktifMi { get; set; }

        /// <summary>Paket türü (Baslangic, Standart, Premium)</summary>
        [Required(ErrorMessage = "Paket türü gereklidir")]
        [StringLength(50)]
        public required string PaketTuru { get; set; }

        /// <summary>Oluşturma tarihi</summary>
        public DateTime CreationTime { get; set; }

        /// <summary>Son güncelleme tarihi</summary>
        public DateTime? LastModificationTime { get; set; }
    }

    /// <summary>
    /// Mağaza oluşturma ve güncelleme DTO
    /// </summary>
    public class CreateUpdateMagazaDto
    {
        /// <summary>Mağaza adı (gerekli, 3-200 karakter)</summary>
        [Required(ErrorMessage = "Mağaza adı gereklidir")]
        [StringLength(200, MinimumLength = 3,
            ErrorMessage = "Mağaza adı 3 ile 200 karakter arasında olmalıdır")]
        public required string MagazaAdi { get; set; }

        /// <summary>E-posta adresi (gerekli, unique)</summary>
        [Required(ErrorMessage = "E-posta adresi gereklidir")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin")]
        [StringLength(256)]
        public required string Eposta { get; set; }

        /// <summary>Şifre (gerekli, minimum 6 karakter)</summary>
        [Required(ErrorMessage = "Şifre gereklidir")]
        [StringLength(256, MinimumLength = 6,
            ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
        [DataType(DataType.Password)]
        public required string SifreHash { get; set; }

        /// <summary>Komisyon oranı (0-100)</summary>
        [Range(0, 100, ErrorMessage = "Komisyon oranı 0-100 arasında olmalıdır")]
        public decimal KomisyonOrani { get; set; } = 10m;

        /// <summary>Minimum komisyon eşiği (opsiyonel)</summary>
        [Range(0, double.MaxValue, ErrorMessage = "Minimum komisyon eşiği negatif olamaz")]
        public decimal? MinimumKomisyonEsigi { get; set; }

        /// <summary>Paket türü (gerekli: Baslangic, Standart, Premium)</summary>
        [Required(ErrorMessage = "Paket türü gereklidir")]
        [StringLength(50)]
        [RegularExpression("^(Baslangic|Standart|Premium)$",
            ErrorMessage = "Geçersiz paket türü. Seçenekler: Baslangic, Standart, Premium")]
        public required string PaketTuru { get; set; }

        /// <summary>Mağaza aktif durumu</summary>
        public bool AktifMi { get; set; } = true;
    }
}