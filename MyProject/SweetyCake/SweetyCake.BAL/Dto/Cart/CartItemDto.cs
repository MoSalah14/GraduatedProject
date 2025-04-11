using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.Cart
{
    public class CartItemDto
    {
        public Guid ProductSizeId { get; set; }
        public int Quantity { get; set; }
        public decimal? ItemPrice { get; set; }
    }
}