using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.ProductImages
{
    public class ProductImageDto
    {
        public Guid? Id { get; set; }
        public string ImageUrl { get; set; }
        
    }
}
