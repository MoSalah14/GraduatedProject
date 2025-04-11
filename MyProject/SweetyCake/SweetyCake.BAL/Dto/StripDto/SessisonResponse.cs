namespace Infrastructure.Services.PaymentWithStripeService.Models
{
    public class SessisonResponse
    {
        public string SessionId { get; set; }
        public string SessionUrl { get; set; }
        public Guid OrderId { get; set; }
    }
}