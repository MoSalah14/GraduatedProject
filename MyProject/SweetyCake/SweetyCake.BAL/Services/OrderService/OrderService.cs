using Infrastructure.Services.PaymentWithStripeService;
using Infrastructure.Services.PaymentWithStripeService.Models;
using Mapster;
using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.BAL.Dto.Address;
using OutbornE_commerce.BAL.Dto.Cart;
using OutbornE_commerce.BAL.Dto.OrderDto;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.BAL.Repositories.OrderRepo;
using OutbornE_commerce.BAL.Services.Cart_Service;
using OutbornE_commerce.DAL.Enums;
using OutbornE_commerce.DAL.Models;
using OutbornE_commerce.BAL.Repositories.Address;
using OutbornE_commerce.BAL.Dto.Delivery;
using OutbornE_commerce.BAL.Services.CouponsService;
using OutbornE_commerce.BAL.Repositories.ShippingPriceRepo;
using Microsoft.AspNetCore.Http;
using Stripe.Climate;
using System.Threading;
using OutbornE_commerce.FilesManager;
using Order = OutbornE_commerce.DAL.Models.Order;
using OutbornE_commerce.BAL.Dto.Delivery.DeliveryOrders;

namespace OutbornE_commerce.BAL.Services.OrderService
{
    public class OrderService : IOrderService
    {
        #region Ctor

        private readonly IOrderRepository orderRepository;
        private readonly ICartService cartService;
        private readonly IPaymentWithStripeService paymentWithStripeService;
        private readonly IBaseRepository<User> baseRepository;
        private readonly IAddressRepository addressRepository;
        private readonly CouponService couponService;
        private readonly IShippingPriceRepo _shippingPriceRepo;
        private readonly IFilesManager _FilesManager;

        public OrderService(
            IOrderRepository orderRepository,
            ICartService cartService,
            IPaymentWithStripeService paymentWithStripeService,
            IBaseRepository<User> baseRepository,
            IAddressRepository addressRepository,
            CouponService couponService, IShippingPriceRepo shippingPriceRepo,
            IFilesManager filesManager)
        {
            this.orderRepository = orderRepository;
            this.cartService = cartService;
            this.paymentWithStripeService = paymentWithStripeService;
            this.baseRepository = baseRepository;
            this.addressRepository = addressRepository;
            this.couponService = couponService;
            _shippingPriceRepo = shippingPriceRepo;
            _FilesManager = filesManager;
        }

        #endregion Ctor

        public async Task<Response<string>> CreateOrderAsync(CreateOrderDto model, string userId, CancellationToken cancellationToken)
        {

            return null;
            //var userCart = await cartService.GetUserCartAsync(userId);
            //await orderRepository.BeginTransactionAsync();

            //try
            //{
            //    var order = InitializeOrder(model, userId);
            //    long totalAmount = 0;
            //    decimal TotalProductWeight = 0;
            //    var productItemsForDelivery = new List<ProductItem>();

            //    foreach (var item in userCart.Items)
            //    {
            //        var existingProduct = await productSizeRepository.Find(i => i.Id == item.ProductSizeId && i.Quantity > 0, false, new string[] { "ProductColor.Product" });
            //        if (existingProduct != null && existingProduct.Quantity >= item.Quantity)
            //        {
            //            var orderItem = AddOrderItem(item, existingProduct, ref totalAmount);
            //            order.OrderItems.Add(orderItem);

            //            TotalProductWeight += existingProduct.ProductWeight;

            //            productItemsForDelivery.Add(new ProductItem
            //            {
            //                Quantity = item.Quantity,
            //                Name = existingProduct.ProductColor.Product.NameEn,
            //                Weight = (double)existingProduct.ProductWeight,
            //                skuNumber = existingProduct.SKU_Size,
            //            });

            //            existingProduct.Quantity -= item.Quantity;
            //            productSizeRepository.Update(existingProduct);
            //        }
            //        else
            //        {
            //            return new Response<string>
            //            {
            //                Message = "Check Your Cart, Some Products Are Out of Stock",
            //                IsError = true,
            //                MessageAr = "افحص العربه الخاصه بك هناك منتجانت غير موجوده",
            //                Status = (int)StatusCodeEnum.BadRequest
            //            };
            //        }
            //    }

            //    // Validate coupon if provided
            //    if (!string.IsNullOrEmpty(model.CouponCode))
            //    {
            //        var couponDiscountAmount = await couponService.ApplyCouponAsync(model.CouponCode, userId, totalAmount);

            //        // Apply discount to total amount
            //        totalAmount = (long)couponDiscountAmount;
            //    }

            //    if (model.address != null)
            //    {
            //        order.Address = await InitializeAddress(model.address, userId, cancellationToken);
            //        order.AddressId = order.Address.Id;
            //    }

            //    #region ShippingPrice

            //    var getUserAdressCountry = await addressRepository.Find(e => e.Id == order.AddressId && e.UserId == userId);

            //    var IsUAEaddress = await countryRepository.IsUAECountry(getUserAdressCountry!.CountryId);
            //    if (IsUAEaddress)
            //    {
            //        if (model.IsOrderExpress)
            //            order.ShippingPrice = 35;
            //        else
            //        {
            //            // Check For Free Delivery
            //            var GetFreeDeliveryPrice = await freeDeliveryRepo.GetFreeDelivery(getUserAdressCountry.CountryId);
            //            if (GetFreeDeliveryPrice != null && totalAmount >= GetFreeDeliveryPrice)
            //            {
            //                order.ShippingPrice = 0;
            //            }
            //            else
            //            {
            //                order.ShippingPrice = 20;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        // Get Shipping Price
            //        order.ShippingPrice = await _shippingPriceRepo.GetShippingPriceBasedOnWeightAndCountryId(getUserAdressCountry!.CountryId, (double)TotalProductWeight);
            //    }

            //    #endregion ShippingPrice

            //    // Allow Tracking For Order
            //    await orderRepository.Create(order);
            //    string sessionUrl = string.Empty;
            //    if (model.PaymentMethod == PaymentMethod.Strip)
            //    {
            //        var sessionResponse = await paymentWithStripeService.CreateCheckoutSession(userId, totalAmount, (long)order.ShippingPrice, "CardPayments", order.Id);
            //        sessionResponse.OrderId = order.Id;
            //        sessionUrl = sessionResponse.SessionUrl;
            //        order.SessionId = sessionResponse.SessionId;
            //    }
            //    else
            //    {
            //        if (model.PaymentMethod == PaymentMethod.Wallet)
            //        {
            //            var GetUsdValue = await currencyRepository.GetAEDValue();
            //            var ShippingPriceWithUsd = order.ShippingPrice / GetUsdValue;
            //            await walletService.PayWithWalletAsync(userId, totalAmount + ShippingPriceWithUsd, cancellationToken);
            //        }
            //        var DeliveryObject = new DeliveryObject
            //        {
            //            AddressID = order.AddressId,
            //            UserID = order.UserId,
            //            OrderNumber = order.OrderNumber,
            //            TotalAmount = totalAmount + order.ShippingPrice,
            //            ProductItems = productItemsForDelivery
            //        };

            //        await deliveryService.CreateDeliveryOrderAsync(DeliveryObject, order.Id, false, model.IsOrderExpress);
            //    }

            //    await orderRepository.SaveAsync(cancellationToken);
            //    await orderRepository.CommitTransactionAsync();

            //    return new Response<string>
            //    {
            //        Data = !string.IsNullOrEmpty(sessionUrl) ? sessionUrl : order.Id.ToString(),
            //        IsError = false,
            //        Status = (int)StatusCodeEnum.Ok,
            //        Message = "Created Successfully",
            //        MessageAr = "تم انشاء اوردر بنجاح"
            //    };
            //}
            //catch (Exception ex)
            //{
            //    await orderRepository.RollbackTransactionAsync();
            //    return new Response<string>
            //    {
            //        Message = "An error occurred while creating the order.",
            //        IsError = true,
            //        MessageAr = ex.Message,
            //        Status = (int)StatusCodeEnum.BadRequest
            //    };
            //}
        }

        private Order InitializeOrder(CreateOrderDto model, string userId)
        {
            var order = model.Adapt<Order>();
            order.CreatedBy = "admin";
            order.CreatedOn = DateTime.Now;
            order.OrderNumber = orderRepository.GenerateOrderNumber();
            order.UserId = userId;
            order.OrderItems = new List<OrderItem>();
            return order;
        }

        //private OrderItem AddOrderItem(CartItemDto item, ProductSize existingProduct, ref long totalAmount)
        //{
        //    var orderItem = item.Adapt<OrderItem>();
        //    if (existingProduct.DiscountedPrice > 0)
        //    {
        //        orderItem.ItemPrice = existingProduct.DiscountedPrice;
        //        totalAmount += (long)(existingProduct.DiscountedPrice * item.Quantity);
        //    }
        //    else
        //    {
        //        orderItem.ItemPrice = existingProduct.Price;
        //        totalAmount += (long)(existingProduct.Price * item.Quantity);
        //    }

        //    return orderItem;
        //}

        private async Task<Address> InitializeAddress(AddressForCreationDto addressDto, string userID, CancellationToken cancellationToken)
        {
            var address = addressDto.Adapt<Address>();
            address.CreatedBy = "admin";
            address.CreatedOn = DateTime.Now;
            address.UserId = userID;
            var NewAddress = await addressRepository.Create(address);
            await addressRepository.SaveAsync(cancellationToken);

            return NewAddress;
        }
    }
}