namespace OutbornE_commerce.DAL.Models
{
    public class CityAreas : BaseEntity
    {
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public Guid CityId { get; set; }
        public City City { get; set; }
    }
}