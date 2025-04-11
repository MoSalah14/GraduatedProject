using OutbornE_commerce.DAL.Enums;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.OrderDto
{
    public class UpdateOrderDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public OrderStatus Status { get; set; }
        public PaymentMethod PaymentMethod { get; set; } 

        public List<OrderItem> OrderItems { get; set; }
       
    }
}
