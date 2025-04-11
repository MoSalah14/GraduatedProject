
namespace OutbornE_commerce.DAL.Models
{
    public sealed class Cart : BaseEntity
    {
        public string UserId { get; set; }
        public ICollection<CartItem>? CartItems { get; set; }  // Collection of items in the cart
        public decimal TotalPrice
        {
            get
            {
                return CartItems?.Sum(item => item.Quantity * item.UnitPrice) ?? 0;
            }
        }
        public User? User { get; set; }
    }

}
