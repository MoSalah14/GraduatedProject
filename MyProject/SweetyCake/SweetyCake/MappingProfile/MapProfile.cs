using OutbornE_commerce.BAL.Dto.Categories;
using OutbornE_commerce.BAL.Dto.ContactUs;
using OutbornE_commerce.BAL.Dto.Profile;
using OutbornE_commerce.BAL.Dto.Review;

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

            TypeAdapterConfig<User, ProfileDto>.NewConfig()
                .Map(dest => dest.UserAddress, src => src.Addresses);
        }
    }
}