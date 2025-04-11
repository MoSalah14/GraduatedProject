using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.BAL.Dto.OrderDto;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Data;
using OutbornE_commerce.DAL.Enums;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Repositories.OrderRepo
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context) : base(context)
        { }

        public async Task<PagainationModel<IEnumerable<OrderDisplayDto>>> GetFilteredOrdersAsync(
     GetFillteringorders? filter,
     int pageNumber = 1,
     int pageSize = 10)
        {
            string[] includes = { "OrderItems", "user" };

            var predicate = BuildPredicate(filter);

            var paginatedOrders = await FindAllAsyncByPagination(predicate, pageNumber, pageSize, includes);

            var orderDisplayDtos = paginatedOrders.Data.Select(o => new OrderDisplayDto
            {
                OrderID = o.Id,
                OrderCode = o.OrderNumber,
                NumberOfProducts = o.OrderItems.Count,
                Customer = o.user.FullName,
                Amount = o.TotalAmount,
                DeliveryStatus = o.ShippedStatus.ToString(),
                PaymentMethod = o.PaymentMethod.ToString(),
                PaymentStatus = o.PaymentStatus.ToString()
            }).ToList();

            return new PagainationModel<IEnumerable<OrderDisplayDto>>
            {
                Data = orderDisplayDtos,
                TotalCount = paginatedOrders.TotalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        private static Expression<Func<Order, bool>> BuildPredicate(GetFillteringorders? filter)
        {
            if (filter == null)
            {
                return o => true;
            }

            return o =>
                (!filter.StartDate.HasValue || o.CreatedOn >= filter.StartDate.Value) &&
                (!filter.EndDate.HasValue || o.CreatedOn <= filter.EndDate.Value) &&
                (!filter.ShippedStatus.HasValue || o.ShippedStatus == filter.ShippedStatus.Value) &&
                (!filter.PaymentStatus.HasValue || o.PaymentStatus == filter.PaymentStatus.Value) &&
                (string.IsNullOrEmpty(filter.OrderNumber) || o.OrderNumber.Contains(filter.OrderNumber));
        }

        //public async Task<List<OrderDisplayDto>> GetFilteredOrdersAsync(GetFillteringorders filter)
        //{
        //    var query = _context.Orders.AsQueryable();

        //    if (filter.ShippedStatus.HasValue)
        //    {
        //        query = query.Where(o => o.ShippedStatus == filter.ShippedStatus.Value);
        //    }

        //    if (filter.PaymentStatus.HasValue)
        //    {
        //        query = query.Where(o => o.PaymentStatus == filter.PaymentStatus.Value);
        //    }

        //    if (!string.IsNullOrEmpty(filter.OrderNumber))
        //    {
        //        query = query.Where(o => o.OrderNumber.Contains(filter.OrderNumber));
        //    }

        //    if (filter.DateRangeOption.HasValue)
        //    {
        //        DateTime startDate;
        //        switch (filter.DateRangeOption.Value)
        //        {
        //            case DateRange.Last10Days:
        //                startDate = DateTime.Now.AddDays(-10);
        //                break;
        //            case DateRange.Last20Days:
        //                startDate = DateTime.Now.AddDays(-20);
        //                break;
        //            case DateRange.Last30Days:
        //                startDate = DateTime.Now.AddDays(-30);
        //                break;
        //            case DateRange.All:
        //                startDate = DateTime.MinValue;
        //                break;
        //            default:
        //                startDate = DateTime.Now;
        //                break;
        //        }

        //        if (filter.DateRangeOption.Value != DateRange.All)
        //        {
        //            query = query.Where(o => o.CreatedOn >= startDate);
        //        }
        //    }

        //    var orders = await query
        //        .Include(o => o.OrderItems)
        //        .Include(o => o.user)
        //        .Select(o => new OrderDisplayDto
        //        {
        //            OrderCode = o.OrderNumber,
        //            NumberOfProducts = o.OrderItems.Count,
        //            Customer = o.user.FullName,
        //            Amount = o.TotalAmount,
        //            DeliveryStatus = o.ShippedStatus.ToString(),
        //            PaymentMethod = o.PaymentMethod.ToString(),
        //            PaymentStatus = o.PaymentStatus.ToString()
        //        })
        //        .ToListAsync();

        //    return orders;
        //}

        public string GenerateOrderNumber()
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");

            var randomPart = new Random().Next(1000, 9999);

            return $"ORD-{timestamp}-{randomPart}";
        }

        public async Task<Order> GetOrderBySessionId(string SessionId)
        {
            string[] includes = { "OrderItems" };
            return await Find(x => x.SessionId == SessionId, false, includes);
        }
    }
}