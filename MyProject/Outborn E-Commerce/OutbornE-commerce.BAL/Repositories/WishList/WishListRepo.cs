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
            string[] includes = new string[] { "ProductWishList", "ProductWishList.ProductColors", "ProductWishList.ProductColors.ProductSizes", "ProductWishList.ProductColors.ProductColorImages", "ProductWishList.Brand" };

            var wishListItems = await FindByCondition(x => x.UserId == userId, includes);
            var items = _context.WishLists.Include(x => x.UserWishList).Include(s => s.ProductWishList).ThenInclude(r => r.ProductColors).Where(x => x.UserId == userId).ToList();

            if (wishListItems == null || !wishListItems.Any())
            {
                return new List<WishListsItemDto>();
            }

            var wishListDtos = wishListItems.Select(wishList => new WishListsItemDto
            {
                ProductId = wishList.ProductId,
                ProductNameEn = wishList.ProductWishList?.NameEn,
                ProductNameAr = wishList.ProductWishList?.NameAr,
                BrandNameEn = wishList.ProductWishList?.Brand?.NameEn,
                BrandNameAr = wishList.ProductWishList?.Brand?.NameAr,

                ImageUrl = wishList.ProductWishList?.ProductColors.SelectMany(e => e.ProductColorImages).FirstOrDefault()?.ImageUrl,

                ItemPrice = wishList.ProductWishList?.ProductColors.FirstOrDefault()?.ProductSizes
                            .OrderBy(size => size.Price)
                            .FirstOrDefault()?.Price
            }).ToList();

            return wishListDtos;
        }
    }
}