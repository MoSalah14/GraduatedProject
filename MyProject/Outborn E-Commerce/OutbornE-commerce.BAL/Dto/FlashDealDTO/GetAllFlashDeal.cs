using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.FlashDealDTO
{
    public class GetAllFlashDeal
    {
        public Guid Id { get; set; }
        public string TitleEn { get; set; } 
        public string TitleAr { get; set; }  
        public string ImageUrl { get; set; }
        public bool IsActive { get; set; } 
        public string PageLink { get; set; }  

        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; }  

        public string DescriptionEn { get; set; }  
        public string DescriptionAr { get; set; } 

    
    }
}
