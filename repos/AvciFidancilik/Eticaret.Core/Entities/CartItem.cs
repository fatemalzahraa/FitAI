using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eticaret.Core.Entities
{
    public class CartItem : IEntity // Sepetteki her ürün ve adeti burada tutulur.
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Sepet ID zorunludur.")]
        [Display(Name = "Sepet ID")]
        public int CartId { get; set; }

        [Display(Name = "Sepet")]
        [ForeignKey("CartId")]
        public Cart? Cart { get; set; }

        [Required(ErrorMessage = "Ürün ID zorunludur.")]
        [Display(Name = "Ürün ID")]
        public int ProductId { get; set; }

        [Display(Name = "Ürün")]
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        [Required(ErrorMessage = "Adet zorunludur.")]
        [Display(Name = "Adet")]
        [Range(1, int.MaxValue, ErrorMessage = "Adet en az 1 olmalıdır.")]
        public int Quantity { get; set; }
    }
}