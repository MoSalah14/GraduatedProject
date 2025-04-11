using OutbornE_commerce.BAL.Dto.Sizes;
using OutbornE_commerce.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.ProductSizes
{
    public class ProductSizeDto
    {
        public Guid SizeId { get; set; }
        public Guid? ActualSizeId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal DiscountedPrice { get; set; }
        public string SKU_Size { get; set; }
        public decimal ProductWeight { get; set; }
    }
}