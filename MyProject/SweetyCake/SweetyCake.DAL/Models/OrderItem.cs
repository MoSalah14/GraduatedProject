using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public class OrderItem
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal ItemPrice { get; set; }
        public double ProductWeight { get; set; }

        public virtual Order? Order { get; set; }
    }
}