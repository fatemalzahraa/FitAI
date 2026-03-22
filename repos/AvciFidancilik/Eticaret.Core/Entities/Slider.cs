using System.ComponentModel.DataAnnotations;

namespace Eticaret.Core.Entities
{
    public class Slider : IEntity // Ana sayfada gösterilecek sliderları tutar
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Slider görseli zorunludur.")]
        [Display(Name = "Slider Görseli")]
        [StringLength(500, ErrorMessage = "Görsel URL'si en fazla 500 karakter olabilir.")]
        public string ImageUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "Başlık zorunludur.")]
        [Display(Name = "Başlık")]
        [StringLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir.")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Alt Başlık")]
        [StringLength(300, ErrorMessage = "Alt başlık en fazla 300 karakter olabilir.")]
        public string SubTitle { get; set; } = string.Empty;

        [Display(Name = "Aktif")]
        public bool IsActive { get; set; } = true;
    }
}