using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.BAL.Repositories.Cart;
using OutbornE_commerce.DAL.Data;
namespace OutbornE_commerce.BAL.Repositories.CartItem
{
    public class CartItemRepo : BaseRepository<DAL.Models.CartItem>, ICartItemRepo
    {
        public CartItemRepo(ApplicationDbContext context) : base(context) { 
        }


    }
}
