using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public class ProductColor : BaseEntity
    {
        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; }
        public Guid ColorId { get; set; }
        public virtual Color Color { get; set; }
        public virtual List<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();
        public ICollection<ProductColorImage> ProductColorImages { get; set; }
    }
}