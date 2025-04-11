using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public class Header : BaseEntity
    {
        public string Title1Ar { get; set; }
        public string Title1En { get; set; }
        public string Title2Ar { get; set; }
        public string Title2En { get; set; }
        public string ImageUrl { get; set; }

    }
}
