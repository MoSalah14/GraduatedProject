using Infrastructure.Services.PaymentWithStripeService;
using OutbornE_commerce.BAL.AuthServices;
using OutbornE_commerce.BAL.EmailServices;
using OutbornE_commerce.BAL.Repositories;
using OutbornE_commerce.BAL.Repositories.Address;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.BAL.Repositories.Categories;
using OutbornE_commerce.BAL.Repositories.ContactUs;
using OutbornE_commerce.BAL.Repositories.HomeSections;
using OutbornE_commerce.BAL.Repositories.OrderItemRepo;
using OutbornE_commerce.BAL.Repositories.OrderRepo;
using OutbornE_commerce.BAL.Repositories.ProductImageRepo;
using OutbornE_commerce.BAL.Repositories.Products;
using OutbornE_commerce.BAL.Repositories.ShippingPriceRepo;
using OutbornE_commerce.BAL.Repositories.UserRepo;
using OutbornE_commerce.BAL.Repositories.UserReviews;
using OutbornE_commerce.BAL.Services.OrderService;
using OutbornE_commerce.BAL.Services.ReviewService;
using OutbornE_commerce.FilesManager;
using OutbornE_commerce.MappingProfile;

namespace OutbornE_commerce.Extensions
{
    public static class ServicesEntensions
    {
        public static void ConfigureLifeTime(this IServiceCollection services)
        {
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IFilesManager, FilesManager.FilesManager>();
            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            services.AddScoped<IPaymentWithStripeService, PaymentWithStripeService>();
            services.AddScoped<IContactUsRepository, ContactUsRepository>();

            services.AddScoped<IEmailSenderCustom, EmailSender>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IUserReposatry, UserReposatry>();
            services.AddScoped<IReviewsRepository, ReviewsRepository>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IProductImageRepositry, ProductImageRepositry>();
            services.AddScoped<IShippingPriceRepo, ShippingPriceRepo>();
            services.AddScoped<IHomeSectionRepository, HomeSectionRepository>();
            services.AddScoped<IBagItemsRepo, BagItemsRepo>();

            MapProfile.RegisterMappings();
        }
    }
}