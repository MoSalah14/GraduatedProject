using Stripe.Billing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public class FlashDeal :BaseEntity
    {
        public string TitleEn { get; set; }
        public string TitleAr { get; set; } 
        public bool IsActive { get; set; } =false;

        public string ImageUrl { get; set; } 

        public string PageLink { get; set; } 

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; } 

        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; } 
    }
}
