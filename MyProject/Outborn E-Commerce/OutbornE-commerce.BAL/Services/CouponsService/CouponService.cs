using Mapster;
using OutbornE_commerce.BAL.Dto.Copun;
using OutbornE_commerce.BAL.Repositories.Coupon;
using OutbornE_commerce.DAL.Enums;
using OutbornE_commerce.DAL.Models;

namespace OutbornE_commerce.BAL.Services.CouponsService
{
    public class CouponService
    {
        private readonly ICouponRepository _couponRepository;

        public CouponService(ICouponRepository couponRepository)
        {
            _couponRepository = couponRepository;
        }

        public async Task<Coupons> AddCouponAsync(CreateCouponDto couponDto, CancellationToken cancellationToken)
        {
            var existingCoupon = await _couponRepository.Find(e => e.Code == couponDto.Code);
            if (existingCoupon != null) throw new Exception("Coupon code already exists.");

            var coupon = couponDto.Adapt<Coupons>();

            await _couponRepository.Create(coupon);
            await _couponRepository.SaveAsync(cancellationToken);
            return coupon;
        }

        public async Task<decimal> ApplyCouponAsync(string code, string userId, decimal cartTotal)
        {
            var coupon = await _couponRepository.Find(e => e.Code == code);
            if (coupon == null || coupon.Status != CouponStatus.Active) throw new Exception("Invalid coupon.");
            if (DateTime.Now < coupon.StartDate || DateTime.Now > coupon.EndDate) throw new Exception("Coupon is not valid.");
            if (coupon.MinSpend.HasValue && cartTotal < coupon.MinSpend.Value) throw new Exception("Minimum spend not met.");

            var userUsageCount = await _couponRepository.GetUserCouponUsageAsync(coupon.Id, Guid.Parse(userId));
            if (coupon.PerUserLimit.HasValue && userUsageCount >= coupon.PerUserLimit) throw new Exception("User usage limit reached.");
            if (coupon.UsageLimit.HasValue && await _couponRepository.GetTotalUsageAsync(coupon.Id) >= coupon.UsageLimit) throw new Exception("Total usage limit reached.");

            decimal discountAmount = coupon.DiscountType == DiscountType.Percentage ? (cartTotal * coupon.DiscountValue / 100) : coupon.DiscountValue;
            if (coupon.MaxDiscount.HasValue) discountAmount = Math.Min(discountAmount, coupon.MaxDiscount.Value);

            await _couponRepository.IncrementCouponUsageAsync(coupon.Id, Guid.Parse(userId));
            return cartTotal - discountAmount; // Final total after discount
        }
    }
}