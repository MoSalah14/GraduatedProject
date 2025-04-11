using Microsoft.AspNetCore.Http;
using OutbornE_commerce.BAL.Dto.Address;
using OutbornE_commerce.BAL.Dto.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.ReturnItemreasonDto
{
    public class ReturnedOrdersDto
    {
        public Guid OrderId { get; set; }
        public Guid? AddressId { get; set; }
        public AddressForCreationDto? Address { get; set; }
        public List<ReturnItemsReasonDto> ReturnItems { get; set; }
    }
}