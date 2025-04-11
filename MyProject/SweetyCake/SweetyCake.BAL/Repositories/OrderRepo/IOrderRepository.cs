using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.BAL.Dto.OrderDto;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Repositories.OrderRepo
{
    public interface IOrderRepository :IBaseRepository<Order>
    {
        string GenerateOrderNumber();
        Task<Order> GetOrderBySessionId(string SessionId);
        Task<PagainationModel<IEnumerable<OrderDisplayDto>>> GetFilteredOrdersAsync(GetFillteringorders? filter, int pageNumber = 1, int pageSize = 10);
    }
}
