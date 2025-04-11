using OutbornE_commerce.BAL.Dto.Countries;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.Cities
{
    public class CityDto
    {
        public Guid Id { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public Guid CountryId { get; set; }
        public CountryDto? Country { get; set; }
    }
}
