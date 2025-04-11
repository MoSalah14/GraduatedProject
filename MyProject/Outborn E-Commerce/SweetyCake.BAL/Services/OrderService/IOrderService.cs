using Azure;
using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.BAL.Dto.Address;
using OutbornE_commerce.BAL.Dto.Cart;
using OutbornE_commerce.BAL.Dto.Delivery;
using OutbornE_commerce.BAL.Dto.OrderDto;
using OutbornE_commerce.BAL.Dto.ReturnItemreasonDto;
using OutbornE_commerce.DAL.Models;

namespace OutbornE_commerce.BAL.Services.OrderService
{
    public interface IOrderService
    {
        Task<Dto.Response<string>> CreateOrderAsync(CreateOrderDto model, string userId, CancellationToken cancellationToken);

        Task<ReturnedDeliveryObject> CreateReturnOrder(string UserId, ReturnedOrdersDto ReturendOrder, CancellationToken cancellationToken);
    }
}