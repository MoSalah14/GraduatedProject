namespace OutbornE_commerce.BAL.Dto.Delivery.DeliveryOrders
{
    public class DeliveryOrderRequest
    {
        public string? Reference { get; set; }
        public string? TrackingNumber { get; set; }
        public CustomerInfo CustomerInfo { get; set; }
        public decimal PaymentAmount { get; set; }
        public bool IsReverse { get; set; } = false;

        public string profileName { get; set; } = "nextday";
        public List<ProductItem> Items { get; set; } = new List<ProductItem>();
    }

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
        public double Weight { get; set; }
        public string? skuNumber { get; set; }
        public string? sku { get; set; } // Item Name
        public string? weightUnit { get; set; } = "Kg";
    }
}