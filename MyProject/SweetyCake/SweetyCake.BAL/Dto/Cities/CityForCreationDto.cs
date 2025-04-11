using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.Cities
{
    public class CityForCreationDto
    {
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public Guid CountryId { get; set; }
    }
}
