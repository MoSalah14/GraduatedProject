using OutbornE_commerce.DAL.Models;

namespace OutbornE_commerce.BAL.Dto.Products
{
    public class ProductNameIdModel
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; }

        public ICollection<Reviews> Review { get; set; }
    }
}