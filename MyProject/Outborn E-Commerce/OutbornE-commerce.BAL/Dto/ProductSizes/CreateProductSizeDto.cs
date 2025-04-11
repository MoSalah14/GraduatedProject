using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.ProductSizes
{
    public class CreateProductSizeDto
    {
        public Guid SizeId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string SKU_Size { get; set; }
        [Range(0.01, 10, ErrorMessage = "ProductWeight must be between 0.01 and 10.")]
        public decimal ProductWeight { get; set; }
    }
}