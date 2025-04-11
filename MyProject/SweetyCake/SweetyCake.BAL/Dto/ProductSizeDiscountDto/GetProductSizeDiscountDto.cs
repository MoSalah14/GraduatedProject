using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.ProductSizeDiscountDto
{
    public class GetProductSizeDiscountDto
    {
        public Guid Id { get; set; }
        public Guid ProductSizeId { get; set; }
        public Guid DiscountId { get; set; }
        public string ProductNameArabic { get; set; }
        public string ProductNameEndlish { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal Number { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; } = DateTime.MinValue;
    }
}
