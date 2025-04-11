using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.Review
{
    public class ProductReviewCount
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int ReviewsCount { get; set; }

        public int RatingAverage { get; set; }
    }
}
