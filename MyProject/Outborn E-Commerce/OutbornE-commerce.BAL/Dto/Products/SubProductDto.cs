using Microsoft.AspNetCore.Http;
using OutbornE_commerce.BAL.Dto.Colors;
using OutbornE_commerce.BAL.Dto.Image;
using OutbornE_commerce.BAL.Dto.Sizes;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.Products
{
    public class SubProductDto
    {

     

        public Guid SizeId { get; set; }

        public Guid ColorId { get; set; }

        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? ImageUrl { get; set; }
        public IFormFile? Photo { get; set; }

    }
}
