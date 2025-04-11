using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public class ContactUsSetup : BaseEntity
    {
        public required string WhatsAppNo {  get; set; }
        public required string Email { get; set; }
    }
}
