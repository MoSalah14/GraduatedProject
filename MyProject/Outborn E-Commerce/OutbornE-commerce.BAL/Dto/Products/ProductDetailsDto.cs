using OutbornE_commerce.BAL.Dto.Brands;
using OutbornE_commerce.BAL.Dto.Image;
using OutbornE_commerce.BAL.Dto.ProductCategories;
using OutbornE_commerce.BAL.Dto.ProductColors;
using OutbornE_commerce.BAL.Dto.ProductSizes;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.Products
{
    public class ProductDetailsDto
    {
        public Guid Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public string AboutEn { get; set; }
        public string AboutAr { get; set; }
        public string MaterialEn { get; set; }
        public string MaterialAr { get; set; }
        public string SizeAndFitEn { get; set; }
        public string SizeAndFitAr { get; set; }
        public string DeliveryAndReturnEn { get; set; }
        public string DeliveryAndReturnAr { get; set; }
        public List<ProductSizeDto>? ProductSizes { get; set; }
        public List<ProductColorDto>? ProductColors { get; set; }
        public List<ProductCategoryDto>? ProductCategories { get; set; }
        public List<ImageDTO>? ProductImages { get; set; }
        public int ProductType { get; set; }
        public Guid BrandId { get; set; }
        public BrandDto? Brand { get; set; }
        public int Label { get; set; }
        public int QuantityInStock { get; set; }
        public decimal ShippingCost { get; set; }
        public int NumberOfReturnDays { get; set; }
    }
}
