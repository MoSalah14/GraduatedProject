using Infrastructure.Services.PaymentWithStripeService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using OutbornE_commerce.DAL.Data;
using OutbornE_commerce.DAL.Models;
using OutbornE_commerce.Extensions;
using Stripe;
using Stripe.Checkout;
using Microsoft.Extensions.Hosting;
using OutbornE_commerce.BAL.Extentions;

namespace Infrastructure.Services.PaymentWithStripeService
{
    public class PaymentWithStripeService : IPaymentWithStripeService
    {
        private readonly IConfiguration configuration;
        private readonly UserManager<User> _userManager;
        protected readonly ApplicationDbContext _context;
        private readonly FrontBaseUrlSettings FrontBaseUrl;
        private readonly IHostEnvironment env;
        private readonly StripeSettings _stripeSettings;

        public PaymentWithStripeService(IConfiguration configuration, UserManager<User> userManager,
            IOptions<StripeSettings> stripeSettings, ApplicationDbContext context,
            IOptions<FrontBaseUrlSettings> option, IHostEnvironment _env)
        {
            this.configuration = configuration;
            _userManager = userManager;
            _stripeSettings = stripeSettings.Value;
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
            _context = context;
            FrontBaseUrl = option.Value;
            env = _env;
        }

        public async Task<SessisonResponse> CreateCheckoutSession(string UserId, long productPrice, long deliveryPrice, string description, Guid orderId)
        {
            var baseUrl = env.IsDevelopment() ? FrontBaseUrl.Local : FrontBaseUrl.Production;

            StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];

            var userWithCurrency = await _userManager.FindByIdAsync(UserId);

            var customerOptions = new CustomerCreateOptions
            {
                Name = userWithCurrency!.FullName,
                Email = userWithCurrency.Email,
            };
            var customerService = new CustomerService();
            var customer = await customerService.CreateAsync(customerOptions);

            var customerId = customer.Id;

            var options = new SessionCreateOptions
            {
                Customer = customerId,
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
            PriceData = new SessionLineItemPriceDataOptions
            {
                Currency = "EGP",
                ProductData = new SessionLineItemPriceDataProductDataOptions
                {
                    Name = description,
                },
                UnitAmount = productPrice * 100,
            },
            Quantity = 1,
             },
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "EGP",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Delivery Charge",
                            },
                            UnitAmount = deliveryPrice * 100,
                        },
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                SuccessUrl = $"{baseUrl}en/cart/check-out/accepted",
                CancelUrl = $"{baseUrl}en/cart/check-out/rejected",
                Metadata = new Dictionary<string, string>
                {
                    { "OrderId", orderId.ToString() },
                    { "UserId", UserId.ToString() }
                }
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);
            return new SessisonResponse
            {
                SessionId = session.Id,
                SessionUrl = session.Url
            };
        }

        public async Task<Session> GetCheckoutSession(string sessionId)
        {
            StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];
            var service = new SessionService();
            var resopnse = await service.GetAsync(sessionId);
            return resopnse;
        }

        public async Task<Session> GetSessionByPaymentIntentAsync(string paymentIntentId)
        {
            var sessionService = new SessionService();

            var sessionListOptions = new SessionListOptions
            {
                PaymentIntent = paymentIntentId
            };

            var sessions = await sessionService.ListAsync(sessionListOptions);

            return sessions.FirstOrDefault();
        }
    }
}