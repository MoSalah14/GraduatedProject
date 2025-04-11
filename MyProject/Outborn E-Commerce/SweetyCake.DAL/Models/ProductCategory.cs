using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public class ProductCategory
    {
        public Guid CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
