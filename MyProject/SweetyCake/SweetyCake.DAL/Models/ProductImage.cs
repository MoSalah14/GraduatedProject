namespace OutbornE_commerce.DAL.Models
{
    public class ProductImage
    {
        public Guid ID { get; set; }
        public string ImageUrl { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
    }
}