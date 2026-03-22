using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace AvciFidancilik.WebUI.ViewModels
{
    public class ProductImageViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Görsel")]
        [Required(ErrorMessage = "Görsel seçmek zorunludur.")]
        public IFormFile? ImageFile { get; set; }

        [Display(Name = "Mevcut Görsel")]
        public string? ExistingImageUrl { get; set; }

        [Display(Name = "Ana Resim")]
        public bool IsMain { get; set; }

        [Required(ErrorMessage = "Ürün ID zorunludur.")]
        [Display(Name = "Ürün ID")]
        public int ProductId { get; set; }

        [Display(Name = "Ürün Adı")]
        public string? ProductName { get; set; }
    }

    public class ProductImageMultiViewModel
    {
        [Required(ErrorMessage = "Ürün seçmek zorunludur.")]
        [Display(Name = "Ürün")]
        public int ProductId { get; set; }

        [Display(Name = "Ürün Adı")]
        public string? ProductName { get; set; }

        [Display(Name = "Görseller")]
        [Required(ErrorMessage = "En az bir görsel seçmelisiniz.")]
        public List<IFormFile> ImageFiles { get; set; } = new List<IFormFile>();

        [Display(Name = "İlk görseli ana resim yap")]
        public bool SetFirstAsMain { get; set; } = true;
    }
}