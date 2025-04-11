using Infrastructure.Services.PaymentWithStripeService;
using Infrastructure.Services.PaymentWithStripeService.Models;
using Mapster;
using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.BAL.Dto.Address;
using OutbornE_commerce.BAL.Dto.Cart;
using OutbornE_commerce.BAL.Dto.OrderDto;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.BAL.Repositories.OrderRepo;
using OutbornE_commerce.BAL.Repositories.ProductSizes;
using OutbornE_commerce.BAL.Services.Cart_Service;
using OutbornE_commerce.DAL.Enums;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OutbornE_commerce.BAL.Services.DeliveryService;
using OutbornE_commerce.BAL.Repositories.Address;
using OutbornE_commerce.BAL.Dto.Delivery;
using OutbornE_commerce.BAL.Services.CouponsService;
using OutbornE_commerce.BAL.Services.Wallet;
using OutbornE_commerce.BAL.Repositories.FreeDeliverys;
using OutbornE_commerce.BAL.Repositories.ShippingPriceRepo;
using OutbornE_commerce.BAL.Repositories.Countries;
using OutbornE_commerce.BAL.Repositories.Currencies;
using OutbornE_commerce.BAL.Dto.ReturnItemreasonDto;
using Microsoft.AspNetCore.Http;
using Stripe.Climate;
using System.Threading;
using OutbornE_commerce.FilesManager;
using Order = OutbornE_commerce.DAL.Models.Order;
using OutbornE_commerce.BAL.Repositories.ReturnedItems;
using OutbornE_commerce.BAL.Dto.Image;
using OutbornE_commerce.BAL.Dto.Delivery.DeliveryOrders;

namespace OutbornE_commerce.BAL.Services.OrderService
{
    public class OrderService : IOrderService
    {
        #region Ctor

        private readonly IOrderRepository orderRepository;
        private readonly IProductSizeRepository productSizeRepository;
        private readonly ICartService cartService;
        private readonly IPaymentWithStripeService paymentWithStripeService;
        private readonly IBaseRepository<User> baseRepository;
        private readonly DeliveryService.DeliveryService deliveryService;
        private readonly IAddressRepository addressRepository;
        private readonly WalletService walletService;
        private readonly FreeDeliveryRepo freeDeliveryRepo;
        private readonly CouponService couponService;
        private readonly IShippingPriceRepo _shippingPriceRepo;
        private readonly ICountryRepository countryRepository;
        private readonly ICurrencyRepository currencyRepository;
        private readonly IFilesManager _FilesManager;
        private readonly IReturnOrderRepositry _ReturnOrderRepositry;

        public OrderService(
            IOrderRepository orderRepository,
            IProductSizeRepository productSizeRepository,
            ICartService cartService,
            IPaymentWithStripeService paymentWithStripeService,
            IBaseRepository<User> baseRepository,
            DeliveryService.DeliveryService deliveryService,
            IAddressRepository addressRepository,
            WalletService walletService, FreeDeliveryRepo freeDeliveryRepo,
            CouponService couponService, IShippingPriceRepo shippingPriceRepo,
            ICountryRepository countryRepository, ICurrencyRepository currencyRepository,
            IFilesManager filesManager, IReturnOrderRepositry returnOrderRepositry)
        {
            this.orderRepository = orderRepository;
            this.productSizeRepository = productSizeRepository;
            this.cartService = cartService;
            this.paymentWithStripeService = paymentWithStripeService;
            this.baseRepository = baseRepository;
            this.deliveryService = deliveryService;
            this.addressRepository = addressRepository;
            this.walletService = walletService;
            this.freeDeliveryRepo = freeDeliveryRepo;
            this.couponService = couponService;
            _shippingPriceRepo = shippingPriceRepo;
            this.countryRepository = countryRepository;
            this.currencyRepository = currencyRepository;
            _FilesManager = filesManager;
            _ReturnOrderRepositry = returnOrderRepositry;
        }

        #endregion Ctor

        public async Task<Response<string>> CreateOrderAsync(CreateOrderDto model, string userId, CancellationToken cancellationToken)
        {
            var userCart = await cartService.GetUserCartAsync(userId);
            await orderRepository.BeginTransactionAsync();

            try
            {
                var order = InitializeOrder(model, userId);
                long totalAmount = 0;
                decimal TotalProductWeight = 0;
                var productItemsForDelivery = new List<ProductItem>();

                foreach (var item in userCart.Items)
                {
                    var existingProduct = await productSizeRepository.Find(i => i.Id == item.ProductSizeId && i.Quantity > 0, false, new string[] { "ProductColor.Product" });
                    if (existingProduct != null && existingProduct.Quantity >= item.Quantity)
                    {
                        var orderItem = AddOrderItem(item, existingProduct, ref totalAmount);
                        order.OrderItems.Add(orderItem);

                        TotalProductWeight += existingProduct.ProductWeight;

                        productItemsForDelivery.Add(new ProductItem
                        {
                            Quantity = item.Quantity,
                            Name = existingProduct.ProductColor.Product.NameEn,
                            Weight = (double)existingProduct.ProductWeight,
                            skuNumber = existingProduct.SKU_Size,
                            sku = existingProduct.ProductColor.Product.SKU
                        });

                        existingProduct.Quantity -= item.Quantity;
                        productSizeRepository.Update(existingProduct);
                    }
                    else
                    {
                        return new Response<string>
                        {
                            Message = "Check Your Cart, Some Products Are Out of Stock",
                            IsError = true,
                            MessageAr = "افحص العربه الخاصه بك هناك منتجانت غير موجوده",
                            Status = (int)StatusCodeEnum.BadRequest
                        };
                    }
                }

                // Validate coupon if provided
                if (!string.IsNullOrEmpty(model.CouponCode))
                {
                    var couponDiscountAmount = await couponService.ApplyCouponAsync(model.CouponCode, userId, totalAmount);

                    // Apply discount to total amount
                    totalAmount = (long)couponDiscountAmount;
                }

                if (model.address != null)
                {
                    order.Address = await InitializeAddress(model.address, userId, cancellationToken);
                    order.AddressId = order.Address.Id;
                }

                #region ShippingPrice

                var getUserAdressCountry = await addressRepository.Find(e => e.Id == order.AddressId && e.UserId == userId);

                var IsUAEaddress = await countryRepository.IsUAECountry(getUserAdressCountry!.CountryId);
                if (IsUAEaddress)
                {
                    if (model.IsOrderExpress)
                        order.ShippingPrice = 35;
                    else
                    {
                        // Check For Free Delivery
                        var GetFreeDeliveryPrice = await freeDeliveryRepo.GetFreeDelivery(getUserAdressCountry.CountryId);
                        if (GetFreeDeliveryPrice != null && totalAmount >= GetFreeDeliveryPrice)
                        {
                            order.ShippingPrice = 0;
                        }
                        else
                        {
                            order.ShippingPrice = 20;
                        }
                    }
                }
                else
                {
                    // Get Shipping Price
                    order.ShippingPrice = await _shippingPriceRepo.GetShippingPriceBasedOnWeightAndCountryId(getUserAdressCountry!.CountryId, (double)TotalProductWeight);
                }

                #endregion ShippingPrice

                // Allow Tracking For Order
                await orderRepository.Create(order);
                string sessionUrl = string.Empty;
                if (model.PaymentMethod == PaymentMethod.Strip)
                {
                    var sessionResponse = await paymentWithStripeService.CreateCheckoutSession(userId, totalAmount, (long)order.ShippingPrice, "CardPayments", order.Id);
                    sessionResponse.OrderId = order.Id;
                    sessionUrl = sessionResponse.SessionUrl;
                    order.SessionId = sessionResponse.SessionId;
                }
                else
                {
                    if (model.PaymentMethod == PaymentMethod.Wallet)
                    {
                        var GetUsdValue = await currencyRepository.GetAEDValue();
                        var ShippingPriceWithUsd = order.ShippingPrice / GetUsdValue;
                        await walletService.PayWithWalletAsync(userId, totalAmount + ShippingPriceWithUsd, cancellationToken);
                    }
                    var DeliveryObject = new DeliveryObject
                    {
                        AddressID = order.AddressId,
                        UserID = order.UserId,
                        OrderNumber = order.OrderNumber,
                        TotalAmount = totalAmount + order.ShippingPrice,
                        ProductItems = productItemsForDelivery
                    };

                    await deliveryService.CreateDeliveryOrderAsync(DeliveryObject, order.Id, false, model.IsOrderExpress);
                }

                await orderRepository.SaveAsync(cancellationToken);
                await orderRepository.CommitTransactionAsync();

                return new Response<string>
                {
                    Data = !string.IsNullOrEmpty(sessionUrl) ? sessionUrl : order.Id.ToString(),
                    IsError = false,
                    Status = (int)StatusCodeEnum.Ok,
                    Message = "Created Successfully",
                    MessageAr = "تم انشاء اوردر بنجاح"
                };
            }
            catch (Exception ex)
            {
                await orderRepository.RollbackTransactionAsync();
                return new Response<string>
                {
                    Message = "An error occurred while creating the order.",
                    IsError = true,
                    MessageAr = ex.Message,
                    Status = (int)StatusCodeEnum.BadRequest
                };
            }
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

        private OrderItem AddOrderItem(CartItemDto item, ProductSize existingProduct, ref long totalAmount)
        {
            var orderItem = item.Adapt<OrderItem>();
            if (existingProduct.DiscountedPrice > 0)
            {
                orderItem.ItemPrice = existingProduct.DiscountedPrice;
                totalAmount += (long)(existingProduct.DiscountedPrice * item.Quantity);
            }
            else
            {
                orderItem.ItemPrice = existingProduct.Price;
                totalAmount += (long)(existingProduct.Price * item.Quantity);
            }

            return orderItem;
        }

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

        //public async Task<Response<string>> CreateReturnOrder(string UserId, ReturnedOrdersDto ReturendOrder, CancellationToken cancellationToken)
        //{
        //    await _ReturnOrderRepositry.BeginTransactionAsync();

        //    var addressId = Guid.Empty;

        //    if (ReturendOrder.Address != null)
        //    {
        //        var address = await InitializeAddress(ReturendOrder.Address, UserId, cancellationToken);
        //        addressId = address.Id;
        //    }
        //    else if (ReturendOrder.AddressId.HasValue)
        //    {
        //        addressId = ReturendOrder.AddressId.Value;
        //    }
        //    else
        //    {
        //        await _ReturnOrderRepositry.RollbackTransactionAsync();
        //        return new Response<string>
        //        {
        //            Message = "Either AddressId or address data must be provided.",
        //            IsError = true,
        //            MessageAr = "يجب تقديم معرف العنوان أو بيانات العنوان.",
        //            Status = (int)StatusCodeEnum.BadRequest
        //        };
        //    }

        //    // Prepare Return Order
        //    var ReturnedOrderEntity = ReturendOrder.Adapt<ReturnedOrders>();

        //    ReturnedOrderEntity.OrderReturnNumber = GenerateReturnOrderNumber();
        //    ReturnedOrderEntity.AddressId = addressId;

        //    foreach (var item in ReturendOrder.ReturnedItems)
        //    {
        //        var returnItemReason = item.Adapt<ReturnItemReason>();

        //        if (item.Images != null && item.Images.Any())
        //        {
        //            returnItemReason.Images = new List<ImageReturned>();

        //            foreach (var image in item.Images)
        //            {
        //                var fileModel = await _FilesManager.UploadFile(image, "ReturnImages");
        //                if (fileModel == null)
        //                {
        //                    return new Response<string>
        //                    {
        //                        Message = "Failed to upload image.",
        //                        IsError = true,
        //                        MessageAr = "فشل في تحميل الصورة.",
        //                        Status = (int)StatusCodeEnum.BadRequest
        //                    };
        //                }

        //                returnItemReason.Images.Add(new ImageReturned
        //                {
        //                    ImageUrl = fileModel.Url,
        //                });
        //            }
        //        }
        //        ReturnedOrderEntity.ReturnItems.Add(returnItemReason);
        //    }
        //    try
        //    {
        //        await _ReturnOrderRepositry.Create(ReturnedOrderEntity);

        //        await _ReturnOrderRepositry.SaveAsync(cancellationToken);
        //        await _ReturnOrderRepositry.CommitTransactionAsync();
        //    }
        //    catch
        //    {
        //        await _ReturnOrderRepositry.RollbackTransactionAsync();
        //    }

        //    return new Response<string>
        //    {
        //        Message = "Created SuccessFully",
        //        IsError = false,
        //        MessageAr = "تم الاضافة بنجاح.",
        //        Status = (int)StatusCodeEnum.Ok
        //    };
        //}

        public async Task<ReturnedDeliveryObject> CreateReturnOrder(string userId, ReturnedOrdersDto returnedOrder, CancellationToken cancellationToken)
        {
            // Start transaction
            await _ReturnOrderRepositry.BeginTransactionAsync();

            try
            {
                // Validate address
                var addressId = await GetAddressId(returnedOrder, userId, cancellationToken);
                if (addressId == Guid.Empty)
                {
                    throw new InvalidOperationException();
                }

                // Map and prepare the return order
                var returnedOrderEntity = MapToReturnedOrderEntity(returnedOrder, addressId);

                // Clear the existing list
                returnedOrderEntity.ReturnItems.Clear();

                // Process each return item and map it
                foreach (var item in returnedOrder.ReturnItems)
                {
                    var returnItemReason = await ProcessReturnItem(item);
                    returnedOrderEntity.ReturnItems.Add(returnItemReason);
                }

                await _ReturnOrderRepositry.Create(returnedOrderEntity);
                await _ReturnOrderRepositry.SaveAsync(cancellationToken);
                await _ReturnOrderRepositry.CommitTransactionAsync();

                return new ReturnedDeliveryObject
                {
                    ReturnedOrderID = returnedOrderEntity.Id,
                    AddressID = addressId,
                    OrderNumber = returnedOrderEntity.OrderReturnNumber,
                    UserID = userId,
                    ProductItems = returnedOrderEntity.ReturnItems.ToList(),
                };
            }
            catch (Exception ex)
            {
                // Rollback the transaction in case of errors
                await _ReturnOrderRepositry.RollbackTransactionAsync();

                return null;
            }
        }

        #region Helper Methods

        private string GenerateReturnOrderNumber()
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var randomPart = new Random().Next(1000, 9999);
            return $"RTO-{timestamp}-{randomPart}";
        }

        private async Task<Guid> GetAddressId(ReturnedOrdersDto returnedOrder, string userId, CancellationToken cancellationToken)
        {
            if (returnedOrder.Address != null)
            {
                var address = await InitializeAddress(returnedOrder.Address, userId, cancellationToken);
                return address.Id;
            }

            if (returnedOrder.AddressId.HasValue)
            {
                return returnedOrder.AddressId.Value;
            }

            return Guid.Empty;
        }

        private ReturnedOrders MapToReturnedOrderEntity(ReturnedOrdersDto returnedOrder, Guid addressId)
        {
            var returnedOrderEntity = returnedOrder.Adapt<ReturnedOrders>();
            returnedOrderEntity.OrderReturnNumber = GenerateReturnOrderNumber();
            returnedOrderEntity.AddressId = addressId;

            //returnedOrderEntity.ReturnItems = new List<ReturnItemReason>();

            return returnedOrderEntity;
        }

        private async Task<ReturnItemReason> ProcessReturnItem(ReturnItemsReasonDto item)
        {
            var returnItemReason = item.Adapt<ReturnItemReason>();

            if (item.Images != null && item.Images.Any())
            {
                returnItemReason.Images = await ProcessReturnItemImages(item.Images);
            }

            return returnItemReason;
        }

        private async Task<List<ImageReturned>> ProcessReturnItemImages(List<IFormFile> images)
        {
            var imageReturnedList = new List<ImageReturned>();

            foreach (var image in images)
            {
                var fileModel = await _FilesManager.UploadFile(image, "ReturnImages");
                if (fileModel == null)
                {
                    throw new InvalidOperationException("Failed to upload image.");
                }

                imageReturnedList.Add(new ImageReturned
                {
                    ImageUrl = fileModel.Url
                });
            }

            return imageReturnedList;
        }

        #endregion Helper Methods
    }
}