using Microsoft.AspNetCore.Mvc;
using OutbornE_commerce.BAL.Dto.Cart;
using OutbornE_commerce.BAL.Dto.WishList;
using OutbornE_commerce.BAL.Repositories.Cart;
using OutbornE_commerce.BAL.Repositories.CartItem;
using OutbornE_commerce.BAL.Repositories.Products;
using OutbornE_commerce.BAL.Services.Cart_Service;
using OutbornE_commerce.DAL.Models;
using OutbornE_commerce.Extensions;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService CartService;

        public CartController(ICartService _cartService)
        {
            CartService = _cartService;
        }

        [HttpGet("GetCartDetails")]
        public async Task<IActionResult> GetCartDetails()
        {
            var UserId = User.GetUserIdFromToken();
            if (UserId is null)
                return Unauthorized(new
                {
                    MessageEn = "Please Login First",
                    MessageAr = "برجاء تسجيل الدخول اولا"
                });

            var UserCart = await CartService.GetCartDetails(UserId);
            if (UserCart == null)
            {
                return BadRequest(new Response<GetUserCart>
                {
                    Data = null,
                    IsError = true,
                    Message = "العربه فارغه",
                    MessageAr = "العربه فارغه"
                });
            }
            return Ok(new Response<GetCartResponseDto>
            {
                Data = UserCart,
                IsError = false,
                Message = "Success  Cart",
                MessageAr = "تمت العملية بنجاح "
            });
        }

        [HttpDelete("RemoveFromWishList")]
        public async Task<IActionResult> RemoveFromWishList(Guid productSizeId)
        {
            var userId = User.GetUserIdFromToken();
            if (userId == null) return Unauthorized(new
            {
                MessageEn = "Please Login First",
                MessageAr = "برجاء تسجيل الدخول اولا"
            });

            var removed = await CartService.RemoveFromCartAsync(userId, productSizeId);
            if (!removed) return BadRequest(new Response<string>
            {
                IsError = true,
                Message = "Faild to Remove From Cart",
                MessageAr = "فشلت عملية المسح"
            });

            return Ok(new Response<bool>
            {
                Data = removed,
                IsError = false,
                Message = "Item removed from Cart successfully",
                MessageAr = "تم إزالة المنتج من عربة التسوق بنجاح"
            });
        }

        [HttpPost("UpdateUserCart")]
        public async Task<IActionResult> CreateOrUpdateUserCart(CreateCartDto cartDto)
        {
            var UserId = User.GetUserIdFromToken();

            if (UserId is null)
                return Unauthorized(new
                {
                    MessageEn = "Please Login First",
                    MessageAr = "برجاء تسجيل الدخول اولا"
                });

            var UserCart = await CartService.CreateOrUpdateCartAsync(UserId, cartDto);

            if (UserCart is not null)
            {
                UserCart.UserId = Guid.Parse(UserId);
                UserCart.CartId = UserCart.UserId;
                return Ok(new Response<CartDto>
                {
                    Data = UserCart,
                    IsError = false,
                    Message = "Success Create Or Update Cart",
                    MessageAr = "تمت العملية بنجاح "
                });
            }
            return BadRequest(new Response<CartDto>
            {
                Data = UserCart,
                IsError = true,
                Message = "Faild to Create Or Update Cart",
                MessageAr = "فشلت العملية "
            });
        }

        [HttpDelete("ClearUSerCart")]
        public async Task<IActionResult> ClearUserCart()
        {
            var UserId = User.GetUserIdFromToken();
            await CartService.ClearCartAsync(UserId);

            return Ok("Deleted Succefully");
        }
    }
}