using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public class HomeSection
    {
        public Guid Id { get; set; }
        [MaxLength(DBRules.ShortCaptionLenght)]
        public string TitleAr {  get; set; }
        [MaxLength(DBRules.ShortCaptionLenght)]
        public string TitleEn { get; set; }
        public string ImageUrl { get; set; }
    }
}
