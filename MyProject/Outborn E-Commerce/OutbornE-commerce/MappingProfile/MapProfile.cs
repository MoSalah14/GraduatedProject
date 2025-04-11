using OutbornE_commerce.BAL.Dto.Categories;
using OutbornE_commerce.BAL.Dto.OrderDto;
using OutbornE_commerce.BAL.Dto.OrderItemDto;
using OutbornE_commerce.BAL.Dto.ProductCategories;
using OutbornE_commerce.BAL.Dto.ProductColors;
using OutbornE_commerce.BAL.Dto.Products;
using OutbornE_commerce.BAL.Dto.ProductSizes;
using OutbornE_commerce.BAL.Dto.Profile;
using OutbornE_commerce.BAL.Dto.Review;
using OutbornE_commerce.BAL.Dto.TypeDto;
using OutbornE_commerce.DAL.Models;

namespace OutbornE_commerce.MappingProfile
{
    public class MapProfile
    {
        public static void RegisterMappings()
        {
            TypeAdapterConfig<Reviews, ReviewDataDto>.NewConfig()
                .Map(dest => dest.User, src => src.User.FullName);

            TypeAdapterConfig<Product, ProductDto>.NewConfig()
                 .Map(dest => dest.SubCategories,
         src => src.ProductType.SubCategoryTypes != null
                ? src.ProductType.SubCategoryTypes.Select(csc => csc.SubCategory.Adapt<AllSubCategoryWithTypes>()).ToList()
                : new List<AllSubCategoryWithTypes>())
                 .Map(dest => dest.categories,
         src => src.ProductType.SubCategoryTypes != null
                ? src.ProductType.SubCategoryTypes.SelectMany(csc => csc.SubCategory.CategorySubCategories
                .Select(x => x.Category.Adapt<CategoryDto>()).ToList())
                : new List<CategoryDto>())

            .Map(dest => dest.ProductTypeLabel, src => src.ProductColors.FirstOrDefault()!.ProductSizes.FirstOrDefault()!.Size!.Type)
           .Map(dest => dest.PreOrderDetails, src => src.PreOrderDetails)
            .Map(dest => dest.ProductColors,
                 src => src.ProductColors.Select(pc => new ProductColorDto
                 {
                     ColorId = pc.Color.Id,
                     Hexadecimal = pc.Color.IsMultiColor ? "https://deemaabdo.com/Uploads/multicolor.jpeg" : pc.Color.Hexadecimal,
                     NameAr = pc.Color.NameAr,
                     NameEn = pc.Color.NameEn,
                     ProductColorImages = pc.ProductColorImages.Select(pci => new ProductColorImagesDto
                     {
                         ImageUrl = pci.ImageUrl,
                         IsDefault = pci.IsDefault,
                         ProductColorId = pci.ProductColorId
                     }).ToList(),
                     ProductSizes = pc.ProductSizes.Select(ps => new ProductSizeDto
                     {
                         SizeId = ps.Id,
                         ActualSizeId = ps.SizeId,
                         Name = ps.Size.Name,
                         Price = ps.Price,
                         Quantity = ps.Quantity,
                         DiscountedPrice = ps.DiscountedPrice,
                         Type = ps.Size.Type.ToString(),
                         SKU_Size = ps.SKU_Size,
                         ProductWeight = ps.ProductWeight,
                     }).ToList()
                 }).ToList());
            //   .Map(dest => dest.ProductCategories,
            //src => src.ProductType.SubCategoryTypes.Select(e => e.SubCategory)
            //          .SelectMany(e => e.CategorySubCategories).Select(e => e.Category)
            //.Select(pc => new ProductCategoryDto
            //{
            //    Id = pc.Id,
            //    NameEn = pc.NameEn,
            //    NameAr = pc.NameAr,
            //}).ToList());

            //TypeAdapterConfig<Product, ProductCardDto>.NewConfig()
            //.Map(dest => dest.ImageUrl, src => src.ProductColors.SelectMany(e => e.ProductColorImages).FirstOrDefault(e => e.IsDefault).ImageUrl)
            //.Map(dest => dest.Price, src => src.ProductColors
            //                                  .Where(pc => pc.ProductSizes.Any())
            //                                  .SelectMany(pc => pc.ProductSizes)
            //                                  .DefaultIfEmpty(new ProductSize { Price = 0 })
            //                                  .Min(ps => ps.Price));

            TypeAdapterConfig<Product, GetAllProductForUserDto>.NewConfig()
           .Map(dest => dest.Price, src => src.ProductColors!.SelectMany(e => e.ProductSizes.Select(e => e.Price)).DefaultIfEmpty(0).Min())
           .Map(dest => dest.ProductColors, src => src.ProductColors.Select(s => s.Color).ToList())
            //.Map(dest => dest.ImageUrl, src => src.ProductColors.SelectMany(e => e.ProductColorImages).Where(e => e.IsDefault == true).Select(s => s.ImageUrl).Take(2).ToList())
            .Map(dest => dest.ImageUrl, src => src.ProductColors
            .SelectMany(e => e.ProductColorImages)
            .OrderBy(e => e.IsDefault ? 0 : 1)
            .Select(e => e.ImageUrl)
            .Take(2)
            .ToList());

            TypeAdapterConfig<User, ProfileDto>.NewConfig()
                .Map(dest => dest.UserAddress, src => src.Addresses);

            TypeAdapterConfig<OrderItem, OrderItemDto>.NewConfig()

               .Map(dest => dest.ColorNameEn, src => src.ProductSize.ProductColor.Color.NameEn)
               .Map(dest => dest.ColorNameAr, src => src.ProductSize.ProductColor.Color.NameAr)
               .Map(dest => dest.Size, src => src.ProductSize.Size.Name)
               .Map(dest => dest.ImageUrl, src => src.ProductSize.ProductColor.ProductColorImages.FirstOrDefault().ImageUrl)
               .Map(dest => dest.ProductNameEn, src => src.ProductSize.ProductColor.Product.NameEn)
               .Map(dest => dest.ProductNameAr, src => src.ProductSize.ProductColor.Product.NameAr)
               .Map(dest => dest.NumberOfReturn, src => src.ProductSize.ProductColor.Product.NumberOfReturnDays)
               .Map(dest => dest.ProductCode, src => src.ProductSize.ProductColor.Product.ProductCode);

            TypeAdapterConfig<SubCategory, AllSubCategoryWithTypes>
    .NewConfig()
    .Map(dest => dest.TypessDtos,
         src => src.SubCategoryTypes.Select(type => new GetAllTypessDto
         {
             Id = type.TypeId,
             NameEn = type.type.NameEn,
             NameAr = type.type.NameAr
         }).ToList());

            TypeAdapterConfig<SubCategory, SubCategoryDto>.NewConfig()
              .Map(dest => dest.CategoriesId,
              src => src.CategorySubCategories.Select(c => c.CategoryId).ToList());

            TypeAdapterConfig<TypeEntity, TypeDto>.NewConfig()
             .Map(dest => dest.SubCategoriesIds,
             src => src.SubCategoryTypes.Select(c => c.SubCategoryId).ToList());
            TypeAdapterConfig<ProductForCreateDto, Product>
        .NewConfig()
        .Ignore(dest => dest.ProductColors); // Ignore the navigation property

            TypeAdapterConfig<TypeEntity, ReturnTypeDto>.NewConfig()
       .Map(dest => dest.SubCategories,
           src => src.SubCategoryTypes.Select(subType => new getAllSubCategoriesDto
           {
               Id = subType.SubCategoryId,
               NameEn = subType.SubCategory.NameEn,
               NameAr = subType.SubCategory.NameAr,
           }).ToList())
       .Map(dest => dest.categories,
           src => src.SubCategoryTypes
                   .SelectMany(s => s.SubCategory.CategorySubCategories)
                   .Select(x => x.Category != null ? x.Category.Adapt<CategoryDto>() : null)
                   .Where(c => c != null)
                   .ToList());

            TypeAdapterConfig<Category, AllCategoriesWithSubsAndTypesDto>.NewConfig()
     .Map(dest => dest.SubCategories,
          src => src.CategorySubCategories != null
                 ? src.CategorySubCategories.Select(csc => csc.SubCategory.Adapt<AllSubCategoryWithTypes>()).ToList()
                 : new List<AllSubCategoryWithTypes>());
            //.Map(dest => dest.TypessDtos,
            //     src => src.CategorySubCategories != null
            //            ? src.CategorySubCategories.SelectMany(csc => csc.SubCategory.SubCategoryTypes)
            //                                      .Select(subType => subType.type.Adapt<GetAllTypessDto>())
            //                                      .ToList()
            //            : new List<GetAllTypessDto>());

            TypeAdapterConfig<SubCategory, AllSubsWithCategoryDto>.NewConfig()
    .Map(dest => dest.categories,
         src => src.CategorySubCategories != null
                ? src.CategorySubCategories.Select(csc => csc.Category.Adapt<CategoryDto>()).ToList()
                : new List<CategoryDto>());

            TypeAdapterConfig<Order, OrderDto>.NewConfig()
                .Map(e => e.MinReturnDay, m => m.OrderItems.Select(e => e.ProductSize.ProductColor.Product.NumberOfReturnDays).Max());

            TypeAdapterConfig<Product, GetAllProductForUserDtoًWithCategory>.NewConfig()
        .Map(dest => dest.Id, src => src.Id)
        .Map(dest => dest.NameEn, src => src.NameEn)
      .Map(dest => dest.CategoryNameEn,
     src => src.ProductType.SubCategoryTypes
         .SelectMany(x => x.SubCategory.CategorySubCategories ?? new List<CategorySubCategoryBridge>())
         .Select(y => y.Category.NameEn)
         .FirstOrDefault())

.Map(dest => dest.CategoryNameAr,
     src => src.ProductType.SubCategoryTypes
         .SelectMany(x => x.SubCategory.CategorySubCategories ?? new List<CategorySubCategoryBridge>())
         .Select(y => y.Category.NameAr)
         .FirstOrDefault())
.Map(dest => dest.CategoryID, src => src.ProductType.SubCategoryTypes
         .SelectMany(x => x.SubCategory.CategorySubCategories ?? new List<CategorySubCategoryBridge>())
         .Select(y => y.Category.Id)
         .FirstOrDefault())
.Map(dest => dest.ImageUrl, src => src.ProductColors
         .SelectMany(x => x.ProductColorImages)
         .Select(y => y.ImageUrl)
         .ToList())
.Map(dest => dest.BrandID, src => src.BrandId)
.Map(dest => dest.ProductColors, src => src.ProductColors.Select(s => s.Color))
.Map(dest => dest.Price, src => src.ProductColors
    .SelectMany(s => s.ProductSizes)
    .Select(e => e.Price)
    .FirstOrDefault())
.Map(dest => dest.DiscountPrice, src => src.ProductColors
    .SelectMany(s => s.ProductSizes)
    .Select(e => e.DiscountedPrice)
    .FirstOrDefault())

;
        }
    }
}