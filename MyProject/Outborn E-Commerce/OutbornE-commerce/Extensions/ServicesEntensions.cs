using Infrastructure.Services.PaymentWithStripeService;
using Microsoft.AspNetCore.Identity.UI.Services;
using OutbornE_commerce.BAL;
using OutbornE_commerce.BAL.AuthServices;
using OutbornE_commerce.BAL.EmailServices;
using OutbornE_commerce.BAL.Repositories.AboutUs;
using OutbornE_commerce.BAL.Repositories.Address;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.BAL.Repositories.Brands;
using OutbornE_commerce.BAL.Repositories.Cart;
using OutbornE_commerce.BAL.Repositories.CartItem;
using OutbornE_commerce.BAL.Repositories.Categories;
using OutbornE_commerce.BAL.Repositories.CategorySubCategory;
using OutbornE_commerce.BAL.Repositories.Cities;
using OutbornE_commerce.BAL.Repositories.Colors;
using OutbornE_commerce.BAL.Repositories.ContactUs;
using OutbornE_commerce.BAL.Repositories.ContactUsSetups;
using OutbornE_commerce.BAL.Repositories.Countries;
using OutbornE_commerce.BAL.Repositories.Coupon;
using OutbornE_commerce.BAL.Repositories.Coupones;
using OutbornE_commerce.BAL.Repositories.Currencies;
using OutbornE_commerce.BAL.Repositories.DiscountCategoryRepo;
using OutbornE_commerce.BAL.Repositories.DiscountRepository;
using OutbornE_commerce.BAL.Repositories.FAQs;
using OutbornE_commerce.BAL.Repositories.FlashDealRepo;
using OutbornE_commerce.BAL.Repositories.FooterRepo;
using OutbornE_commerce.BAL.Repositories.FreeDeliverys;
using OutbornE_commerce.BAL.Repositories.Hashtags;
using OutbornE_commerce.BAL.Repositories.Headers;
using OutbornE_commerce.BAL.Repositories.HomeSections;
using OutbornE_commerce.BAL.Repositories.InquiryTypes;
using OutbornE_commerce.BAL.Repositories.NavTitles;
using OutbornE_commerce.BAL.Repositories.Newsletters;
using OutbornE_commerce.BAL.Repositories.NewsletterSubscribers;
using OutbornE_commerce.BAL.Repositories.OrderItemRepo;
using OutbornE_commerce.BAL.Repositories.OrderRepo;
using OutbornE_commerce.BAL.Repositories.ProductCateories;
using OutbornE_commerce.BAL.Repositories.ProductColors;
using OutbornE_commerce.BAL.Repositories.ProductImageRepo;
using OutbornE_commerce.BAL.Repositories.Products;
using OutbornE_commerce.BAL.Repositories.ProductSizeDiscountRepo;
using OutbornE_commerce.BAL.Repositories.ProductSizes;
using OutbornE_commerce.BAL.Repositories.ReceivePoints;
using OutbornE_commerce.BAL.Repositories.ReturnedImagesRep;
using OutbornE_commerce.BAL.Repositories.ReturnedItems;
using OutbornE_commerce.BAL.Repositories.SEOs;
using OutbornE_commerce.BAL.Repositories.ShippingPriceRepo;
using OutbornE_commerce.BAL.Repositories.Sizes;
using OutbornE_commerce.BAL.Repositories.SMTP_Server;
using OutbornE_commerce.BAL.Repositories.SubCategoryRepo;
using OutbornE_commerce.BAL.Repositories.SubCategoryTypeRepo;
using OutbornE_commerce.BAL.Repositories.Tickets;
using OutbornE_commerce.BAL.Repositories.TypeEntityRepo;
using OutbornE_commerce.BAL.Repositories.UserPermissionRepo;
using OutbornE_commerce.BAL.Repositories.UserRepo;
using OutbornE_commerce.BAL.Repositories.UserReviews;
using OutbornE_commerce.BAL.Repositories.Wallet;
using OutbornE_commerce.BAL.Repositories.WalletTransactionsRepository;
using OutbornE_commerce.BAL.Repositories.WishList;
using OutbornE_commerce.BAL.Services;
using OutbornE_commerce.BAL.Services.Cart_Service;
using OutbornE_commerce.BAL.Services.CouponsService;
using OutbornE_commerce.BAL.Services.DeliveryService;
using OutbornE_commerce.BAL.Services.OrderService;
using OutbornE_commerce.BAL.Services.ReviewService;
using OutbornE_commerce.BAL.Services.Wallet;
using OutbornE_commerce.BAL.Services.WishListsService;
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
            services.AddScoped<IBrandRepository, BrandRepository>();
            services.AddScoped<IFilesManager, OutbornE_commerce.FilesManager.FilesManager>();
            services.AddScoped<IHeaderRepository, HeaderRepository>();
            services.AddScoped<IColorRepository, ColorRepository>();
            services.AddScoped<IHomeSectionRepository, HomeSectionRepository>();
            services.AddScoped<ISizeRepository, SizeRepository>();
            services.AddScoped<ISEORepository, SEORepository>();
            services.AddScoped<IReceivePointsRepository, ReceivePointsRepository>();
            services.AddScoped<ICurrencyRepository, CurrencyRepository>();
            services.AddScoped<ICityRepository, CityRepository>();
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<ISMTPRepository, SMTPRepository>();
            services.AddScoped<ITicketRepository, TicketRepository>();
            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<IHashtagRepository, HashtagRepository>();
            services.AddScoped<IFAQRepository, FAQRepository>();
            services.AddScoped<IDiscountCategoryRepository, DiscountCategoryRepository>();
            services.AddScoped<IAboutUsRepository, AboutUsRepository>();
            services.AddScoped<IContactUsSetupRepository, ContactUsSetupRepository>();
            services.AddScoped<IInquiryTypeRepository, InquiryTypeRepository>();
            services.AddScoped<IContactUsRepository, ContactUsRepository>();
            //services.AddHostedService<DiscountCleanupService>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductSizeRepository, ProductSizeRepository>();
            services.AddScoped<IProductColorRepository, ProductColorRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            services.AddScoped<IPaymentWithStripeService, PaymentWithStripeService>();
            services.AddScoped<IDiscountRepository, DiscountRepository>();
            services.AddScoped<IUserPermissionRepo, UserPermissionRepo>();
            services.AddScoped<IproductSizeDiscountRepository, productSizeDiscountRepository>();
            services.AddScoped<ISubcategoryTypeRepository, SubCategoryTypeRepository>();
            services.AddScoped<ICategorySubCategoryBridgeRepository, CategorySubCategoryBridgeRepository>();
            services.AddScoped<IproductSizeDiscountRepository, productSizeDiscountRepository>();

            services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();

            services.AddScoped<INewsletterRepository, NewsletterRepository>();
            services.AddScoped<INewsletterSubscriberRepository, NewsletterSubscriberRepository>();

            services.AddScoped<IEmailSenderCustom, EmailSender>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IUserReposatry, UserReposatry>();
            services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();
            services.AddScoped<ITypeEntityRepository, TypeEntityRepository>();

            //Start From Here
            services.AddScoped<INavTitleRepo, NavTitleRepo>();
            services.AddScoped<IWishListRepo, WishListRepo>();
            services.AddScoped<IFlashDealRepo, FlashDealRepo>();
            services.AddScoped<IFooterRepository, FooterRepository>();

            services.AddScoped<ICartItemRepo, CartItemRepo>();
            services.AddScoped<ICartRepo, CartRepo>();
            services.AddScoped<ICurrencyService, CurrencyService>();
            services.AddScoped<IReviewsRepository, ReviewsRepository>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IWishListsService, WishListsService>();
            services.AddScoped<IProductImageRepositry, ProductImageRepositry>();
            services.AddScoped<ICouponRepository, CouponRepository>();
            services.AddScoped<IWalletRepository, WalletRepository>();
            services.AddScoped<IWalletTransactionsRepository, WalletTransactionsRepository>();
            services.AddScoped<IShippingPriceRepo, ShippingPriceRepo>();
            services.AddScoped<IReturnOrderRepositry, ReturnOrderRepositry>();
            services.AddScoped<IReturnedImagesRepositry, ReturnedImagesRepositry>();
            services.AddScoped<CouponService>();
            services.AddScoped<WalletService>();

            services.AddScoped<DeliveryService>();
            services.AddScoped<FreeDeliveryRepo>();
            services.AddHostedService<CurrencyUpdateService>();

            MapProfile.RegisterMappings();
        }
    }
}