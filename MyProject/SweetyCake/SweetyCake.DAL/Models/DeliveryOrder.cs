using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public class DeliveryOrder : BaseEntity
    {
        public Guid? OrderId { get; set; } // Foreign key to Order
        public string TrackingNumber { get; set; }
        public string? eventTrigger { get; set; } = "REQUESTED";
        public string Status { get; set; }
        public bool IsReverse { get; set; } = false;
        public DateTime? DeliveryDate { get; set; }
        public string TrackingUrl { get; set; }
        public string shippingLabelUrl { get; set; }
        public string? Comment { get; set; }
        public Order Order { get; set; }

        public Guid? ReturnedOrdersID { get; set; }
    }
}