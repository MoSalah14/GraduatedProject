using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.HomeSections
{
    public class HomeSectionForCreationDto
    {
        public string TitleAr { get; set; }
        public string TitleEn { get; set; }
        public IFormFile Image { get; set; }
    }
}
