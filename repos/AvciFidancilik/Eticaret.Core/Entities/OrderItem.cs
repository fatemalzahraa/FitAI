using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eticaret.Core.Entities
{
    public class OrderItem : IEntity // Siparişte hangi ürünlerden kaç adet olduğu ve fiyat bilgisi.
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Sipariş ID zorunludur.")]
        [Display(Name = "Sipariş ID")]
        public int OrderId { get; set; }

        [Display(Name = "Sipariş")]
        [ForeignKey("OrderId")]
        public Order? Order { get; set; }

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

        [Required(ErrorMessage = "Birim fiyat zorunludur.")]
        [Display(Name = "Birim Fiyat")]
        [DataType(DataType.Currency)]
        [Range(0.01, double.MaxValue, ErrorMessage = "Geçerli bir fiyat giriniz.")]
        public decimal UnitPrice { get; set; }
    }
}