using Microsoft.AspNetCore.Http;
using OutbornE_commerce.BAL.Dto.ProductSizes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.ProductColors
{
    public class ProductColorForCreateDto
    {
        public Guid ProductId { get; set; }

        public Guid ColorId { get; set; }
        public List<ProductPhotosUrl>? ProductPhotos { get; set; }
        //public List<PhotosUrl>? PhotesUrl { get; set; }

        public List<CreateProductSizeDto> ProductSizes { get; set; } = new List<CreateProductSizeDto>();

        public bool IsValid()
        {
            return ProductPhotos.Count <= 10;
        }
    }

    public class ProductPhotosUrl
    {
        public IFormFile Photo { get; set; }
        public bool IsDefault { get; set; } = false;
    }
}