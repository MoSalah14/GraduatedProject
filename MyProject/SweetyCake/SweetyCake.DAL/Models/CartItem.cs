using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public class CartItem:BaseEntity
    {
        public Guid CartId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; } = 0;
        public decimal UnitPrice { get; set; }
        public Product? Product { get; set; }
        public Cart? Cart { get; set; } 
    }

}
