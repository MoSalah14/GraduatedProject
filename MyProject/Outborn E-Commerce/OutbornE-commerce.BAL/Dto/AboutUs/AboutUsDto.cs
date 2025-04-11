using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.AboutUs
{
    public class AboutUsDto
    {
        public Guid Id { get; set; }
        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        public string Description1En { get; set; }
        public string Description1Ar { get; set; }
        public string ImageUrl { get; set; }
        public IFormFile? Image { get; set; }
    }
}
