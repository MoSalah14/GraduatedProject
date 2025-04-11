using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public class Discount :BaseEntity
    {
        public decimal Percentage { get; set; }
        public decimal Number { get; set; }
        public virtual ICollection<DiscountCategory>? DiscountCategories { get; set; }
        public virtual ICollection<ProductSizeDiscount>? ProductSizeDiscounts   { get; set; }
    }
}
