using OutbornE_commerce.DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public class Order : BaseEntity
    {
        public string OrderNumber { get; set; }

        public string UserId { get; set; }
        public Guid AddressId { get; set; }
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.UnPaid;
        public ShippedStatus ShippedStatus { get; set; } = ShippedStatus.Processing;
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Strip;
        public string? SessionId { get; set; }

        public decimal ShippingPrice { get; set; } = 50;

        public DeliveryOrder Delivery { get; set; }

        public List<OrderItem> OrderItems { get; set; }
        public User user { get; set; }
        public Address? Address { get; set; }

        public decimal TotalAmount
        {
            get
            {
                return OrderItems?.Sum(item => item.Quantity * item.ItemPrice) ?? 0;
            }
        }
    }
}