
using OutbornE_commerce.BAL.Dto.Review;

namespace OutbornE_commerce.BAL.Dto.Products
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string AboutEn { get; set; }
        public string AboutAr { get; set; }
        public string MaterialEn { get; set; }
        public string MaterialAr { get; set; }
        public bool IsActive { get; set; }
        public bool IsPreOrder { get; set; }
        public int QuantityInStock { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPrice { get; set; }
        public string MainImageUrl { get; set; }
        public string CategoryNameEn { get; set; }
        public string CategoryNameAr { get; set; }
        public List<string> ProductImage { get; set; }
        public string? ProductCode { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public List<ReviewDataDto> Reviews { get; set; }

    }
}