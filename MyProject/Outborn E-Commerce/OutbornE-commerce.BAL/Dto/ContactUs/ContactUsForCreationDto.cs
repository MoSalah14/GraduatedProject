using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.ContactUs
{
    public class ContactUsForCreationDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string OrderNo { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public Guid InquireTypeId { get; set; }
    }
}
