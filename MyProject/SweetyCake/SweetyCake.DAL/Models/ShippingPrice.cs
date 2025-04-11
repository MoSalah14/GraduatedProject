using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public class ShippingPrice : BaseEntity
    {
        public double Weight { get; set; } // Weight Product
        public decimal Price { get; set; }
        public Guid CountryId { get; set; }
        public Country Country { get; set; }
    }
}