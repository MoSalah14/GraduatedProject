using OutbornE_commerce.BAL.Dto.Brands;
using OutbornE_commerce.BAL.Dto.Categories;
using OutbornE_commerce.BAL.Dto.Image;
using OutbornE_commerce.BAL.Dto.ProductCategories;
using OutbornE_commerce.BAL.Dto.ProductColors;
using OutbornE_commerce.BAL.Dto.ProductSizes;
using OutbornE_commerce.BAL.Dto.Review;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.Products
{
    public class ProductDto
    {
        public Guid Id { get; set; }
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
        public string Label { get; set; }
        public int? ProductTypeLabel { get; set; }
        public bool IsActive { get; set; }
        public decimal ShippingCost { get; set; }

        public bool IsPeopleAlsoBought { get; set; }
        public int NumberOfReturnDays { get; set; }
        public bool IsPreOrder { get; set; }
        public PreOrderDetailsForCreateDto? PreOrderDetails { get; set; }

        public string? ProductCode { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string SKU { get; set; }

        public TypeDto.GetAllTypessDto ProductType { get; set; }

        // Related entities
        public ProductBrandDto Brand { get; set; }

        public List<ProductColorDto> ProductColors { get; set; }
        //public List<ProductCategoryDto> ProductCategories { get; set; }
        public List<ReviewDataDto> Reviews { get; set; }

        public List<getAllSubCategoriesDto>? SubCategories { get; set; }
        public List<CategoryDto>? categories { get; set; }

    }
}