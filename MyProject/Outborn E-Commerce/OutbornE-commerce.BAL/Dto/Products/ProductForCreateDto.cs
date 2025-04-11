using OutbornE_commerce.BAL.Dto.ProductColors;

namespace OutbornE_commerce.BAL.Dto.Products
{
    public class ProductForCreateDto
    {
        public Guid? Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string AboutEn { get; set; }
        public string AboutAr { get; set; }
        public string MaterialEn { get; set; }
        public string MaterialAr { get; set; }
        public string SizeAndFitEn { get; set; }
        public string SizeAndFitAr { get; set; }
        public string DeliveryAndReturnEn { get; set; }
        public string DeliveryAndReturnAr { get; set; }
        public string SKU { get; set; }
        public bool IsActive { get; set; } = true;
        public List<ProductColorForCreateDto> ProductColors { get; set; }
        public List<PhotosUrlOnUpdateProduct>? ProductPhotosUrl { get; set; }
        public Guid BrandId { get; set; }
        public int Label { get; set; }
        public decimal ShippingCost { get; set; }
        public bool IsPeopleAlseBought { get; set; }
        public int NumberOfReturnDays { get; set; }

        public bool IsPreOrder { get; set; } = false;
        public string ProductCode { get; set; }
        public decimal ProductWeight { get; set; }

        public Guid ProductTypeId { get; set; }

        //public Guid CategoryId { get; set; }
        //public Guid SubCategoryId { get; set; }
        public PreOrderDetailsForCreateDto? PreOrderDetails { get; set; }
    }

    public class PhotosUrlOnUpdateProduct
    {
        public string ImageUrl { get; set; }
        public bool IsDefault { get; set; }
    }
}