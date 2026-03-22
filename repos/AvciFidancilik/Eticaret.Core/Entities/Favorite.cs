using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eticaret.Core.Entities
{
    public class Favorite : IEntity // Kullanıcının favori ürünlerini tutar.
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı ID zorunludur.")]
        [Display(Name = "Kullanıcı ID")]
        public int UserId { get; set; }

        [Display(Name = "Kullanıcı")]
        [ForeignKey("UserId")]
        public AppUser? User { get; set; }

        [Required(ErrorMessage = "Ürün ID zorunludur.")]
        [Display(Name = "Ürün ID")]
        public int ProductId { get; set; }

        [Display(Name = "Ürün")]
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }
    }
}