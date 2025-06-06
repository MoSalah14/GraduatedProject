﻿using Microsoft.AspNetCore.Authorization;
using OutbornE_commerce.BAL.Dto.Cart;
using OutbornE_commerce.BAL.Repositories;
using OutbornE_commerce.Extensions;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly IBagItemsRepo _BagItemsRepo;

        public CartController(IBagItemsRepo CartRepo)
        {
            _BagItemsRepo = CartRepo;
        }

        [HttpGet("GetAllByUserId")]
        public async Task<ActionResult<List<CartItemDto>>> GetAllByUserId()
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

            var Cart = await _BagItemsRepo.MapCartDto(userId);

            if (Cart is null)
                return Ok(new Response<List<CartItemDto>>
                {
                    IsError = false,
                    Data = new List<CartItemDto>(),
                    Message = "لا توجد قائمة امنيات",
                    MessageAr = "Empty With List",
                    Status = (int)StatusCodeEnum.Ok
                });
            return Ok(new Response<List<CartItemDto>>
            {
                Data = Cart,
                IsError = false,
                Message = "Cart retrieved successfully",
                MessageAr = "تم استرجاع قائمة الرغبات بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart(Guid ProductId, CancellationToken cancellationToken)
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

            var existingCartItem = await _BagItemsRepo
                .FindByCondition(w => w.UserId == userId && w.ProductId == ProductId);

            if (existingCartItem.Any())
            {
                return BadRequest(new Response<string>
                {
                    Message = "Product is already in your Cart",
                    MessageAr = "المنتج موجود بالفعل في قائمة الرغبات",
                    IsError = true,
                    Status = (int)StatusCodeEnum.BadRequest
                });
            }

            var UserCart = new BagItem()
            {
                UserId = userId,
                ProductId = ProductId,
                CreatedBy = "Admin",
                CreatedOn = DateTime.Now
            };

            await _BagItemsRepo.Create(UserCart);
            await _BagItemsRepo.SaveAsync(cancellationToken);

            return Ok(new Response<Guid>
            {
                Data = UserCart.ProductId,
                IsError = false,
                Message = "Cart updated successfully",
                MessageAr = "تم تحديث قائمة الرغبات بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }

        [HttpDelete("ClearCart")]
        public async Task<IActionResult> ClearCart(CancellationToken cancellationToken)
        {
            try
            {
                var userId = User.GetUserIdFromToken();
                if (userId == null)
                    return Unauthorized(new
                    {
                        MessageEn = "Please Login First",
                        MessageAr = "برجاء تسجيل الدخول اولا"
                    });

                await _BagItemsRepo.ClearCartAsync(userId, cancellationToken);
                return Ok(new Response<object>
                {
                    Data = null,
                    IsError = false,
                    Message = "Cart cleared successfully",
                    MessageAr = "تمت إزالة قائمة الرغبات بنجاح"
                });
            }

            catch
            {
                return BadRequest(new Response<object>
                {
                    Data = null,
                    IsError = false,
                    Message = "An Error With Delete",
                    MessageAr = "حدثت مشكلة اثناء المسح"
                });
            }
        }

        [HttpDelete("RemoveFromCart/{productId}")]
        public async Task<IActionResult> RemoveFromCart(Guid productId, CancellationToken cancellationToken)
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

            var removed = await _BagItemsRepo.Find(x => x.ProductId == productId && x.UserId == userId);

            if (removed == null)
                return BadRequest(new Response<string>
                {
                    IsError = true,
                    Message = "Failed to remove item from Cart",
                    MessageAr = "فشلت عملية المسح",
                    Status = (int)StatusCodeEnum.BadRequest,
                    Data = null
                });
            _BagItemsRepo.Delete(removed);
            await _BagItemsRepo.SaveAsync(cancellationToken);

            return Ok(new Response<Guid>
            {
                Data = removed.ProductId,
                IsError = false,
                Message = "Item removed from Cart successfully",
                MessageAr = "تمت إزالة المنتج من قائمة الرغبات بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }
    }
}