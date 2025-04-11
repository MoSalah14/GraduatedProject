using Newtonsoft.Json;
using OutbornE_commerce.BAL.Dto.Cart;
using OutbornE_commerce.BAL.Dto.WishList;
using OutbornE_commerce.BAL.Repositories.Products;
using OutbornE_commerce.DAL.Models;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Services.Cart_Service
{
    public class CartService : ICartService
    {
        private readonly IDatabase _Redis;
        private readonly IProductRepository productRepository;

        public CartService(IConnectionMultiplexer Redis, IProductRepository productRepository)
        {
            _Redis = Redis.GetDatabase();
            this.productRepository = productRepository;
        }

        public async Task<CartDto?> GetUserCartAsync(string userId)
        {
            var UserCart = await _Redis.StringGetAsync(userId);
            return UserCart.IsNullOrEmpty ? null : JsonConvert.DeserializeObject<CartDto>(UserCart);
        }

        public async Task<GetCartResponseDto?> GetCartDetails(string userId)
        {
            var UserCart = await _Redis.StringGetAsync(userId);
            var cart = UserCart.IsNullOrEmpty ? null : JsonConvert.DeserializeObject<CartDto>(UserCart);

            if (cart == null) return null;

            decimal totalCartPrice = 0;
            decimal totalCartWeight = 0;
            var userCartDetails = new List<GetUserCart>();

            foreach (var item in cart.Items)
            {
                var product = await productRepository.Find(p =>
                    p.ProductColors.Any(c => c.ProductSizes.Any(s => s.Id == item.ProductSizeId)), false,
                    new string[] { "ProductColors", "ProductColors.Color", "ProductColors.ProductSizes", "ProductColors.ProductColorImages", "ProductColors.ProductSizes.Size" }
                );

                if (product != null)
                {
                    var productColor = product.ProductColors.FirstOrDefault(c =>
                        c.ProductSizes.Any(s => s.Id == item.ProductSizeId));
                    var productSize = productColor?.ProductSizes.FirstOrDefault(s => s.Id == item.ProductSizeId);

                    var cartItem = new GetUserCart
                    {
                        ProductId = product.Id,
                        ProductSizeId = item.ProductSizeId,
                        ProductNameEn = product.NameEn,
                        ProductNameAr = product.NameAr,
                        ProductImage = productColor.ProductColorImages.FirstOrDefault().ImageUrl,
                        Color = productColor?.Color.NameEn,
                        Size = productSize?.Size.Name,
                        Quantity = item.Quantity,
                        UnitPrice = productSize.DiscountedPrice > 0 ? productSize.DiscountedPrice : productSize.Price,
                        ProductCode = product.ProductCode,
                        Product_Weight = product.ProductColors!
                                        .SelectMany(pc => pc.ProductSizes!)
                                        .FirstOrDefault()!
                                        .ProductWeight,
                    };
                    totalCartPrice += cartItem.Quantity * cartItem.UnitPrice;

                    if (cartItem.Product_Weight>0)
                    {
                        totalCartWeight  += cartItem.Quantity * cartItem.Product_Weight;
                    }
                    if (productSize.Quantity == 0)
                    {
                        cartItem.IsOutOfStock = true;
                    }
                    else cartItem.IsOutOfStock = false;
                    userCartDetails.Add(cartItem);
                }
                else
                {
                    await RemoveFromCartAsync(userId, item.ProductSizeId);
                }
            }

            return new GetCartResponseDto
            {
                CartItems = userCartDetails,
                TotalCartPrice = totalCartPrice,
                TotalProductWeight= totalCartWeight
            };
        }

        public async Task<CartDto?> CreateOrUpdateCartAsync(string UserID, CreateCartDto newCartDto)
        {
            // Get the existing cart from Redis
            var existingCartJson = await _Redis.StringGetAsync(UserID);
            CartDto existingCart = new CartDto();

            if (existingCartJson.HasValue)
            {
                existingCart = JsonConvert.DeserializeObject<CartDto>(existingCartJson);
            }
            else
            {
                // If no cart exists, create a new empty cart
                existingCart.UserId = Guid.Parse(UserID);
                existingCart.Items = new List<CartItemDto>();
            }

            foreach (var newItem in newCartDto.Items)
            {
                // if the item already exists in the cart
                var existingProduct = existingCart.Items.FirstOrDefault(item => item.ProductSizeId == newItem.ProductSizeId);

                if (existingProduct != null)
                {
                    // If the product exists, update its quantity
                    existingProduct.Quantity = newItem.Quantity;  // Update quantity or override
                }
                else
                {
                    existingCart.Items.Add(new CartItemDto
                    {
                        ProductSizeId = newItem.ProductSizeId,
                        Quantity = newItem.Quantity,
                        ItemPrice = newItem.ItemPrice
                    });
                }
            }

            var createdOrUpdated = await _Redis.StringSetAsync(UserID, JsonConvert.SerializeObject(existingCart), TimeSpan.FromDays(30));

            if (!createdOrUpdated) return null;

            return await GetUserCartAsync(UserID);
        }

        public async Task<bool> RemoveFromCartAsync(string userId, Guid productSizeId)
        {
            var existingCartJson = await _Redis.StringGetAsync(userId);
            if (!existingCartJson.HasValue) return false;

            var existingCart = JsonConvert.DeserializeObject<CreateCartDto>(existingCartJson);

            // Find the item with the matching ProductSizeId
            var itemToRemove = existingCart.Items.FirstOrDefault(e => e.ProductSizeId == productSizeId);

            if (itemToRemove != null)
            {
                existingCart.Items.Remove(itemToRemove);

                var updatedCartJson = JsonConvert.SerializeObject(existingCart);
                var updated = await _Redis.StringSetAsync(userId, updatedCartJson, TimeSpan.FromDays(30));

                return updated;
            }

            return false;
        }

        public async Task ClearCartAsync(string userId)
            => await _Redis.KeyDeleteAsync(userId);
    }
}