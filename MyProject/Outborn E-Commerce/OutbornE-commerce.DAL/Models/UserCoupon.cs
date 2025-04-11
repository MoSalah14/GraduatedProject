namespace OutbornE_commerce.DAL.Models
{
    public class UserCoupon
    {
        public Guid CouponId { get; set; }
        public Coupons Coupon { get; set; }
        public Guid UserId { get; set; }
        public int UsageCount { get; set; } = 0;
    }
}