using Microsoft.AspNetCore.Http;
using OutbornE_commerce.BAL.Dto.Colors;
using OutbornE_commerce.BAL.Dto.ProductSizes;

namespace OutbornE_commerce.BAL.Dto.ProductColors
{
    public class ProductColorDto
    {
        public Guid ColorId { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string Hexadecimal { get; set; }

        public List<ProductColorImagesDto> ProductColorImages { get; set; }
        public List<ProductSizeDto> ProductSizes { get; set; }
    }

    public class ProductColorImagesDto
    {
        public string ImageUrl { get; set; }
        public Guid ProductColorId { get; set; }
        public bool IsDefault { get; set; } = false;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedOn { get; set; }
    }
}