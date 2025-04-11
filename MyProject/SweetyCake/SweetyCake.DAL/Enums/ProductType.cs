using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Enums
{
    //public enum ProductType
    //{
    //    Men = 0,
    //    Women = 1,
    //    Kids= 2,
    //    Accessories = 3,
    //}

    public enum Permission
    {
        CreateAboutUs = 1,
        ReadAboutUs = 2,
        UpdateAboutUs = 3,
        DeleteAboutUs = 4,

        CreateAddress = 5,
        ReadAddress = 6,
        UpdateAddress = 7,
        DeleteAddress = 8,

        CreateBagItem = 9,
        ReadBagItem = 10,
        UpdateBagItem = 11,
        DeleteBagItem = 12,

        CreateBaseEntity = 13,
        ReadBaseEntity = 14,
        UpdateBaseEntity = 15,
        DeleteBaseEntity = 16,

        CreateBlogCategory = 17,
        ReadBlogCategory = 18,
        UpdateBlogCategory = 19,
        DeleteBlogCategory = 20,

        CreateBlogs = 21,
        ReadBlogs = 22,
        UpdateBlogs = 23,
        DeleteBlogs = 24,

        CreateBrand = 25,
        ReadBrand = 26,
        UpdateBrand = 27,
        DeleteBrand = 28,

        CreateCart = 29,
        ReadCart = 30,
        UpdateCart = 31,
        DeleteCart = 32,

        CreateCartItem = 33,
        ReadCartItem = 34,
        UpdateCartItem = 35,
        DeleteCartItem = 36,

        CreateCategory = 37,
        ReadCategory = 38,
        UpdateCategory = 39,
        DeleteCategory = 40,

        CreateCategorySubCategoryBridge = 41,
        ReadCategorySubCategoryBridge = 42,
        UpdateCategorySubCategoryBridge = 43,
        DeleteCategorySubCategoryBridge = 44,

        CreateCity = 45,
        ReadCity = 46,
        UpdateCity = 47,
        DeleteCity = 48,

        CreateCityAreas = 49,
        ReadCityAreas = 50,
        UpdateCityAreas = 51,
        DeleteCityAreas = 52,

        CreateColor = 53,
        ReadColor = 54,
        UpdateColor = 55,
        DeleteColor = 56,

        CreateContactUs = 57,
        ReadContactUs = 58,
        UpdateContactUs = 59,
        DeleteContactUs = 60,

        CreateContactUsSetup = 61,
        ReadContactUsSetup = 62,
        UpdateContactUsSetup = 63,
        DeleteContactUsSetup = 64,

        CreateCountry = 65,
        ReadCountry = 66,
        UpdateCountry = 67,
        DeleteCountry = 68,

        CreateCoupons = 69,
        ReadCoupons = 70,
        UpdateCoupons = 71,
        DeleteCoupons = 72,

        CreateCurrency = 73,
        ReadCurrency = 74,
        UpdateCurrency = 75,
        DeleteCurrency = 76,

        CreateDeliveryOrder = 77,
        ReadDeliveryOrder = 78,
        UpdateDeliveryOrder = 79,
        DeleteDeliveryOrder = 80,

        CreateDiscount = 81,
        ReadDiscount = 82,
        UpdateDiscount = 83,
        DeleteDiscount = 84,

        CreateDiscountCategory = 85,
        ReadDiscountCategory = 86,
        UpdateDiscountCategory = 87,
        DeleteDiscountCategory = 88,

        //CreateFAQ = 89,
        //ReadFAQ = 90,
        //UpdateFAQ = 91,
        //DeleteFAQ = 92,

        CreateFlashDeal = 93,
        ReadFlashDeal = 94,
        UpdateFlashDeal = 95,
        DeleteFlashDeal = 96,

        CreateFooter = 97,
        ReadFooter = 98,
        UpdateFooter = 99,
        DeleteFooter = 100,

        CreateFreeDelivery = 101,
        ReadFreeDelivery = 102,
        UpdateFreeDelivery = 103,
        DeleteFreeDelivery = 104,

        CreateHashtag = 105,
        ReadHashtag = 106,
        UpdateHashtag = 107,
        DeleteHashtag = 108,

        CreateHeader = 109,
        ReadHeader = 110,
        UpdateHeader = 111,
        DeleteHeader = 112,

        CreateHomeSection = 113,
        ReadHomeSection = 114,
        UpdateHomeSection = 115,
        DeleteHomeSection = 116,

        //CreateInquiryType = 117,
        //ReadInquiryType = 118,
        //UpdateInquiryType = 119,
        //DeleteInquiryType = 120,

        CreateNavTitle = 121,
        ReadNavTitle = 122,
        UpdateNavTitle = 123,
        DeleteNavTitle = 124,

        CreateNewsletter = 125,
        ReadNewsletter = 126,
        UpdateNewsletter = 127,
        DeleteNewsletter = 128,

        CreateNewsletterSubscriber = 129,
        ReadNewsletterSubscriber = 130,
        UpdateNewsletterSubscriber = 131,
        DeleteNewsletterSubscriber = 132,

        CreateOrder = 133,
        ReadOrder = 134,
        UpdateOrder = 135,
        DeleteOrder = 136,

        CreateOrderItem = 137,
        ReadOrderItem = 138,
        UpdateOrderItem = 139,
        DeleteOrderItem = 140,

        CreatePermissionEntity = 141,
        ReadPermissionEntity = 142,
        UpdatePermissionEntity = 143,
        DeletePermissionEntity = 144,

        CreateProduct = 145,
        ReadProduct = 146,
        UpdateProduct = 147,
        DeleteProduct = 148,

        CreateProductCategory = 149,
        ReadProductCategory = 150,
        UpdateProductCategory = 151,
        DeleteProductCategory = 152,

        CreateProductColor = 153,
        ReadProductColor = 154,
        UpdateProductColor = 155,
        DeleteProductColor = 156,

        CreateProductColorImage = 157,
        ReadProductColorImage = 158,
        UpdateProductColorImage = 159,
        DeleteProductColorImage = 160,

        CreateProductSize = 161,
        ReadProductSize = 162,
        UpdateProductSize = 163,
        DeleteProductSize = 164,

        CreateProductSizeDiscount = 165,
        ReadProductSizeDiscount = 166,
        UpdateProductSizeDiscount = 167,
        DeleteProductSizeDiscount = 168,

        CreateReceivePoints = 169,
        ReadReceivePoints = 170,
        UpdateReceivePoints = 171,
        DeleteReceivePoints = 172,

        CreateRefreshToken = 173,
        ReadRefreshToken = 174,
        UpdateRefreshToken = 175,
        DeleteRefreshToken = 176,

        CreateReviews = 177,
        ReadReviews = 178,
        UpdateReviews = 179,
        DeleteReviews = 180,

        CreateSEO = 181,
        ReadSEO = 182,
        UpdateSEO = 183,
        DeleteSEO = 184,

        CreateShippingPrice = 185,
        ReadShippingPrice = 186,
        UpdateShippingPrice = 187,
        DeleteShippingPrice = 188,

        CreateSize = 189,
        ReadSize = 190,
        UpdateSize = 191,
        DeleteSize = 192,

        CreateSMTPServer = 193,
        ReadSMTPServer = 194,
        UpdateSMTPServer = 195,
        DeleteSMTPServer = 196,

        CreateSubCategory = 197,
        ReadSubCategory = 198,
        UpdateSubCategory = 199,
        DeleteSubCategory = 200,

        CreateSubCategoryType = 201,
        ReadSubCategoryType = 202,
        UpdateSubCategoryType = 203,
        DeleteSubCategoryType = 204,

        CreateTicket = 205,
        ReadTicket = 206,
        UpdateTicket = 207,
        DeleteTicket = 208,

        CreateTypeEntity = 209,
        ReadTypeEntity = 210,
        UpdateTypeEntity = 211,
        DeleteTypeEntity = 212,

        CreateUser = 213,
        ReadUser = 214,
        UpdateUser = 215,
        DeleteUser = 216,

        CreateUserCoupon = 217,
        ReadUserCoupon = 218,
        UpdateUserCoupon = 219,
        DeleteUserCoupon = 220,

        CreateUserPermission = 221,
        ReadUserPermission = 222,
        UpdateUserPermission = 223,
        DeleteUserPermission = 224,

        CreateWallet = 225,
        ReadWallet = 226,
        UpdateWallet = 227,
        DeleteWallet = 228,

        //CreateWalletTransaction = 229,
        //ReadWalletTransaction = 230,
        //UpdateWalletTransaction = 231,
        //DeleteWalletTransaction = 232,

        CreateWishList = 233,
        ReadWishList = 234,
        UpdateWishList = 235,
        DeleteWishList = 236
    }

    public enum TypePermission
    {
        AboutUs = 1,
        Address = 2,
        BagItem = 3,
        BaseEntity = 4,
        BlogCategory = 5,
        Blogs = 6,
        Brand = 7,
        Cart = 8,
        CartItem = 9,
        Category = 10,
        CategorySubCategoryBridge = 11,
        City = 12,
        CityAreas = 13,
        Color = 14,
        ContactUs = 15,
        ContactUsSetup = 16,
        Country = 17,
        Coupons = 18,
        Currency = 19,
        DeliveryOrder = 20,
        Discount = 21,
        DiscountCategory = 22,
        FAQ = 23,
        FlashDeal = 24,
        Footer = 25,
        FreeDelivery = 26,
        Hashtag = 27,
        Header = 28,
        HomeSection = 29,
        InquiryType = 30,
        NavTitle = 31,
        Newsletter = 32,
        NewsletterSubscriber = 33,
        Order = 34,
        OrderItem = 35,
        PermissionEntity = 36,
        Product = 37,
        ProductCategory = 38,
        ProductColor = 39,
        ProductColorImage = 40,
        ProductSize = 41,
        ProductSizeDiscount = 42,
        ReceivePoints = 43,
        RefreshToken = 44,
        Reviews = 45,
        SEO = 46,
        ShippingPrice = 47,
        Size = 48,
        SMTPServer = 49,
        SubCategory = 50,
        SubCategoryType = 51,
        Ticket = 52,
        TypeEntity = 53,
        User = 54,
        UserCoupon = 55,
        UserPermission = 56,
        Wallet = 57,
        WalletTransaction = 58,
        WishList = 59
    }


}
