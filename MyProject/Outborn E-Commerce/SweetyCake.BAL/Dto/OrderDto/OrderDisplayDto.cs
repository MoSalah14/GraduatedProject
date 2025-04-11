using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.OrderDto
{
    public class OrderDisplayDto
    {
        public Guid OrderID { get; set; }
        public string OrderCode { get; set; }
        public int NumberOfProducts { get; set; }
        public string Customer { get; set; }
        public decimal Amount { get; set; }
        public string DeliveryStatus { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
    }
}