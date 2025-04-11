using OutbornE_commerce.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public class ReturnItemReason
    {
        public Guid ID { get; set; }
        public int Quantity { get; set; }
        public string? Comment { get; set; }
        public Guid ReturnOrderId { get; set; }
        public ReturnedOrders OrderReturn { get; set; }
        public Guid ProductSizeId { get; set; }
        public virtual ProductSize? ProductSize { get; set; }
        public List<ImageReturned>? Images { get; set; }
    }
}