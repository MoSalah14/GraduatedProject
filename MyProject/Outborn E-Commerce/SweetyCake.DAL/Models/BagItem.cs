using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public class BagItem : BaseEntity
    {
        public string? UserId { get; set; }
        public User? User { get; set; }
        public Guid ProductId {  get; set; }
        public Product? Product { get; set; }
        public Guid SizeId { get; set; }
        public Size? Size { get; set; }
        public string Code { get; set; }
        public int Quantity { get; set; }
    }
}
