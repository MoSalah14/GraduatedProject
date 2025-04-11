using OutbornE_commerce.BAL.Dto.Colors;
using OutbornE_commerce.BAL.Dto.ProductSizes;
using OutbornE_commerce.BAL.Dto.Sizes;
using OutbornE_commerce.DAL.Enums;

namespace OutbornE_commerce.BAL.Dto.Products
{
    public class GetAllProductForUserDto
    {
        public Guid? Id { get; set; }
        public string? NameEn { get; set; }
        public string? NameAr { get; set; }
        public List<string>? ImageUrl { get; set; }
        public Guid? BrandID { get; set; }
        public string? BrandNameEn { get; set; }
        public string? BrandNameAr { get; set; }
        public ProductLabelEnum? Label { get; set; }
        public List<ColorDto>? ProductColors { get; set; }
        public List<SizeDto>? ProductSizes { get; set; }
        public decimal? Price { get; set; }
        public decimal? DiscountPrice { get; set; }

        public DateTime? CreatedOn { get; set; }
    }

    public class GetAllProductForUserDtoًWithCategory : GetAllProductForUserDto
    {
        public Guid? CategoryID { get; set; }
        public string? CategoryNameEn { get; set; }
        public string? CategoryNameAr { get; set; }
    }
}