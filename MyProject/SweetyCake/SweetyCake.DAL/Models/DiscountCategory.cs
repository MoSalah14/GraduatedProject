using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public class DiscountCategory : BaseEntity
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }=false;

        public Guid DiscountId { get; set; }
        public virtual Discount? Discount { get; set; }
        public Guid CategoryId { get; set; }
        public virtual Category?  Category { get; set; }

    }
}
