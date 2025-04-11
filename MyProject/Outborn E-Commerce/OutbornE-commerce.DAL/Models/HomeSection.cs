using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public class HomeSection : BaseEntity
    {
        public string TitleAr {  get; set; }
        public string TitleEn { get; set; }
        public string DescriptionAr {  get; set; }
        public string DescriptionEn { get; set; }
        public string Link {  get; set; }   
        public string ImageUrl { get; set; }
    }
}
