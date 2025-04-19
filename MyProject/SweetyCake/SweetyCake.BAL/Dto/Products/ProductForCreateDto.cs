using Microsoft.AspNetCore.Http;
namespace OutbornE_commerce.BAL.Dto.Products
{
    public class ProductForCreateDto
    {
        public Guid Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }

        public string AboutEn { get; set; }
        public string AboutAr { get; set; }
        public string MaterialEn { get; set; }
        public string MaterialAr { get; set; }
        public int NumberOfPieceAvailable { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPrice { get; set; }

        public bool IsPreOrder { get; set; } = false;

        public bool IsActive { get; set; } = true;
        public string MainImageUrl { get; set; }
        public Guid CategoryId { get; set; }
        public List<IFormFile> ProductImages { get; set; }
    }
}