using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eticaret.Core.Entities
{
    public class ProductImage : IEntity // Ürünler için birden fazla görsel eklenebilecek
    {
        public int Id { get; set; }

        
        [Display(Name = "Görsel URL")]
        [StringLength(500, ErrorMessage = "Görsel URL'si en fazla 500 karakter olabilir.")]
        public string ImageUrl { get; set; } = string.Empty;

        [Display(Name = "Ana Resim")]
        public bool IsMain { get; set; } = false;

        [Required(ErrorMessage = "Ürün ID zorunludur.")]
        [Display(Name = "Ürün ID")]
        public int ProductId { get; set; }

        [Display(Name = "Ürün")]
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }
    }
}