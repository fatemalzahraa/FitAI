using System.ComponentModel.DataAnnotations;

namespace Eticaret.Core.Entities
{
    public class Category : IEntity // ürünleri kategoriye ayırmak için kullanılır.
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori adı zorunludur.")]
        [Display(Name = "Kategori Adı")]
        [StringLength(100, ErrorMessage = "Kategori adı en fazla 100 karakter olabilir.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Ürünler")]
        public ICollection<Product>? Products { get; set; }
    }
}