using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.Address
{
    public class AddressForCreationDto
    {
        public string Governorate { get; set; }
        public string? Street { get; set; }
        public string? BuildingNumber { get; set; }
        public string? AddressLine { get; set; }
        public string? LandMark { get; set; }
        public bool? IsDeafult { get; set; }
    }
}