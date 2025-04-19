using OutbornE_commerce.DAL.Enums;
using OutbornE_commerce.BAL.Dto.OrderItemDto;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OutbornE_commerce.BAL.Dto.Address;

namespace OutbornE_commerce.BAL.Dto.OrderDto
{
    public class ConfirmOrderRequstDto
    {
        public string OrderNumber { get; set; }

        public OrderStatus OrderStatus { get; set; } 
        public PaymentStatus PaymentStatus { get; set; } 
        public ShippedStatus ShippedStatus { get; set; } 
        public PaymentMethod PaymentMethod { get; set; }

        public decimal ShippingPrice { get; set; }

        //public DeliveryOrder Delivery { get; set; }
        public string FullName { get; set; }
        public List<OrderItemDto.OrderItemDto>? OrderItems { get; set; }
        public AddressDto Address { get; set; }
        public decimal TotalAmount { get; set; }
 

    }
}
