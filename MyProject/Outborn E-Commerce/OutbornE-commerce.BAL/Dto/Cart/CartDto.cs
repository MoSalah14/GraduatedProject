
using OutbornE_commerce.DAL.Models;

namespace OutbornE_commerce.BAL.Dto.Cart
{
    public class CartDto
    {
        public Guid CartId { get; set; }
        public Guid UserId { get; set; }
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();

        public decimal TotalPrice => Items?.Sum(item => item.Quantity * item.ItemPrice) ?? 0;

        public CartDto() { }

        public CartDto(Guid user)
        {
            CartId = Guid.NewGuid();
            Items = new List<CartItemDto>();
            UserId = user;
        }
    }

}
