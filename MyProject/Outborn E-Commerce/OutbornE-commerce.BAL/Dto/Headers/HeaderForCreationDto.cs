using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.Headers
{
    public class HeaderForCreationDto
    {
        public string Title1Ar { get; set; }
        public string Title1En { get; set; }
        public string Title2Ar { get; set; }
        public string Title2En { get; set; }
        public IFormFile Image { get; set; }
    }
}
