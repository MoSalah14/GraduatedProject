using OutbornE_commerce.BAL.Dto.WishList;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;

namespace OutbornE_commerce.BAL.Repositories.WishList
{
    public interface IWishListRepo : IBaseRepository<DAL.Models.WishList>
    {
        Task<List<WishListsItemDto>> MapWishListDto(string userId);
    }
}
