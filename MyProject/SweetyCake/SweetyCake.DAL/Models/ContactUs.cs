using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public class ContactUs : BaseEntity
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public string PhoneNo { get; set; }
        public string OrderNo {  get; set; }
        public required string Subject {  get; set; }
        public required string Body { get; set; }
        public InquiryType? InquiryType { get; set; }
        public Guid InquireTypeId { get; set; }

    }
}
