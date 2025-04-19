
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.Products
{
    public class SearchedProductDto
    {
        public Guid Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public decimal? Price { get; set; }
        public int Label {  get; set; }
        public Guid BrandId { get; set; }
    }
}
