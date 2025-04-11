using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.NavTitle
{
    public sealed class NavTitleDto
    {
        public Guid? Id { get; set; }
        public string TitleAr { get; set; }
        public string TitleEn { get; set; }
        public string pageUrl { get; set; }
    }
}
