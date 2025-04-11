using OutbornE_commerce.BAL.Dto.Cart;
using OutbornE_commerce.BAL.Repositories.CartItem;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Services.Cart_Service
{
    public interface ICartService
    {
        Task<CartDto?> GetUserCartAsync(string userId);

        Task<GetCartResponseDto?> GetCartDetails(string userId);

        Task<CartDto?> CreateOrUpdateCartAsync(string UserID, CreateCartDto userCart);

        Task ClearCartAsync(string userId);

        Task<bool> RemoveFromCartAsync(string userId, Guid productSizeId);
    }
}