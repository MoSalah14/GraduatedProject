using Newtonsoft.Json;
using OutbornE_commerce.BAL.Dto.Cart;
using OutbornE_commerce.BAL.Dto.WishList;
using OutbornE_commerce.BAL.Repositories.Products;
using OutbornE_commerce.DAL.Models;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Services.WishListsService
{
    public class WishListsService : IWishListsService
    {
        private readonly IDatabase _Redis;
        private readonly IProductRepository productRepository;

        public WishListsService(IConnectionMultiplexer Redis, IProductRepository productRepository)
        {
            _Redis = Redis.GetDatabase();
            this.productRepository = productRepository;
        }

    //    public async Task<CartDto?> GetUserWishListsAsync(string userId)
    //    {
    //        var UserCart = await _Redis.StringGetAsync(userId);
    //        return UserCart.IsNullOrEmpty ? null : JsonConvert.DeserializeObject<CartDto>(UserCart);
    //    }

    //    public async Task<List<WishListsItemDto>?> GetWishListsDetails(string userId)
    //    {
    //        var UserCart = await _Redis.StringGetAsync(userId);
    //        var cart = UserCart.IsNullOrEmpty ? null : JsonConvert.DeserializeObject<CreateWishListDto>(UserCart);

    //        if (cart == null) return null;

    //        var userCartDetails = new List<WishListsItemDto>();

    //        foreach (var productId in cart.ProductIds)
    //        {
    //            var product = await productRepository.Find(p => p.Id == productId, false,
    //                new string[] { "ProductColors", "ProductColors.ProductSizes", "Brand", "ProductColors.ProductSizes.Size" }
    //            );

    //            if (product != null)
    //            {
    //                var cartItem = new WishListsItemDto
    //                {
    //                    ProductId = productId,
    //                    ProductNameEn = product.NameEn,
    //                    ProductNameAr = product.NameAr,
    //                    ImageUrl = product.ProductColors.Select(e => e.ImageUrl).FirstOrDefault(),
    //                    ItemPrice = product.ProductColors.SelectMany(e => e.ProductSizes).Select(e => e.Price).FirstOrDefault(),
    //                    BrandNameEn = product.Brand.NameEn,
    //                    BrandNameAr = product.Brand.NameAr,
    //                };

    //                userCartDetails.Add(cartItem);
    //            }
    //            else
    //                userCartDetails.Add(null);
    //        }

    //        return userCartDetails;
    //    }

    //    public async Task<CreateWishListDto?> CreateOrUpdateWishListsAsync(string UserID, CreateWishListDto newWishListDto)
    //    {
    //        // Get the existing WishList from Redis
    //        var existingWishListJson = await _Redis.StringGetAsync(UserID);
    //        CreateWishListDto existingWishList = new CreateWishListDto();

    //        if (existingWishListJson.HasValue)
    //        {
    //            existingWishList = JsonConvert.DeserializeObject<CreateWishListDto>(existingWishListJson);
    //        }

    //        // Update the WishList
    //        foreach (var productId in newWishListDto.ProductIds)
    //        {
    //            // Add unique product IDs to the WishList
    //            if (!existingWishList.ProductIds.Contains(productId))
    //            {
    //                existingWishList.ProductIds.Add(productId);
    //            }
    //        }

    //        var createdOrUpdated = await _Redis.StringSetAsync(UserID, JsonConvert.SerializeObject(existingWishList), TimeSpan.FromDays(180));

    //        if (!createdOrUpdated) return null;

    //        return existingWishList;
    //    }

    //    public async Task<bool> ClearWishListsAsync(string userId)
    //    {
    //        var Result = await _Redis.KeyDeleteAsync(userId);
    //        return Result;
    //    }

    //    public async Task<bool> RemoveFromWishListAsync(string UserID, Guid productId)
    //    {
    //        var existingWishListJson = await _Redis.StringGetAsync(UserID);
    //        if (!existingWishListJson.HasValue) return false; // WishList doesn't exist.

    //        var existingWishList = JsonConvert.DeserializeObject<CreateWishListDto>(existingWishListJson);

    //        if (existingWishList.ProductIds.Remove(productId))
    //        {
    //            var updated = await _Redis.StringSetAsync(UserID, JsonConvert.SerializeObject(existingWishList), TimeSpan.FromDays(30));
    //            return updated;
    //        }

    //        return false;
    //    }
    }
}