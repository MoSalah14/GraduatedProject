
using OutbornE_commerce.BAL.Dto.OrderDto;

namespace OutbornE_commerce.BAL.Services.OrderService
{
    public interface IOrderService
    {
        Task<Dto.Response<string>> CreateOrderAsync(CreateOrderDto model, string userId, CancellationToken cancellationToken);

    }
}