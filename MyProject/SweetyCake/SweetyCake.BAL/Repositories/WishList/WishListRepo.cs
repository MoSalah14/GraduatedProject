using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.BAL.Dto.WishList;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Data;

namespace OutbornE_commerce.BAL.Repositories.WishList
{
    public class WishListRepo : BaseRepository<DAL.Models.WishList>, IWishListRepo
    {
        public WishListRepo(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<WishListsItemDto>> MapWishListDto(string userId)
        {
            string[] includes = new string[] { "ProductWishList" };

            var wishListItems = await FindByCondition(x => x.UserId == userId, includes);

            if (wishListItems == null || !wishListItems.Any())
            {
                return new List<WishListsItemDto>();
            }

            var wishListDtos = wishListItems.Select(wishList => new WishListsItemDto
            {
                ProductId = wishList.ProductId,
                ProductNameEn = wishList.ProductWishList.NameEn,
                ImageUrl = wishList.ProductWishList.MainImageUrl,
                ItemPrice = wishList.ProductWishList?.Price,
                QuantityInStock = wishList.ProductWishList.QuantityInStock
            }).ToList();

            return wishListDtos;
        }
    }
}