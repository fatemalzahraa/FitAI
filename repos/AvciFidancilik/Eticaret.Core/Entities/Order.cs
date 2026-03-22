
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eticaret.Core.Entities
{
    public class Order : IEntity // Kullanıcının verdiği sipariş bilgileri ve ürün detayları ilişkilendirilecek
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı ID zorunludur.")]
        [Display(Name = "Kullanıcı ID")]
        public int UserId { get; set; }

        [Display(Name = "Kullanıcı")]
        [ForeignKey("UserId")]
        public AppUser? User { get; set; }

        [Required(ErrorMessage = "Sipariş tarihi zorunludur.")]
        [Display(Name = "Sipariş Tarihi")]
        [DataType(DataType.DateTime)]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        [StringLength(50, ErrorMessage = "Durum en fazla 50 karakter olabilir.")]
        [Display(Name = "Sipariş Öğeleri")]
        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}