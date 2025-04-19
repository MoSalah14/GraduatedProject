using Microsoft.AspNetCore.Http;

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
