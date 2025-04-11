using OutbornE_commerce.BAL.Dto.Delivery.DeliveryOrders;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.Delivery
{
    public class DeliveryObject
    {
        public string UserID { get; set; }
        public Guid AddressID { get; set; }

        public string OrderNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public List<ProductItem> ProductItems { get; set; } = new List<ProductItem>();
    }

    public class ReturnedDeliveryObject
    {
        public Guid ReturnedOrderID { get; set; }
        public string UserID { get; set; }
        public Guid AddressID { get; set; }
        public string OrderNumber { get; set; }
        public List<ReturnItemReason> ProductItems { get; set; } = new List<ReturnItemReason>();
    }
}