using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public class City : BaseEntity
    {
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public Guid CountryId { get; set; }
        public Country? Country { get; set; }
        public ICollection<CityAreas> CityAreas { get; set; }
    }
}