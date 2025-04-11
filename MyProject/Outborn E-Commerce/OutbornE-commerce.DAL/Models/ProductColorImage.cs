namespace OutbornE_commerce.DAL.Models
{
    public class ProductColorImage : BaseEntity
    {
        public string ImageUrl { get; set; }
        public Guid ProductColorId { get; set; }
        public bool IsDefault { get; set; } = false;
        public ProductColor Product_Color { get; set; }
    }
}