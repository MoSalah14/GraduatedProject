using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public class WishList
    {
        public string UserId { get; set; }
        public User? UserWishList { get; set; }
        public Guid ProductId { get; set; }
        public Product ProductWishList { get; set; }
        public string CreatedBy { get; set; } = "Admin";
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
