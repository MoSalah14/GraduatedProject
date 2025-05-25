using OutbornE_commerce.BAL.Dto.Cart;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Data;
using System.Threading;

namespace OutbornE_commerce.BAL.Repositories
{
    public class BagItemsRepo : BaseRepository<DAL.Models.BagItem>, IBagItemsRepo
    {
        public BagItemsRepo(ApplicationDbContext context) : base(context)
        {

        }

        public async Task ClearCartAsync(string userId, CancellationToken cancellationToken)
        {

            await DeleteRange(x => x.UserId == userId);
            await SaveAsync(cancellationToken);
        }

        public async Task<List<CartItemDto>> MapCartDto(string userId)
        {
            string[] includes = new string[] { "Product" };

            var CartItems = await FindByCondition(x => x.UserId == userId, includes);

            if (CartItems == null || !CartItems.Any())
            {
                return new List<CartItemDto>();
            }

            var CartDtos = CartItems.Select(Cart => new CartItemDto
            {
                ProductId = Cart.ProductId,
                ProductNameEn = Cart.Product.NameEn,
                ProductNameAr = Cart.Product.NameAr,
                ImageUrl = Cart.Product.MainImageUrl,
                ItemPrice = Cart.Product?.Price
            }).ToList();

            return CartDtos;
        }
    }
}