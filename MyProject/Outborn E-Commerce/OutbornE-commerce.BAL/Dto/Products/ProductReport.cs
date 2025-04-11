using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.Products
{
    public class ProductReport
    {

        public Guid Id { get; set; }
        public string ProductNameEnglish { get; set; } 
        public string ProductNameArabic { get; set; } 
        public int TotalQuantity { get; set; }
    }
}
