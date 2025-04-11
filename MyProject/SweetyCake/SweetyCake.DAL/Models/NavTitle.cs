using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public sealed class NavTitle :BaseEntity
    {
        public string TitleAr { get; set; }
        public string TitleEn { get; set; }
        public string pageUrl { get; set; }
    }
}
