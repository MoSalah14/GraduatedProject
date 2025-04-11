using OutbornE_commerce.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public class ReturnedOrders : BaseEntity
    {
        //  Reference
        public string OrderReturnNumber { get; set; }

        public Guid OrderId { get; set; }
        public Order Order { get; set; }
        public string Status { get; set; } = OrderStatus.Pending.ToString();

        public Guid AddressId { get; set; }
        public Address? Address { get; set; }
        public DeliveryOrder Delivery { get; set; }
        public ICollection<ReturnItemReason> ReturnItems { get; set; }
    }
}