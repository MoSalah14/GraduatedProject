using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public class AboutUs : BaseEntity
    {
        public string TitleEn {  get; set; }
        public string TitleAr {  get; set; }
        public string Description1En { get; set; }
        public string Description1Ar {  get; set; }
        public string ImageUrl { get; set; }

    }
}
