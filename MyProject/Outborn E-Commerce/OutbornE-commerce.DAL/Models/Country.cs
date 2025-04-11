using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public class Country : BaseEntity
    {
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public bool HasCities { get; set; } // True for countries like UAE; False for others

        [MaxLength(5)]
        public string CountryCode { get; set; }

        public ICollection<City> Cities { get; set; }
        public ICollection<ShippingPrice> ShippingPrices { get; set; }
    }
}