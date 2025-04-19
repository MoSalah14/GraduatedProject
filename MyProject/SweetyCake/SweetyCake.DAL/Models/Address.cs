using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public class Address : BaseEntity
    {
        public string? Street { get; set; }
        public string? BuildingNumber { get; set; }
        public string? AddressLine { get; set; }
        public string? LandMark { get; set; }
        public bool? IsDeafult { get; set; }
        public string? UserId { get; set; }
        public User? User { get; set; }

    }
}