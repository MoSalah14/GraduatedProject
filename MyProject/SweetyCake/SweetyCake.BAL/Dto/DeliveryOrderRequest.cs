namespace SweetyCake.BAL.Dto
{
    public class CustomerInfo
    {
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? CountryCode { get; set; }
        public string? AddressLine1 { get; set; }
        public string? Street { get; set; }
        public string? Building { get; set; }
        public string? LandMark { get; set; }
    }

    public class ProductItem
    {
        public Guid ProductSizeID { get; set; }
        public int Quantity { get; set; }
        public string? Name { get; set; }
    }
}