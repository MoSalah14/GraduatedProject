using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public class FreeDelivery : BaseEntity
    {
        public decimal FreeDeliveryBasedOnOrderPrice { get; set; } // Order Value
        public Guid CountryId { get; set; }
        public Country Country { get; set; }
    }
}