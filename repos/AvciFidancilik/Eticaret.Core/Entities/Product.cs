using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eticaret.Core.Entities
{
    public class Product : IEntity // Ürünün tüm bilgileri burada tutulacak
                                   // Navigasyon özellikleri sayesinde sipariş, sepet, favori ve görseller ilişkilendirilecek
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ürün adı zorunludur.")]
        [Display(Name = "Ürün Adı")]
        [StringLength(200, ErrorMessage = "Ürün adı en fazla 200 karakter olabilir.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ürün açıklaması zorunludur.")]
        [Display(Name = "Açıklama")]
        [StringLength(2000, ErrorMessage = "Açıklama en fazla 2000 karakter olabilir.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Fiyat zorunludur.")]
        [Display(Name = "Fiyat")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Geçerli bir fiyat giriniz.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stok adedi zorunludur.")]
        [Display(Name = "Stok")]
        [Range(0, int.MaxValue, ErrorMessage = "Stok adedi 0 veya daha büyük olmalıdır.")]
        public int Stock { get; set; }

        [Display(Name = "Ana Görsel")]
        [StringLength(500, ErrorMessage = "Görsel URL'si en fazla 500 karakter olabilir.")]
        public string ImageUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kategori ID zorunludur.")]
        [Display(Name = "Kategori ID")]
        public int CategoryId { get; set; }

        [Display(Name = "Kategori")]
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        [Display(Name = "Sipariş Öğeleri")]
        public ICollection<OrderItem>? OrderItems { get; set; }

        [Display(Name = "Sepet Öğeleri")]
        public ICollection<CartItem>? CartItems { get; set; }

        [Display(Name = "Favoriler")]
        public ICollection<Favorite>? Favorites { get; set; }

        [Display(Name = "Ürün Görselleri")]
        public ICollection<ProductImage>? ProductImages { get; set; }
    }
}