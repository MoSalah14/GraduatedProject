using OutbornE_commerce.BAL.Dto.Categories;
using OutbornE_commerce.BAL.Dto.ContactUs;
using OutbornE_commerce.BAL.Dto.OrderDto;
using OutbornE_commerce.BAL.Dto.OrderItemDto;
using OutbornE_commerce.BAL.Dto.ProductCategories;
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


            TypeAdapterConfig<ContactUs, ContactUsDto>.NewConfig()
                .Map(dest => dest.FullName, src => src.User.FullName);

            // TypeAdapterConfig<Product, GetAllProductForUserDto>.NewConfig()
            //.Map(dest => dest.Price, src => src.ProductColors!.SelectMany(e => e.ProductSizes.Select(e => e.Price)).DefaultIfEmpty(0).Min())
            //.Map(dest => dest.ProductColors, src => src.ProductColors.Select(s => s.Color).ToList())
            // //.Map(dest => dest.ImageUrl, src => src.ProductColors.SelectMany(e => e.ProductColorImages).Where(e => e.IsDefault == true).Select(s => s.ImageUrl).Take(2).ToList())
            // .Map(dest => dest.ImageUrl, src => src.ProductColors
            // .SelectMany(e => e.ProductColorImages)
            // .OrderBy(e => e.IsDefault ? 0 : 1)
            // .Select(e => e.ImageUrl)
            // .Take(2)
            // .ToList());

            TypeAdapterConfig<User, ProfileDto>.NewConfig()
                .Map(dest => dest.UserAddress, src => src.Addresses);

            //TypeAdapterConfig<OrderItem, OrderItemDto>.NewConfig()

            //   .Map(dest => dest.ColorNameEn, src => src.ProductSize.ProductColor.Color.NameEn)
            //   .Map(dest => dest.ColorNameAr, src => src.ProductSize.ProductColor.Color.NameAr)
            //   .Map(dest => dest.Size, src => src.ProductSize.Size.Name)
            //   .Map(dest => dest.ImageUrl, src => src.ProductSize.ProductColor.ProductColorImages.FirstOrDefault().ImageUrl)
            //   .Map(dest => dest.ProductNameEn, src => src.ProductSize.ProductColor.Product.NameEn);

        }
    }
}