using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public class Address : BaseEntity
    {
        public Guid CountryId { get; set; } // Any Address Must Have Country
        public Country Country { get; set; }

        public Guid? CityId { get; set; } // Nullable because some countries don't have cities
        public City? City { get; set; }
        public Guid? ServiceableAreaId { get; set; } // Nullable because areas are only relevant in certain countries (e.g., UAE)
        public CityAreas? CityAreas { get; set; }

        public string? Street { get; set; }
        public string? BuildingNumber { get; set; }
        public string? AddressLine { get; set; }
        public string? LandMark { get; set; }
        public bool? IsDeafult { get; set; }
        public string? UserId { get; set; }
        public User? User { get; set; }
        public List<ReturnItemReason>? ReturnItemReasons { get; set; }

    }
}