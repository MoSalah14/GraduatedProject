using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.HomeSections
{
    public class HomeSectionDto
    {
        public Guid Id { get; set; }
        public string TitleAr { get; set; }
        public string TitleEn { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public string Link { get; set; }
        public string ImageUrl { get; set; }
        public IFormFile? Image { get; set; }
    }
}
