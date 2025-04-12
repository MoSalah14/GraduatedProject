using Infrastructure.Services.PaymentWithStripeService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using OutbornE_commerce.DAL.Data;
using OutbornE_commerce.DAL.Models;
using OutbornE_commerce;

using OutbornE_commerce.Extensions;
using Stripe;
using Stripe.Checkout;
using OutbornE_commerce.BAL.Repositories.OrderRepo;
using OutbornE_commerce.DAL.Enums;
using Microsoft.AspNetCore.Mvc;
using OutbornE_commerce.BAL.Services.DeliveryService;
using OutbornE_commerce.BAL.Dto.Delivery;
using Microsoft.Extensions.Hosting;
using OutbornE_commerce.BAL.Extentions;
using OutbornE_commerce.BAL.Repositories.Currencies;
using OutbornE_commerce.BAL.Dto.Delivery.DeliveryOrders;
using OutbornE_commerce.BAL.Dto.Address;
using OutbornE_commerce.BAL.Dto.OrderDto;
using OutbornE_commerce.BAL.Dto.OrderItemDto;

namespace Infrastructure.Services.PaymentWithStripeService
{
    public class PaymentWithStripeService : IPaymentWithStripeService
    {
        private readonly IConfiguration configuration;
        private readonly UserManager<User> _userManager;
        protected readonly ApplicationDbContext _context;
        private readonly IOrderRepository orderRepository;
        private readonly DeliveryService deliveryService;
        private readonly FrontBaseUrlSettings FrontBaseUrl;
        private readonly IHostEnvironment env;
        private readonly ICurrencyRepository currencyRepository;
        private readonly StripeSettings _stripeSettings;

        public PaymentWithStripeService(IConfiguration configuration, UserManager<User> userManager,
            IOptions<StripeSettings> stripeSettings, ApplicationDbContext context,
            IOrderRepository orderRepository, DeliveryService deliveryService,
            IOptions<FrontBaseUrlSettings> option, IHostEnvironment _env, ICurrencyRepository currencyRepository)
        {
            this.configuration = configuration;
            _userManager = userManager;
            _stripeSettings = stripeSettings.Value;
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
            _context = context;
            this.orderRepository = orderRepository;
            this.deliveryService = deliveryService;
            FrontBaseUrl = option.Value;
            env = _env;
            this.currencyRepository = currencyRepository;
        }

        public async Task<SessisonResponse> CreateCheckoutSession(string UserId, long productPrice, long deliveryPrice, string description, Guid orderId)
        {
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
                    UnitAmount = productPrice * 100,
                },
                Quantity = 1,
            },
        },
                Mode = "payment",
                SuccessUrl = (env.IsDevelopment() ? $"{FrontBaseUrl.Local}" : $"{FrontBaseUrl.Production}") + "user/order-submition",
                CancelUrl = env.IsDevelopment() ? $"{FrontBaseUrl.Local}" : $"{FrontBaseUrl.Production}",
                Metadata = new Dictionary<string, string>
                {
                    { "OrderId", orderId.ToString() }
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

        //public async Task<Refund> RefundCheckoutSessionAsync(string sessionId)
        //{
        //    StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];

        //    var sessionService = new SessionService();
        //    var session = await sessionService.GetAsync(sessionId);

        //    var paymentIntentId = session.PaymentIntentId;

        //    var refundOptions = new RefundCreateOptions
        //    {
        //        PaymentIntent = paymentIntentId,
        //    };
        //    //var refundOptions = new RefundCreateOptions
        //    //{
        //    //    Charge = chargeId, // The original charge ID
        //    //    Amount = partialRefundAmount // Amount in cents, e.g., 5000 cents = $50
        //    //};

        //    //// Create the refund
        //    //var refund = refundService.Create(refundOptions);

        //    // Create the refund
        //    var refundService = new RefundService();
        //    var refund = await refundService.CreateAsync(refundOptions);

        //    return refund; // Return the refund details
        //}

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

        public async Task<ConfirmOrderRequstDto> HandlePaymentStatusAsync(string sessionId, bool isPaymentSucceeded, CancellationToken cancellationToken)
        {
            try
            {
                string[] include =
                    {
                       "user","Address", "OrderItems","OrderItems.ProductSize","OrderItems.ProductSize.ProductColor.Product",
                       "OrderItems.ProductSize.ProductColor.Color","OrderItems.ProductSize.Size"
                    };

                var order = await orderRepository.Find(d => d.SessionId == sessionId, true, include);

                if (order == null)
                    return null;

                var ProductItem = order.OrderItems.Select(e => new ProductItem
                {
                    Name = e.ProductSize!.ProductColor!.Product.NameEn,
                    Quantity = e.Quantity,
                    skuNumber = e.ProductSize.SKU_Size,
                    Weight = (double)e.ProductSize.ProductWeight,
                    sku = e.ProductSize!.ProductColor!.Product.SKU
                }).ToList();

                if (isPaymentSucceeded)
                {
                    order.PaymentStatus = PaymentStatus.Paid;
                    // Prepare Data To Save Shipping Data
                    var deliveryObject = new DeliveryObject
                    {
                        AddressID = order.AddressId,
                        OrderNumber = order.OrderNumber,
                        TotalAmount = order.TotalAmount,
                        UserID = order.UserId,
                        ProductItems = ProductItem
                    };

                    await deliveryService.CreateDeliveryOrderAsync(deliveryObject, order.Id, true);
                }
                else
                {
                    order.PaymentStatus = PaymentStatus.UnPaid;
                }

                orderRepository.Update(order);
                await orderRepository.SaveAsync(cancellationToken);

                var confirmOrderDto = new ConfirmOrderRequstDto
                {
                    OrderNumber = order.OrderNumber,
                    OrderStatus = order.OrderStatus,
                    ShippedStatus = order.ShippedStatus,
                    PaymentMethod = order.PaymentMethod,
                    ShippingPrice = order.ShippingPrice,
                    FullName=order.user.FullName,
                    TotalAmount=order.TotalAmount,
                    Address = order.Address != null ? new AddressDto
                    {
                        Id = order.Address.Id,
                        CountryId = order.Address.CountryId,
                        CityId = order.Address.CityId,
                        ServiceableAreaId = order.Address.ServiceableAreaId,
                        Street = order.Address.Street,
                        BuildingNumber = order.Address.BuildingNumber,
                        AddressLine = order.Address.AddressLine,
                        LandMark = order.Address.LandMark,
                        IsDeafult = order.Address.IsDeafult,
                    } : null,

                    OrderItems = order.OrderItems?.Select(item => new OrderItemDto
                    {
                        OrderId = item.OrderId,
                        ProductSizeId = item.ProductSizeId,
                        ProductNameEn = item.ProductSize?.ProductColor?.Product?.NameEn ?? string.Empty,
                        ProductNameAr = item.ProductSize?.ProductColor?.Product?.NameAr ?? string.Empty,
                        Quantity = item.Quantity,
                        ItemPrice = item.ItemPrice,
                        NumberOfReturn = item.ProductSize.ProductColor.Product.NumberOfReturnDays,
                        ColorNameEn = item.ProductSize?.ProductColor?.Color?.NameEn ?? string.Empty,
                        ColorNameAr = item.ProductSize?.ProductColor?.Color?.NameAr ?? string.Empty,
                        Size = item.ProductSize?.Size?.Name ?? string.Empty,
                        ImageUrl = item.ProductSize?.ProductColor?.ProductColorImages?.FirstOrDefault()?.ImageUrl ?? string.Empty,
                        ProductCode = item.ProductSize?.ProductColor?.Product?.ProductCode ?? string.Empty
                    }).ToList() ?? new List<OrderItemDto>()
                };

                return confirmOrderDto;
            }
            catch
            {
                return null;
            }
        }
        

        // Method to convert the price using the fetched exchange rate
        public long ConvertToTargetCurrency(long priceInUsdCents, decimal exchangeRate)
        {
            // Convert the price to the target currency based on the exchange rate
            var convertedPrice = priceInUsdCents * exchangeRate;
            return (long)Math.Round(convertedPrice);
        }

        public long ConvertFromAedToTargetCurrency(long priceInAedCents, decimal aedToUsdRate, decimal usdToTargetRate)
        {
            // Step 1: Convert from AED to USD (using the AED to USD exchange rate)
            decimal priceInUsdCents = priceInAedCents / aedToUsdRate;

            // Step 2: Convert from USD to the target currency (using the USD to target currency exchange rate)
            decimal convertedPriceInTargetCurrency = priceInUsdCents * usdToTargetRate;

            // Round the converted price and return it in long format (in cents)
            return (long)Math.Round(convertedPriceInTargetCurrency);
        }
    }
}