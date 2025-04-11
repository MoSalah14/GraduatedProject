using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.StripDto
{
    public class CancelSubscriptionRequest
    {
        public string SubscriptionId { get; set; }
        public bool ProrateRefund { get; set; }
    }
}
