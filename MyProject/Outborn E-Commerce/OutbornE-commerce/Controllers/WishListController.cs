using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OutbornE_commerce.BAL.Dto.WishList;
using OutbornE_commerce.BAL.Repositories.WishList;
using OutbornE_commerce.BAL.Services.Cart_Service;
using OutbornE_commerce.BAL.Services.WishListsService;
using OutbornE_commerce.Extensions;
using System.Collections.Generic;
using System.Threading;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WishListController : ControllerBase
    {
        private readonly IWishListRepo wishListRepo;

        public WishListController(IWishListRepo wishListRepo)
        {
            this.wishListRepo = wishListRepo;
        }

        [HttpGet("GetAllByUserId")]
        public async Task<ActionResult<List<WishListsItemDto>>> GetAllByUserId()
        {
            var userId = User.GetUserIdFromToken();
            if (userId == null)
                return Unauthorized(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = "Please Login First",
                    MessageAr = "برجاء تسجيل الدخول اولا",
                    Status = (int)StatusCodeEnum.Unauthorized
                });

            var wishList = await wishListRepo.MapWishListDto(userId);

            if (wishList is null)
                return Ok(new Response<List<WishListsItemDto>>
                {
                    IsError = false,
                    Data = new List<WishListsItemDto>(),
                    Message = "لا توجد قائمة امنيات",
                    MessageAr = "Empty With List",
                    Status = (int)StatusCodeEnum.Ok
                });
            return Ok(new Response<List<WishListsItemDto>>
            {
                Data = wishList,
                IsError = false,
                Message = "WishList retrieved successfully",
                MessageAr = "تم استرجاع قائمة الرغبات بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpPost("AddToWishList")]
        public async Task<IActionResult> AddToWishList(Guid ProductId, CancellationToken cancellationToken)
        {
            var userId = User.GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = "Please Login First",
                    MessageAr = "برجاء تسجيل الدخول اولا",
                    Status = (int)StatusCodeEnum.Unauthorized
                });
            }

            var existingWishListItem = await wishListRepo
                .FindByCondition(w => w.UserId == userId && w.ProductId == ProductId);

            if (existingWishListItem.Count() != 0)
            {
                return BadRequest(new Response<string>
                {
                    Message = "Product is already in your wishlist",
                    MessageAr = "المنتج موجود بالفعل في قائمة الرغبات",
                    IsError = true,
                    Status = (int)StatusCodeEnum.BadRequest
                });
            }

            var UserWishList = new WishList()
            {
                UserId = userId,
                ProductId = ProductId,
                CreatedBy = "Admin",
                CreatedOn = DateTime.Now
            };

            await wishListRepo.Create(UserWishList);
            await wishListRepo.SaveAsync(cancellationToken);

            return Ok(new Response<Guid>
            {
                Data = UserWishList.ProductId,
                IsError = false,
                Message = "WishList updated successfully",
                MessageAr = "تم تحديث قائمة الرغبات بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpDelete("ClearWishList")]
        public async Task<IActionResult> ClearWishList(CancellationToken cancellationToken)
        {
            var userId = User.GetUserIdFromToken();
            if (userId == null)
                return Unauthorized(new
                {
                    MessageEn = "Please Login First",
                    MessageAr = "برجاء تسجيل الدخول اولا"
                });

            await wishListRepo.DeleteRange(x => x.UserId == userId);
            await wishListRepo.SaveAsync(cancellationToken);

            return Ok(new Response<object>
            {
                Data = null,
                IsError = false,
                Message = "WishList cleared successfully",
                MessageAr = "تمت إزالة قائمة الرغبات بنجاح"
            });
        }

        [HttpDelete("RemoveFromWishList/{productId}")]
        public async Task<IActionResult> RemoveFromWishList(Guid productId, CancellationToken cancellationToken)
        {
            var userId = User.GetUserIdFromToken();
            if (userId == null)
                return Unauthorized(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = "Please Login First",
                    MessageAr = "برجاء تسجيل الدخول اولا",
                    Status = (int)StatusCodeEnum.Unauthorized
                });

            var removed = await wishListRepo.Find(x => x.ProductId == productId && x.UserId == userId);

            if (removed == null)
                return BadRequest(new Response<string>
                {
                    IsError = true,
                    Message = "Failed to remove item from WishList",
                    MessageAr = "فشلت عملية المسح",
                    Status = (int)StatusCodeEnum.BadRequest,
                    Data = null
                });
            wishListRepo.Delete(removed);
            await wishListRepo.SaveAsync(cancellationToken);

            return Ok(new Response<Guid>
            {
                Data = removed.ProductId,
                IsError = false,
                Message = "Item removed from WishList successfully",
                MessageAr = "تمت إزالة المنتج من قائمة الرغبات بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }
    }
}