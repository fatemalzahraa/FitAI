using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eticaret.Core.Entities
{
    public class Brand : IEntity
    {
        public int Id { get; set; } // Primary key
        public string Name { get; set; } = string.Empty; // Marka adı
        public ICollection<Product>? Products { get; set; } // Bu markaya ait ürünler
    }

}
