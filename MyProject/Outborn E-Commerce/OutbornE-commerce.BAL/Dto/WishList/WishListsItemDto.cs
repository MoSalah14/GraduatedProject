using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.WishList
{
    public class WishListsItemDto
    {
        public Guid ProductId { get; set; }
        public string ProductNameEn { get; set; }
        public string ProductNameAr { get; set; }
        public string ImageUrl { get; set; }
        public string BrandNameEn { get; set; }
        public string BrandNameAr { get; set; }
        public decimal? ItemPrice { get; set; }
    }
}
