using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Enums
{
    public enum DiscountType
    {
        Percentage,
        Fixed
    }

    public enum CouponStatus
    {
        Active,
        Expired,
        Disabled,
    }
}