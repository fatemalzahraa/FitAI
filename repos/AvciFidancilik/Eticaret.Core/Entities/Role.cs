using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Eticaret.Core.Entities
{
    public class Role : IEntity // Projede birden fazla olan rol yönetimini kolaylaştıracak
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Rol adı zorunludur.")]
        [Display(Name = "Rol Adı")]
        [StringLength(50, ErrorMessage = "Rol adı en fazla 50 karakter olabilir.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Kullanıcı Rolleri")]
        public ICollection<UserRole>? UserRoles { get; set; }
    }
}