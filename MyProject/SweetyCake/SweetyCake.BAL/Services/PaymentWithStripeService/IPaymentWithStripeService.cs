using Infrastructure.Services.PaymentWithStripeService.Models;
using OutbornE_commerce.BAL.Dto.OrderDto;
using Stripe;
using Stripe.Checkout;

namespace Infrastructure.Services.PaymentWithStripeService
{
    public interface IPaymentWithStripeService
    {
        Task<SessisonResponse> CreateCheckoutSession(string UserId, long productPrice, long deliveryPrice, string description, Guid orderId);

        Task<Session> GetCheckoutSession(string sessionId);

        Task<Session> GetSessionByPaymentIntentAsync(string paymentIntentId);
    }
}