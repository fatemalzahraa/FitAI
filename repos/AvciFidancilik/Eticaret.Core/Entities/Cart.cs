using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eticaret.Core.Entities
{
    public class Cart : IEntity // Kullanıcının sepetini tutar.
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı ID zorunludur.")]
        [Display(Name = "Kullanıcı ID")]
        public int UserId { get; set; }

        [Display(Name = "Kullanıcı")]
        [ForeignKey("UserId")]
        public AppUser? User { get; set; }

        [Display(Name = "Sepet Öğeleri")]
        public ICollection<CartItem>? CartItems { get; set; }
    }
}