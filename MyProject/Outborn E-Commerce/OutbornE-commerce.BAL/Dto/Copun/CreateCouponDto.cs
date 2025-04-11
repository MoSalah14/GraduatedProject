using OutbornE_commerce.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.Copun
{
    public class CreateCouponDto
    {
        public string Code { get; set; }
        public DiscountType DiscountType { get; set; } // "Percentage" or "Fixed"
        public decimal DiscountValue { get; set; }
        public decimal? MinSpend { get; set; }
        public decimal? MaxDiscount { get; set; }
        public int? UsageLimit { get; set; }
        public int? PerUserLimit { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public CouponStatus Status { get; set; }
    }

    public class GetCouponDto : CreateCouponDto
    {
        public Guid? Id { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public new DiscountType DiscountType { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public new CouponStatus Status { get; set; }

        public DateTime? UpdatedOn { get; set; }
    }
}