using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eticaret.Core.Entities
{
    // Kullanıcılar ve roller arasındaki Many-to-Many ilişkiyi tutar
    public class UserRole : IEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı ID zorunludur.")]
        [Display(Name = "Kullanıcı ID")]
        public int UserId { get; set; }

        [Display(Name = "Kullanıcı")]
        [ForeignKey(nameof(UserId))]
        public AppUser? User { get; set; }

        [Required(ErrorMessage = "Rol ID zorunludur.")]
        [Display(Name = "Rol ID")]
        public int RoleId { get; set; }

        [Display(Name = "Rol")]
        [ForeignKey("RoleId")]
        public Role? Role { get; set; }
    }
}