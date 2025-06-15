using OutbornE_commerce.BAL.Dto.Address;
using OutbornE_commerce.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.OrderDto
{
    public class CreateOrderDto
    {
        public PaymentMethod PaymentMethod { get; set; }
        public Guid? AddressId { get; set; }
        public AddressForCreationDto? address { get; set; }
    }
}