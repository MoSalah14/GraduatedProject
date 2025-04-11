using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.Brands
{
    public class BrandDto
    {
        public Guid? Id { get; set; }
        public string? NameEn { get; set; }
        public string? NameAr { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageBanerUrl { get; set; }
        public string? BrandSizeChartUrl { get; set; }
        public bool? IsFeatured { get; set; }

        public int? BrandNumber { get; set; } = default;

        public IFormFile? ImageHome { get; set; }
        public IFormFile? ImageBaner { get; set; }
        public IFormFile? BrandSizeChart { get; set; }
    }
}