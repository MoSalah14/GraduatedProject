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

        public async Task<DisplayCartItemDto> GetUserCartAsync(string userId)
        {
            string[] includes = new string[] { "Product" };
            var cartItems = await FindByCondition(x => x.UserId == userId, includes);

            if (cartItems == null || !cartItems.Any())
            {
                return new DisplayCartItemDto();
            }

            var cartDtos = cartItems.Select(item => new CartItemDto
            {
                ProductId = item.ProductId,
                ProductNameEn = item.Product.NameEn,
                ProductNameAr = item.Product.NameAr,
                ImageUrl = item.Product.MainImageUrl,
                ItemPrice = item.Product?.Price ?? 0,
                Quantity = item.Quantity

            }).ToList();

            var totalPrice = cartItems.Sum(x => x.Product.Price * x.Quantity);

            return new DisplayCartItemDto
            {
                cartItemDtos = cartDtos,
                TotalPrice = totalPrice
            };
        }

    }
}