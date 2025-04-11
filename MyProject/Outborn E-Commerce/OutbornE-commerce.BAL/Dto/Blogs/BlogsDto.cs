using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.Blogs
{
    public class BlogsDto
    {
        public Guid Id { get; set; }
        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        public string ContentEn { get; set; }
        public string ContentAr { get; set; }
        public string Image1Url { get; set; }
        public string Image2Url { get; set; }
        public string ShortDescriptionEn { get; set; }
        public string ShortDescriptionAr { get; set; }
        public Guid BlogCategoryId { get; set; }
    }
}
