using OutbornE_commerce.BAL.Dto.Address;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.Profile
{
    public class ProfileDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public List<AddressDto> UserAddress { get; set; }
        public Guid? CurrencyId { get; set; }
    }
}
