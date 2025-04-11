using OutbornE_commerce.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.OrderDto
{
    public class GetFillteringorders
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public ShippedStatus? ShippedStatus { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
       public string? OrderNumber { get; set; }


    }
}
