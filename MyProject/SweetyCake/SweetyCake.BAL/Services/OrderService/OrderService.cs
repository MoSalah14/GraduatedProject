using Infrastructure.Services.PaymentWithStripeService;
using Mapster;
using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.BAL.Dto.Address;
using OutbornE_commerce.BAL.Dto.OrderDto;
using OutbornE_commerce.BAL.Repositories.OrderRepo;
using OutbornE_commerce.DAL.Models;
using OutbornE_commerce.BAL.Repositories.Address;
using Order = OutbornE_commerce.DAL.Models.Order;
using OutbornE_commerce.DAL.Enums;
using OutbornE_commerce.BAL.Repositories;
using OutbornE_commerce.BAL.Repositories.Products;
using OutbornE_commerce.BAL.Dto.Cart;
using SweetyCake.BAL.Dto;

namespace OutbornE_commerce.BAL.Services.OrderService
{
    public class OrderService : IOrderService
    {
        #region Ctor

        private readonly IOrderRepository orderRepository;
        private readonly IPaymentWithStripeService paymentWithStripeService;
        private readonly IProductRepository _ProductRepository;
        private readonly IAddressRepository addressRepository;
        private readonly IBagItemsRepo _BagItemsRepo;

        public OrderService(
            IOrderRepository orderRepository,
            IPaymentWithStripeService paymentWithStripeService, IProductRepository productRepository,
            IAddressRepository addressRepository,IBagItemsRepo bagItemsRepo)
        {
            this.orderRepository = orderRepository;
            this.paymentWithStripeService = paymentWithStripeService;
            _ProductRepository = productRepository;
            this.addressRepository = addressRepository;
            _BagItemsRepo = bagItemsRepo;
        }

        #endregion Ctor

        public async Task<Response<string>> CreateOrderAsync(CreateOrderDto model, string userId, CancellationToken cancellationToken)
        {
            var userCart = await _BagItemsRepo.GetUserCartAsync(userId);
            await orderRepository.BeginTransactionAsync();

            try
            {
                var order = InitializeOrder(model, userId);
                long totalAmount = 0;
                var productItemsForDelivery = new List<ProductItem>();

                if (!userCart.cartItemDtos.Any())
                {
                    return new Response<string>
                    {
                        Message = "Cart is Empty",
                        IsError = true,
                        MessageAr = "العربة فارغة",
                        Status = (int)StatusCodeEnum.BadRequest
                    };
                }

                foreach (var item in userCart.cartItemDtos)
                {
                    var existingProduct = await _ProductRepository.Find(i => i.Id == item.ProductId && i.QuantityInStock > 0, true, new string[] { "BagItem" });

                    if (existingProduct != null && existingProduct.QuantityInStock >= item.Quantity)
                    {
                        var orderItem = AddOrderItem(item, existingProduct, ref totalAmount);
                        order.OrderItems.Add(orderItem);


                        productItemsForDelivery.Add(new ProductItem
                        {
                            Quantity = item.Quantity,
                            Name = existingProduct.NameEn,
                        });

                        existingProduct.QuantityInStock -= item.Quantity;
                        //_ProductRepository.Update(existingProduct);
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

                if (model.address != null)
                {
                    order.Address = await InitializeAddress(model.address, userId, cancellationToken);
                    order.AddressId = order.Address.Id;
                }

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

            order.OrderItems = new List<OrderItem>();

            order.CreatedBy = "admin";
            order.CreatedOn = DateTime.Now;
            order.OrderNumber = orderRepository.GenerateOrderNumber();
            order.UserId = userId;

            return order;
        }


        private OrderItem AddOrderItem(CartItemDto item, Product existingProduct, ref long totalAmount)
        {
            var orderItem = new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                ItemPrice = existingProduct.Price
            };

            totalAmount += (long)(existingProduct.Price * item.Quantity);
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
    }
}