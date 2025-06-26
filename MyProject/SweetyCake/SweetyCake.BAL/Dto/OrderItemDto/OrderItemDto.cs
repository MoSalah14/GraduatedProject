
namespace OutbornE_commerce.BAL.Dto
{
    public class OrderItemDto
    {
        public Guid OrderId { get; set; }
        public string ProductNameEn { get; set; }
        public string ProductNameAr { get; set; }
        public int Quantity { get; set; }
        public decimal ItemPrice { get; set; }

    }
}