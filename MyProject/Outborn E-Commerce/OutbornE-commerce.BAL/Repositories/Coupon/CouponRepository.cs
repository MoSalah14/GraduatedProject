using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.BAL.Repositories.Coupon;
using OutbornE_commerce.DAL.Data;
using OutbornE_commerce.DAL.Models;

namespace OutbornE_commerce.BAL.Repositories.Coupones
{
    public class CouponRepository : BaseRepository<Coupons>, ICouponRepository
    {
        public CouponRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<int> GetUserCouponUsageAsync(Guid couponId, Guid userId)
        {
            return await _context.CouponUsers
                .Where(cu => cu.CouponId == couponId && cu.UserId == userId)
                .Select(cu => cu.UsageCount)
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetTotalUsageAsync(Guid couponId)
        {
            return await _context.CouponUsers
                .Where(cu => cu.CouponId == couponId)
                .SumAsync(cu => cu.UsageCount);
        }

        public async Task IncrementCouponUsageAsync(Guid couponId, Guid userId)
        {
            var couponUser = await _context.CouponUsers
                .FirstOrDefaultAsync(cu => cu.CouponId == couponId && cu.UserId == userId);

            if (couponUser == null)
            {
                couponUser = new UserCoupon { CouponId = couponId, UserId = userId, UsageCount = 1 };
                await _context.CouponUsers.AddAsync(couponUser);
            }
            else
            {
                couponUser.UsageCount++;
            }

            await _context.SaveChangesAsync();
        }
    }
}