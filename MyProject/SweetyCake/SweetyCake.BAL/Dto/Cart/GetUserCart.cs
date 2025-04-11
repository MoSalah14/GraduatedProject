using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.Cart
{
    public class GetUserCart
    {
        public Guid ProductId { get; set; }
        public Guid ProductSizeId { get; set; }
        public string ProductNameEn { get; set; }
        public string ProductNameAr { get; set; }
        public string ProductImage { get; set; }
        public string ProductCode { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Product_Weight { get; set; }
        public bool IsOutOfStock { get; set; }
    }
}