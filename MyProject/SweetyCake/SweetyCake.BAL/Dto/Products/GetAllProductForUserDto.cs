using OutbornE_commerce.BAL.Dto.Colors;
using OutbornE_commerce.BAL.Dto.ProductSizes;
using OutbornE_commerce.BAL.Dto.Sizes;
using OutbornE_commerce.DAL.Enums;

namespace OutbornE_commerce.BAL.Dto.Products
{
    public class GetAllProductForUserDto
    {
        public Guid? Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string MainImageUrl { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int RatingAverage { get; set; }

        public DateTime? CreatedOn { get; set; }
    }

    public class GetAllProductForUserDtoًWithCategory : GetAllProductForUserDto
    {
        public Guid? CategoryID { get; set; }
        public string CategoryNameEn { get; set; }
        public string CategoryNameAr { get; set; }
    }
}