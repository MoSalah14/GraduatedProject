using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Repositories.Coupon
{
    public interface ICouponRepository : IBaseRepository<Coupons>
    {
        Task<int> GetUserCouponUsageAsync(Guid couponId, Guid userId);

        Task<int> GetTotalUsageAsync(Guid couponId);

        Task IncrementCouponUsageAsync(Guid couponId, Guid userId);
    }
}